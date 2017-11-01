
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
    public class GhcPhysXDynamicSphere : GH_Component
    {
        public GhcPhysXDynamicSphere()
          : base("PX Sphere", "PX Sphere",
              "Generate PhysX Shpere like mesh sphere",
              "PhysX", "Geometries")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Base Plane", "B", "Base plane for sphere", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Radius for PhysX sphere", GH_ParamAccess.item, 1.0);
            pManager.AddGenericParameter("Material Propertie", "P", "PhysX Material", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Dynamic", "D", "PhysX Dynamics", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane iPlane = new Plane();
            double iRadius = 0;
            Material iMaterial = PhysXManager.Physics.CreateMaterial(50.0f, 50.0f, 0.6f);

            DA.GetData(0, ref iPlane);
            DA.GetData(1, ref iRadius);
            DA.GetData(2, ref iMaterial);

            PxGhRigidDynamic dynamic = new PxGhRigidDynamicSphere(iPlane, (float)iRadius, iMaterial, 1f);
            
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
            get { return new Guid("6644fa46-e7f2-470d-a027-dff3fd32b516"); }
        }
    }
}