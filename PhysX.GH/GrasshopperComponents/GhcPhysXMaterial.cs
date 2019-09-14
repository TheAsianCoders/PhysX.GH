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


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Material;
        public override Guid ComponentGuid => new Guid("64277da4-f0f1-4591-8bbd-5e8507e395e3");


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Static Friction", "Static Friction", "Static Friction (0.0 - 1.0)", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("Dynamic Friction", "Dynamic Friction", "Dynamic Friction (0.0 - 1.0)", GH_ParamAccess.item, 0.5);
            pManager.AddNumberParameter("Restitution", "Restitution", "Restitution (0.0 - 1.0)", GH_ParamAccess.item, 0.5);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "M", "PhysX material", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double iStaticFriction = 0.5, iDynamicFriction = 0.5, iRestitution = 0.5;

            DA.GetData(0, ref iStaticFriction);
            DA.GetData(1, ref iDynamicFriction);
            DA.GetData(2, ref iRestitution);

            Material material = PxGhManager.Physics.CreateMaterial((float)iStaticFriction, (float)iDynamicFriction, (float)iRestitution);

            DA.SetData(0, material);
        }
    }
}