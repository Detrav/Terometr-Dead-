using Crypt;
using PacketDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer
{
    internal class Client
    {
        public string port { get; private set; }
        public string serverIp { get; private set; }
        private int state;
        private Session session;

        private byte[] recvStream;
        private byte[] sendStream;

        private Queue<TeraPacket> teraPackets;

        private static byte[] initPacket = new byte[4] { 0x01, 0x00, 0x00, 0x00 };

        public Client(string dstPort, string serverIp)
        {
            // TODO: Complete member initialization
            this.port = dstPort;
            this.serverIp = serverIp;
            teraPackets = new Queue<TeraPacket>();
            reStart();
        }


        internal void reStart()
        {
            state = 0;
            session = new Session();
            recvStream = new byte[0];
            sendStream = new byte[0];
            lock (teraPackets)
            {
                teraPackets.Clear();
            }
        }

        internal void recv(byte[] data)
        {
            switch (state)
            {
                case 0:
                    if (data.Length != 128)
                        return;
                    session.ServerKey1 = (byte[])data.Clone();;
                    state++;
                    return;
                case 1:
                    if (data.Length != 128)
                        return;
                    session.ServerKey2 = (byte[])data.Clone();
                    session.Init();
                    state++;
                    return;
                default:
                    session.Encrypt(ref data);
                    Array.Resize(ref recvStream, recvStream.Length + data.Length);
                    Array.Copy(data, 0, recvStream, recvStream.Length - data.Length, data.Length);
                    while (processRecv()) ;
                    return;
            }
        }

        private bool processRecv()
        {
            if (recvStream.Length < 4)
                return false;
            ushort length = BitConverter.ToUInt16(recvStream, 0);
            if (recvStream.Length < length)
                return false;
            var packet = new TeraPacket(getRecvData(length), TeraPacket.Type.Recv);
            lock (teraPackets)
            {
                teraPackets.Enqueue(packet);
            }
            return true;
        }

        private byte[] getRecvData(ushort length)
        {
            byte[] result = new byte[length];
            Array.Copy(recvStream, result, length);
            byte[] reserve = (byte[])recvStream.Clone();
            recvStream = new byte[recvStream.Length - length];
            Array.Copy(reserve, length, recvStream, 0, recvStream.Length);
            return result;
        }



        internal void send(byte[] data)
        {
            switch (state)
            {
                case 0:
                    if (data.Length != 128)
                        return;
                    session.ClientKey1 = (byte[])data.Clone();
                    return;
                case 1:
                    if (data.Length != 128)
                        return;
                    session.ClientKey2 = (byte[])data.Clone();
                    return;
                default:
                    session.Decrypt(ref data);
                    Array.Resize(ref sendStream, sendStream.Length + data.Length);
                    Array.Copy(data, 0, sendStream, sendStream.Length - data.Length, data.Length);
                    while (processSend()) ;
                    return;
            }
        }

        private bool processSend()
        {
            if (sendStream.Length < 4)
                return false;
            ushort length = BitConverter.ToUInt16(sendStream, 0);
            if (sendStream.Length < length)
                return false;
            var packet = new TeraPacket(getSendData(length), TeraPacket.Type.Send);
            lock (teraPackets)
            {
                teraPackets.Enqueue(packet);
            }
            return true;
        }

        private byte[] getSendData(ushort length)
        {
            byte[] result = new byte[length];
            Array.Copy(sendStream, result, length);
            byte[] reserve = (byte[])sendStream.Clone();
            sendStream = new byte[sendStream.Length - length];
            Array.Copy(reserve, length, sendStream, 0, sendStream.Length);
            return result;
        }


        uint seq_client;
        uint seq_server;
        public System.IO.TextWriter tw;
        internal TeraPacket parsePacket()
        {
            if (teraPackets.Count == 0)
                return null;
            lock (teraPackets)
            {
                return teraPackets.Dequeue();
            }
        }
    }
}