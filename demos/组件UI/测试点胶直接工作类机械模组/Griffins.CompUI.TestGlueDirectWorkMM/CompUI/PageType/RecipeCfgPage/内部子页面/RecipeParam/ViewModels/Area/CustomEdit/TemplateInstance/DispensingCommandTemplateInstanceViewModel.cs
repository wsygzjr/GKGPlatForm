using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 点胶指令模板实例 视图模型
    /// </summary>
    public class DispensingCommandTemplateInstanceViewModel: ReactiveObject
    {
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 选中的模板ID-下拉框数据模型
        /// </summary>
        public ComboxViewModel SelectedTemplateModel { get; }

        /// <summary>
        /// 选中的模板ID
        /// </summary>
        private Guid _belongTemplateID= Guid.Empty;
        public Guid BelongTemplateID
        {
            get
            {
                return _belongTemplateID;
            }
            set
            {
                if (_belongTemplateID == value)
                    return;
                if (value == Guid.Empty)
                    return;
                _belongTemplateID = value;
                if (SelectedTemplateModel.ItemsSource != null)
                {
                    var targetItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (Guid)o.Value == value);
                    if (targetItem != null)
                    {
                        SelectedTemplateModel.SelectedItem = targetItem;
                    }
                    //选中模板改变后同步更新模板中的指令
                    DispensingCommandSequenceListViewModel.ReLoadCommandSequence(BelongTemplateID);
                    TrajectoryimagingWorkAreaViewModel.ReLoadTrajectory(BelongTemplateID);
                    this.RaisePropertyChanged(nameof(BelongTemplateID));

                }
            }
        }

        /// <summary>
        /// 相机教导移动
        /// </summary>
        public CamreaPositionViewModel CamreaPositionViewModel { get; }
        /// <summary>
        /// 指令列表
        /// </summary>
        public DispensingCommandSequenceListViewModel DispensingCommandSequenceListViewModel { get; }
        /// <summary>
        /// 轨迹成像工作区
        /// </summary>
        public TrajectoryimagingWorkAreaViewModel TrajectoryimagingWorkAreaViewModel { get; }
        /// <summary>
        /// 模板实例工作区视图模型
        /// </summary>
        public DispensingCommandTemplateInstanceViewModel()
        {
            CacheDataExchange.SubscribTemplateChanged(cacheData_TemplateItemChanged);
            CamreaPositionViewModel = new CamreaPositionViewModel();
            TrajectoryimagingWorkAreaViewModel = new TrajectoryimagingWorkAreaViewModel();
            DispensingCommandSequenceListViewModel = new DispensingCommandSequenceListViewModel(TrajectoryimagingWorkAreaViewModel);

            SelectedTemplateModel = new ComboxViewModel();
            SelectedTemplateModel.ItemsSource = getTemplates();
            SelectedTemplateModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            SelectedTemplateModel.ValueChanged += SelectedTemplateModel_ValueChanged;
            SelectedTemplateModel.SelectedItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault();
            // 订阅值变更事件
            subscribeValueChanges();
        }

        private void SelectedTemplateModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            if(e.NewValue!=null&& e.NewValue is ComBoxItem comBoxItem)
            {
                BelongTemplateID = (Guid)comBoxItem.Value;
            }
        }

        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <returns></returns>
        private List<ComBoxItem> getTemplates()
        {
            return CacheDataExchange.GetTemplates();
        }
        /// <summary>
        /// 模板数据源改变通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cacheData_TemplateItemChanged(object? sender, EventArgs e)
        {
            SelectedTemplateModel.ItemsSource = getTemplates();
            SelectedTemplateModel.SelectedItem = SelectedTemplateModel.ItemsSource.Cast<ComBoxItem>().FirstOrDefault(o=>(Guid)o.Value== BelongTemplateID);
        }

        /// <summary>
        /// 从数据模型复制
        /// </summary>
        public  void CopyFrom(DispensingCommandTemplateInstance info)
        {
            this.BelongTemplateID=info.TemplateId;   
            CamreaPositionViewModel.X = info.BaseCoordinate.X;
            CamreaPositionViewModel.Y = info.BaseCoordinate.Y;
            CamreaPositionViewModel.Z = info.BaseCoordinate.Z;
        }

        /// <summary>
        /// 复制到数据模型
        /// </summary>
        public  void CopyTo(DispensingCommandTemplateInstance targetInfo)
        {
            targetInfo.TemplateId = this.BelongTemplateID;
            targetInfo.BaseCoordinate.X=CamreaPositionViewModel.X;
            targetInfo.BaseCoordinate.Y=CamreaPositionViewModel.Y;
            targetInfo.BaseCoordinate.Z=CamreaPositionViewModel.Z;

          
        }
        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
            CamreaPositionViewModel.SetViewReference(view);
            DispensingCommandSequenceListViewModel.SetViewReference(view);
            TrajectoryimagingWorkAreaViewModel.SetViewReference(view);

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            CamreaPositionViewModel.AfterModified += onAfterModified;
            DispensingCommandSequenceListViewModel.AfterModified += onAfterModified;
            SelectedTemplateModel.ValueChanged += onValueChanged;
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
