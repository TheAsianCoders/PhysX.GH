using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using PhysX;
using Rhino.Geometry;

namespace PhysX.GH.Kernel
{
    public class PxGhRigidStaticSphere : PxGhRigidStatic
    {
        private GH_Mesh ghMesh;

        public PxGhRigidStaticSphere(Plane plane, float radius, Material material)
        {
            actor = PhysXManager.Physics.CreateRigidStatic();
            actor.CreateShape(new SphereGeometry(radius), material);
            actor.GlobalPose = plane.ToMatrix();
            ghMesh = new GH_Mesh(Mesh.CreateFromSphere(new Sphere(plane, radius), 32, 16));
        }

        public override void GetDisplayGhMeshes(List<GH_Mesh> ghMeshes)
        {
            ghMeshes.Add(ghMesh);
        }
    }
}
