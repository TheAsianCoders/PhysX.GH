
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
    public class GhcPhysXPilot2 : GH_Component
    {
        public GhcPhysXPilot2()
            : base(
                "PX Pilot2",
                "PX Pilot2",
                "Description",
                "PhysX",
                "PhysX")
        {
       
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Meshes", "Meshes", "Meshes", GH_ParamAccess.list);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Statics", "Statics", "Statics", GH_ParamAccess.list);
            pManager.AddGenericParameter("Dynamics", "Dynamics", "Dynamics", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<PxGhRigidStatic> statics = new List<PxGhRigidStatic>();
            List<PxGhRigidDynamic> dynamics = new List<PxGhRigidDynamic>();

            Material material = PhysXManager.Physics.CreateMaterial(50.0f, 50.0f, 0.6f);

            statics.Add(new PxGhRigidStaticBox(new Plane(new Point3d(0.0, 0.0, -0.5), Vector3d.ZAxis), 200f, 200f, 1f, material));

            for (int k = 2; k < 4; k++)
                for (int i = -3; i < 3; i++)
                    for (int j = -3; j < 3; j++)
                    {
                        Plane plane = new Plane(
                                new Point3d(i * 4, j * 4, k * 6),
                                Vector3d.ZAxis);
                        plane.Rotate(Util.GetRandomDouble(6.28), plane.XAxis, plane.Origin);
                        plane.Rotate(Util.GetRandomDouble(6.28), plane.YAxis, plane.Origin);
                        plane.Rotate(Util.GetRandomDouble(6.28), plane.ZAxis, plane.Origin);

                        if (Util.GetRandomDouble() < 0.5)
                        {
                            dynamics.Add(new PxGhRigidDynamicBox(
                                plane,
                                1.5f, 1.5f, 1.5f,
                                material,
                                1));
                        }
                        else
                        {
                            dynamics.Add(new PxGhRigidDynamicSphere(plane, 1.0f, material, 1f));
                        }
                    }


            List<Mesh> iMeshes = new List<Mesh>();

            DA.GetDataList("Meshes", iMeshes);

            foreach (Mesh mesh in iMeshes)
            {
                mesh.Faces.ConvertQuadsToTriangles();
                dynamics.Add(new PxGhRigidDynamicMesh(Plane.WorldXY, mesh, material, 1));
            }

            DA.SetDataList("Statics", statics);
            DA.SetDataList("Dynamics", dynamics);
        }


        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{a9df47a7-b646-46c1-9824-35d02d482bc4}");
    }
}