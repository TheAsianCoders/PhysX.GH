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
    public abstract class PxGhRigidStatic : PxGhRigidBody
    {
        internal RigidStatic Actor = PxGhManager.Physics.CreateRigidStatic();

        public void SetFrame(Plane frame)
        {
            Plane currentFrame = Actor.GlobalPose.ToRhinoPlane();
            Transform transform = Transform.PlaneToPlane(currentFrame, frame);
            foreach (Mesh displayMesh in DisplayMeshes)
                displayMesh.Transform(transform);
            Actor.GlobalPose = frame.ToMatrix();
        }
    }
}
