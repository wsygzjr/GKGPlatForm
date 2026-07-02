using Griffins.ImeIOT;

namespace GKG.SubMM
{
    internal class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
    {
        public ImeCompEventDefInfoList Events => MotionCalculateSubMachineModulesConst.Events;

        public ImeCompMethodDefInfoList Methods => MotionCalculateSubMachineModulesConst.Methods;

        public ImeCompPropDefInfoList UIDataObjProps => throw new NotImplementedException();

        public ImeCompMethodDefInfoList UICommands => throw new NotImplementedException();
    }
}