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
    public class GhcPhysXBox : GH_Component
    {
        public GhcPhysXBox()
            : base(
                "PX Box",
                "PX Box",
                "Create a PhysX box rigid body",
                "PhysX",
                "Geometries")
        {
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.Box;
        public override Guid ComponentGuid => new Guid("fdf90b10-f520-4ea8-abb3-a85fafca0298");


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBoxParameter("Box", "Box", "Box", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dynamic", "Dynamic", "Dynamic or Static, True: Dynamic, False: Static", GH_ParamAccess.item, true);
            pManager.AddGenericParameter("Material (Optional)", "Material", "PhysX material", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Mass", "Mass", "Mass", GH_ParamAccess.item, 1.0);
            pManager.AddVectorParameter("Initial Linear Velocity", "Linear Vel.", "Initial linear velocity", GH_ParamAccess.item, Vector3d.Zero);
            pManager.AddVectorParameter("Initial Angular Velocity", "Angular Vel.", "Initial angular velocity", GH_ParamAccess.item, Vector3d.Zero);
            pManager.AddMeshParameter("Displayed Meshes (Optional)", "Displayed Meshes", "Custom displayed meshes", GH_ParamAccess.list);
            pManager[6].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Rigid Box", "R", "PhysX rigid box", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Box iBox = Box.Empty;
            bool iDynamic = true;
            Material iMaterial = PxGhManager.DefaultMaterial;
            double iMass = double.NaN;
            Vector3d iInitialLinearVelocity = Vector3d.Unset;
            Vector3d iInitialLAngularVelocity = Vector3d.Unset;
            List<Mesh> iDisplayeMeshes = new List<Mesh>();

            DA.GetData(0, ref iBox);
            DA.GetData(1, ref iDynamic);
            DA.GetData(2, ref iMaterial);
            DA.GetData(3, ref iMass);
            DA.GetData(4, ref iInitialLinearVelocity);
            DA.GetData(5, ref iInitialLAngularVelocity);
            DA.GetDataList(6, iDisplayeMeshes);

            if (iDynamic)
                DA.SetData(0, new PxGhRigidDynamicBox(iBox, iMaterial, (float)iMass, iInitialLinearVelocity, iInitialLAngularVelocity));
            else
                DA.SetData(0, new PxGhRigidStaticBox(iBox, iMaterial));
        }
    }
}