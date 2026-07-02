using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKG.UI.General;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.Map;
using Griffins.Map.UI;
using ReactiveUI;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    /// <summary>
    /// 气缸延时组件UI ViewModel
    /// </summary>
    public class CylinderDelayCompUIViewModel : ReactiveObject, IDisposable
    {
        #region 私有字段

        private bool isDesign;

        private ICompUIRunTimeCallBack callBack;

        private CylinderDelayCompUIModel cylinderDelayCompUIModel;

        #endregion

        #region 组件UI模型

        /// <summary>
        /// 气缸延时子组件 ViewModel
        /// </summary>
        public CylinderDelayViewModel CylinderDelayViewModel { get; }

        #endregion

        #region 响应式属性

        private object _viewTag;
        /// <summary>
        /// 对应View的Tag属性（支持双向绑定）
        /// </summary>
        public object ViewTag
        {
            get => _viewTag;
            set => this.RaiseAndSetIfChanged(ref _viewTag, value);
        }

        private bool _readOnly;
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
        }

        /// <summary>
        /// 气缸延时子组件响应式属性
        /// </summary>
        public CylinderDelayViewModel CylinderDelay
        {
            get => CylinderDelayViewModel;
            set { this.RaisePropertyChanged(nameof(CylinderDelay)); }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public CylinderDelayCompUIViewModel(bool isDesign, ICompUIRunTimeCallBack callBack)
        {
            this.isDesign = isDesign;
            this.callBack = callBack;

            CylinderDelayViewModel = new();
            CylinderDelayViewModel.AfterModified += CylinderDelayViewModel_AfterModified;

            this.WhenAnyValue(x => x.ReadOnly)
                .Subscribe(_ => ApplyReadOnly());

            ApplyReadOnly();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData(CylinderDelayCompUIModel model)
        {
            cylinderDelayCompUIModel = model ?? new CylinderDelayCompUIModel();
            CylinderDelayViewModel.DelayNumeric = cylinderDelayCompUIModel.DelayNumeric;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public CylinderDelayCompUIModel GetData()
        {
            var model = new CylinderDelayCompUIModel();
            model.DelayNumeric = CylinderDelayViewModel.DelayNumeric;
            return model;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            CylinderDelayViewModel.AfterModified -= CylinderDelayViewModel_AfterModified;
        }

        #endregion

        #region 私有方法

        private void CylinderDelayViewModel_AfterModified(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(nameof(CylinderDelay));
        }

        private void ApplyReadOnly()
        {
            var enabled = !ReadOnly;
            CylinderDelayViewModel.DelayNumericViewModel.IsEnabled = enabled;
        }

        #endregion
    }
}
