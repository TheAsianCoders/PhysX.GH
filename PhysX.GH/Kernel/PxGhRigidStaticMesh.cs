using System;
using System.Collections.Generic;
using System.IO;
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
    public class PxGhRigidStaticMesh : PxGhRigidStatic
    {
        private GH_Mesh ghMesh;

        public PxGhRigidStaticMesh(Plane plane, Mesh mesh, Material material)
        {
            
            Mesh meshLocal = mesh.DuplicateMesh();
            meshLocal.Transform(Transform.PlaneToPlane(plane, Plane.WorldXY));

            List<Vector3> points = new List<Vector3>();
            foreach (Point3d v in meshLocal.Vertices) points.Add(new Vector3((float)v.X, (float)v.Y, (float)v.Z));

            List<int> faceVertexIndices = new List<int>();
            foreach (MeshFace face in meshLocal.Faces)
            {
                faceVertexIndices.Add(face.A);
                faceVertexIndices.Add(face.B);
                faceVertexIndices.Add(face.C);
            }

            ConvexMeshDesc convexMeshDescription = new ConvexMeshDesc();
            convexMeshDescription.Flags = ConvexFlag.ComputeConvex;
            convexMeshDescription.SetPositions(points.ToArray());
            convexMeshDescription.SetTriangles(faceVertexIndices.ToArray());

            MemoryStream memoryStream = new MemoryStream();
            PhysXManager.Physics.CreateCooking().CookConvexMesh(convexMeshDescription, memoryStream);
            memoryStream.Position = 0;

            ConvexMesh convexMesh = PhysXManager.Physics.CreateConvexMesh(memoryStream);
            ConvexMeshGeometry convexMeshGeometry = new ConvexMeshGeometry(convexMesh);

            actor = PhysXManager.Physics.CreateRigidStatic();
            actor.GlobalPose = plane.ToMatrix();
            actor.CreateShape(convexMeshGeometry, material);

            ghMesh = new GH_Mesh(mesh);
        }


        public override void GetDisplayGhMeshes(List<GH_Mesh> ghMeshes)
        {
            ghMeshes.Add(ghMesh);
        }
    }
}
