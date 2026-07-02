using GF_Gereric;
using GKG;
using GKG.SubMM.StorageDeviceModule;
using GKG.SubMM.TransportMechanismModule;
using Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage.MaterialBoxConfig;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using MaterialBoxSubMachineModules.FeedPort;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BackendMaterialBoxPPCfg = GKG.SubMM.MaterialBoxSubMachineModulesPPCfg;

namespace Griffins.CompUI.MaterialBox.CompUI_MaterialBox.PageType.RecipeCfgPage
{
    /// <summary>
    /// 配方页运行态入口，负责料盒配方前后端数据转换以及视图初始化。
    /// </summary>
    internal class RecipeCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        /// <summary>上料 Z 轴默认显示名称。</summary>
        private const string LoadAxisName = "\u4e0a\u6599Z\u8f74";
        /// <summary>下料 Z 轴默认显示名称。</summary>
        private const string UnloadAxisName = "\u4e0b\u6599Z\u8f74";
        /// <summary>上层料盒在存储列表中的索引。</summary>
        private const int UpperIndex = 0;
        /// <summary>下层料盒在存储列表中的索引。</summary>
        private const int LowerIndex = 1;

        /// <summary>配方页主视图对象。</summary>
        private IPageTypeRunTimeCompUIView? _materialBoxConfigView;
        /// <summary>前端使用的配方数据对象。</summary>
        private BackendMaterialBoxPPCfg _data = new();
        /// <summary>后端保存用的配方数据对象。</summary>
        private BackendMaterialBoxPPCfg _backendData = new();

        /// <summary>初始化配方页运行态默认数据。</summary>
        protected override void _OnInit()
        {
            _data = new BackendMaterialBoxPPCfg();
            _backendData = new BackendMaterialBoxPPCfg();
        }

        /// <summary>返回配方页对应的页面类型 ID。</summary>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.PPCfgPage; }

        /// <summary>按视图 ID 返回配方页运行态视图。</summary>
        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != RecipeCfgPageTypeConst.ViewID_MaterialBoxConfig)
                return null;

            if (_materialBoxConfigView == null)
            {
                _materialBoxConfigView = new MaterialBoxConfigPageTypeRunTimeCompUIView(this.CallBack);
                (_materialBoxConfigView as MaterialBoxConfigPageTypeRunTimeCompUIView)?.SetData(_data ?? new BackendMaterialBoxPPCfg());
            }

            return _materialBoxConfigView;
        }

        /// <summary>接收后端配方数据并刷新到前端页面。</summary>
        protected override void _SetData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                _backendData = new BackendMaterialBoxPPCfg();
                _data = new BackendMaterialBoxPPCfg();
            }
            else
            {
                _backendData = JsonObjConvert.FromJSonBytes<BackendMaterialBoxPPCfg>(data);
                _data = _backendData ?? new BackendMaterialBoxPPCfg();
            }

            if (_materialBoxConfigView is MaterialBoxConfigPageTypeRunTimeCompUIView view)
                view.SetData(_data);
        }

        /// <summary>从前端页面收集配方数据并转换成后端保存格式。</summary>
        protected override byte[] _GetData()
        {
            if (_materialBoxConfigView is MaterialBoxConfigPageTypeRunTimeCompUIView view)
                _data = view.GetData();

            _backendData = _data ?? new BackendMaterialBoxPPCfg();
            return JsonObjConvert.ToJSonBytes(_backendData ?? new BackendMaterialBoxPPCfg()) ?? Array.Empty<byte>();
        }

        /// <summary>当前配方页没有额外视图命令处理逻辑。</summary>
        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        /// <summary>当前配方页没有子页面运行时对象。</summary>
        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        /// <summary>当前配方页默认通过数据校验。</summary>
        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }

    }
}
