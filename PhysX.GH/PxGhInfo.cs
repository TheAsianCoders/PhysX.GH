using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace PhysX.GH
{
    public class PxGhInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Penguin PhysX.GH";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Properties.Resources.logo_24x24;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "PhysX rigid body simulation in Grasshopper";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("6227da2e-e266-4a99-b1ff-2ce693a9025b");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Gene Ting-Chun Kao & Long Nguyen, The Asian Coders";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "kao.gene@gmail.com";
            }
        }
    }
}
