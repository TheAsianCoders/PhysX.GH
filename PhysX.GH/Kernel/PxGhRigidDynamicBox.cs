using Rhino.Geometry;

using Plane = Rhino.Geometry.Plane;


namespace PhysX.GH.Kernel
{
    public class PxGhRigidDynamicBox : PxGhRigidDynamic
    {
        private float hX, hY, hZ;


        public PxGhRigidDynamicBox(
            Box box,
            Material material,
            float mass,
            Vector3d initialLinearVelocity,
            Vector3d initialAngularVelocity)
            : this(
                box.Plane,
                (float)(box.X.Max - box.X.Min),
                (float)(box.Y.Max - box.Y.Min),
                (float)(box.Z.Max - box.Z.Min),
                material,
                mass,
                initialLinearVelocity,
                initialAngularVelocity)
        {
        }


        public PxGhRigidDynamicBox(
            Plane frame,
            float length,
            float width,
            float height,
            Material material,
            float mass,
            Vector3d initialLinearVelocity,
            Vector3d initialAngularVelocity)
            : base(
                frame,
                initialLinearVelocity,
                initialAngularVelocity)
        {
            Actor.CreateShape(new BoxGeometry(length * 0.5f, width * 0.5f, height * 0.5f), material);
            Mass = mass;
            hX = length * 0.5f;
            hY = width * 0.5f;
            hZ = height * 0.5f;
            DisplayMeshes.Add(Mesh.CreateFromBox(new BoundingBox(-hX, -hY, -hZ, hX, hY, hZ), 1, 1, 1));
        }
    }
}
