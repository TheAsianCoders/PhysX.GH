﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using PhysX.GH.Kernel;
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
            pManager.AddNumberParameter("Radius", "Radius", "Radius", GH_ParamAccess.item, 1.0);
            pManager.AddPlaneParameter("Frame", "Frame", "Frame", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddBooleanParameter("Dynamic", "Dynamic", "Dynamic or Static, True: Dynamic, False: Static", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Material (Optional)", "Material", "PhysX material", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Mass", "Mass", "Mass", GH_ParamAccess.item, 1.0);
            pManager.AddVectorParameter("Initial Linear Velocity", "Linear Vel.", "Initial linear velocity", GH_ParamAccess.item, Vector3d.Zero);
            pManager.AddVectorParameter("Initial Angular Velocity", "Angular.", "Initial angular velocity", GH_ParamAccess.item, Vector3d.Zero);
            pManager.AddNumberParameter("Linear Damping", "Linear Damp", "Linear Damping", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Angular Damping", "Angular Damp", "Angular Damping", GH_ParamAccess.item, 1.0);
            pManager.AddMeshParameter("Displayed Meshes (Optional)", "Displayed Meshes", "Custom displayed meshes", GH_ParamAccess.list);
            pManager[9].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rigid Sphere", "R", "PhysX rigid sphere", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double iRadius = double.NaN;
            Plane iFrame = Plane.Unset;
            bool iDynamic = true;
            Material iMaterial = PxGhManager.DefaultMaterial;
            double iMass = double.NaN;
            Vector3d iInitialLinearVelocity = Vector3d.Unset;
            Vector3d iInitialLAngularVelocity = Vector3d.Unset;
            double iLinearDamping = double.NaN;
            double iAngularDamping = double.NaN;
            List<Mesh> iDisplayedMeshes = new List<Mesh>();

            DA.GetData(0, ref iRadius);
            DA.GetData(1, ref iFrame);
            DA.GetData(2, ref iDynamic);
            DA.GetData(3, ref iMaterial);
            DA.GetData(4, ref iMass);
            DA.GetData(5, ref iInitialLinearVelocity);
            DA.GetData(6, ref iInitialLAngularVelocity);
            DA.GetData(7, ref iLinearDamping);
            DA.GetData(8, ref iAngularDamping);
            DA.GetDataList(9, iDisplayedMeshes);

            if (iDynamic)
            {
                PxGhRigidDynamic rigidObject = new PxGhRigidDynamicSphere(iFrame, (float)iRadius, iMaterial, (float)iMass, iInitialLinearVelocity, iInitialLAngularVelocity);
                rigidObject.Actor.LinearDamping = (float)iLinearDamping;
                rigidObject.Actor.AngularDamping = (float)iAngularDamping;
                DA.SetData(0, rigidObject);
            }
            else
                DA.SetData(0, new PxGhRigidStaticSphere(iFrame, (float) iRadius, iMaterial));
        }
    }
}