using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using PhysX;
using Rhino.Geometry;

using Plane = Rhino.Geometry.Plane;

namespace PhysX.GH.GrasshopperComponents
{
    public class GhcPhysXPilot : GH_Component
    {
        private Scene scene;
        private RigidDynamic actor;
        private Stopwatch stopwatch = new Stopwatch();

        public GhcPhysXPilot()
            : base(
                "PX Pilot",
                "PX Pilot",
                "Description",
                "PhysX",
                "PhysX")
        {
       
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset", "Reset", "Reset", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Run", "Run", "Run", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Timestep", "Timestep", "Timestep", GH_ParamAccess.item, 0.005);
            pManager.AddIntegerParameter("SubIterationCount", "SubIterationCount", "SubIterationCount", GH_ParamAccess.item, 10);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "Info", "Info", GH_ParamAccess.item);
            pManager.AddMeshParameter("Meshes", "Meshes", "Meshes", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Geometries", "Geometries", "Geometries", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Frames", "Frames", "Frames", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool iReset = false;
            bool iRun = false;
            int iSubIterationCount = 10;
            double iTimestep = double.NaN;
            DA.GetData("Reset", ref iReset);
            DA.GetData("Run", ref iRun);
            DA.GetData("SubIterationCount", ref iSubIterationCount);
            DA.GetData("Timestep", ref iTimestep);


            if (iReset || scene == null)
            {
                SceneDesc sceneDesc = new SceneDesc();
                sceneDesc.Gravity = new Vector3(0.0f, 0.0f, -9.8f);
                scene = PhysXManager.Physics.CreateScene(sceneDesc);
                Material material = PhysXManager.Physics.CreateMaterial(50.0f, 50.0f, 0.7f);

                RigidActor boxActor = PhysXManager.Physics.CreateRigidStatic();
                boxActor.CreateShape(new BoxGeometry(new Vector3(100f, 100f, 1.0f)), material);
                boxActor.GlobalPose = Matrix4x4.CreateTranslation(0f, 0f, -1f);
                scene.AddActor(boxActor);

                actor = scene.Physics.CreateRigidDynamic();
                actor.SetMassAndUpdateInertia(1.0f);
                Shape shape = actor.CreateShape(new BoxGeometry(1f, 2f, 5f), material, Matrix4x4.CreateTranslation(0f, 0f, 2f));
            
                actor.GlobalPose = new Plane(new Point3d(2, 4, 30), new Vector3d(0, 1, 2)).ToMatrix();
                scene.AddActor(actor);
                //actor.AddForceAtLocalPosition(new Vector3(0f, 0f, -1f) * 100.1f, new Vector3(0f, 0f, 4f), ForceMode.Force, true);
            }

            if (iRun) ExpireSolution(true);

            if (actor.LinearVelocity.Length() < 0.001 && actor.AngularVelocity.Length() < 0.001)
                actor.GlobalPose = new Plane(new Point3d(2, 4, 30), new Vector3d(0, 1, 2)).ToMatrix();


            stopwatch.Restart();
            for (int i = 0; i < iSubIterationCount; i++)
            {
                scene.Simulate((float) iTimestep);
                scene.FetchResults(true);
            }
            stopwatch.Stop();

            


            List<GH_Mesh> oMeshes = new List<GH_Mesh>();
            List<GH_Plane> oFrames = new List<GH_Plane>();

            Mesh boxMesh = Mesh.CreateFromBox(new Box(Plane.WorldXY, new BoundingBox(-1, -2, -5, 1, 2, 5)), 1, 1, 1);
            Mesh boxMeshT = boxMesh.DuplicateMesh();
            boxMeshT.Transform(actor.Shapes[0].GlobalPose.ToRhinoTransform());
            oMeshes.Add(new GH_Mesh(boxMeshT));

            oFrames.Add(new GH_Plane(actor.GlobalPose.ToRhinoPlane()));

            DA.SetDataList("Geometries", oMeshes);
            DA.SetDataList("Frames", oFrames);

            DA.SetData("Info", decimal.Round((decimal)stopwatch.Elapsed.TotalMilliseconds, 2) + "ms");
        }


        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{a9df47a7-b646-46c1-9824-35d02d482bc3}");
    }
}