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
                for (int i = 0; i < adapters.m_nAdapterCount; i++)
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

            string server = "192.168.1.25";//Ноутбук
            IP_ADDRESS_V4 serverIp = new IP_ADDRESS_V4{
                 m_AddressType = Ndisapi.IP_SUBNET_V4_TYPE,
                 m_IpSubnet = new IP_SUBNET_V4{
                      m_Ip = BitConverter.ToUInt32(IPAddress.Parse(server).GetAddressBytes(), 0),
                      m_IpMask = 0xFFFFFFFF
                 }
            };

            var filtersTable = new STATIC_FILTER_TABLE();
            filtersTable.m_StaticFilters = new STATIC_FILTER[256];

            filtersTable.m_TableSize = 3;
            filtersTable.m_StaticFilters[0].m_Adapter = 0; // applied to all adapters
            filtersTable.m_StaticFilters[0].m_ValidFields = Ndisapi.NETWORK_LAYER_VALID;
            filtersTable.m_StaticFilters[0].m_FilterAction = Ndisapi.FILTER_PACKET_REDIRECT;
            filtersTable.m_StaticFilters[0].m_dwDirectionFlags = Ndisapi.PACKET_FLAG_ON_SEND;
            filtersTable.m_StaticFilters[0].m_NetworkFilter.m_dwUnionSelector = Ndisapi.IPV4;
            filtersTable.m_StaticFilters[0].m_NetworkFilter.m_IPv4.m_ValidFields = Ndisapi.IP_V4_FILTER_DEST_ADDRESS;
            filtersTable.m_StaticFilters[0].m_NetworkFilter.m_IPv4.m_DestAddress = serverIp;
            filtersTable.m_StaticFilters[1].m_Adapter = 0; // applied to all adapters
            filtersTable.m_StaticFilters[1].m_ValidFields = Ndisapi.NETWORK_LAYER_VALID;
            filtersTable.m_StaticFilters[1].m_FilterAction = Ndisapi.FILTER_PACKET_REDIRECT;
            filtersTable.m_StaticFilters[1].m_dwDirectionFlags = Ndisapi.PACKET_FLAG_ON_RECEIVE;
            filtersTable.m_StaticFilters[1].m_NetworkFilter.m_dwUnionSelector = Ndisapi.IPV4;
            filtersTable.m_StaticFilters[1].m_NetworkFilter.m_IPv4.m_ValidFields = Ndisapi.IP_V4_FILTER_SRC_ADDRESS;
            filtersTable.m_StaticFilters[1].m_NetworkFilter.m_IPv4.m_SrcAddress = serverIp;
            filtersTable.m_StaticFilters[2].m_Adapter = 0; // applied to all adapters
            filtersTable.m_StaticFilters[2].m_ValidFields = 0;
            filtersTable.m_StaticFilters[2].m_FilterAction = Ndisapi.FILTER_PACKET_PASS;
            filtersTable.m_StaticFilters[2].m_dwDirectionFlags = Ndisapi.PACKET_FLAG_ON_RECEIVE | Ndisapi.PACKET_FLAG_ON_SEND;

            Ndisapi.SetPacketFilterTable(driverPtr, ref filtersTable);

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

            while (DateTime.Now < end)
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