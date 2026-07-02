using GF_Gereric;
using Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage.RailMotorInit;
using GKG;
using GKG.SubMM;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.RailMotor.CompUI_RailMotor.PageType.InitCfgPage
{
    /// <summary>
    /// 初始化配置页运行态宿主：负责 InitCfg 字节流与界面之间的读写与变更通知。
    /// </summary>
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        /// <summary>当前激活的初始化配置视图包装。</summary>
        private IPageTypeRunTimeCompUIView _initView;

        /// <summary>与后端同步的初始化配置数据缓存。</summary>
        private RailMotorSubMachineModulesInitCfg _data;

        /// <summary>为 true 时表示正在由代码回填界面，避免误触发脏标记。</summary>
        private bool _isApplyingViewData;

        /// <summary>初始化页面数据模型。</summary>
        protected override void _OnInit()
        {
            _data = new RailMotorSubMachineModulesInitCfg();
        }

        /// <summary>返回本页面对应的页面类型 ID。</summary>
        protected override PageTypeID _GetPageTypeID()
        {
            return ImeIOTConst.InitCfgPage;
        }

        /// <summary>按 ViewID 创建运行态视图，并绑定数据变更事件。</summary>
        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != InitCfgPageTypeConst.ViewID_RailMotorInit)
                return null;

            if (_initView != null)
                _initView.AfterModified -= OnAfterModified;

            _initView = new RailMotorInitPageTypeRunTimeCompUIView(this.CallBack);
            _initView.AfterModified += OnAfterModified;
            ApplyViewData(_data ?? new RailMotorSubMachineModulesInitCfg());
            return _initView;
        }

        /// <summary>将宿主下发的 InitCfg 字节流反序列化并刷新到界面。</summary>
        protected override void _SetData(byte[] data)
        {
            _data = data == null || data.Length == 0
                ? new RailMotorSubMachineModulesInitCfg()
                : JsonObjConvert.FromJSonBytes<RailMotorSubMachineModulesInitCfg>(data)
                    ?? new RailMotorSubMachineModulesInitCfg();
            ApplyViewData(_data);
        }

        /// <summary>从界面收集最新配置并序列化为字节流返回给宿主。</summary>
        protected override byte[] _GetData()
        {
            if (_initView is RailMotorInitPageTypeRunTimeCompUIView initView)
                _data = initView.GetData();

            return JsonObjConvert.ToJSonBytes(_data ?? new RailMotorSubMachineModulesInitCfg());
        }

        /// <summary>执行视图命令（本模块暂无自定义命令）。</summary>
        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        /// <summary>返回子页面运行时（本模块未使用子页）。</summary>
        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        /// <summary>校验当前配置是否合法。</summary>
        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }

        /// <summary>界面编辑后同步缓存并通知宿主数据已修改。</summary>
        private void OnAfterModified(object sender, EventArgs e)
        {
            if (_isApplyingViewData)
                return;

            if (_initView is RailMotorInitPageTypeRunTimeCompUIView initView)
                _data = initView.GetData();

            AfterDataModified?.Invoke(sender, e);
        }

        /// <summary>将配置数据写入运行态视图，且不触发脏标记。</summary>
        private void ApplyViewData(RailMotorSubMachineModulesInitCfg data)
        {
            if (_initView is not RailMotorInitPageTypeRunTimeCompUIView initView)
                return;

            _isApplyingViewData = true;
            try
            {
                initView.SetData(data);
            }
            finally
            {
                _isApplyingViewData = false;
            }
        }
    }
}
