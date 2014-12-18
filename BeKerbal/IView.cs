using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeKerbal
{
    public interface IView
    {
        void Start(KerbalInfo kerbalInfo);
        void Update(KerbalInfo kerbalInfo);
        void PausedUpdate(KerbalInfo kerbalInfo);
        void Stop(KerbalInfo kerbalInfo);
    }
}
