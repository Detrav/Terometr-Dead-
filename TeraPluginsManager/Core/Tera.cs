using Detrav.Sniffer;
using Detrav.TeraApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraPluginsManager.Core
{
    class Tera
    {
        public delegate void OnStartSnifferSync(object sender, EventArgs e);
        public event OnStartSnifferSync onStartSnifferSync;

        public delegate void OnNewConnectionSync(object sender, ConnectionEventArgs e);
        public event OnNewConnectionSync onNewConnectionSync;

        public delegate void OnEndConnectionSync(object sender, ConnectionEventArgs e);
        public event OnEndConnectionSync onEndConnectionSync;

        public delegate void OnParsePacketSync(object sender, PacketEventArgs e);
        public event OnParsePacketSync onParsePacketSync;



        Queue<ConnectionEventArgs> newConnections = new Queue<ConnectionEventArgs>();
        Queue<ConnectionEventArgs> endConnections = new Queue<ConnectionEventArgs>();
        Queue<PacketEventArgs> packets = new Queue<PacketEventArgs>();
        bool startSniffer = false;

        Capture capture;
        public Tera(Capture cap)
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
        public void stop()
        {
            if(capture!=null)
                capture.Dispose();
            capture=null;
        }
        void capture_onParsePacket(object sender, PacketEventArgs e)
        {
            lock(packets)
            {
                packets.Enqueue(e);
            }
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
        public void doEvents()
        {
            if (startSniffer)
            {
                if (onStartSnifferSync != null)
                    onStartSnifferSync(this, new EventArgs());
                startSniffer = false;
            }
            ConnectionEventArgs eCon;
            do//Первый раз пригодился до антил :)
            {
                eCon = null;
                lock (newConnections)
                {
                    if (newConnections.Count > 0)
                        eCon = newConnections.Dequeue();
                }
                if (eCon != null)
                {
                    if (onNewConnectionSync != null)
                        onNewConnectionSync(this, eCon);
                }
            } while (eCon != null);
            do
            {
                eCon = null;
                lock (endConnections)
                {
                    if (endConnections.Count > 0)
                        eCon = endConnections.Dequeue();
                }
                if (eCon != null)
                {
                    if (onEndConnectionSync != null)
                        onEndConnectionSync(this, eCon);
                }
            } while (eCon != null);
            PacketEventArgs packet;
            do
            {
                packet = null;
                lock(packets)
                {
                    if (packets.Count > 0)
                        packet = packets.Dequeue();
                }
                if(packet!=null)
                {
                    if (onParsePacketSync != null)
                        onParsePacketSync(this, packet);
                }
            } while (packet != null);
        }
    }
}
