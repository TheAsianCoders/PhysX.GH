using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;
using PhysX;
using Rhino.Geometry;

namespace PhysX.GH.Kernel
{
    public abstract class PxGhRigidDynamic : PxGhRigidBody
    {
        internal RigidDynamic actor;
        internal Matrix4x4 initialGlobalPose;
        internal Vector3 initialLinearVelocity = Vector3.Zero;
        internal Vector3 initialAngularVelocity = Vector3.Zero;

        public void Reset()
        {
            actor.GlobalPose = initialGlobalPose;    
            actor.LinearVelocity = initialLinearVelocity;
            actor.AngularVelocity = initialAngularVelocity;
        }
    }
}
