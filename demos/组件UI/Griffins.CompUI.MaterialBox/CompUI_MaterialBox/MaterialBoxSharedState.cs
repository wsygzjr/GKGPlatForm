using System;
using BackendMaterialBoxFactoryCfg = GKG.SubMM.MaterialBoxSubMachineModulesFactoryCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox
{
    /// <summary>
    /// 在不同页面之间共享料盒工厂配置数据，方便初始化页与配方页读取同一份配置。
    /// </summary>
    internal static class MaterialBoxSharedState
    {
        /// <summary>当前缓存的料盒工厂配置。</summary>
        private static BackendMaterialBoxFactoryCfg _factoryCfg = new();

        /// <summary>工厂配置发生变化时触发，通知其它页面同步刷新。</summary>
        public static event EventHandler? FactoryCfgChanged;

        /// <summary>获取当前共享的料盒工厂配置。</summary>
        public static BackendMaterialBoxFactoryCfg FactoryCfg => _factoryCfg;

        /// <summary>
        /// 更新共享的料盒工厂配置，并广播配置变更事件。
        /// </summary>
        public static void UpdateFactoryCfg(BackendMaterialBoxFactoryCfg? factoryCfg)
        {
            _factoryCfg = factoryCfg ?? new BackendMaterialBoxFactoryCfg();
            FactoryCfgChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
