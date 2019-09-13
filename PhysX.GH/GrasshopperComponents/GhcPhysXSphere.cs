﻿using System;
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
            : base(
                "PX Sphere",
                "PX Sphere",
                "Create a PhysX sphere rigid body",
                "PhysX",
                "Geometries")
        {
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Sphere;
        public override Guid ComponentGuid => new Guid("6644fa46-e7f2-470d-a027-dff3fd32b516");


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Radius", "R", "Radius", GH_ParamAccess.item, 1.0);
            pManager.AddPlaneParameter("Frame", "F", "Frame", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddBooleanParameter("Dynamic", "D", "Dynamic or Static, True: Dynamic, False: Static", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Material", "M", "PhysX Material", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Mass", "M", "Mass", GH_ParamAccess.item, 1.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rigid Body", "R", "PhysX rigid body", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double iRadius = double.NaN;
            Plane iFrame = Plane.Unset;
            bool iDynamic = true;
            Material iMaterial = PxGhManager.DefaultMaterial;
            double iMass = double.NaN;

            DA.GetData(0, ref iRadius);
            DA.GetData(1, ref iFrame);
            DA.GetData(2, ref iDynamic);
            DA.GetData(3, ref iMaterial);
            DA.GetData(4, ref iMass);

            if (iDynamic)
                DA.SetData(0, new PxGhRigidDynamicSphere(iFrame, (float)iRadius, iMaterial, (float) iMass, Vector3d.Zero, Vector3d.Zero));
            else
                DA.SetData(0, new PxGhRigidStaticSphere(iFrame, (float) iRadius, iMaterial));
        }
    }
}