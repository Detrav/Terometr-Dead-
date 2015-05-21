using Detrav.Sniffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.Terometr.Core
{
    partial class TeraApi
    {
        Capture capture;
        public TeraApi(Capture cap)
        {
            this.capture = cap;
            capture.onNewConnection += capture_onNewConnection;
            capture.onEndConnection += capture_onEndConnection;
            capture.onStartedSniffer += capture_onStartedSniffer;
            capture.onParsePacket += capture_onParsePacket;
        }

        public void start(int d)
        {
            capture.start(d);
        }

        void capture_onParsePacket(object sender, PacketEventArgs e)
        {
            throw new NotImplementedException();
        }

        void capture_onStartedSniffer(object sender, EventArgs e)
        {
            startSniffer = true;
        }

        void capture_onEndConnection(object sender, ConnectionEventArgs e)
        {
            lock(newConnections)
            {
                newConnections.Enqueue(e);
            }
            
        }

        void capture_onNewConnection(object sender, ConnectionEventArgs e)
        {
            lock(endConnections)
            {
                endConnections.Enqueue(e);
            }
        }
    }
}
