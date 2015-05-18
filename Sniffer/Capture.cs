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
    public partial class Capture : IDisposable
    {
        //Драйвер на перехват
        private IntPtr driverPtr = Ndisapi.OpenFilterDriver();
        private TCP_AdapterList adapters = new TCP_AdapterList();
        private INTERMEDIATE_BUFFER buffer;
        private IntPtr bufferPtr;
        private ETH_REQUEST request;
        public string[] serverIps { get; set; }
        public string[] devices
        {
            get
            {
                string[] strs = new string[adapters.m_nAdapterCount];
                for (int i = 0; i < strs.Length; i++)
                {
                    strs[i] = adapters.GetName(i);
                }
                return strs;
            }
        }
        public bool ready { get; private set; }
        private bool needToStop = false;
        //Поток
        private Thread threadLookingForPacket;
        private Thread threadParsePacket;
        //Для обработки
        private Dictionary<Connection, TcpClient> tcpClients;
        private Dictionary<Connection, Client> clients;
        private bool existToDelete = false;
        //Делегаты//Смотрите CaptureEvents
        //Loger
        private bool flagToDebug = false; 
        private bool flagToPacketLog = false;
        private bool flagToSnifferLog = false;
        TextWriter packetLogWriter;
        TextWriter snifferLogWriter;
        //Деструктор
        private bool disposed = false;

        public Capture()
        {
            GCHandle.Alloc(adapters);
            if ((Ndisapi.IsDriverLoaded(driverPtr)))
            {
                ready = Ndisapi.GetTcpipBoundAdaptersInfo(driverPtr, ref adapters);
            }
            threadLookingForPacket = new Thread(lookingForPacket);
            threadParsePacket = new Thread(parsePacket);
            tcpClients = new Dictionary<Connection, TcpClient>();
            clients = new Dictionary<Connection, Client>();
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
                IP_ADDRESS_V4[] serversIp = new IP_ADDRESS_V4[serverIps.Length];
                for (int i = 0; i <= serverIps.Length; i++)
                    serversIp[i] = new IP_ADDRESS_V4()
                {
                    m_AddressType = Ndisapi.IP_SUBNET_V4_TYPE,
                    m_IpSubnet = new IP_SUBNET_V4
                    {
                        m_Ip = BitConverter.ToUInt32(IPAddress.Parse(serverIps[i]).GetAddressBytes(), 0),
                        m_IpMask = 0xFFFFFFFF
                    }
                };
                //Filters
                STATIC_FILTER_TABLE filtersTable = new STATIC_FILTER_TABLE();
                filtersTable.m_StaticFilters = new STATIC_FILTER[256];
                filtersTable.m_TableSize = (uint)(3 * serverIps.Length);
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

                if (onStartedSniffer != null) onStartedSniffer(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                needToStop = true;
                threadLookingForPacket.Join(1000);
                threadParsePacket.Join(1000);
                if (onStoppedSniffer != null) onStoppedSniffer(this, EventArgs.Empty);
            }
            disposed = true;
        }

        ~Capture()
        {
            Dispose();
        }
    }
}
