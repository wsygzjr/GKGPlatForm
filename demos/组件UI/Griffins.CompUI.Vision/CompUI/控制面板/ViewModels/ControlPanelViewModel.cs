using Avalonia.Controls;
using AvaloniaVisionControl;
using GF_Gereric;
using GKG.UI.General;
using Griffins;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Reactive;

namespace Griffins.CompUI.Vision
{
	/// <summary>
	/// 视觉分析控制面板 ViewModel
	/// </summary>
	public class ControlPanelViewModel : ReactiveObject, IDisposable
	{
        /// <summary>
        /// 相机与光源控制
        /// </summary>
        public CameraShowViewModel CameraShowViewModel { set; get; }
        /// <summary>
        /// 组件控制面板回调接口实例（用于向后端请求数据）
        /// </summary>
        private readonly IControlPanelCallBack iControlPanelCallBack;

		private string _visionTestResult;
		/// <summary>
		/// 视觉测试结果
		/// </summary>
		public string VisionTestResult
		{
			get => _visionTestResult;
			set => this.RaiseAndSetIfChanged(ref _visionTestResult, value);
		}

		private string _ninePointCalibrationResult;
		/// <summary>
		/// 9点标定结果
		/// </summary>
		public string NinePointCalibrationResult
		{
			get => _ninePointCalibrationResult;
			set => this.RaiseAndSetIfChanged(ref _ninePointCalibrationResult, value);
		}

		private string _scanTestResult;
		/// <summary>
		/// 扫码测试结果
		/// </summary>
		public string ScanTestResult
		{
			get => _scanTestResult;
			set => this.RaiseAndSetIfChanged(ref _scanTestResult, value);
		}

		public ReactiveCommand<Unit, Unit> VisionTestCommand { get; }
		public ReactiveCommand<Unit, Unit> NinePointCalibrationCommand { get; }
		public ReactiveCommand<Unit, Unit> ScanTestCommand { get; }

		public ControlPanelViewModel()
		{
            //初始化视图模型
            CameraShowViewModel = new CameraShowViewModel();
            // 设计器/空构造兜底，避免 XAML 设计时崩溃
            VisionTestCommand = ReactiveCommand.Create(() => { });
			NinePointCalibrationCommand = ReactiveCommand.Create(() => { });
			ScanTestCommand = ReactiveCommand.Create(() => { });
        }

		public ControlPanelViewModel(IControlPanelCallBack iControlPanelCallBack, Control _cameraPreControl)
		{
			this.iControlPanelCallBack = iControlPanelCallBack;
            //初始化视图模型
            CameraShowViewModel = new CameraShowViewModel();
            VisionTestCommand = ReactiveCommand.Create(ExecuteVisionTest);
            NinePointCalibrationCommand = ReactiveCommand.Create(ExecuteNinePointCalibration);
            ScanTestCommand = ReactiveCommand.Create(ExecuteScanTest);
            var visionView = (VisionControlShowView)_cameraPreControl;
            CameraShowViewModel.RegisterActiveVisionView(visionView);
            visionView.CameraPreControl.ImageClick += ExecuteMachineMove;
        }

        private void ExecuteVisionTest()
		{
			VisionTestResult = ExecuteBackendCmdToString("VisionTest");
		}

		private void ExecuteNinePointCalibration()
		{
			NinePointCalibrationResult = ExecuteBackendCmdToString("NinePointCalibration");
		}

		private void ExecuteScanTest()
		{
			ScanTestResult = ExecuteBackendCmdToString("ScanTest");
		}

		private void ExecuteMachineMove(object sender, ImageClickEventArgs e)
		{
            if (iControlPanelCallBack == null)
            {
                throw new Exception("回调接口未初始化");
            }

            try
            {
                GFBaseTypeParamValueList param = new GFBaseTypeParamValueList();
                GKG.Point2D point = new GKG.Point2D(e.ImagePosition.X, e.ImagePosition.Y);
                param.Add(new GFBaseTypeParamValue("ImagePoint", new GriffinsBaseValue(JsonObjConvert.ToJSon(e.ImagePosition))));
                var response = iControlPanelCallBack.ExecNormalCtlCmd("MachineMove", param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过回调接口执行后端命令，并尽可能把返回内容转换为字符串
        /// </summary>
        private string ExecuteBackendCmdToString(string cmdId)
		{
			if (iControlPanelCallBack == null)
			{
				return "回调接口未初始化";
			}

			try
			{
				var response = iControlPanelCallBack.ExecNormalCtlCmd(cmdId, null);
				return ConvertResponseToString(response);
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		private static string ConvertResponseToString(GFBaseTypeParamValueList response)
		{
			if (response == null)
			{
				return string.Empty;
			}

			// 约定优先取 "Result"，其次取第一个参数值，最后兜底 ToString()
			try
			{
				var resultValue = response["Result"];
				if (resultValue != null)
				{
					return resultValue.ToString();
				}
			}
			catch
			{
				// ignored
			}

			try
			{
				if (response.Count > 0 && response[0] != null && response[0].Value != null)
				{
					return response[0].Value.ToString();
				}
			}
			catch
			{
				// ignored
			}

			return response.ToString();
		}

		public void Dispose()
		{
			// 暂无需要释放的资源，保留接口以便后续扩展
		}
	}
}
