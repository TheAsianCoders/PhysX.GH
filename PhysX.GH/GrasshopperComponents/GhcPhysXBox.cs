
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
          : base("PX Box", "PX Box",
              "Generate PhysX Box from GH Box",
              "PhysX", "Geometries")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Base", "B", "Base Box", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dynamic", "D", "Dynamic or Static, True: Dyamic, False: Static", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Material Property", "P", "PhysX Material", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("RigidBody", "R", "PhysX Rigid body", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Box iBox = new Box();
            bool isDynamic = true;
            Material iMaterial = PhysXManager.Physics.CreateMaterial(0.5f, 0.5f, 0.5f);

            DA.GetData(0, ref iBox);
            DA.GetData(1, ref isDynamic);
            DA.GetData(2, ref iMaterial);

            Plane plane = iBox.Plane;
            plane.Translate(iBox.Center - iBox.Plane.Origin);

            if (isDynamic)
            {
                PxGhRigidDynamic rigidDynamic =
                    new PxGhRigidDynamicBox(plane, (float)iBox.X.Length, (float)iBox.Y.Length, (float)iBox.Z.Length, iMaterial, 1);

                DA.SetData(0, rigidDynamic);
            }
            else
            {
                PxGhRigidStaticBox rigidStatic =
                    new PxGhRigidStaticBox(plane, (float)iBox.X.Length, (float)iBox.Y.Length, (float)iBox.Z.Length, iMaterial);

                DA.SetData(0, rigidStatic);
            }

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.Box;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("fdf90b10-f520-4ea8-abb3-a85fafca0298"); }
        }
    }
}