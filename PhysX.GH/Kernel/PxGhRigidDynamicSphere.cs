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
    public class PxGhRigidDynamicSphere : PxGhRigidDynamic
    {
        private Mesh sphereMesh;

        public PxGhRigidDynamicSphere(Plane plane, float radius, Material material, float mass)
        {
            actor = PhysXManager.Physics.CreateRigidDynamic();
            actor.CreateShape(new SphereGeometry(radius), material);
            initialGlobalPose = actor.GlobalPose = plane.ToMatrix();
            actor.UpdateMassAndInertia(mass);
            sphereMesh = Mesh.CreateFromSphere(new Sphere(Point3d.Origin, radius), 12, 6);
        }


        public override void GetDisplayGhMeshes(List<GH_Mesh> ghMeshes)
        {
            Mesh mesh = sphereMesh.DuplicateMesh();
            mesh.Transform(actor.GlobalPose.ToRhinoTransform());
            ghMeshes.Add(new GH_Mesh(mesh));
        }
    }
}
