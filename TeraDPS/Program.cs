using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TeraDPS
{
    class Program
    {
        public static string[] names = loadXmlConfig();
        public class counts
        {
            public uint s = 0; public uint r = 0;
        }
        public static counts count = new counts();
        static void Main(string[] args)
        {
            Sniffer.Capture sniffer = new Sniffer.Capture("91.225.237.8");
            if (sniffer.ready)
            {
                string[] devices = sniffer.getDevices();
                for (int i = 0; i < devices.Count(); i++)
                {
                    Console.WriteLine("{0}) {1}", i + 1, devices[i]);
                }
                int num = int.Parse(Console.ReadLine()) - 1;
                sniffer.onParsePacket += sniffer_onParsePacket;
                sniffer.start(num);
                Console.ReadLine();
                sniffer.stop();
            }
        }

        static void sniffer_onParsePacket(string port, string ip, Sniffer.TeraPacket packet)
        {
            switch(packet.type)
            { 
                case Sniffer.TeraPacket.Type.Recv:
                    count.r++;
                    Console.WriteLine("{0,15} {1,6} {2,6} {3,6}", count.r, "recv", packet.size, names[packet.opCode]);
                    break;
                case Sniffer.TeraPacket.Type.Send:
                    count.s++;
                    Console.WriteLine("{0,15} {1,6} {2,6} {3,6}", count.s, "send", packet.size, names[packet.opCode]);
                    break;
            }
        }


        static void saveXmlConfig()
        {
            string[] test = new string[] { "Test", "test2", "for get" };
            //test.Add(2, "test"); test.Add(4, "tes312t"); test.Add(7, "te213st");
            TextWriter writer = new StreamWriter("config.txt");
            //TextReader tr = new StreamReader("tr");
            XmlSerializer ser = new XmlSerializer(typeof(string[]));
            ser.Serialize(writer, test);
            writer.Close();
        }

        static string[] loadXmlConfig()
        {
            string[] result;
            TextReader reader = new StreamReader("config.xml");
            XmlSerializer ser = new XmlSerializer(typeof(string[]));
            result = (string[])ser.Deserialize(reader);
            reader.Close();
            return result;
        }
    }
}
