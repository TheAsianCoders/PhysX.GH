
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

        public GhcPhysXSimulate()
            : base(
                "PX Simulate",
                "PX Simulate",
                "Description",
                "PhysX",
                "PhysX")
        {

        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Rigid Statics", "Rigid Statics", "Rigid Statics", GH_ParamAccess.list);
            pManager.AddGenericParameter("Rigid Dynamics", "Rigid Dynamics", "Rigid Dynamics", GH_ParamAccess.list);
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
            pManager.AddGeometryParameter("Statics", "Statics", "Statics", GH_ParamAccess.list);
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

                var iRigidStatics = new List<PxGhRigidStatic>();
                var iRigidDynamics = new List<PxGhRigidDynamic>();
                DA.GetDataList("Rigid Statics", iRigidStatics);
                DA.GetDataList("Rigid Dynamics", iRigidDynamics);

                foreach (PxGhRigidDynamic o in iRigidDynamics)
                {       
                    system.AddRigidDynamic(o);
                    o.Reset();
                }

                foreach (PxGhRigidStatic o in iRigidStatics)
                    system.AddRigidStatic(o);

                staticGhMeshes = system.GetRigidStaticDisplayGhMeshes();
            }


            if (iRun) ExpireSolution(true);

            stopwatch.Restart();
            system.Gravity = iGravity;
            system.Update((float)iTimestep, iSteps);
            stopwatch.Stop();

            string info = "";

            foreach (var o in system.ghPxRigidDynamics)
                info += "\n" + o.actor.IsSleeping;

            DA.SetData("Info", decimal.Round((decimal)stopwatch.Elapsed.TotalMilliseconds, 2) + "ms" + info);
            DA.SetDataList("Statics", staticGhMeshes);
            DA.SetDataList("Dynamics", system.GetRigidDynamicDisplayGhMeshes());
        }


        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{776F3532-145A-4F5D-BB95-2B81B1CC936C}");
    }
}