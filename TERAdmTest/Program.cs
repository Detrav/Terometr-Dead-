using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TERAdmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            CaptureDeviceList deviceList = CaptureDeviceList.Instance;
            for (int i = 0; i < deviceList.Count; i++)
            {
                Console.WriteLine("{0}) {1}", i + 1, deviceList[i].Description);
            }
            try
            {
                int index = int.Parse(Console.ReadLine());
                Capture capture = new Capture(deviceList[index - 1]);
                capture.Init("91.225.237.8");
                capture.onParsePacket += capture_onParsePacket;
                capture.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

        }

        static void capture_onParsePacket(TeraPacket packet)
        {
            switch(packet.opCode)
            {
                case TeraPacket.S_EACH_SKILL_RESULT:
                    Console.WriteLine("S_EACH_SKILL_RESULT");
                    break;
                case TeraPacket.S_SPAWN_NPC:
                    Console.WriteLine("S_SPAWN_NPC");
                    break;
                case TeraPacket.S_SPAWN_USER:
                    Console.WriteLine("S_SPAWN_USER");
                    break;
            }
        }

        static void captureDevice_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            
        }
    }
}
