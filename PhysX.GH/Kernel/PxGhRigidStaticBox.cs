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
        public PxGhRigidStaticBox(Plane frame, float length, float width, float height, Material material)
        {
            Actor.CreateShape(new BoxGeometry(length * 0.5f, width * 0.5f, height * 0.5f), material);
            Actor.GlobalPose = frame.ToMatrix();

            DisplayMeshes.Add(
                Mesh.CreateFromBox(
                    new Box(
                        frame,
                        new BoundingBox(-length * 0.5f, -width * 0.5f, -height * 0.5f, length * 0.5f, width * 0.5f, height * 0.5f)),
                    1, 1, 1));
        }


        public PxGhRigidStaticBox(Box box, Material material)
        {
            Point3d max = box.BoundingBox.Max;
            Point3d min = box.BoundingBox.Min;
            float length = (float)(max.X - min.X);
            float width = (float)(max.Y - min.Y);
            float height = (float)(max.Z - min.Z);
            Actor.CreateShape(new BoxGeometry(length * 0.5f, width * 0.5f, height * 0.5f), material);
            Actor.GlobalPose = box.Plane.ToMatrix();
            DisplayMeshes.Add(Mesh.CreateFromBox(box, 1, 1, 1));
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
