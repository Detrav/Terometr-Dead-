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
                Console.WriteLine("{0,15} {1,15} {2,6} {3,6} {4}", srcIp, dstIp, srcPort, dstPort, data.Length);
            }
        }
        
    }
}

/*
 * Пишем снифер который ловит всё подрят
 * Дописываем фильтр, который будет ловить только 1 порт
 * Дописываем фильтр, который будет ловить только 1 ip
 * Может пропустить порт
*/