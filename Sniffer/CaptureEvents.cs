using Detrav.Sniffer.Tera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Sniffer
{
    public partial class Capture
    {
        //started
        public delegate void OnStartedSniffer(object sender, EventArgs e);
        public event OnStartedSniffer onStartedSniffer;
        //newconnection
        public delegate void OnNewConnection(object sender, ConnectionEventArgs e);
        public event OnNewConnection onNewConnection;
        //closeconnection
        public delegate void OnEndConnection(object sender, ConnectionEventArgs e);
        public event OnEndConnection onEndConnection;
        //stopped
        public delegate void OnStoppedSniffer(object sender, EventArgs e);
        public event OnStoppedSniffer onStoppedSniffer;
        //При получении тера пакета
        public delegate void OnParsePacket(object sender, PacketEventArgs e);
        public event OnParsePacket onParsePacket;
    }


    public class PacketEventArgs
    {
        public PacketEventArgs(Connection c, TeraPacket p) { connection = c; packet = p; }
        public Connection connection { get; private set; }
        public TeraPacket packet { get; private set; }
    }

    public class ConnectionEventArgs
    {
        public ConnectionEventArgs(Connection c) { connection = c; }
        public Connection connection { get; private set; } // readonly
    }
}
