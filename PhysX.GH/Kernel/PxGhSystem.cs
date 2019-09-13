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

namespace PhysX.GH.Kernel
{
    public class GhPxSystem
    {
        internal readonly LinkedList<PxGhRigidStatic> GhPxRigidStatics = new LinkedList<PxGhRigidStatic>();
        internal readonly LinkedList<PxGhRigidDynamic> GhPxRigidDynamics = new LinkedList<PxGhRigidDynamic>();


        public GhPxSystem()
        {
            PxGhManager.ClearScene();
        }


        public Vector3d Gravity
        {
            get { return PxGhManager.Scene.Gravity.ToRhinoVector(); }
            set { PxGhManager.Scene.Gravity = value.ToSystemVector(); }
        }


        public void Update(float timeStep = 0.01f, int steps = 1)
        {
            for (int i = 0; i < steps; i++)
            {
                PxGhManager.Scene.Simulate(timeStep);
                PxGhManager.Scene.FetchResults(true);
            }
        }


        public void Reset()
        {
            foreach (PxGhRigidDynamic o in GhPxRigidDynamics)
                o.Reset();
        }


        public void AddRigidStatic(PxGhRigidStatic o)
        {
            GhPxRigidStatics.AddLast(o);
            PxGhManager.Scene.AddActor(o.Actor);
        }


        public void AddRigidDynamic(PxGhRigidDynamic o)
        {
            GhPxRigidDynamics.AddLast(o);
            PxGhManager.Scene.AddActor(o.Actor);
        }


        public void RemoveRigidStatic(PxGhRigidStatic o)
        {
            GhPxRigidStatics.Remove(o);
            PxGhManager.Scene.RemoveActor(o.Actor);
        }


        public void RemoveRigidDynamic(PxGhRigidDynamic o)
        {
            GhPxRigidDynamics.Remove(o);
            PxGhManager.Scene.RemoveActor(o.Actor);
        }


        public List<GH_Mesh> GetRigidDynamicDisplayGhMeshes()
        {
            List<GH_Mesh> ghMeshes = new List<GH_Mesh>();
            foreach (PxGhRigidDynamic o in GhPxRigidDynamics)
                o.GetDisplayGhMeshes(ghMeshes);
            return ghMeshes;
        }


        public List<GH_Mesh> GetRigidStaticDisplayGhMeshes()
        {
            List<GH_Mesh> ghMeshes = new List<GH_Mesh>();
            foreach (PxGhRigidStatic o in GhPxRigidStatics)
                o.GetDisplayGhMeshes(ghMeshes);
            return ghMeshes;
        }
    }
}
