using System;

namespace Griffins.CompUI.RailMotor.CompUI_RailMotor.Interop
{
    /// <summary>
    /// 轨道运输电机 CompUI 与后端子机械模组对接用的常量定义。
    /// </summary>
    internal static class RailMotorInteropConst
    {
        /// <summary>后端 SubMM 模型名，须与 RailMotorSubMachineModulesConst.SubMMModelStr 一致。</summary>
        public const string ModelName = "RailMotor";

        /// <summary>轨道运输电机子机械模组对象 GUID。</summary>
        public const string SubMMObjID = "7D7A868F-3C40-4AA5-A2C1-E6C072EEA3D1";

        /// <summary>组件在配置界面中的显示名称。</summary>
        public const string DisplayName = "轨道运输电机";

        /// <summary>运行时向配置服务请求可选轴列表的命令 ID。</summary>
        public const string CmdGetAxisOptions = "GetAxisOptions";

        /// <summary>
        /// 判断给定 GUID 是否为当前 CompUI 支持的运输电机实例。
        /// </summary>
        public static bool IsSupported(Guid guid)
            => guid == Guid.Parse(SubMMObjID);
    }
}
