using NdisApiWrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sniffer
{
    public class Capture
    {
        //Драйвер на перехват
        private IntPtr driverPtr = Ndisapi.OpenFilterDriver();
        private TCP_AdapterList adapters = new TCP_AdapterList();
        public bool ready { get; private set; }
        private Thread threadLookingForPacket;
        private INTERMEDIATE_BUFFER buffer;
        private IntPtr bufferPtr;
        private ETH_REQUEST request;
        //Для обработки
        private string server;
        private Dictionary<string, Client> clients;
        public delegate void OnParsePacket(string port,string ip, TeraPacket packet);
        public event OnParsePacket onParsePacket;
        private Thread threadParsePacket;
        bool needToStop = false;

        public Capture(string serverIp)
        {
            GCHandle.Alloc(adapters);
            if ((Ndisapi.IsDriverLoaded(driverPtr)))
            {
                ready = true;
            }
            this.server = serverIp;
            clients = new Dictionary<string, Client>();
            threadLookingForPacket = new Thread(lookingForPacket);
            threadParsePacket = new Thread(parsePacket);

        }

        public string[] getDevices()
        {
            bool flag = Ndisapi.GetTcpipBoundAdaptersInfo(driverPtr, ref adapters);
            if (!flag) return null;
            string[] result = new string[adapters.m_nAdapterCount];
            for (int i = 0; i < adapters.m_nAdapterCount; i++)
            {
                result[i] = adapters.GetName(i);
            }
            return result;
        }

        public void start(int num)
        {
            if (ready)
            {
                ADAPTER_MODE mode = new ADAPTER_MODE
                {
                    dwFlags = Ndisapi.MSTCP_FLAG_SENT_LISTEN | Ndisapi.MSTCP_FLAG_RECV_LISTEN,
                    hAdapterHandle = adapters.m_nAdapterHandle[num]
                };
                Ndisapi.SetAdapterMode(driverPtr, ref mode);
                IP_ADDRESS_V4 serverIp = new IP_ADDRESS_V4
                {
                    m_AddressType = Ndisapi.IP_SUBNET_V4_TYPE,
                    m_IpSubnet = new IP_SUBNET_V4
                    {
                        m_Ip = BitConverter.ToUInt32(IPAddress.Parse(server).GetAddressBytes(), 0),
                        m_IpMask = 0xFFFFFFFF
                    }
                };
                //Filters
                STATIC_FILTER_TABLE filtersTable = new STATIC_FILTER_TABLE();
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
                buffer = new INTERMEDIATE_BUFFER();
                bufferPtr = Marshal.AllocHGlobal(Marshal.SizeOf(buffer));
                Win32Api.ZeroMemory(bufferPtr, Marshal.SizeOf(buffer));

                request = new ETH_REQUEST
                {
                    hAdapterHandle = adapters.m_nAdapterHandle[num],
                    EthPacket = { Buffer = bufferPtr }
                };

                threadLookingForPacket.Start();
                threadParsePacket.Start();
                ready = false;
            }
        }

        public void stop()
        {
                needToStop = true;
        }

        ~Capture()
        {
            stop();
        }

        void lookingForPacket()
        {
            while(true)
            {
                if(needToStop)
                {
                    Ndisapi.CloseFilterDriver(driverPtr);
                    return;
                }
                if (Ndisapi.ReadPacket(driverPtr, ref request))
                {
                    buffer = (INTERMEDIATE_BUFFER)Marshal.PtrToStructure(bufferPtr, typeof(INTERMEDIATE_BUFFER));
                    captureDevice_OnPacketArrival(buffer);
                }
                else
                {
                    System.Threading.Thread.Sleep(16);
                }
            }
        }
        uint prev = 0;
        System.IO.TextWriter tw = new System.IO.StreamWriter("log");
        void captureDevice_OnPacketArrival(INTERMEDIATE_BUFFER packetBuffer)
        {
            PacketDotNet.Packet packet = PacketDotNet.Packet.ParsePacket(PacketDotNet.LinkLayers.Ethernet, packetBuffer.m_IBuffer);
            PacketDotNet.TcpPacket tcpPacket = (PacketDotNet.TcpPacket)packet.Extract(typeof(PacketDotNet.TcpPacket));
            PacketDotNet.IpPacket ipPacket = (PacketDotNet.IpPacket)packet.Extract(typeof(PacketDotNet.IpPacket));
            if (tcpPacket != null && ipPacket != null)
            {
                var srcIp = ipPacket.SourceAddress.ToString();
                var dstIp = ipPacket.DestinationAddress.ToString();
                var srcPort = tcpPacket.SourcePort.ToString();
                var dstPort = tcpPacket.DestinationPort.ToString();
                var data = tcpPacket.PayloadData;

                tw.WriteLine("{0,5} {1,15} {2,15} {3,5} {4}", srcPort, tcpPacket.SequenceNumber,tcpPacket.AcknowledgmentNumber,tcpPacket.Syn,data.Length);
                tw.Flush();

                if(srcIp == server) // Клиент <- Сервер
                {
                    Client c;
                    if (clients.TryGetValue(dstPort, out c))
                    {
                        c.addPacket(tcpPacket);
                    }
                    else
                    {
                        c = new Client(dstPort, server);
                        clients.Add(dstPort, c);
                        c.tw = tw;
                        c.addPacket(tcpPacket);
                    }
                }
                else if (dstIp == server)
                {
                    Client c;
                    if (clients.TryGetValue(srcPort, out c))
                    {
                        c.addPacket(tcpPacket);
                    }
                    else
                    {
                        c = new Client(srcPort, server);
                        clients.Add(srcPort, c);
                        c.tw = tw;
                        c.addPacket(tcpPacket);
                    }
                }
            }
        }

        private void parsePacket()
        {
            TeraPacket packet;
            while(true)
            {
                if (needToStop) return;
                foreach(var client in clients)
                {
                    while((packet = client.Value.parsePacket())!=null)
                    {
                        onParsePacket(client.Value.port, client.Value.serverIp, packet);
                    }
                }
                Thread.Sleep(16);
            }
        }
    }
}
