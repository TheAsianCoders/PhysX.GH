using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using PhysX.GH.Kernel;
using Rhino.Display;
using Rhino.Geometry;


namespace PhysX.GH.GrasshopperComponents
{
    public class GhcPhysXSimulate : GH_Component
    {
        private GhPxSystem system;
        private Stopwatch stopwatch = new Stopwatch();
        private List<GH_Mesh> staticGhMeshes;
        private string info;
        private bool outputDynamicFrames = true;
        private bool visualizeMouseManipulation = true;
        private bool asynchronous = false;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Solver;
        public override Guid ComponentGuid => new Guid("{776F3532-145A-4F5D-BB95-2B81B1CC936C}");


        public GhcPhysXSimulate()
            : base(
                "PX Simulate",
                "PX Simulate",
                "PhysX Simulation Solver",
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
            pManager.AddIntegerParameter("Mouse Manipulation Mode", "Mouse Mode", "0 = Force, 1 = Acceleration", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Mouse Manipulation Strength", "Mouse Strength", "Mouse Manipulation Strength", GH_ParamAccess.item, 50.0);
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
            int iMouseManipulationMode = 0;
            double iMouseManipulationStrength = double.NaN;
            bool iReset = false;
            bool iRun = false;

            DA.GetData(1, ref iGravity);
            DA.GetData(2, ref iTimestep);
            DA.GetData(3, ref iSteps);
            DA.GetData(4, ref iMouseManipulationMode);
            DA.GetData(5, ref iMouseManipulationStrength);
            DA.GetData(6, ref iReset);
            DA.GetData(7, ref iRun);

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
                system.MouseManipulationStrength = iMouseManipulationStrength;
                system.MouseManipulationMode = (MouseManipulationMode)iMouseManipulationMode;
                system.Iterate((float)iTimestep, iSteps);
                stopwatch.Stop();
            }

            DA.SetData(0, decimal.Round((decimal)stopwatch.Elapsed.TotalMilliseconds, 2) + "ms" + info);
            DA.SetDataList(1, system.GetRigidDynamicDisplayedGhMeshes());
            if (outputDynamicFrames) DA.SetDataList(2, system.GetDynamicFramesAsGhPlanes());
            DA.SetDataList(3, staticGhMeshes);
        }


        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            if (visualizeMouseManipulation)
            {
                args.Display.DrawPoint(system.HookPoint, PointStyle.RoundSimple, 5, Color.Teal);
                args.Display.DrawPoint(system.HookPointOnMouseLine, PointStyle.RoundSimple, 5, Color.Teal);
                args.Display.DrawLine(system.HookPoint, system.HookPointOnMouseLine, Color.Teal, 2);
            }
        }


        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(
                menu,
                "Output dynamic frames",
                (s, e) => { outputDynamicFrames = !outputDynamicFrames; },
                true,
                outputDynamicFrames);

            Menu_AppendItem(
                menu,
                "Visualize mouse manipulation",
                (s, e) => { visualizeMouseManipulation = !visualizeMouseManipulation; },
                true,
                visualizeMouseManipulation);

            Menu_AppendItem(
                menu,
                "Asynchronous",
                (s, e) => { asynchronous = !asynchronous; },
                true,
                asynchronous);
        }


        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("outputDynamicFrames", outputDynamicFrames);
            writer.SetBoolean("visualizeMouseManipulation", visualizeMouseManipulation);
            writer.SetBoolean("asynchronous", asynchronous);
            return base.Write(writer);
        }


        public override bool Read(GH_IReader reader)
        {
            outputDynamicFrames = reader.GetBoolean("outputDynamicFrames");
            visualizeMouseManipulation = reader.GetBoolean("visualizeMouseManipulation");
            asynchronous = reader.GetBoolean("asynchronous");
            return base.Read(reader);
        }
    }
}