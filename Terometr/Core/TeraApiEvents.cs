using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Detrav.Sniffer;

namespace Detrav.Terometr.Core
{
    partial class TeraApi
    {
        Queue<ConnectionEventArgs> newConnections = new Queue<ConnectionEventArgs>();
        Queue<ConnectionEventArgs> endConnections = new Queue<ConnectionEventArgs>();
        bool startSniffer = false;

        public delegate void OnStartSnifferSync(object sender, EventArgs e);
        public event OnStartSnifferSync onStartSnifferSync;

        public delegate void OnNewConnectionSync(object sender, ConnectionEventArgs e);
        public event OnNewConnectionSync onNewConnectionSync;

        public delegate void OnEndConnectionSync(object sender, ConnectionEventArgs e);
        public event OnEndConnectionSync onEndConnectionSync;

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
        }
    }
}
