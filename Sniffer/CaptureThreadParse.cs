using Detrav.Sniffer.Tera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Detrav.Sniffer
{
    public partial class Capture
    {
        private void parsePacket()
        {
            TeraPacket packet;
            while (true)
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
                            //onParsePacket(client.Key, packet);
                        }
                    }
                    if(existToDelete)
                    {
                        var itemsToRemove = clients.Where(f => f.Value.delete).ToArray();
                        foreach (var item in itemsToRemove)
                            clients.Remove(item.Key);
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
    }
}
