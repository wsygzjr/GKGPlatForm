using GF_Gereric;
using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage.MeasureHeightFactory;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.FactoryCfgPage
{
    /// <summary>
    /// 工厂配置页面类型运行时组件UI
    /// </summary>
    internal class FactoryCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private MeasureHeightFactoryPageTypeRunTimeCompUIView measureHeightFactoryView;
        private MeasureHeightFunctionHeadSubMachineModulesFactoryCfg data;

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void _OnInit()
        {
            data = new MeasureHeightFunctionHeadSubMachineModulesFactoryCfg();
        }

        /// <summary>
        /// 获取页面类型ID
        /// </summary>
        /// <returns>页面类型ID</returns>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.FactoryCfgPage; }

        /// <summary>
        /// 获取页面类型组件UI视图
        /// </summary>
        /// <param name="viewID">视图ID</param>
        /// <returns>页面类型组件UI视图</returns>
        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != FactoryCfgPageTypeConst.ViewID_MeasureHeightFactory)
            {
                return null;
            }

            if (measureHeightFactoryView != null)
            {
                measureHeightFactoryView.AfterModified -= OnAfterModified;
            }

            measureHeightFactoryView = new MeasureHeightFactoryPageTypeRunTimeCompUIView();
            measureHeightFactoryView.AfterModified += OnAfterModified;
            measureHeightFactoryView.SetData(data ?? new MeasureHeightFunctionHeadSubMachineModulesFactoryCfg());
            return measureHeightFactoryView;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="rawData">原始数据</param>
        protected override void _SetData(byte[] rawData)
        {
            if (rawData == null)
            {
                return;
            }

            data = JsonObjConvert.FromJSonBytes<MeasureHeightFunctionHeadSubMachineModulesFactoryCfg>(rawData)
                ?? new MeasureHeightFunctionHeadSubMachineModulesFactoryCfg();
            measureHeightFactoryView?.SetData(data);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        protected override byte[] _GetData()
        {
            if (measureHeightFactoryView != null)
            {
                data = measureHeightFactoryView.GetData();
            }

            data ??= new MeasureHeightFunctionHeadSubMachineModulesFactoryCfg();
            return JsonObjConvert.ToJSonBytes(data);
        }

        /// <summary>
        /// 执行视图命令
        /// </summary>
        /// <param name="cmdID">命令ID</param>
        /// <param name="cmdParam">命令参数</param>
        /// <returns>执行结果</returns>
        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        /// <summary>
        /// 获取子页面运行时
        /// </summary>
        /// <param name="subPageKindInfo">子页面类型信息</param>
        /// <returns>子页面运行时</returns>
        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            return null;
        }

        /// <summary>
        /// 检查数据有效性
        /// </summary>
        /// <param name="inValidMsg">无效消息</param>
        /// <returns>是否有效</returns>
        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = Array.Empty<string>();
            return true;
        }

        /// <summary>
        /// 数据修改后事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OnAfterModified(object sender, EventArgs e)
        {
            if (measureHeightFactoryView != null)
            {
                data = measureHeightFactoryView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }
    }
}
