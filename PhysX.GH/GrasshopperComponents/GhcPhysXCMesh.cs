
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using PhysX.GH;
using PhysX.GH.Kernel;
using PhysX;
using Rhino.Geometry;

using Plane = Rhino.Geometry.Plane;

namespace PhysX.GH.GrasshopperComponents
{
    public class GhcPhysXCMesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcPhysXCMesh class.
        /// </summary>
        public GhcPhysXCMesh()
          : base("PX CompoConvexMesh", "PX CCxMesh",
              "Input ConvexMesh or PhysX will automatically convert it to convex and the result won't be as expected",
              "PhysX", "Geometries")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGroupParameter("Group Meshes", "GM", "Group of Meshes", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dynamic", "D", "Dynamic or Static, True: Dyamic, False: Static", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Material Property", "P", "PhysX Material", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("RigidBody", "R", "PhysX Rigid body", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_GeometryGroup g = new GH_GeometryGroup();
            bool isDynamic = true;
            Material iMaterial = PhysXManager.Physics.CreateMaterial(0.5f, 0.5f, 0.5f);

            DA.GetData(0, ref g);
            DA.GetData(1, ref isDynamic);
            DA.GetData(2, ref iMaterial);

            List<Mesh> meshes = new List<Mesh>();

            List<IGH_GeometricGoo> group = new List<IGH_GeometricGoo>(g.Objects);

            for (int i = 0; i < group.Count; i++)
            {
                Mesh m = new Mesh();
                group[i].CastTo(out m);
                meshes.Add(m);
            }


            if (isDynamic)
            {
                PxGhRigidDynamic rigidDynamic = new PxGhRigidDynamicCMesh(Plane.WorldXY, meshes, iMaterial, 1);
                DA.SetData(0, rigidDynamic);
            }
            else
            {
                PxGhRigidStatic rigidStatic = new PxGhRigidStaticCMesh(Plane.WorldXY, meshes, iMaterial);
                DA.SetData(0, rigidStatic);
            }
            

        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.CMesh;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1043b79b-ec38-47de-8ef9-dc638fbd0091"); }
        }
    }
}