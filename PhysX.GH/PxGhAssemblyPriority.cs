using Grasshopper.Kernel;


namespace PhysX.GH
{
    public class PxGhAssemblyPriority : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            PxGhManager.Initialize();
            return GH_LoadingInstruction.Proceed;
        }
    }
}