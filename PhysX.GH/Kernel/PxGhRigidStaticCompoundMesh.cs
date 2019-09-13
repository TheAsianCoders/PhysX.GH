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
    public class PxGhRigidStaticCompoundMesh : PxGhRigidStatic
    {
        public PxGhRigidStaticCompoundMesh(Plane frame, List<Mesh> meshes, Material material)
        {
            DisplayMeshes = new List<Mesh>(meshes);
            Actor.GlobalPose = frame.ToMatrix();

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

                ConvexMeshGeometry convexMeshGeometry = new ConvexMeshGeometry(PxGhManager.Physics.CreateConvexMesh(memoryStream));

                Actor.CreateShape(convexMeshGeometry, material);
            }
        }


        public override void GetDisplayGhMeshes(List<GH_Mesh> ghMeshes)
        {
            foreach (Mesh displayMesh in DisplayMeshes)
                ghMeshes.Add(new GH_Mesh(displayMesh));
        }


        public override void GetDisplayMeshes(List<Mesh> meshes)
        {
            meshes.AddRange(DisplayMeshes);
        }
    }
}
