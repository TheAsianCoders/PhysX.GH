using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace PhysX.GH.GrasshopperComponents
{
    public class GhcPhysXMaterial : GH_Component
    {

        public GhcPhysXMaterial()
          : base("PX Material", "PX Mat",
              "PhysX Material",
              "PhysX", "Material")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Static Friction", "Fs", "Static Friction for the PhysX material (0.0 - 1.0)", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("Dynamic Friction", "Fd", "Dynamic Friction for the PhysX material (0.0 - 1.0)", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("Restitution", "R", "Restitution for the PhysX material (0.0 - 1.0)", GH_ParamAccess.item, 0.5);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "PhysX Material", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double fs = 0.5, fd = 0.5, r = 0.5; 

            DA.GetData(0, ref fs);
            DA.GetData(1, ref fd);
            DA.GetData(2, ref r);

            Material material = PhysXManager.Physics.CreateMaterial((float)fs, (float)fd, (float)r);

            DA.SetData(0, material);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.PenguinMaterial;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("64277da4-f0f1-4591-8bbd-5e8507e395e3"); }
        }
    }
}