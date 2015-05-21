using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi
{
    public delegate void OnLogin(object sender, EventArgs e);
    public interface ITera
    {
        event OnLogin onLogin;
    }
}
