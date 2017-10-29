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
    public class PxGhRigidStaticBox : PxGhRigidStatic
    {
        private GH_Mesh ghMesh;

        public PxGhRigidStaticBox(Plane plane, float length, float width, float height, Material material)
        {
            actor = PhysXManager.Physics.CreateRigidStatic();
            actor.CreateShape(new BoxGeometry(length * 0.5f, width * 0.5f, height * 0.5f), material);
            actor.GlobalPose = plane.ToMatrix();

            ghMesh = new GH_Mesh(
                Mesh.CreateFromBox(
                    new Box(
                        plane,
                        new BoundingBox(-length * 0.5f, -width * 0.5f, -height * 0.5f, length * 0.5f, width * 0.5f, height * 0.5f)), 
                    1, 1, 1));
        }


        public override void GetDisplayGhMeshes(List<GH_Mesh> ghMeshes)
        {
            ghMeshes.Add(ghMesh);
        }
    }
}
