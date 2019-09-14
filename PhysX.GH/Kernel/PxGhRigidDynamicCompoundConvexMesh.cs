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
    public class PxGhRigidDynamiCompoundConvexMesh : PxGhRigidDynamic
    {
        public PxGhRigidDynamiCompoundConvexMesh(List<Mesh> meshes, Plane frame, Material material, float mass, Vector3d initialLinearVelocity, Vector3d initialAngularVelocity)
            : base(frame, initialLinearVelocity, initialAngularVelocity)
        {
            DisplayMeshes = new List<Mesh>(meshes);

            foreach (Mesh mesh in DisplayMeshes)
            {
                mesh.Transform(Transform.PlaneToPlane(frame, Plane.WorldXY));

                Vector3[] vertices = new Vector3[mesh.Vertices.Count];
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = mesh.Vertices[i].ToSystemVector();

                int[] faceVertexIndices = new int[mesh.Faces.Count * 3];
                for (int i = 0; i < mesh.Faces.Count; i++)
                {
                    MeshFace face = mesh.Faces[i];
                    faceVertexIndices[3 * i + 0] = face.A;
                    faceVertexIndices[3 * i + 1] = face.B;
                    faceVertexIndices[3 * i + 2] = face.C;
                }

                ConvexMeshDesc convexMeshDescription = new ConvexMeshDesc();
                convexMeshDescription.Flags = ConvexFlag.ComputeConvex;
                convexMeshDescription.SetPositions(vertices);
                convexMeshDescription.SetTriangles(faceVertexIndices);

                MemoryStream memoryStream = new MemoryStream();
                PxGhManager.Physics.CreateCooking().CookConvexMesh(convexMeshDescription, memoryStream);
                memoryStream.Position = 0;

                ConvexMesh convexMesh = PxGhManager.Physics.CreateConvexMesh(memoryStream);
                ConvexMeshGeometry convexMeshGeometry = new ConvexMeshGeometry(convexMesh);

                Actor.CreateShape(convexMeshGeometry, material);
            }

            Actor.SetMassAndUpdateInertia(mass);
        }
    }
}
