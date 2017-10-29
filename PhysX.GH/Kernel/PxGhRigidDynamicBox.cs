using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using PhysX;
using Rhino.Geometry;
using Plane = Rhino.Geometry.Plane;

namespace PhysX.GH.Kernel
{
    public class PxGhRigidDynamicBox : PxGhRigidDynamic
    {
        private double hX, hY, hZ;

        public PxGhRigidDynamicBox(Plane plane, float length, float width, float height, Material material, float mass)
        {
            actor = PhysXManager.Physics.CreateRigidDynamic();           
            actor.CreateShape(new BoxGeometry(length * 0.5f, width * 0.5f, height * 0.5f), material);
            initialGlobalPose = actor.GlobalPose = plane.ToMatrix();
            actor.UpdateMassAndInertia(mass);

            hX = length * 0.5;
            hY = width * 0.5;
            hZ = height * 0.5;
        }

        public override void GetDisplayGhMeshes(List<GH_Mesh> ghMeshes)
        {
            Plane plane = actor.GlobalPose.ToRhinoPlane();
            Point3d A = plane.Origin - hX * plane.XAxis - hY * plane.YAxis - hZ * plane.ZAxis;
            Point3d B = plane.Origin + hX * plane.XAxis - hY * plane.YAxis - hZ * plane.ZAxis;
            Point3d C = plane.Origin + hX * plane.XAxis + hY * plane.YAxis - hZ * plane.ZAxis;
            Point3d D = plane.Origin - hX * plane.XAxis + hY * plane.YAxis - hZ * plane.ZAxis;
            Point3d E = plane.Origin - hX * plane.XAxis - hY * plane.YAxis + hZ * plane.ZAxis;
            Point3d F = plane.Origin + hX * plane.XAxis - hY * plane.YAxis + hZ * plane.ZAxis;
            Point3d G = plane.Origin + hX * plane.XAxis + hY * plane.YAxis + hZ * plane.ZAxis;
            Point3d H = plane.Origin - hX * plane.XAxis + hY * plane.YAxis + hZ * plane.ZAxis;

            Mesh mesh = new Mesh();
            mesh.Vertices.AddVertices(new[] { D, C, B, A, E, F, G, H, A, B, F, E, B, C, G, F, C, D, H, G, D, A, E, H });
            mesh.Faces.AddFace(0, 1, 2, 3);
            mesh.Faces.AddFace(4, 5, 6, 7);
            mesh.Faces.AddFace(8, 9, 10, 11);
            mesh.Faces.AddFace(12, 13, 14, 15);
            mesh.Faces.AddFace(16, 17, 18, 19);
            mesh.Faces.AddFace(20, 21, 22, 23);         

            ghMeshes.Add(new GH_Mesh(mesh));
        }
    }
}
