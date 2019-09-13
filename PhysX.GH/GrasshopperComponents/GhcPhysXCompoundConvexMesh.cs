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
                "PX CompoundConvexMesh",
                "PX Mesh",
                "Create a PhysX rigid body consisting of one or more convex meshes",
                "PhysX", "Geometries")
        {
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.CMesh;
        public override Guid ComponentGuid => new Guid("1043b79b-ec38-47de-8ef9-dc638fbd0091");


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGroupParameter("Meshes", "M", "Meshes", GH_ParamAccess.item);
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
            GH_GeometryGroup iGroup = new GH_GeometryGroup();
            Plane iFrame = Plane.WorldXY;
            bool iDynamic = true;
            Material iMaterial = PxGhManager.DefaultMaterial;
            double iMass = double.NaN;

            DA.GetData(0, ref iGroup);
            DA.GetData(1, ref iFrame);
            DA.GetData(2, ref iDynamic);
            DA.GetData(3, ref iMaterial);
            DA.GetData(4, ref iMass);

            List<Mesh> meshes = new List<Mesh>();

            List<IGH_GeometricGoo> group = new List<IGH_GeometricGoo>(iGroup.Objects);

            for (int i = 0; i < group.Count; i++)
            {
                group[i].CastTo(out Mesh mesh);
                meshes.Add(mesh);
            }

            if (iDynamic)
                DA.SetData(0, new PxGhRigidDynamicConvexMeshGroup(meshes, iFrame, iMaterial, (float) iMass, Vector3d.Zero, Vector3d.Zero));
            else
                DA.SetData(0, new PxGhRigidStaticCompoundMesh(iFrame, meshes, iMaterial));
        }
    }
}