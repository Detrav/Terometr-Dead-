
namespace Detrav.Teroniffer.Core
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
        public static string[] getServersName(ServerInfoItem[] servs)
        {
            string[] result = new string[servs.Length];
            for (int i = 0; i < servs.Length; i++)
                result[i] = servs[i].serverName;
            return result;
        }
    }
}
