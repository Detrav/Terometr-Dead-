﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detrav.TeraApi
{
    public interface IPlugin
    {
        void register(ITera parent);
        void show();
        void unRegister();
    }
}
