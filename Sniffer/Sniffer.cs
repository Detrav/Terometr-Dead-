using SharpPcap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sniffer
{
    public class Sniffer
    {
        private ICaptureDevice captureDevice { get; set; }
        private string serverIp;
        private static byte[] initPacket = new byte[4] { 0x01, 0x00, 0x00, 0x00 };
        private Dictionary<string, Client> clients;
        public delegate void OnParsePacket(string port,string ip, Packet packet);
        public event OnParsePacket onParsePacket;
        Thread thread; bool needToStop = false;

        public Sniffer(string device, string serverIp)
        {
            CaptureDeviceList deviceList = CaptureDeviceList.Instance;
            captureDevice = null;
            foreach(var d in deviceList)
            {
                if (d.Description == device)
                {
                    captureDevice = d;
                    break;
                }
            }
            if (captureDevice == null) throw new NullReferenceException("Не существует такого устройства!");
            captureDevice.OnPacketArrival += captureDevice_OnPacketArrival;
            captureDevice.Open(DeviceMode.Promiscuous, 0);
            this.serverIp = serverIp;
            clients = new Dictionary<string, Client>();
            thread = new Thread(parsePacket);
        }

        static public string[] getDevices()
        {
            CaptureDeviceList deviceList = CaptureDeviceList.Instance;
            List<string> devices = new List<string>();
            foreach(var d in deviceList)
            {
                devices.Add(d.Description);
            }
            return devices.ToArray();
        }

        public void start()
        {
            captureDevice.Filter += "host " + serverIp;
            thread.Start();
            captureDevice.Capture();
        }

        public void stop()
        {
            captureDevice.StopCapture();
            needToStop = true;
        }

         ~Sniffer()
        {
            stop();
        }

        void captureDevice_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            PacketDotNet.Packet packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            PacketDotNet.TcpPacket tcpPacket = (PacketDotNet.TcpPacket)packet.Extract(typeof(PacketDotNet.TcpPacket));
            PacketDotNet.IpPacket ipPacket = (PacketDotNet.IpPacket)packet.Extract(typeof(PacketDotNet.IpPacket));
            if (tcpPacket != null && ipPacket != null)
            {
                DateTime time = e.Packet.Timeval.Date;
                int len = e.Packet.Data.Length;
                var srcIp = ipPacket.SourceAddress.ToString();
                var dstIp = ipPacket.DestinationAddress.ToString();
                var srcPort = tcpPacket.SourcePort.ToString();
                var dstPort = tcpPacket.DestinationPort.ToString();
                var data = tcpPacket.PayloadData;

                if(srcIp == serverIp) // Клиент <- Сервер
                {
                    Client c;
                    if (StructuralComparisons.StructuralEqualityComparer.Equals(initPacket, data))
                    {
                        if (clients.TryGetValue(dstPort,out c))
                        {
                            c.reStart();
                        }
                        else
                        {
                            c = new Client(dstPort, serverIp);
                            clients.Add(dstPort, c);
                        }
                    }
                    if (clients.TryGetValue(dstPort, out c))
                    {
                        c.recv((byte[])data.Clone());
                    }

                }
                else if(dstIp == serverIp) // Клиент -> Сервер
                {
                    Client c;
                    if (clients.TryGetValue(srcPort, out c))
                    {
                        c.send((byte[])data.Clone());  
                    }
                }

            }
        }

        private void parsePacket()
        {
            Packet packet;
            while(true)
            {
                if (needToStop) return;
                foreach(var client in clients)
                {
                    while((packet = client.Value.parsePacket())!=null)
                    {
                        onParsePacket(client.Value.dstPort, client.Value.serverIp, packet);
                    }
                }
                Thread.Sleep(16);
            }
        }
    }
}
