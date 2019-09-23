using System;
using Rhino.Commands;


namespace PhysX.GH
{
    public static class PxGhManager
    {
        internal static Foundation Foundation;
        internal static Physics Physics;
        public static Scene Scene { get; private set; }
        internal static Material DefaultMaterial;


        internal static void Initialize()
        {
            if (Foundation != null) return;
            Foundation = new Foundation();
            Physics = new Physics(Foundation);
            DefaultMaterial = Physics.CreateMaterial(0.5f, 0.5f, 0.5f);
            Scene = Physics.CreateScene();

            Command.BeginCommand += DisposePhysXOnGrasshopperUnloaded;
        }


        private static void DisposePhysXOnGrasshopperUnloaded(object sender, CommandEventArgs args)
        {
            // Check if the Rhino command being executed is "GrasshopperUnloadPlugin" or "GrasshopperReloadAssemblies"
            if ((args.CommandId != new Guid("fc760140-ef99-42da-b2b4-2cdff7a4c8c4")) &&
                 args.CommandId != new Guid("6b0eef26-3046-4f19-b4de-ffe6539d60e7"))
                return;

            Command.BeginCommand -= DisposePhysXOnGrasshopperUnloaded;
            Destroy();
        }


        internal static void ClearScene()
        {
            Scene.Dispose();
            Scene = Physics.CreateScene();
        }


        internal static void Destroy()
        {
            Scene.Dispose();
            Physics.Dispose();
            Foundation.Dispose();
        }
    }
}
