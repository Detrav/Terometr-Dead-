using NdisApiWrapper;
using PacketDotNet;
using Detrav.Sniffer.Tera;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Detrav.Sniffer
{
    public class Capture
    {
        //Драйвер на перехват
        
        private IntPtr driverPtr = Ndisapi.OpenFilterDriver();
        private TCP_AdapterList adapters = new TCP_AdapterList();
        public bool ready { get; private set; }
        private Thread threadLookingForPacket;
        private INTERMEDIATE_BUFFER[] buffers;
        private IntPtr[] bufferPtrs;
        private ETH_REQUEST[] requests;
        private uint requestCount;
        //Для обработки
        private string[] servers;
        private Dictionary<Connection, TcpClient> tcpClients;
        private Dictionary<Connection, Client> clients;
        public delegate void OnParsePacket(Connection connection, TeraPacket packet);
        public event OnParsePacket onParsePacket;
        private Thread threadParsePacket;
        bool needToStop = false;
        //Loger
        public bool flagToDebug
        {
            get { return _flagToDebug; }
            set
            {
                lock (tcpClients)
                {
                    foreach (var con in tcpClients)
                    {
                        con.Value.flagToDebug = value;
                    }
                }
                _flagToDebug = value;
            }
        }
        private bool _flagToDebug = false;
        public bool flagToPacketLog = false;
        public bool flagToSnifferLog = false;
        TextWriter packetLogWriter;
        TextWriter snifferLogWriter;

        public Capture(string[] serversIp)
        {
            GCHandle.Alloc(adapters);
            if ((Ndisapi.IsDriverLoaded(driverPtr)))
            {
                ready = Ndisapi.GetTcpipBoundAdaptersInfo(driverPtr, ref adapters);
            }
            this.servers = null;
            if(serversIp != null)
                this.servers = serversIp.Clone() as string[];
            threadLookingForPacket = new Thread(lookingForPacket);
            threadParsePacket = new Thread(parsePacket);
            tcpClients = new Dictionary<Connection, TcpClient>();
            clients = new Dictionary<Connection, Client>();
        }
        /// <summary>
        /// 
        /// </summary>
        public Capture(string str) : this(new string[1] { str }) { }

        public string[] getDevices()
        {
            if (ready)
            {
                string[] result = new string[adapters.m_nAdapterCount];
                for (int i = 0; i < adapters.m_nAdapterCount; i++)
                {
                    result[i] = adapters.GetName(i);
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Устарелый, используйте без параметров
        /// </summary>
        /// <param name="num"></param>
        public void start(int num)
        {
            //Для совместимости
            start();
        }

        public void start()
        {
            if (ready)
            {
                ADAPTER_MODE mode = new ADAPTER_MODE
                {
                    dwFlags = Ndisapi.MSTCP_FLAG_SENT_LISTEN | Ndisapi.MSTCP_FLAG_RECV_LISTEN,
                    hAdapterHandle = adapters.m_nAdapterHandle[num]
                };
                Ndisapi.SetAdapterMode(driverPtr, ref mode);
                IP_ADDRESS_V4[] serversIp = new IP_ADDRESS_V4[servers.Length];
                for(int i =0;i<=servers.Length;i++)
                    serversIp[i] = new IP_ADDRESS_V4()
                {
                    m_AddressType = Ndisapi.IP_SUBNET_V4_TYPE,
                    m_IpSubnet = new IP_SUBNET_V4
                    {
                        m_Ip = BitConverter.ToUInt32(IPAddress.Parse(servers[i]).GetAddressBytes(), 0),
                        m_IpMask = 0xFFFFFFFF
                    }
                };
                //Filters
                STATIC_FILTER_TABLE filtersTable = new STATIC_FILTER_TABLE();
                filtersTable.m_StaticFilters = new STATIC_FILTER[256];
                filtersTable.m_TableSize = (uint)(3*servers.Length);
                for (int i = 0; i < filtersTable.m_TableSize; i += 3)
                {
                    filtersTable.m_StaticFilters[i].m_Adapter = 0; // applied to all adapters
                    filtersTable.m_StaticFilters[i].m_ValidFields = Ndisapi.NETWORK_LAYER_VALID;
                    filtersTable.m_StaticFilters[i].m_FilterAction = Ndisapi.FILTER_PACKET_REDIRECT;
                    filtersTable.m_StaticFilters[i].m_dwDirectionFlags = Ndisapi.PACKET_FLAG_ON_SEND;
                    filtersTable.m_StaticFilters[i].m_NetworkFilter.m_dwUnionSelector = Ndisapi.IPV4;
                    filtersTable.m_StaticFilters[i].m_NetworkFilter.m_IPv4.m_ValidFields = Ndisapi.IP_V4_FILTER_DEST_ADDRESS;
                    filtersTable.m_StaticFilters[i].m_NetworkFilter.m_IPv4.m_DestAddress = serversIp[i / 3];
                    filtersTable.m_StaticFilters[i + 1].m_Adapter = 0; // applied to all adapters
                    filtersTable.m_StaticFilters[i + 1].m_ValidFields = Ndisapi.NETWORK_LAYER_VALID;
                    filtersTable.m_StaticFilters[i + 1].m_FilterAction = Ndisapi.FILTER_PACKET_REDIRECT;
                    filtersTable.m_StaticFilters[i + 1].m_dwDirectionFlags = Ndisapi.PACKET_FLAG_ON_RECEIVE;
                    filtersTable.m_StaticFilters[i + 1].m_NetworkFilter.m_dwUnionSelector = Ndisapi.IPV4;
                    filtersTable.m_StaticFilters[i + 1].m_NetworkFilter.m_IPv4.m_ValidFields = Ndisapi.IP_V4_FILTER_SRC_ADDRESS;
                    filtersTable.m_StaticFilters[i + 1].m_NetworkFilter.m_IPv4.m_SrcAddress = serversIp[i / 3];
                    filtersTable.m_StaticFilters[i + 2].m_Adapter = 0; // applied to all adapters
                    filtersTable.m_StaticFilters[i + 2].m_ValidFields = 0;
                    filtersTable.m_StaticFilters[i + 2].m_FilterAction = Ndisapi.FILTER_PACKET_PASS;
                    filtersTable.m_StaticFilters[i + 2].m_dwDirectionFlags = Ndisapi.PACKET_FLAG_ON_RECEIVE | Ndisapi.PACKET_FLAG_ON_SEND;
                }
                Ndisapi.SetPacketFilterTable(driverPtr, ref filtersTable);

                // Allocate and initialize packet structures
                requestCount = adapters.m_nAdapterCount;
                buffers = new INTERMEDIATE_BUFFER[requestCount];
                bufferPtrs = new IntPtr[requestCount];
                requests = new ETH_REQUEST[requestCount];
                for (int i = 0; i < requestCount; i++)
                {
                    buffers[i] = new INTERMEDIATE_BUFFER();
                    Win32Api.ZeroMemory(bufferPtrs[i], Marshal.SizeOf(buffers[i]));
                    requests[i] = new ETH_REQUEST
                    {
                        hAdapterHandle = adapters.m_nAdapterHandle[i],
                        EthPacket = { Buffer = bufferPtrs[i] }
                    };
                }
                threadLookingForPacket.Start();
                threadParsePacket.Start();
                ready = false;
            }
        }

        public void stop()
        {
            needToStop = true;
            threadLookingForPacket.Join(1000);
            threadParsePacket.Join(1000);
        }

        ~Capture()
        {
            stop();
        }

        void lookingForPacket()
        {
            while (true)
            {
                if (needToStop)
                {
                    snifferLog("Запуск снифера");
                    for (int i = 0; i < requestCount; i++)
                        Marshal.FreeHGlobal(bufferPtrs[i]);
                    Ndisapi.CloseFilterDriver(driverPtr);
                    if (snifferLogWriter != null) snifferLogWriter.Close();
                    return;
                }
                bool sleep = true;
                for (int i = 0; i < requestCount; i++)
                    if (Ndisapi.ReadPacket(driverPtr, ref requests[i]))
                    {
                        buffers[i] = (INTERMEDIATE_BUFFER)Marshal.PtrToStructure(bufferPtrs[i], typeof(INTERMEDIATE_BUFFER));
                        captureDevice_OnPacketArrival(buffers[i]);
                        sleep = false;
                    }
                if (sleep) System.Threading.Thread.Sleep(16);
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
            if (tcpPacket == null)
                return;
            Connection connection = new Connection(tcpPacket);
            TcpClient tcpClient; bool connected = false;
            lock (tcpClients)
            {
                connected = tcpClients.TryGetValue(connection, out tcpClient);
            }
            //Проводим проверку второго из трёх сообщений для соединения, если такой есть то создаём новый клиент
            if (tcpPacket.Syn && tcpPacket.Ack && 0 == tcpPacket.PayloadData.Length && !connected)
            {
                tcpClient = new TcpClient(_flagToDebug);
                tcpClients.Add(connection, tcpClient);
                lock (clients)
                {
                    clients.Add(connection, tcpClient.teraClient);
                }
                connected = true;
                snifferLog("Новое соединение: " + connection.ToString());
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
                snifferLog("Конец соединения: " + connection.ToString());
            }
        }

        private void parsePacket()
        {
            TeraPacket packet;
            while(true)
            {
                if (needToStop)
                {
                    if (packetLogWriter != null) packetLogWriter.Close();
                    return;
                }
                lock (clients)
                {
                    foreach (var client in clients)
                    {
                        while ((packet = client.Value.parsePacket()) != null)
                        {
                            packetLog(packet);
                            onParsePacket(client.Key, packet);
                        }
                    }
                }
                Thread.Sleep(16);
            }
        }

        void packetLog(TeraPacket p)
        {
            if (flagToPacketLog)
            {
                //Создать директорию и файл
                if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
                if (!Directory.Exists("logs/packets")) Directory.CreateDirectory("logs/packets");
                if (packetLogWriter == null)
                    packetLogWriter = new StreamWriter(String.Format("logs/packets/Tera_{0}.packet", DateTime.Now.ToString("MMM_dd_HH_mm_ss")));
                packetLogWriter.WriteLine("{0} {1}", DateTime.Now.ToString("HH:mm:ss"), p.ToString());
                packetLogWriter.Flush();
            }
        }

        void snifferLog(string str)
        {
            if(flagToSnifferLog)
            {
                if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
                if (!Directory.Exists("logs/sniffer")) Directory.CreateDirectory("logs/sniffer");
                if(snifferLogWriter == null)
                    snifferLogWriter = new StreamWriter(String.Format("logs/sniffer/WinPKFilter_{0}.log", DateTime.Now.ToString("MMM_dd_HH_mm_ss")));
                snifferLogWriter.WriteLine("{0} {1}", DateTime.Now.ToString("HH:mm:ss"), str);
                snifferLogWriter.Flush();
            }
        }
    }
}
