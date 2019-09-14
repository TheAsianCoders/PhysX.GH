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
    public class GhcPhysXCompoundConvexMesh : GH_Component
    {
        public GhcPhysXCompoundConvexMesh()
            : base(
                "PX Compound Convex Mesh",
                "PX Mesh",
                "Create a PhysX rigid body consisting of one or more convex meshes",
                "PhysX", "Geometries")
        {
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.CMesh;
        public override Guid ComponentGuid => new Guid("1043b79b-ec38-47de-8ef9-dc638fbd0091");


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Meshes", "Meshes", "Meshes used for the computation in physics simulation", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Frame", "Frame", "Frame", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddBooleanParameter("Dynamic", "Dynamic", "Dynamic or Static, True: Dynamic, False: Static", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Material (Optional)", "Material", "PhysX material", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("Mass", "Mass", "Mass", GH_ParamAccess.item, 1.0);
            pManager.AddVectorParameter("Initial Linear Velocity", "Linear Vel.", "Initial linear velocity", GH_ParamAccess.item, Vector3d.Zero);
            pManager.AddVectorParameter("Initial Angular Velocity", "Angular Vel.", "Initial angular velocity", GH_ParamAccess.item, Vector3d.Zero);
            pManager.AddMeshParameter("Displayed Meshes (Optional)", "Displayed Meshes", "The displayed meshes can be different and typically has more details and higher resolution that the actual meshes used in physics simulation", GH_ParamAccess.list);
            pManager[7].Optional = true;
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rigid Body", "R", "PhysX rigid body", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Mesh> iMeshes = new List<Mesh>();
            Plane iFrame = Plane.WorldXY;
            bool iDynamic = true;
            Material iMaterial = PxGhManager.DefaultMaterial;
            double iMass = double.NaN;
            Vector3d iInitialLinearVelocity = Vector3d.Unset;
            Vector3d iInitialLAngularVelocity = Vector3d.Unset;
            List<Mesh> iDisplayeMeshes = new List<Mesh>();

            DA.GetDataList(0, iMeshes);
            DA.GetData(1, ref iFrame);
            DA.GetData(2, ref iDynamic);
            DA.GetData(3, ref iMaterial);
            DA.GetData(4, ref iMass);
            DA.GetData(5, ref iInitialLinearVelocity);
            DA.GetData(6, ref iInitialLAngularVelocity);
            DA.GetDataList(7, iDisplayeMeshes);

            if (iDynamic)
                DA.SetData(0, new PxGhRigidDynamiCompoundConvexMesh(iMeshes, iFrame, iMaterial, (float)iMass, iInitialLinearVelocity, iInitialLAngularVelocity));
            else
                DA.SetData(0, new PxGhRigidStaticCompoundConvexMesh(iFrame, iMeshes, iMaterial));
        }
    }
}