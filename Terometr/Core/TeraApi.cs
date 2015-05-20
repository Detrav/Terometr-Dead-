using Detrav.Sniffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    class TeraApi
    {
        public bool ready { get; set; }
        public TeraApi(Capture capture,int d)
        {
            ready = false;
            capture.onNewConnection += capture_onNewConnection;
            capture.onEndConnection += capture_onEndConnection;
            capture.onStartedSniffer += capture_onStartedSniffer;
            capture.onParsePacket += capture_onParsePacket;
            capture.start(d);
        }

        void capture_onParsePacket(object sender, PacketEventArgs e)
        {
            throw new NotImplementedException();
        }

        void capture_onStartedSniffer(object sender, EventArgs e)
        {
            ready = true;
        }

        void capture_onEndConnection(object sender, ConnectionEventArgs e)
        {
            throw new NotImplementedException();
        }

        void capture_onNewConnection(object sender, ConnectionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
