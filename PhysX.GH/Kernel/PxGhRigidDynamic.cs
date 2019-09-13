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
    public abstract class PxGhRigidDynamic : PxGhRigidBody
    {
        internal RigidDynamic Actor = PxGhManager.Physics.CreateRigidDynamic();
        protected readonly Matrix4x4 InitialGlobalPose = Matrix4x4.Identity;
        protected readonly Vector3 InitialLinearVelocity = Vector3.Zero;
        protected readonly Vector3 InitialAngularVelocity = Vector3.Zero;


        public Vector3 LinearVelocity => Actor.LinearVelocity;
        public Vector3 AngularVelocity => Actor.AngularVelocity;
        public Plane Frame => Actor.GlobalPose.ToRhinoPlane();
        public Transform Transform => Actor.GlobalPose.ToRhinoTransform();


        protected PxGhRigidDynamic(Plane frame)
        {
            Actor.GlobalPose = this.InitialGlobalPose = frame.ToMatrix();
            Reset();
        }


        protected PxGhRigidDynamic(Plane frame, Vector3d initialLinearVelocity, Vector3d initialAngularVelocity)
        {
            Actor.GlobalPose = this.InitialGlobalPose = frame.ToMatrix();
            Actor.LinearVelocity = this.InitialLinearVelocity = initialLinearVelocity.ToSystemVector();
            Actor.AngularVelocity = this.InitialAngularVelocity = initialAngularVelocity.ToSystemVector();
        }


        public double Mass
        {
            set { Actor.SetMassAndUpdateInertia((float)value); }
            get { return Actor.Mass; }
        }


        public void Reset()
        {
            Actor.GlobalPose = InitialGlobalPose;
            Actor.LinearVelocity = InitialLinearVelocity;
            Actor.AngularVelocity = InitialAngularVelocity;
        }


        public override void GetDisplayGhMeshes(List<GH_Mesh> ghMeshes)
        {
            Transform transform = Actor.GlobalPose.ToRhinoTransform();
            foreach (Mesh displayMesh in DisplayMeshes)
            {
                Mesh mesh = (Mesh)displayMesh.DuplicateShallow();
                mesh.Transform(transform);
                ghMeshes.Add(new GH_Mesh(mesh));
            }
        }


        public override void GetDisplayMeshes(List<Mesh> meshes)
        {
            Transform transform = Actor.GlobalPose.ToRhinoTransform();
            foreach (Mesh displayMesh in DisplayMeshes)
            {
                Mesh mesh = (Mesh)displayMesh.DuplicateShallow();
                mesh.Transform(transform);
                meshes.Add(mesh);
            }
        }
    }
}
