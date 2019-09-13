using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PhysX.GH
{
    public class PxGhAssemblyPriority : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            PxGhManager.Initialize();
            return GH_LoadingInstruction.Proceed;
        }
    }
}