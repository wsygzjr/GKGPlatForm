using GKG.UI;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models;
using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Tools;
using ReactiveUI;
using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 点胶中线样式配置-模型
    /// </summary>
    public class DispensingMiddleLineStyleCfgInfoModel : DataGridItemBaseViewModel<DispensingMiddleLineStyleCfgInfo>
    {

        #region 响应式字段与属性
        private Guid _styleID;
        /// <summary>
        /// 样式ID
        /// </summary>
        public Guid StyleID
        {
            get => _styleID;
            set => this.RaiseAndSetIfChanged(ref _styleID, value);
        }

        public TextInputViewModel StyleNameViewModel { get; }

        private string _styleName = string.Empty;
        /// <summary>
        /// 样式名称
        /// </summary>
        public string StyleName
        {
            get => StyleNameViewModel.Text;
            set
            {
                StyleNameViewModel.Text = value;
                this.RaisePropertyChanged(nameof(StyleName));
            }
        }

        private bool _isChecked;
        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public NumericViewModel DispensingHeightViewModel { get; }
        /// <summary>
        /// 点胶高度（mm，建议取值范围 0.1-50.0）
        /// </summary>
        public decimal DispensingHeight
        {
            get => DispensingHeightViewModel.Value;
            set
            {
                DispensingHeightViewModel.Value = value;
                this.RaisePropertyChanged(nameof(DispensingHeight));
            }
        }

        public NumericViewModel DispensingSpeedViewModel { get; }
        /// <summary>
        /// 点胶速度（mm/s）
        /// </summary>
        public decimal DispensingSpeed
        {
            get => DispensingSpeedViewModel.Value;
            set
            {
                DispensingSpeedViewModel.Value = value;
                this.RaisePropertyChanged(nameof(DispensingSpeed));
            }
        }
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingMiddleLineStyleCfgInfoModel()
        {
            StyleID = Guid.NewGuid();
            StyleNameViewModel = new TextInputViewModel();
            DispensingHeightViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            DispensingSpeedViewModel = new NumericViewModel { Minimum = 1, Increment = 1, Value = 1 };
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从配置复制数据
        /// </summary>
        /// <param name="cfgInfo">点胶中线样式配置</param>
        public override void CopyFrom(DispensingMiddleLineStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "配置模型不能为空");

            base.CopyBasePropertiesFrom(cfgInfo);
            StyleID = cfgInfo.StyleID;
            StyleName = cfgInfo.StyleName;
            DispensingHeight = cfgInfo.DispensingHeight;
            DispensingSpeed = cfgInfo.DispensingSpeed;
        }

        /// <summary>
        /// 复制到配置信息
        /// </summary>
        /// <param name="cfgInfo">待填充的点胶中线样式配置</param>
        public override void CopyTo(DispensingMiddleLineStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "配置不能为空");

            base.CopyBasePropertiesTo(cfgInfo);
            cfgInfo.StyleID = StyleID;
            cfgInfo.StyleName = StyleName;
            cfgInfo.DispensingHeight = DispensingHeight;
            cfgInfo.DispensingSpeed = DispensingSpeed;
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            StyleNameViewModel.ValueChanged += onValueChanged;
            DispensingHeightViewModel.ValueChanged += onValueChanged;
            DispensingSpeedViewModel.ValueChanged += onValueChanged;
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }
}