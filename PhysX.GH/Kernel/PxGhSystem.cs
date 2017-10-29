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

namespace PhysX.GH
{
    public class GhPxSystem
    {
        internal LinkedList<PxGhRigidStatic> ghPxRigidStatics;
        internal LinkedList<PxGhRigidDynamic> ghPxRigidDynamics;


        public GhPxSystem()
        {
            ghPxRigidStatics = new LinkedList<PxGhRigidStatic>();
            ghPxRigidDynamics = new LinkedList<PxGhRigidDynamic>();
            PhysXManager.ClearScene();
        }


        public Vector3d Gravity
        {
            get { return PhysXManager.Scene.Gravity.ToRhinoVector(); }
            set { PhysXManager.Scene.Gravity = value.ToSystemVector(); }
        }


        public void Update(float timeStep = 0.01f, int steps = 1)
        {
            for (int i = 0; i < steps; i++)
            {
                PhysXManager.Scene.Simulate(timeStep);
                PhysXManager.Scene.FetchResults(true);
            }
        }


        public void Reset()
        {
            foreach (PxGhRigidDynamic o in ghPxRigidDynamics)
                o.Reset();
        }


        public void AddRigidStatic(PxGhRigidStatic o)
        {
            ghPxRigidStatics.AddLast(o);
            PhysXManager.Scene.AddActor(o.actor);
        }


        public void AddRigidDynamic(PxGhRigidDynamic o)
        {
            ghPxRigidDynamics.AddLast(o);
            PhysXManager.Scene.AddActor(o.actor);
        }


        public void RemoveRigidStatic(PxGhRigidStatic o)
        {
            ghPxRigidStatics.Remove(o);
            PhysXManager.Scene.RemoveActor(o.actor);
        }


        public void RemoveRigidDynamic(PxGhRigidDynamic o)
        {
            ghPxRigidDynamics.Remove(o);
            PhysXManager.Scene.RemoveActor(o.actor);
        }


        public List<GH_Mesh> GetRigidDynamicDisplayGhMeshes()
        {
            List<GH_Mesh> ghMeshes = new List<GH_Mesh>();
            foreach (PxGhRigidDynamic o in ghPxRigidDynamics)
                o.GetDisplayGhMeshes(ghMeshes);
            return ghMeshes;
        }


        public List<GH_Mesh> GetRigidStaticDisplayGhMeshes()
        {
            List<GH_Mesh> ghMeshes = new List<GH_Mesh>();
            foreach (PxGhRigidStatic o in ghPxRigidStatics)
                o.GetDisplayGhMeshes(ghMeshes);
            return ghMeshes;
        }
    }
}
