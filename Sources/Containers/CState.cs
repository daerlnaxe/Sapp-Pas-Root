using System;
using System.Collections.Generic;
using System.Text;

namespace SPR.Containers
{
    public struct CState
    {
        public string Name { get; set; }
        public bool State { get; set; }

        public CState(string n, bool s)
        {
            Name = n;
            State = s;
        }
    }

}
