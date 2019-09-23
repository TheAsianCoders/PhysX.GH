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
        public PxGhRigidDynamicSphere(Plane frame, float radius, Material material, float mass, Vector3d initialLinearVelocity, Vector3d initialAngularVelocity)
            : base(frame, initialLinearVelocity, initialAngularVelocity)
        {
            Actor.CreateShape(new SphereGeometry(radius), material);
            Mass = mass;
            DisplayMeshes.Add(Mesh.CreateFromSphere(new Sphere(Point3d.Origin, radius), 12, 6));
        }


        public PxGhRigidDynamicSphere(Sphere sphere, Material material, float mass, Vector3d initialLinearVelocity, Vector3d initialAngularVelocity)
            : this(sphere.EquatorialPlane, (float)sphere.Radius, material, mass, initialAngularVelocity, initialAngularVelocity)
        {}
    }
}
