
namespace Detrav.Terometr.TeraApi.Data
{
    public class ServerInfoItem
    {
        public string serverIp;
        public string serverName;
        public static ServerInfoItem[] servers()
        {
            using (System.IO.TextReader tr = new System.IO.StreamReader("assets/servers.xml"))
            {
                System.Xml.Serialization.XmlSerializer xer = new System.Xml.Serialization.XmlSerializer(typeof(ServerInfoItem[]));
                return (ServerInfoItem[])xer.Deserialize(tr);
            }
        }
    }
}
