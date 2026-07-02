using Griffins.CompUI.Calibration.ViewModels;
using Griffins.CompUI.Calibration.Views;
using Griffins.ImeIOT.Map;
using Griffins.Map.UI;
using System;

namespace Griffins.CompUI.Calibration
{
    /// <summary>
    /// 标定内部子页面运行时接口实现对象
    /// </summary>
    internal sealed class CalibrationInnerSubPageRunTime : IInnerSubPageRunTime
    {
        private readonly CalibrationSubPageViewModel _calibrationSubPageViewModel;
        private EventHandler? afterDataModified;
        private ICompUIRunTimeCallBack? callBack;

        public CalibrationInnerSubPageRunTime()
        {
            _calibrationSubPageViewModel = new CalibrationSubPageViewModel();
            _calibrationSubPageViewModel.AfterModified += onAfterModified;
        }

        public void Init(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
        }

        event EventHandler IInnerSubPageRunTime.AfterModified
        {
            add { afterDataModified += value; }
            remove { afterDataModified -= value; }
        }

        void IInnerSubPageRunTime.OnInit()
        {
        }

        void IInnerSubPageRunTime.SetData(byte[] data)
        {
            _calibrationSubPageViewModel.Init(data);
        }

        byte[] IInnerSubPageRunTime.GetData()
        {
            return _calibrationSubPageViewModel.CfgInfo ?? Array.Empty<byte>();
        }

        void ISubPageRunTime.Init(byte[] viewCfgInfo)
        {
        }

        object ISubPageRunTime.View
        {
            get
            {
                var calibrationSubPageView = new CalibrationSubPageView();
                calibrationSubPageView.DataContext = _calibrationSubPageViewModel;
                _calibrationSubPageViewModel.SetViewReference(calibrationSubPageView);
                return calibrationSubPageView;
            }
        }

        void ISubPageRunTime.SetViewCfgInfo(byte[] viewCfgInfo)
        {
        }

        private void onAfterModified(object? sender, EventArgs e)
        {
            afterDataModified?.Invoke(sender, e);
        }

        public bool CheckDataValid(out string[] inValidMsg)
        {
            inValidMsg = null;
            return true;
        }
    }
}
