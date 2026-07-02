using GF_Gereric;
using Griffins.ImeIOT;
using Griffins.ImeIOT.Map;
using Griffins.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.Calibration.PageType.InitCfgPage
{
    internal class InitCfgPageTypeRunTimeCompUI : PageTypeRunTimeCompUIBase
    {
        private InitCfgPageTypeData _initCfgPageTypeData;

        /// <summary>
        /// 标定内部子页面运行时接口实现对象
        /// </summary>
        private CalibrationInnerSubPageRunTime calibrationInnerSubPageRunTime = new();

        protected override void _OnInit()
        {
            _initCfgPageTypeData = new InitCfgPageTypeData();

            //创建内部子页面实例
            calibrationInnerSubPageRunTime.Init(base.CallBack);
            (calibrationInnerSubPageRunTime as IInnerSubPageRunTime).AfterModified += OnAfterModified;
        }

        protected override PageTypeID _GetPageTypeID() { return ImeIOTConst.InitCfgPage; }

        protected override IPageTypeRunTimeCompUIView _GetPageTypeCompUIView(string viewID)
        {

            return null;
        }

        protected override void _SetData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            _initCfgPageTypeData = JsonObjConvert.FromJSonBytes<InitCfgPageTypeData>(data);
            (calibrationInnerSubPageRunTime as IInnerSubPageRunTime).SetData(_initCfgPageTypeData.CalibrationInnerSubPageCfgs);
        }

        protected override byte[] _GetData()
        {
            _initCfgPageTypeData.CalibrationInnerSubPageCfgs = (calibrationInnerSubPageRunTime as IInnerSubPageRunTime).GetData();
            return JsonObjConvert.ToJSonBytes(_initCfgPageTypeData);
        }

        protected override string _ExecViewCmd(string cmdID, string cmdParam)
        {
            return string.Empty;
        }

        private void OnAfterModified(object sender, EventArgs e)
        {
            AfterDataModified?.Invoke(sender, e);
        }

        protected override ISubPageRunTime _GetSubPageRunTime(SubPageKindInfoBase subPageKindInfo)
        {
            InnerSubPageKindInfo innerSubPageKindInfo = (InnerSubPageKindInfo)subPageKindInfo;
            switch (innerSubPageKindInfo.InnerSubPageTypeID.ToString())
            {
                //标定内部子页面
                case InitCfgPageTypeConst.InnerSubPageTypeIDStr_Calibration:
                    return calibrationInnerSubPageRunTime;

                default:
                    throw new Exception("不存在对应内部子页面类型ID的内部子页面信息");
            }
        }

        protected override bool _CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = null;
            return true;
        }

        public class InitCfgPageTypeData
        {
            /// <summary>
            /// 标定内部子页面配置信息
            /// </summary>
            public byte[] CalibrationInnerSubPageCfgs { get; set; } = Array.Empty<byte>();
        }
    }
}
