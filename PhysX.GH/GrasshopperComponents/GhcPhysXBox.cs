using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using PhysX.GH;
using PhysX.GH.Kernel;
using PhysX;
using Rhino.Geometry;

using Plane = Rhino.Geometry.Plane;


namespace PhysX.GH.GrasshopperComponents
{
    public class GhcPhysXBox : GH_Component
    {
        public GhcPhysXBox()
            : base(
                "PX Box",
                "PX Box",
                "Create a PhysX box rigid body",
                "PhysX",
                "Geometries")
        {
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Box;
        public override Guid ComponentGuid => new Guid("fdf90b10-f520-4ea8-abb3-a85fafca0298");


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Box", "B", "Box", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dynamic", "D", "Dynamic or Static, True: Dynamic, False: Static", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Material", "M", "PhysX Material", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Mass", "M", "Mass", GH_ParamAccess.item, 1.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rigid Body", "R", "PhysX rigid body", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Box iBox = Box.Empty;
            bool iDynamic = true;
            Material iMaterial = PxGhManager.DefaultMaterial;
            double iMass = double.NaN;

            DA.GetData(0, ref iBox);
            DA.GetData(1, ref iDynamic);
            DA.GetData(2, ref iMaterial);
            DA.GetData(3, ref iMass);

            if (iDynamic)
                DA.SetData(0, new PxGhRigidDynamicBox(iBox, iMaterial, (float)iMass, Vector3d.Zero, Vector3d.Zero));
            else
                DA.SetData(0, new PxGhRigidStaticBox(iBox, iMaterial));
        }
    }
}