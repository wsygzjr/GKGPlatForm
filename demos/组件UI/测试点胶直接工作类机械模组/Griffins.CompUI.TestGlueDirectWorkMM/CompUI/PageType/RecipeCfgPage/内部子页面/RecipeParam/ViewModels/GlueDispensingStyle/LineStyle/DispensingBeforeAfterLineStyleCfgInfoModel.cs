using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.GlueDispensingStyle.LineStyle;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.GlueDispensingStyle
{
    /// <summary>
    /// 点胶前后线样式配置-模型
    /// </summary>
    public class DispensingBeforeAfterLineStyleCfgInfoModel : DataGridItemBaseViewModel<DispensingBeforeAfterLineStyleCfgInfo>
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

        /// <summary>
        /// 内部扩展线样式工艺参数
        /// </summary>
        public ExtendLineStyleCfgInfoViewModel ExtendLineStyleCfgInfoViewModel { get; }
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingBeforeAfterLineStyleCfgInfoModel()
        {
            StyleID = Guid.NewGuid(); 
            StyleNameViewModel = new TextInputViewModel();
            ExtendLineStyleCfgInfoViewModel = new ExtendLineStyleCfgInfoViewModel();
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从配置复制数据
        /// </summary>
        /// <param name="cfgInfo">点胶后线样式配置</param>
        public override void CopyFrom(DispensingBeforeAfterLineStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "配置模型不能为空");
            base.CopyBasePropertiesFrom(cfgInfo);
            StyleID = cfgInfo.StyleID;
            StyleName = cfgInfo.StyleName;
            ExtendLineStyleCfgInfoViewModel.CopyFrom(cfgInfo.ExtendLineStyleCfgInfo);
        }

        /// <summary>
        /// 复制到配置信息
        /// </summary>
        /// <param name="cfgInfo">待填充的点胶后线样式配置</param>
        public override void CopyTo(DispensingBeforeAfterLineStyleCfgInfo cfgInfo)
        {
            if (cfgInfo == null)
                throw new ArgumentNullException(nameof(cfgInfo), "配置不能为空");
            base.CopyBasePropertiesTo(cfgInfo);
            cfgInfo.StyleID = StyleID;
            cfgInfo.StyleName = StyleName;
            ExtendLineStyleCfgInfoViewModel.CopyTo(cfgInfo.ExtendLineStyleCfgInfo);
        }

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            ExtendLineStyleCfgInfoViewModel.AfterModified += onAfterModified;
            StyleNameViewModel.ValueChanged += onValueChanged;
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
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion

    }
}