
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
    public class GhcPhysXDynamicMesh : GH_Component
    {
        public GhcPhysXDynamicMesh()
          : base("PX ConvexMesh", "PX CxMesh",
              "Input ConvexMesh or PhysX will automatically convert it to convex and the result won't be as expected",
              "PhysX", "Geometries")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "PhysX Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("Material Propertie", "P", "PhysX Material", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Dynamic", "D", "PhysX Dynamics", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh iMesh = new Mesh();
            Material iMaterial = PhysXManager.Physics.CreateMaterial(50.0f, 50.0f, 0.6f);

            DA.GetData(0, ref iMesh);
            DA.GetData(1, ref iMaterial);

            iMesh.Faces.ConvertQuadsToTriangles();
            PxGhRigidDynamic dynamic = new PxGhRigidDynamicMesh(Plane.WorldXY, iMesh, iMaterial, 1);
            

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
            get { return new Guid("bdfdbe0b-af09-4f85-be3b-b969c9a7ad7f"); }
        }
    }
}