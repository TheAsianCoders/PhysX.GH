using System.Collections.Generic;
using System.Numerics;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using Plane = Rhino.Geometry.Plane;


namespace PhysX.GH.Kernel
{
    public enum MouseManipulationMode { Position = 0, Acceleration, Force };

    public class GhPxSystem
    {
        internal double MouseManipulationStrength = 0.0;
        internal MouseManipulationMode MouseManipulationMode = MouseManipulationMode.Acceleration;

        private readonly LinkedList<PxGhRigidStatic> ghPxRigidStatics = new LinkedList<PxGhRigidStatic>();
        private readonly LinkedList<PxGhRigidDynamic> ghPxRigidDynamics = new LinkedList<PxGhRigidDynamic>();

        private RigidDynamic hitActor = null;
        private Point3d hookPoint;
        private bool checkForHit = true;

        internal Point3d HookPoint = Point3d.Unset;
        internal Point3d HookPointOnMouseLine = Point3d.Unset;


        public Vector3d Gravity
        {
            get { return PxGhManager.Scene.Gravity.ToRhinoVector(); }
            set { PxGhManager.Scene.Gravity = value.ToSystemVector(); }
        }


        public GhPxSystem()
        {
            PxGhManager.ClearScene();
        }


        private void ProcessMouseManipulation()
        {
            if (!MouseTracker.LeftMousePressed)
            {
                checkForHit = true;
                hitActor = null;

                HookPoint = Point3d.Unset;
                HookPointOnMouseLine = Point3d.Unset;
            }
            else
            {
                if (checkForHit)
                {
                    checkForHit = false;
                    Vector3d mouseLineDirection = MouseTracker.MouseLine.Direction;
                    mouseLineDirection.Unitize();
                    PxGhManager.Scene.Raycast(
                        MouseTracker.MouseLine.From.ToSystemVector(),
                        mouseLineDirection.ToSystemVector(),
                        9999f,
                        999,
                        HitCallback,
                        HitFlag.Position);
                }

                if (hitActor != null)
                {
                    Rhino.Geometry.Plane frame = hitActor.GlobalPose.ToRhinoPlane();
                    HookPoint = frame.Origin + frame.XAxis * hookPoint.X + frame.YAxis * hookPoint.Y + frame.ZAxis * hookPoint.Z;
                    HookPointOnMouseLine = MouseTracker.MouseLine.ClosestPoint(HookPoint, false);

                    if (MouseManipulationMode == MouseManipulationMode.Position)
                    {
                        frame.Origin = HookPointOnMouseLine + (frame.Origin - HookPoint);
                        hitActor.GlobalPose = frame.ToMatrix();
                        hitActor.LinearVelocity = Vector3.Zero;
                        hitActor.AddForceAtLocalPosition((hitActor.Mass * -Gravity).ToSystemVector(), hookPoint.ToSystemVector(), ForceMode.Force, true);
                    }
                    else
                    {
                        Vector3d force = (HookPointOnMouseLine - HookPoint) * MouseManipulationStrength *
                                         (MouseManipulationMode == MouseManipulationMode.Acceleration ? hitActor.Mass : 1.0);
                        hitActor.AddForceAtLocalPosition(force.ToSystemVector(), hookPoint.ToSystemVector(), ForceMode.Force, true);
                    }
                }
            }
        }


        public void Iterate(float timeStep = 0.01f, int steps = 1)
        {
            for (int i = 0; i < steps; i++)
            {
                PxGhManager.Scene.Simulate(timeStep);
                PxGhManager.Scene.FetchResults(true);
            }

            foreach (PxGhRigidDynamic o in ghPxRigidDynamics)
                o.CacheFrame();

            if (MouseManipulationStrength > 0.0) ProcessMouseManipulation();
        }


        public void Reset()
        {
            foreach (PxGhRigidDynamic o in ghPxRigidDynamics)
                o.Reset();
        }


        public void AddRigidStatic(PxGhRigidStatic o)
        {
            ghPxRigidStatics.AddLast(o);
            PxGhManager.Scene.AddActor(o.Actor);
        }


        public void AddRigidDynamic(PxGhRigidDynamic o)
        {
            ghPxRigidDynamics.AddLast(o);
            PxGhManager.Scene.AddActor(o.Actor);
        }


        public void RemoveRigidStatic(PxGhRigidStatic o)
        {
            ghPxRigidStatics.Remove(o);
            PxGhManager.Scene.RemoveActor(o.Actor);
        }


        public void RemoveRigidDynamic(PxGhRigidDynamic o)
        {
            ghPxRigidDynamics.Remove(o);
            PxGhManager.Scene.RemoveActor(o.Actor);
        }


        public List<GH_Mesh> GetRigidDynamicDisplayedGhMeshes()
        {
            List<GH_Mesh> ghMeshes = new List<GH_Mesh>();
            foreach (PxGhRigidDynamic o in ghPxRigidDynamics)
                o.GetDisplayGhMeshes(ghMeshes);
            return ghMeshes;
        }


        public List<GH_Mesh> GetRigidStaticDisplayedGhMeshes()
        {
            List<GH_Mesh> ghMeshes = new List<GH_Mesh>();
            foreach (PxGhRigidStatic o in ghPxRigidStatics)
                o.GetDisplayGhMeshes(ghMeshes);
            return ghMeshes;
        }


        public List<Plane> GetDynamicFrames()
        {
            List<Plane> planes = new List<Plane>();
            foreach (PxGhRigidDynamic o in ghPxRigidDynamics)
                planes.Add(o.Frame);
            return planes;
        }


        public List<GH_Plane> GetDynamicFramesAsGhPlanes()
        {
            List<GH_Plane> ghPlanes = new List<GH_Plane>();
            foreach (PxGhRigidDynamic o in ghPxRigidDynamics)
                ghPlanes.Add(new GH_Plane(o.Frame));
            return ghPlanes;
        }


        private bool HitCallback(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits)
            {
                if (!(hit.Actor is RigidDynamic)) continue;
                hitActor = (RigidDynamic)hit.Actor;
                Rhino.Geometry.Plane frame = hitActor.GlobalPose.ToRhinoPlane();
                Vector3d v = hit.Position.ToRhinoPoint() - frame.Origin;
                hookPoint = new Point3d(v * frame.XAxis, v * frame.YAxis, v * frame.ZAxis);
                return true;
            }
            return false;
        }
    }
}
