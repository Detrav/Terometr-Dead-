using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi
{
    public interface IPlugin
    {
        void load(ITeraConnection parent);
        void show();
        void hide();
        void unLoad();
    }
}
