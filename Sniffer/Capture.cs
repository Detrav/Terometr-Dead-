using NdisApiWrapper;
using PacketDotNet;
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
        private Dictionary<Connection, TcpClient> tcpClients;
        private Dictionary<Connection, Client> clients;
        public delegate void OnParsePacket(Connection connection, TeraPacket packet);
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
            threadLookingForPacket = new Thread(lookingForPacket);
            threadParsePacket = new Thread(parsePacket);
            tcpClients = new Dictionary<Connection, TcpClient>();
            clients = new Dictionary<Connection, Client>();
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
        void captureDevice_OnPacketArrival(INTERMEDIATE_BUFFER packetBuffer)
        {
            //Не люблю вары, пока оставлю так, потом переделаю
            var packet = EthernetPacket.ParsePacket(LinkLayers.Ethernet, packetBuffer.m_IBuffer);
            var ethPacket = packet as EthernetPacket;
            //Автор http://www.theforce.dk/hearthstone/ Описывает как откидывания не нужных пакетов
            if (packet.PayloadPacket == null || ethPacket.Type != EthernetPacketType.IpV4)
                return;
            IPv4Packet ipv4Packet = ethPacket.PayloadPacket as IPv4Packet;
            //А тут видимо отбрасываем если нет содержимого (нужного нам TCP) пакета
            if (ipv4Packet.PayloadPacket == null)
                return;
            TcpPacket tcpPacket = ipv4Packet.PayloadPacket as TcpPacket;

            Connection connection = new Connection(tcpPacket);
            TcpClient tcpClient; bool connected = false;
            lock (tcpClients)
            {
                connected = tcpClients.TryGetValue(connection, out tcpClient);
            }
            //Проводим проверку второго из трёх сообщений для соединения, если такой есть то создаём новый клиент
            if (tcpPacket.Syn && tcpPacket.Ack && 0 == tcpPacket.PayloadData.Length && !connected)
            {
                tcpClient = new TcpClient();
                tcpClients.Add(connection, tcpClient);
                lock (clients)
                {
                    clients.Add(connection, tcpClient.teraClient);
                }
                connected = true;
            }

            if (tcpPacket.Ack && connected)
            {
                tcpClient.reConstruction(tcpPacket);
            }

            if (tcpPacket.Fin && tcpPacket.Ack && connected)
            {
                tcpClients.Remove(connection);
                lock (clients)
                {
                    clients.Remove(connection);
                }
            }
        }

        private void parsePacket()
        {
            TeraPacket packet;
            while(true)
            {
                if (needToStop) return;
                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        while ((packet = client.Value.parsePacket()) != null)
                        {
                            onParsePacket(client.Key, packet);
                        }
                    }
                }
                Thread.Sleep(16);
            }
        }
    }
}
