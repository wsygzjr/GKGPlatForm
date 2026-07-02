
using Avalonia.Controls;
using Griffins.UI;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.SubTemplate;
namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.SubTemplate
{
    /// <summary>
    /// 子模板-视图模型
    /// </summary>
    public class SubTemplateItemViewModel : DataGridItemBaseViewModel<SubTemplateItemInfo>
    { 
        /// <summary>
        /// 选中的模板ID-下拉框数据模型
        /// </summary>
        public ComboxViewModel SelectedTemplateModel { get; }
       
        /// <summary>
        /// 子模板ID
        /// </summary>
        private Guid _subTemplateID;
        public Guid SubTemplateID
        {
            get => _subTemplateID;
            set => this.RaiseAndSetIfChanged(ref _subTemplateID, value);
        }

        /// <summary>
        /// 子模板名称
        /// </summary>
        private string _subTemplateName="";
        public string SubTemplateName
        {
            get => _subTemplateName;
            set => this.RaiseAndSetIfChanged(ref _subTemplateName, value);
        }

        /// <summary>
        /// 选中的模板ID
        /// </summary>
        public Guid BelongTemplateID
        {
            get
            {
                if (SelectedTemplateModel.SelectedItem is ComBoxItem template)
                {
                    return (Guid)template.Value;
                }
                return Guid.Empty;
            }
            set
            {
                if (SelectedTemplateModel.ItemsSource != null)
                {
                    var targetItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (Guid)o.Value == value);
                    if (targetItem != null)
                    {
                        SelectedTemplateModel.SelectedItem = targetItem;
                        TemplateName = targetItem.DisplayName;
                    }
                    this.RaisePropertyChanged(nameof(BelongTemplateID));

                }
            }
           
        }

        /// <summary>
        /// 选中的（嵌套）模板名称
        /// </summary>
        private string _TemplateName = "";
        public string TemplateName
        {
            get => _TemplateName;
            set => this.RaiseAndSetIfChanged(ref _TemplateName, value);
        }

        /// <summary>
        ///子模板基准坐标位置校准-视图模型
        /// </summary>
        public SubTemplateBasicPositionViewModel SubTemplateBasicPositionViewModel { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SubTemplateItemViewModel()
        {
            SubTemplateBasicPositionViewModel = new SubTemplateBasicPositionViewModel();

            // 初始化模板数据源
            SelectedTemplateModel = new ComboxViewModel();
            SelectedTemplateModel.ItemsSource = CacheDataExchange.GetTemplates();
            SelectedTemplateModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            SelectedTemplateModel.SelectedItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();
            SelectedTemplateModel.IsEnabled = false;
            SelectedTemplateModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(BelongTemplateID));
        }

        /// <summary>
        /// 复制
        /// </summary>
        public override void CopyFrom(SubTemplateItemInfo subTemplateItemInfo)
        {
            base.CopyBasePropertiesFrom(subTemplateItemInfo);
            SubTemplateID = subTemplateItemInfo.SubTemplateID;
            SubTemplateName = subTemplateItemInfo.SubTemplateName;
            BelongTemplateID = subTemplateItemInfo.BelongTemplateID;
            SerialNumber = subTemplateItemInfo.SerialNumber;
            SubTemplateBasicPositionViewModel.CopyFrom(subTemplateItemInfo.CoordinateAxisValues);
        }

        /// <summary>
        /// 复制到指定对象
        /// </summary>
        public override void CopyTo(SubTemplateItemInfo subTemplateItemInfo)
        {
            base.CopyBasePropertiesTo(subTemplateItemInfo);
            subTemplateItemInfo.SubTemplateID = SubTemplateID;
            subTemplateItemInfo.SubTemplateName = SubTemplateName;
            subTemplateItemInfo.BelongTemplateID = BelongTemplateID;
            subTemplateItemInfo.SerialNumber = SerialNumber;
            SubTemplateBasicPositionViewModel.CopyTo(subTemplateItemInfo.CoordinateAxisValues);

        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public override void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            SubTemplateBasicPositionViewModel.SetViewReference(view);
            //GlobalVisionViewModel.CameraShowViewModel.SetViewReference(view);
        }
    }
}
