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
            loadPacketName();
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
        internal string[] packetName;
        internal bool whiteListEnable = false;
        internal bool blackListEnable = false;
        internal string[] whiteList;
        internal string[] blackList;

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
                        xw.WriteElementString("whiteListElement", el);
                    xw.WriteEndElement();
                }
                if (Instance.blackList != null)
                {
                    xw.WriteStartElement("blackList");
                    foreach (var el in Instance.blackList)
                        xw.WriteElementString("blackListElement", el);
                    xw.WriteEndElement();
                }
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
                if(elemets.Count>0)
                {
                    foreach(XmlNode el in elemets[0].ChildNodes)
                    {
                        switch(el.Name)
                        {
                            case "serverIp":
                                if(el.InnerText!=null)
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
                                List<string> wl = new List<string>();
                                foreach(XmlNode wlel in el.ChildNodes)
                                    if (wlel.Name == "whiteListElement")
                                        wl.Add(wlel.InnerText);
                                whiteList = wl.ToArray();
                                break;
                            case "blackList":
                                List<string> bl = new List<string>();
                                foreach (XmlNode blel in el.ChildNodes)
                                    if (blel.Name == "blackListElement")
                                        bl.Add(blel.InnerText);
                                blackList = bl.ToArray();
                                break;
                        }
                    }
                }
            }
        }
        private void loadPacketName()
        {
            using (TextReader tr = new StreamReader(Path.Combine("assets", "packets.xml")))
            {
                XmlSerializer ser = new XmlSerializer(typeof(string[]));
                packetName = (string[])ser.Deserialize(tr);
            }
        }
    }
}