
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
    public class GhcPhysXMesh : GH_Component
    {
        public GhcPhysXMesh()
          : base("PX ConvexMesh", "PX CxMesh",
              "Input ConvexMesh or PhysX will automatically convert it to convex and the result won't be as expected",
              "PhysX", "Geometries")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "PhysX Mesh", GH_ParamAccess.item);
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
            Mesh iMesh = new Mesh();
            bool isDynamic = true;
            Material iMaterial = PhysXManager.Physics.CreateMaterial(0.5f, 0.5f, 0.5f);

            DA.GetData(0, ref iMesh);
            DA.GetData(1, ref isDynamic);
            DA.GetData(2, ref iMaterial);

            if (isDynamic)
            {
                PxGhRigidDynamic rigidDynamic = new PxGhRigidDynamicMesh(Plane.WorldXY, iMesh, iMaterial, 1);
                DA.SetData(0, rigidDynamic);
            }
            else
            {
                PxGhRigidStatic rigidStatic = new PxGhRigidStaticMesh(Plane.WorldXY, iMesh, iMaterial);
                DA.SetData(0, rigidStatic);
            }

        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.Mesh;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("bdfdbe0b-af09-4f85-be3b-b969c9a7ad7f"); }
        }
    }
}