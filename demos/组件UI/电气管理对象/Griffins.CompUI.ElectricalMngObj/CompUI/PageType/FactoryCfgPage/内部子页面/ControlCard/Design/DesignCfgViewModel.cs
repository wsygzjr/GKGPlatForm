using GF_Gereric;
using Griffins.ImeIOT;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Design
{
	/// <summary>
	///配置设计时配置-视图模型
	/// </summary>
	public class DesignCfgViewModel : ReactiveObject
	{
		/// <summary>
		/// 内部子页面配置信息
		/// </summary>
		private byte[]? _cfgInfo;
        /// <summary>
        /// 参数设计时配置信息
        /// </summary>
        private ControlCardInnerSubPageDesignCfg _controlCardInnerSubPageDesignCfg;
        /// <summary>
        /// 参数设计配置信息模型
        /// </summary>
        public ControlCardInnerSubPageDesignCfgViewModel DesignCfgInfoViewModel { get; }
		/// <summary>
		/// 配置变更事件
		/// </summary>
		public event EventHandler? AfterModified;

		/// <summary>
		/// 内部子页面配置信息
		/// </summary>
		public byte[] CfgInfo
		{
			get
			{
				_cfgInfo = JsonObjConvert.ToJSonBytes(_controlCardInnerSubPageDesignCfg);
				return _cfgInfo;
			}
			set
			{
				_cfgInfo = value;
				if (_cfgInfo != null)
				{
                    _controlCardInnerSubPageDesignCfg = JsonObjConvert.FromJSonBytes<ControlCardInnerSubPageDesignCfg>(_cfgInfo);
                    DesignCfgInfoViewModel.CopyFrom(_controlCardInnerSubPageDesignCfg);

                }
            }
		}

		public DesignCfgViewModel()
		{
			_controlCardInnerSubPageDesignCfg = new ControlCardInnerSubPageDesignCfg();
            DesignCfgInfoViewModel = new ControlCardInnerSubPageDesignCfgViewModel();

            this.WhenAnyValue(
					x => x.DesignCfgInfoViewModel.Width
				)
				.Subscribe(_ =>
				{
					DesignCfgInfoViewModel.CopyTo(_controlCardInnerSubPageDesignCfg);
                    onAfterModified();
				});
		}

		/// <summary>
		/// 配置信息改变后事件触发1
		/// </summary>

		private void onAfterModified()
		{
			AfterModified?.Invoke(this, EventArgs.Empty);
		}

       
    }
    /// <summary>
    ///运控卡配置内部子页面设计时配置-视图模型
    /// </summary>
    public class ControlCardInnerSubPageDesignCfgViewModel : ReactiveObject
    {

        /// <summary>
        /// 宽度
        /// </summary>
        private decimal _width;
        /// <summary>
        /// 选中的机械模组别名
        /// </summary>
        public decimal Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        public ControlCardInnerSubPageDesignCfgViewModel()
        {
        }
        /// <summary>
        /// 从源复制
        /// </summary>
        /// <param name="recipeParamPageDesignCfgInfo"></param>
        public void CopyFrom(ControlCardInnerSubPageDesignCfg recipeParamPageDesignCfgInfo)
        {
            Width = recipeParamPageDesignCfgInfo.Width;
        }
        /// <summary>
        /// 复制到
        /// </summary>
        /// <param name="recipeParamPageDesignCfgInfo"></param>
        public void CopyTo(ControlCardInnerSubPageDesignCfg recipeParamPageDesignCfgInfo)
        {
            recipeParamPageDesignCfgInfo.Width = Width;
        }
    }
}
