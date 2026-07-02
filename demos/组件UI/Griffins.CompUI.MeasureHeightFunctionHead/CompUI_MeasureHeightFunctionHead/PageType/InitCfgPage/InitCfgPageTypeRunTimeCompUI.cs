using GF_Gereric;
using Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage.MeasureHeightInit;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using GKG.SubMM;
using System;

namespace Griffins.CompUI.MeasureHeightFunctionHead.CompUI_MeasureHeightFunctionHead.PageType.InitCfgPage
{
    /// <summary>
    /// 初始化配置页面类型运行时组件UI
    /// </summary>
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private MeasureHeightInitPageTypeRunTimeCompUIView measureHeightInitView;
        private MeasureHeightFunctionHeadSubMachineModulesInitCfg data;

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void _OnInit()
        {
            data = new MeasureHeightFunctionHeadSubMachineModulesInitCfg();
        }

        /// <summary>
        /// 获取页面类型ID
        /// </summary>
        /// <returns>页面类型ID</returns>
        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.InitCfgPage; }

        /// <summary>
        /// 获取页面类型组件UI视图
        /// </summary>
        /// <param name="viewID">视图ID</param>
        /// <returns>页面类型组件UI视图</returns>
        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {
            if (viewID != InitCfgPageTypeConst.ViewID_MeasureHeightInit)
            {
                return null;
            }

            if (measureHeightInitView != null)
            {
                measureHeightInitView.AfterModified -= OnAfterModified;
                measureHeightInitView.Dispose();
            }

            measureHeightInitView = new MeasureHeightInitPageTypeRunTimeCompUIView(CallBack);
            measureHeightInitView.AfterModified += OnAfterModified;
            measureHeightInitView.SetData(data ?? new MeasureHeightFunctionHeadSubMachineModulesInitCfg());
            return measureHeightInitView;
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

            data = JsonObjConvert.FromJSonBytes<MeasureHeightFunctionHeadSubMachineModulesInitCfg>(rawData)
                ?? new MeasureHeightFunctionHeadSubMachineModulesInitCfg();
            measureHeightInitView?.SetData(data);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        protected override byte[] _GetData()
        {
            if (measureHeightInitView != null)
            {
                data = measureHeightInitView.GetData();
            }

            return JsonObjConvert.ToJSonBytes(data ?? new MeasureHeightFunctionHeadSubMachineModulesInitCfg());
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
            if (measureHeightInitView != null)
            {
                data = measureHeightInitView.GetData();
            }

            AfterDataModified?.Invoke(sender, e);
        }
    }
}
