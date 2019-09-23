using System;
using System.Drawing;
using Grasshopper.Kernel;


namespace PhysX.GH
{
    public class PxGhInfo : GH_AssemblyInfo
    {
        public override string Name => "PhysX.GH";
        public override Bitmap Icon => Properties.Resources.logo_24x24;
        public override string Description => "PhysX rigid body simulation in Grasshopper";
        public override Guid Id => new Guid("6227da2e-e266-4a99-b1ff-2ce693a9025b");
        public override string AuthorName => "Gene Ting-Chun Kao & Long Nguyen, The Asian Coders";
        public override string AuthorContact => "Gene Kao (gene@gmail.com), Long Nguyen (longnguyen.connect@gmail.com)";
    }
}
