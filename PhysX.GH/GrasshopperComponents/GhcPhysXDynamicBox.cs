
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
    public class GhcPhysXDynamicBox : GH_Component
    {
        public GhcPhysXDynamicBox()
          : base("PX Box", "PX Box",
              "Generate PhysX Box from GH Box",
              "PhysX", "Geometries")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Base", "B", "Base Box", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material Propertie", "P", "PhysX Material", GH_ParamAccess.item);
            pManager[1].Optional = true;

        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Dynamic", "D", "PhysX Dynamics", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Box iBox = new Box();
            Material iMaterial = PhysXManager.Physics.CreateMaterial(50.0f, 50.0f, 0.6f);

            DA.GetData(0, ref iBox);
            DA.GetData(1, ref iMaterial);

            Plane plane = iBox.Plane;
            plane.Translate(iBox.Center - iBox.Plane.Origin);
            PxGhRigidDynamic dynamic = 
                new PxGhRigidDynamicBox(plane, (float)iBox.X.Length, (float)iBox.Y.Length, (float)iBox.Z.Length, iMaterial, 1);

            DA.SetData(0, dynamic);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("fdf90b10-f520-4ea8-abb3-a85fafca0298"); }
        }
    }
}