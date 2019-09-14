using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using PhysX.GH.Kernel;
using PhysX;
using Rhino.Geometry;
using Rhino.Render.Fields;


namespace PhysX.GH.GrasshopperComponents
{
    public class GhcPhysXSimulate : GH_Component
    {
        private GhPxSystem system;
        private Stopwatch stopwatch = new Stopwatch();
        private List<GH_Mesh> staticGhMeshes;
        string info;


        public GhcPhysXSimulate()
            : base(
                "PX Simulate",
                "PX Simulate",
                "PhysX Simulation Solver",
                "PhysX",
                "PhysX")
        {

        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Solver;
        public override Guid ComponentGuid => new Guid("{776F3532-145A-4F5D-BB95-2B81B1CC936C}");


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Rigid Bodies", "Rigid Bodies", "Rigid Bodies", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Gravity", "Gravity", "Gravity", GH_ParamAccess.item, new Vector3d(0.0, 0.0, -9.8));
            pManager.AddNumberParameter("Timestep", "Timestep", "Timestep", GH_ParamAccess.item, 0.01);
            pManager.AddIntegerParameter("Steps", "Steps", "Steps", GH_ParamAccess.item, 10);
            pManager.AddBooleanParameter("Reset", "Reset", "Reset", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Run", "Run", "Run", GH_ParamAccess.item, false);

        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "Info", "Info", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Dynamics", "Dynamics", "Dynamics", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Dynamic Frames", "Dynamic Frames", "Dynamic Frames", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Statics", "Statics", "Statics", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Vector3d iGravity = Vector3d.Unset;
            double iTimestep = double.NaN;
            int iSteps = 10;
            bool iReset = false;
            bool iRun = false;

            DA.GetData(1, ref iGravity);
            DA.GetData(2, ref iTimestep);
            DA.GetData(3, ref iSteps);
            DA.GetData(4, ref iReset);
            DA.GetData(5, ref iRun);


            if (iReset || system == null)
            {
                system = new GhPxSystem();

                List<PxGhRigidBody> iRigidBodies = new List<PxGhRigidBody>();
                foreach (var ghGoo in Params.Input[0].VolatileData.AllData(true))
                    iRigidBodies.Add((PxGhRigidBody)(((GH_ObjectWrapper)ghGoo).Value));

                foreach (PxGhRigidBody o in iRigidBodies)
                {
                    if (o is PxGhRigidDynamic)
                    {
                        PxGhRigidDynamic d = (PxGhRigidDynamic)o;
                        system.AddRigidDynamic(d);
                        d.Reset();
                    }
                    else if (o is PxGhRigidStatic)
                    {
                        system.AddRigidStatic((PxGhRigidStatic)o);
                    }
                }

                staticGhMeshes = system.GetRigidStaticDisplayedGhMeshes();
            }

            info = "";

            if (iRun)
            {
                ExpireSolution(true);
                stopwatch.Restart();
                system.Gravity = iGravity;
                system.Iterate((float) iTimestep, iSteps);
                stopwatch.Stop();
            }

            DA.SetData(0, decimal.Round((decimal)stopwatch.Elapsed.TotalMilliseconds, 2) + "ms" + info);
            DA.SetDataList(1, system.GetRigidDynamicDisplayedGhMeshes());
            DA.SetDataList(2, system.GetDynamicFramesAsGhPlanes());
            DA.SetDataList(3, staticGhMeshes);
        }
    }
}