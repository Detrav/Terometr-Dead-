using Crypt;
using PacketDotNet;
using SharpPcap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TERAdmTest
{
    class Capture
    {

        public delegate void OnParsePacket(TeraPacket packet);
        public event OnParsePacket onParsePacket;
        ICaptureDevice captureDevice;
        string serverIP;

        public Capture(ICaptureDevice device)
        {
            captureDevice = device;
            captureDevice.OnPacketArrival += captureDevice_OnPacketArrival;
            captureDevice.Open(DeviceMode.Promiscuous, 1000);
        }
        /*String dirName;
        TextWriter recvTW;
        TextWriter sendTW;
        TextWriter tw, twF478;*/
        public void Start()
        {
            captureDevice.Capture();
        }

        public void Init(string ServerIP)
        {
            serverIP = ServerIP;
            /*dirName = DateTime.Now.Ticks.ToString();
            Directory.CreateDirectory(dirName);
            recvTW = new StreamWriter(Path.Combine(dirName, "_recv_RAW.txt"));
            sendTW = new StreamWriter(Path.Combine(dirName, "_send_RAW.txt"));
            tw = new StreamWriter(Path.Combine(dirName, "log.txt"));
            twF478 = new StreamWriter(Path.Combine(dirName, "F478.txt"));*/
            recvStream = new byte[0];
            sendStream = new byte[0];
        }

        public void Stop()
        {
            /*recvTW.Close();
            sendTW.Close();
            tw.Close();
            twF478.Close();*/
            captureDevice.StopCapture();
        }

        static byte[] test = new byte[4] { 0x01, 0x00, 0x00, 0x00 };
        public void captureDevice_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            var tcpPacket = TcpPacket.GetEncapsulated(packet);
            var ipPacket = IpPacket.GetEncapsulated(packet);
            if (tcpPacket != null && ipPacket != null)
            {
                DateTime time = e.Packet.Timeval.Date;
                int len = e.Packet.Data.Length;
                var srcIp = ipPacket.SourceAddress.ToString();
                var dstIp = ipPacket.DestinationAddress.ToString();
                var srcPort = tcpPacket.SourcePort.ToString();
                var dstPort = tcpPacket.DestinationPort.ToString();

                var data2 = tcpPacket.PayloadData;
                
                    if (srcIp == serverIP)
                    {
                        recv((byte[])data2.Clone());
                        //tw.WriteLine("In  {0,15} {1,15} {2,5} {3,5} {4,5} {5}", srcIp, dstIp, srcPort, dstPort, data2.Length, BitConverter.ToString(data2).Replace("-", ""));
                        //recvTW.Flush();
                    }
                    else if (dstIp == serverIP)
                    {
                        if (StructuralComparisons.StructuralEqualityComparer.Equals(test, data2))
                            state = 0;
                        send((byte[])data2.Clone());
                        //tw.WriteLine("Out {0,15} {1,15} {2,5} {3,5} {4,5} {5}", srcIp, dstIp, srcPort, dstPort, data2.Length, BitConverter.ToString(data2).Replace("-", ""));
                        //sendTW.Flush();
                    }

                /*if (srcPort == "7801")
                {
                    
                    /*
                    if (data2.Length > 80)
                        Console.WriteLine("In  {0,15} {1,15} {2,5} {3,5} {4,5} {5}", srcIp, dstIp, srcPort, dstPort, len, BitConverter.ToString(data).Replace("-", ""));
                    tw.WriteLine("In  {0,15} {1,15} {2,5} {3,5} {4,5} {5}", srcIp, dstIp, srcPort, dstPort, len, BitConverter.ToString(data2).Replace("-", ""));
                    tw.Flush();
                    //Console.WriteLine("Out {0:x2}", BitConverter.ToString(data).Replace("-",""));
                }
                if (dstPort == "7801")
                {
                    //if(data.Length>2)
                    //if (data[0] == 0x9D && data[1] == 0x8F)
                    if (data2.Length > 80)
                        Console.WriteLine("Out {0,15} {1,15} {2,5} {3,5} {4,5} {5}", srcIp, dstIp, srcPort, dstPort, len, BitConverter.ToString(data).Replace("-", ""));
                    tw.WriteLine("Out {0,15} {1,15} {2,5} {3,5} {4,5} {5}", srcIp, dstIp, srcPort, dstPort, len, BitConverter.ToString(data2).Replace("-", ""));
                    tw.Flush();
                    //Console.WriteLine("In  {0:x2}", BitConverter.ToString(data).Replace("-", ""));
                }*/

            }
        }

        public int state = 0;
        public Session session = new Session();

        public byte[] recvStream;
        public byte[] sendStream;

        public byte[] recv(byte[] data)
        {
            switch (state)
            {
                case 0:
                    if (data.Length != 128)
                        return data;
                    session.ServerKey1 = Copy(data,128);
                    state++;
                    return data;
                case 1:
                    if (data.Length != 128)
                        return data;
                    session.ServerKey2 = Copy(data, 128);
                    session.Init();
                    state++;
                    return data;
                default:
                    session.Encrypt(ref data);
                    Array.Resize(ref recvStream, recvStream.Length + data.Length);
                    Array.Copy(data, 0, recvStream, recvStream.Length - data.Length, data.Length);
                    while (ProcessRecv()) ;
                    return data;
            }
        }

        public byte[] send(byte[] data)
        {
            switch (state)
            {
                case 0:
                    if (data.Length != 128)
                        return data;
                    session.ClientKey1 = Copy(data,128);
                    return data;
                case 1:
                    if (data.Length != 128)
                        return data;
                    session.ClientKey2 = Copy(data, 128);
                    return data;
                default:
                    session.Decrypt(ref data);
                    Array.Resize(ref sendStream, sendStream.Length + data.Length);
                    Array.Copy(data, 0, sendStream, sendStream.Length - data.Length, data.Length);
                    while (ProcessSend()) ;
                    return data;
            }
        }

        private byte[] Copy(byte[] data2, int p)
        {
            byte[] data = new byte[p];
            Array.Copy(data2, 0, data, 0, p);
            return data;
        }

        public bool ProcessRecv()
        {
            if (recvStream.Length < 4) 
                return false;
            ushort length = BitConverter.ToUInt16(recvStream, 0);
            //Мусорка нужно удалить данные
            //if (length > 5000 || length<4) { recvStream = new byte[0]; return false; }
            if (recvStream.Length < length)
                return false;
            ushort opCode = BitConverter.ToUInt16(recvStream, 2);
            var packet = new TeraPacket(length, opCode, GetRecvData(length), true);
                /*packet.log(tw); packet.log2(twF478);
                recvTW.WriteLine(BitConverter.ToString(packet.data).Replace("-", ""));
                recvTW.Flush();*/
                //workWithDictionary(packet);
            onParsePacket(packet);
            return true;
        }

        private byte[] GetRecvData(int length)
        {
            byte[] result = new byte[length];
            Array.Copy(recvStream, result, length);

            byte[] reserve = (byte[])recvStream.Clone();
            recvStream = new byte[recvStream.Length - length];
            Array.Copy(reserve, length, recvStream, 0, recvStream.Length);

            return result;
        }

        public bool ProcessSend()
        {
            if (sendStream.Length < 4)
                return false;
            ushort length = BitConverter.ToUInt16(sendStream, 0);
            //if (length > 5000 || length < 4) { sendStream = new byte[0]; return false; }
            if (sendStream.Length < length)
                return false;
            ushort opCode = BitConverter.ToUInt16(sendStream, 2);
            var packet = new TeraPacket(length, opCode, GetSendData(length), false);
                /*packet.log(tw); packet.log2(twF478);
                sendTW.WriteLine(BitConverter.ToString(packet.data).Replace("-", ""));
                sendTW.Flush();
                //workWithDictionary(packet);*/
            onParsePacket(packet);
            return true;
        }



        private byte[] GetSendData(int length)
        {
            byte[] result = new byte[length];
            Array.Copy(sendStream, result, length);

            byte[] reserve = (byte[])sendStream.Clone();
            sendStream = new byte[sendStream.Length - length];
            Array.Copy(reserve, length, sendStream, 0, sendStream.Length);

            return result;
        }
        /*
        Dictionary<ushort, int> teraPacketCount = new Dictionary<ushort,int>();
        Dictionary<ushort, int> teraPacketCountOld = new Dictionary<ushort, int>();
        int i = 100;

        private void workWithDictionary(TeraPacket packet)
        {
            if (!teraPacketCount.ContainsKey(packet.opCode)) teraPacketCount[packet.opCode] = 0;
            teraPacketCount[packet.opCode]++;
            i--;
            if (i == 0)
            {
                TextWriter textWriter = new StreamWriter(Path.Combine(
                    dirName,
                    "P" + DateTime.Now.Ticks.ToString() + ".txt"));
                foreach (var key in teraPacketCount.Keys)
                {
                    bool flag = false;
                    try
                    {
                        if (teraPacketCountOld[key] == teraPacketCount[key]) flag = true;
                    }
                    catch { }
                    finally { teraPacketCountOld[key] = teraPacketCount[key]; }
                    textWriter.WriteLine("{0,10} {1,10} {2,5}", key, teraPacketCount[key],flag);
                }
                textWriter.Close();
                i = 100;
            }
        }*/
    }
}