using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SnifferGUI
{
    /*
     * Сделаю синглтон для конфига с отложеной иницилизацией
     */
    internal class Config
    {
        private static Config instance = null;
        private Config()
        {
            loadConfig();
        }

        internal static Config Instance
        {
            get
            {
                if (instance == null)
                    instance = new Config();
                return instance;
            }
        }
        internal string serverIp = "";
        internal int adapterNumber = 0;
        internal int packetMaxCount = 1000;
        internal bool whiteListEnable = false;
        internal bool blackListEnable = false;
        internal ushort[] whiteList = new ushort[0];
        internal ushort[] blackList = new ushort[0];
        //Логеры для снифера
        internal bool flagToDebug = false;
        internal bool flagToPacketLog = false;
        internal bool flagToSnifferLog = false;

        internal static void saveConfig()
        {
            using (XmlWriter xw = XmlWriter.Create(Path.Combine("assets", "config.xml")))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("Config");
                xw.WriteElementString("serverIp", Instance.serverIp);
                xw.WriteElementString("adapterNumber", Instance.adapterNumber.ToString());
                xw.WriteElementString("packetMaxCount", Instance.packetMaxCount.ToString());
                xw.WriteElementString("whiteListEnable", Instance.whiteListEnable.ToString());
                xw.WriteElementString("blackListEnable", Instance.blackListEnable.ToString());
                if (Instance.whiteList != null)
                {
                    xw.WriteStartElement("whiteList");
                    foreach (var el in Instance.whiteList)
                        xw.WriteElementString("whiteListElement", el.ToString());
                    xw.WriteEndElement();
                }
                if (Instance.blackList != null)
                {
                    xw.WriteStartElement("blackList");
                    foreach (var el in Instance.blackList)
                        xw.WriteElementString("blackListElement", el.ToString());
                    xw.WriteEndElement();
                }
                //Логер
                xw.WriteElementString("flagToDebug", Instance.flagToDebug.ToString());
                xw.WriteElementString("flagToPacketLog", Instance.flagToPacketLog.ToString());
                xw.WriteElementString("flagToSnifferLog", Instance.flagToSnifferLog.ToString());
                //Конец, добавлять сюда
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
        }
        private void loadConfig()
        {
            using (XmlReader xr = XmlReader.Create(Path.Combine("assets", "config.xml")))
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(xr);
                XmlNodeList elemets = xd.GetElementsByTagName("Config");
                if (elemets.Count > 0)
                {
                    foreach (XmlNode el in elemets[0].ChildNodes)
                    {
                        switch (el.Name)
                        {
                            case "serverIp":
                                if (el.InnerText != null)
                                    serverIp = el.InnerText;
                                break;
                            case "adapterNnumber":
                                if (el.InnerText != null)
                                    adapterNumber = Int32.Parse(el.InnerText);
                                break;
                            case "packetMaxCount":
                                if (el.InnerText != null)
                                    packetMaxCount = Int32.Parse(el.InnerText);
                                break;
                            case "whiteListEnable":
                                if (el.InnerText != null)
                                    whiteListEnable = Boolean.Parse(el.InnerText);
                                break;
                            case "blackListEnable":
                                if (el.InnerText != null)
                                    blackListEnable = Boolean.Parse(el.InnerText);
                                break;
                            case "whiteList":
                                List<ushort> wl = new List<ushort>();
                                foreach (XmlNode wlel in el.ChildNodes)
                                    if (wlel.Name == "whiteListElement")
                                        wl.Add(UInt16.Parse(wlel.InnerText));
                                whiteList = wl.ToArray();
                                break;
                            case "blackList":
                                List<ushort> bl = new List<ushort>();
                                foreach (XmlNode blel in el.ChildNodes)
                                    if (blel.Name == "blackListElement")
                                        bl.Add(UInt16.Parse(blel.InnerText));
                                blackList = bl.ToArray();
                                break;
                            case "flagToDebug":
                                if (el.InnerText != null)
                                    flagToDebug = Boolean.Parse(el.InnerText);
                                break;
                            case "flagToPacketLog":
                                if (el.InnerText != null)
                                    flagToPacketLog = Boolean.Parse(el.InnerText);
                                break;
                            case "flagToSnifferLog":
                                if (el.InnerText != null)
                                    flagToSnifferLog = Boolean.Parse(el.InnerText);
                                break;
                        }
                    }
                }
            }
        }

        public static string getPacketName(ushort opCode)
        {
            return ((Sniffer.Tera.OpCode2805)opCode).ToString();
        }
        public static ushort[] getArrayOfPacketsName(string[] names)
        {
            ushort[] result = new ushort[names.Length];
            for (int i = 0; i < names.Length;i++ )
                result [i] = (ushort)Enum.Parse(typeof(Sniffer.Tera.OpCode2805), names[i]);
            return result;
        }
        public static bool isPacketName(string name)
        {
            Sniffer.Tera.OpCode2805 opcodes;
            return Enum.TryParse<Sniffer.Tera.OpCode2805>(name, out opcodes);
        }
        public static bool isPacket(ushort opCode)
        {
            return Enum.IsDefined(typeof(Sniffer.Tera.OpCode2805), opCode);
        }

       
    }
}