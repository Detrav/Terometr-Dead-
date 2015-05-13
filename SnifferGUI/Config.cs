using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        internal string serverIp;
        internal int adapterNumber;
        internal string[] packetName;

        internal static void saveConfig()
        {
            using (XmlWriter xw = XmlWriter.Create(Path.Combine("assets", "config.xml")))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("Config");
                xw.WriteElementString("serverIp", Instance.serverIp);
                xw.WriteElementString("adapterNumber", Instance.adapterNumber.ToString());
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
                        }
                    }
                }
            }
        }
    }
}