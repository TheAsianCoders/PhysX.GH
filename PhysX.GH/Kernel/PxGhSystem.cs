using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using PhysX.GH.Kernel;
using PhysX;
using Rhino.Geometry;
using Plane = Rhino.Geometry.Plane;


namespace PhysX.GH.Kernel
{
    public class GhPxSystem
    {
        private readonly LinkedList<PxGhRigidStatic> ghPxRigidStatics = new LinkedList<PxGhRigidStatic>();
        private readonly LinkedList<PxGhRigidDynamic> ghPxRigidDynamics = new LinkedList<PxGhRigidDynamic>();


        public GhPxSystem()
        {
            PxGhManager.ClearScene();
        }


        public Vector3d Gravity
        {
            get { return PxGhManager.Scene.Gravity.ToRhinoVector(); }
            set { PxGhManager.Scene.Gravity = value.ToSystemVector(); }
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
    }
}
