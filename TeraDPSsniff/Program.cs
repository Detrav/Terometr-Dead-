using NdisApiWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TeraDPSsniff
{
    class Program
    {
        static void Main(string[] args)
        {
                var driverPtr = Ndisapi.OpenFilterDriver();
                var adapters = new TCP_AdapterList();
                GCHandle.Alloc(adapters);

                if ((Ndisapi.IsDriverLoaded(driverPtr)))
                {
                    var result = Ndisapi.GetTcpipBoundAdaptersInfo(driverPtr, ref adapters);
                    for(int i = 0; i< adapters.m_nAdapterCount; i++)
                    {
                        Console.WriteLine("{0}) {1}", i + 1, adapters.GetName(i));
                    }
                }

                int num = int.Parse(Console.ReadLine()) - 1;

                var mode = new ADAPTER_MODE
                {
                    dwFlags = Ndisapi.MSTCP_FLAG_SENT_LISTEN | Ndisapi.MSTCP_FLAG_RECV_LISTEN,
                    hAdapterHandle = adapters.m_nAdapterHandle[num]
                };

                Ndisapi.SetAdapterMode(driverPtr, ref mode);

                // Allocate and initialize packet structures
                var buffer = new INTERMEDIATE_BUFFER();
                var bufferPtr = Marshal.AllocHGlobal(Marshal.SizeOf(buffer));
                Win32Api.ZeroMemory(bufferPtr, Marshal.SizeOf(buffer));
                var request = new ETH_REQUEST
                {
                    hAdapterHandle = adapters.m_nAdapterHandle[num],
                    EthPacket = { Buffer = bufferPtr }
                };

                var end = DateTime.Now.AddSeconds(30);

                while(DateTime.Now<end)
                {
                    if (Ndisapi.ReadPacket(driverPtr, ref request))
                    {

                        buffer = (INTERMEDIATE_BUFFER)Marshal.PtrToStructure(bufferPtr, typeof(INTERMEDIATE_BUFFER));
                        //Console.WriteLine();
                        writeToConsole(buffer);
                        /*Console.WriteLine(buffer.m_IBuffer.Length);
                        Console.WriteLine("{0,6} {1,6}", BitConverter.ToUInt16(buffer.m_IBuffer, 0), BitConverter.ToUInt16(buffer.m_IBuffer, 2));
                        Console.WriteLine(BitConverter.ToUInt32(buffer.m_IBuffer, 4));*/
                    }
                    else
                    {
                        //Console.Write(".");
                        System.Threading.Thread.Sleep(16);
                    }
                }

                Ndisapi.CloseFilterDriver(driverPtr);
                Console.ReadLine();
            
        }

        private static void writeToConsole(INTERMEDIATE_BUFFER packetBuffer)
        {
            var packet = PacketDotNet.Packet.ParsePacket(PacketDotNet.LinkLayers.Ethernet, packetBuffer.m_IBuffer);
            PacketDotNet.TcpPacket tcpPacket = (PacketDotNet.TcpPacket)packet.Extract(typeof(PacketDotNet.TcpPacket));
            PacketDotNet.IpPacket ipPacket = (PacketDotNet.IpPacket)packet.Extract(typeof(PacketDotNet.IpPacket));
            if (tcpPacket != null && ipPacket != null)
            {
                //DateTime time = e.Packet.Timeval.Date;
                //int len = e.Packet.Data.Length;
                var srcIp = ipPacket.SourceAddress.ToString();
                var dstIp = ipPacket.DestinationAddress.ToString();
                var srcPort = tcpPacket.SourcePort.ToString();
                var dstPort = tcpPacket.DestinationPort.ToString();
                var data = tcpPacket.PayloadData;
                Console.WriteLine("{0,15} {1,15} {2,6} {3,6} {4}",srcIp,dstIp,srcPort,dstPort,data.Length);
            }
        }


        private static void WriteToConsole(INTERMEDIATE_BUFFER packetBuffer, IntPtr packetBufferPtr)
        {
            //Console.WriteLine(packetBuffer.m_dwDeviceFlags == Ndisapi.PACKET_FLAG_ON_SEND ? "\nMSTCP --> Interface" : "\nInterface --> MSTCP");
            //Console.WriteLine("Packet size = {0}", packetBuffer.m_Length);
            var ethernetHeaderPtr = IntPtr.Add(packetBufferPtr,Marshal.OffsetOf(typeof(INTERMEDIATE_BUFFER), "m_IBuffer").ToInt32());
            var ethernetHeader = Marshal.PtrToStructure<ETHER_HEADER>(ethernetHeaderPtr);
            /*Console.WriteLine(
                "\tETHERNET {0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2} --> {6:X2}{7:X2}{8:X2}{9:X2}{10:X2}{11:X2}",
                ethernetHeader.source.b1,
                ethernetHeader.source.b2,
                ethernetHeader.source.b3,
                ethernetHeader.source.b4,
                ethernetHeader.source.b5,
                ethernetHeader.source.b6,
                ethernetHeader.dest.b1,
                ethernetHeader.dest.b2,
                ethernetHeader.dest.b3,
                ethernetHeader.dest.b4,
                ethernetHeader.dest.b5,
                ethernetHeader.dest.b6
                );*/

            switch (ntohs(ethernetHeader.proto))
            {
                case ETHER_HEADER.ETH_P_IP:
                    {
                        var ipHeaderPtr = IntPtr.Add(ethernetHeaderPtr,Marshal.SizeOf(typeof(ETHER_HEADER)));
                        var ipHeader = Marshal.PtrToStructure<IPHeader>(ipHeaderPtr);

                        var sourceAddress = new IPAddress(ipHeader.Src);
                        var destinationAddress = new IPAddress(ipHeader.Dest);

                        //Console.WriteLine("\tIP {0} --> {1} PROTOCOL: {2}", sourceAddress, destinationAddress, ipHeader.P);
                        if(ipHeader.P == IPHeader.IPPROTO_TCP)
                        {
                            var tcpHeader = Marshal.PtrToStructure<TcpHeader>(IntPtr.Add(ipHeaderPtr,((ipHeader.IPLenVer) & 0xF) * 4));
                            //Console.WriteLine("\tTCP SRC PORT: {0} DST PORT: {1}", ntohs(tcpHeader.th_sport), ntohs(tcpHeader.th_dport));
                            Console.WriteLine("{0,6} {1,6}", packetBuffer.m_IBuffer.Length, ipHeader.Len);
                            //Console.WriteLine("{0,6} {1,6}", tcpHeader.size, (Marshal.SizeOf(typeof(ETHER_HEADER)) + ((ipHeader.IPLenVer) & 0xF) * 4)/8 + tcpHeader.size);
                        }
                        if(ipHeader.P == IPHeader.IPPROTO_UDP)
                        {
                            var udpHeader = Marshal.PtrToStructure<UdpHeader>(IntPtr.Add(ipHeaderPtr,((ipHeader.IPLenVer) & 0xF) * 4));
                            //Console.WriteLine("\tUDP SRC PORT: {0} DST PORT: {1}", ntohs(udpHeader.th_sport), ntohs(udpHeader.th_dport));
                        }
                        

                        
                            
                    }
                    break;
                case ETHER_HEADER.ETH_P_RARP:
                    Console.WriteLine("\tReverse Addr Res packet");
                    break;
                case ETHER_HEADER.ETH_P_ARP:
                    Console.WriteLine("\tAddress Resolution packet");
                    break;
            }
        }
        
        static ushort ntohs(ushort netshort)
        {
            var hostshort = (ushort)(((netshort >> 8) & 0x00FF) | ((netshort << 8) & 0xFF00));
            return hostshort;
        }
        
    }
}

/*
 * Пишем снифер который ловит всё подрят
 * Дописываем фильтр, который будет ловить только 1 порт
 * Дописываем фильтр, который будет ловить только 1 ip
 * Может пропустить порт
*/