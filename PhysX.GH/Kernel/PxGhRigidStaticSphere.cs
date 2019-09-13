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
        public PxGhRigidStaticSphere(Plane frame, float radius, Material material)
        {
            Actor.CreateShape(new SphereGeometry(radius), material);
            Actor.GlobalPose = frame.ToMatrix();
            DisplayMeshes.Add(Mesh.CreateFromSphere(new Sphere(frame, radius), 32, 16));
        }


        public override void GetDisplayGhMeshes(List<GH_Mesh> ghMeshes)
        {
            ghMeshes.Add(new GH_Mesh(DisplayMeshes[0]));
        }


        public override void GetDisplayMeshes(List<Mesh> meshes)
        {
            meshes.Add(DisplayMeshes[0]);
        }
    }
}
