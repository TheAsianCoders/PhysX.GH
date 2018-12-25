
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
    public class GhcPhysXSphere : GH_Component
    {
        public GhcPhysXSphere()
          : base("PX Sphere", "PX Sphere",
              "Generate PhysX Shpere like mesh sphere",
              "PhysX", "Geometries")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Base Plane", "B", "Base plane for sphere", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Radius for PhysX sphere", GH_ParamAccess.item, 1.0);
            pManager.AddBooleanParameter("Dynamic", "D", "Dynamic or Static, True: Dyamic, False: Static", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Material Property", "P", "PhysX Material", GH_ParamAccess.item);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("RigidBody", "R", "PhysX Rigid body", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane iPlane = new Plane();
            double iRadius = 0;
            bool isDynamic = true;
            Material iMaterial = PhysXManager.Physics.CreateMaterial(0.5f, 0.5f, 0.5f);

            DA.GetData(0, ref iPlane);
            DA.GetData(1, ref iRadius);
            DA.GetData(2, ref isDynamic);
            DA.GetData(3, ref iMaterial);

            PxGhRigidDynamic dynamic = new PxGhRigidDynamicSphere(iPlane, (float)iRadius, iMaterial, 1f);
            
            DA.SetData(0, dynamic);

            if (isDynamic)
            {
                PxGhRigidDynamic rigidDynamic = new PxGhRigidDynamicSphere(iPlane, (float)iRadius, iMaterial, 1f);
                DA.SetData(0, rigidDynamic);
            }
            else
            {
                PxGhRigidStaticSphere rigidStatic = new PxGhRigidStaticSphere(iPlane, (float)iRadius, iMaterial);
                DA.SetData(0, rigidStatic);
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.PenguinSphere;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("6644fa46-e7f2-470d-a027-dff3fd32b516"); }
        }
    }
}