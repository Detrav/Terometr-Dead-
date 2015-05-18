using NdisApiWrapper;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Detrav.Sniffer
{
    public partial class Capture
    {
        void lookingForPacket()
        {
            while (true)
            {
                if (needToStop)
                {
                    snifferLog("Запуск снифера");
                    Marshal.FreeHGlobal(bufferPtr);
                    Ndisapi.CloseFilterDriver(driverPtr);
                    if (snifferLogWriter != null) snifferLogWriter.Close();
                    return;
                }
                if (Ndisapi.ReadPacket(driverPtr, ref request))
                {
                    buffer = (INTERMEDIATE_BUFFER)Marshal.PtrToStructure(bufferPtr, typeof(INTERMEDIATE_BUFFER));
                    captureDevice_OnPacketArrival(buffer);
                }
                else System.Threading.Thread.Sleep(16);
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
                tcpClient = new TcpClient(flagToDebug);
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
                    clients[connection].delete = true;
                    existToDelete = true;
                }
                snifferLog("Конец соединения: " + connection.ToString());
            }
        }

        void snifferLog(string str)
        {
            if (flagToSnifferLog)
            {
                if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
                if (!Directory.Exists("logs/sniffer")) Directory.CreateDirectory("logs/sniffer");
                if (snifferLogWriter == null)
                    snifferLogWriter = new StreamWriter(String.Format("logs/sniffer/WinPKFilter_{0}.log", DateTime.Now.ToString("MMM_dd_HH_mm_ss")));
                snifferLogWriter.WriteLine("{0} {1}", DateTime.Now.ToString("HH:mm:ss"), str);
                snifferLogWriter.Flush();
            }
        }
    }
}
