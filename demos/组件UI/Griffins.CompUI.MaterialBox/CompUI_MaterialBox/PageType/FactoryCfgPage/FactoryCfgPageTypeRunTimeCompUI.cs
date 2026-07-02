using GF_Gereric;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage.MaterialBoxFactory;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;
using BackendMaterialBoxFactoryCfg = GKG.SubMM.MaterialBoxSubMachineModulesFactoryCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.FactoryCfgPage
{
    /// <summary>
    /// 工厂配置页运行态入口，负责前后端工厂配置数据的转换与同步。
    /// </summary>
    internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        /// <summary>前端使用的工厂配置数据。</summary>
        private BackendMaterialBoxFactoryCfg _data = new();
        /// <summary>后端使用的工厂配置数据。</summary>
        private BackendMaterialBoxFactoryCfg _backendData = new();
        /// <summary>工厂配置页运行态视图对象。</summary>
        private MaterialBoxFactoryPageTypeRunTimeCompUIView? _view;

        /// <summary>初始化运行态默认数据，并同步到共享状态。</summary>
        protected override void _OnInit()
        {
            _data = new BackendMaterialBoxFactoryCfg();
            _backendData = _data;
        }

        /// <summary>返回运行态页面类型 ID。</summary>
        protected override PageTypeID _GetPageTypeID()
        {
            return PageTypeID.Parse("FactoryCfgPage");
        }

        /// <summary>按视图 ID 返回对应的运行态页面。</summary>
        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != FactoryCfgPageTypeConst.ViewID_MaterialBoxFactory)
                return null;

            if (_view == null)
            {
                _view = new MaterialBoxFactoryPageTypeRunTimeCompUIView(this.CallBack);
                _view.SetData(_data);
                _view.AfterModified += (_, e) =>
                {
                    _data = _view.GetData();
                    _backendData = _data ?? new BackendMaterialBoxFactoryCfg();
                    MaterialBoxSharedState.UpdateFactoryCfg(_data);
                    AfterDataModified?.Invoke(this, e);
                };
            }

            return _view;
        }

        /// <summary>接收后端字节数据并刷新前端工厂配置页面。</summary>
        protected override void _SetData(byte[] data)
        {
            _backendData = data == null || data.Length == 0
                ? new BackendMaterialBoxFactoryCfg()
                : JsonObjConvert.FromJSonBytes<BackendMaterialBoxFactoryCfg>(data) ?? new BackendMaterialBoxFactoryCfg();

            _data = _backendData;
            MaterialBoxSharedState.UpdateFactoryCfg(_data);
            _view?.SetData(_data);
        }

        /// <summary>收集前端页面数据并转换成后端保存格式。</summary>
        protected override byte[] _GetData()
        {
            if (_view != null)
                _data = _view.GetData();

            _backendData = _data ?? new BackendMaterialBoxFactoryCfg();
            MaterialBoxSharedState.UpdateFactoryCfg(_data);
            return JsonObjConvert.ToJSonBytes(_backendData ?? new BackendMaterialBoxFactoryCfg());
        }

        /// <summary>当前工厂配置页没有额外的视图命令需要处理。</summary>
        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        /// <summary>当前工厂配置页没有子页面运行时对象。</summary>
        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        /// <summary>当前工厂配置页默认返回校验通过。</summary>
        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }

    }
}
