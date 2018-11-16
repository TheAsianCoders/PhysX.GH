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
    public class GhcPhysXSimulateSeq : GH_Component
    {
        private GhPxSystem system;
        private Stopwatch stopwatch = new Stopwatch();
        private List<GH_Mesh> staticGhMeshes;
        private List<PxGhRigidBody> iRigidBodies = new List<PxGhRigidBody>();
        private int counter = 0;
        private bool isAllStable = true;
        private bool isSeq = true;

        public GhcPhysXSimulateSeq() 
            : base("PX SimulateSeq",
                   "PX SimulateSeq",
                   "PhysX Simulation Sequence Solver",
                   "PhysX",
                   "PhysX")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Rigid Bodies", "Rigid Bodies", "Rigid Bodies", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Gravity", "Gravity", "Gravity", GH_ParamAccess.item, new Vector3d(0.0, 0.0, -9.8));
            pManager.AddNumberParameter("Timestep", "Timestep", "Timestep", GH_ParamAccess.item, 0.01);
            pManager.AddIntegerParameter("Steps", "Steps", "Steps", GH_ParamAccess.item, 10);
            pManager.AddBooleanParameter("ReSys", "ReSys", "Restart system during sequence", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Sequence", "Sequence", "Sequence", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Reset", "Reset", "Reset", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Run", "Run", "Run", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "Info", "Info", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Dynamics", "Dynamics", "Dynamics", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Statics", "Statics", "Statics", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Stable", "Stable", "If all rigid dynamics stable", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Vector3d iGravity = Vector3d.Unset;
            double iTimestep = double.NaN;
            int iSteps = 10;
            bool iReset = false;
            bool iRun = false;

            DA.GetData("Gravity", ref iGravity);
            DA.GetData("Timestep", ref iTimestep);
            DA.GetData("Steps", ref iSteps);
            DA.GetData("Reset", ref iReset);
            DA.GetData("Run", ref iRun);

            if (iReset || system == null)
            {
                system = new GhPxSystem();

                iRigidBodies = new List<PxGhRigidBody>();
                foreach (GH_ObjectWrapper flatten in Params.Input[0].VolatileData.AllData(true))
                    iRigidBodies.Add((PxGhRigidBody)flatten.Value);
                
                isAllStable = true;

                counter = 0;
                isSeq = true;
            }

            List<int> sequence = new List<int>();
            bool iResys = true; 
            DA.GetDataList("Sequence", sequence);
            DA.GetData("ReSys", ref iResys);

            // Restart the PhysX system with perfect geometry
            if (isAllStable && isSeq && counter < sequence.Count && iResys)
            {
                system = new GhPxSystem();
                for (int i = 0; i <= counter; i++)
                {
                    PxGhRigidBody rb = iRigidBodies[sequence[i]];
                    if (rb is PxGhRigidDynamic)
                    {
                        PxGhRigidDynamic d = rb as PxGhRigidDynamic;
                        system.AddRigidDynamic(d);
                        d.Reset();
                    }
                    else if (rb is PxGhRigidStatic)
                    {
                        PxGhRigidStatic s = rb as PxGhRigidStatic;
                        system.AddRigidStatic(s);
                    }
                }
                isSeq = false;
                counter++;
            }

            // Adding simulating without restarting the system. Imperfect geometry condition
            if (isAllStable && isSeq && counter < sequence.Count && !iResys)
            {
                PxGhRigidBody rb = iRigidBodies[sequence[counter]];
                if (rb is PxGhRigidDynamic)
                {
                    PxGhRigidDynamic d = rb as PxGhRigidDynamic;
                    system.AddRigidDynamic(d);
                    d.Reset();

                    isSeq = false;
                    counter++;
                }
                else if (rb is PxGhRigidStatic)
                {
                    PxGhRigidStatic s = rb as PxGhRigidStatic;
                    system.AddRigidStatic(s);

                    isSeq = false;
                    counter++;
                }
            }


            isAllStable = true;
            foreach (var o in system.ghPxRigidDynamics)
                isAllStable = isAllStable && o.actor.IsSleeping;

            if (isAllStable)
                isSeq = true;

            staticGhMeshes = system.GetRigidStaticDisplayGhMeshes();

            if (iRun) ExpireSolution(true);
            stopwatch.Restart();
            system.Gravity = iGravity;
            system.Update((float)iTimestep, iSteps);
            stopwatch.Stop();

            string info = "";

            foreach (var o in system.ghPxRigidDynamics)
                info += "\n" + o.actor.IsSleeping;

            info += "\n" + "All Stable --- " + isAllStable;

            DA.SetData("Info", decimal.Round((decimal)stopwatch.Elapsed.TotalMilliseconds, 2) + "ms" + info );
            DA.SetDataList("Statics", staticGhMeshes);
            DA.SetDataList("Dynamics", system.GetRigidDynamicDisplayGhMeshes());
            DA.SetData("Stable", isAllStable);
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("7ad7b566-b4cd-43f6-bb47-578d13f6d22a");

    }
}