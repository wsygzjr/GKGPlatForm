using Avalonia.Controls;
using Avalonia.VisualTree;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence;
using PropertyModels.Extensions;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicData;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 点胶轨迹工作区-视图模型
    /// </summary>
    public class DispensingTrajectoryWorkAreaViewModel :
        DataGridListBaseViewModel<TrajectorySequenceBaseModel, TrajectorySequenceCfgInfo>
    {
        /// <summary>
        /// 所属模板ID
        /// </summary>
        private Guid _templateID;
        /// <summary>
        /// 循环次数
        /// </summary>
        public NumericViewModel CycleCountViewModel { get; }

        /// <summary>
        /// 阀-下拉框模型
        /// </summary>
        public ComboxViewModel ValveInfoModel { get; }
        /// <summary>
        /// 添加轨迹命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddTrajectorySequenceCommand { get; }


        /// <summary>
        /// 循环次数
        /// </summary>
        public decimal CycleCount
        {
            get => CycleCountViewModel.Value;
            set => CycleCountViewModel.Value = value;
        }
        /// <summary>
        /// 当前选中的阀
        /// </summary>
        public string SelectedValveNumber
        {
            get => (string)((ValveInfoModel.SelectedItem as ComBoxItem)?.Value ?? "1");
            set
            {
                if (ValveInfoModel.ItemsSource != null)
                {
                    var targetItem = ValveInfoModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        ValveInfoModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedValveNumber));
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DispensingTrajectoryWorkAreaViewModel(Guid templateID) :base()
        {
            this._templateID=templateID;
            CycleCountViewModel = new NumericViewModel() { Increment =1m, DecimalPlaces = 0, Minimum = 1m, Maximum = 65535m, };
            AddTrajectorySequenceCommand = ReactiveCommand.CreateFromTask(onAddTrajectorySequenceCommand);
            ValveInfoModel = new ComboxViewModel();


            List<ComBoxItem> dataItems = new List<ComBoxItem>();
            foreach (var item in CacheDataExchange.GetValveInfoes())
            {
                dataItems.Add(new ComBoxItem()
                {
                    Value = item.ValveNumber,
                    DisplayName = item.ValveName
                });
            }
            ValveInfoModel.ItemsSource = dataItems;
            ValveInfoModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ValveInfoModel.SelectedItem = ValveInfoModel.ItemsSource.Cast<ComBoxItem>().First();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="dispensingCcommandSequence"></param>
        public void CopyFrom(DispensingCommandSequence dispensingCcommandSequence)
        {
            if (dispensingCcommandSequence == null)
                return;
           var trajectorySequencees= CacheDataExchange.GetTrajectorySequenceModel(_templateID);
            base.ItemsSource.Clear();
            foreach (var trackID in dispensingCcommandSequence.TrajectorySequenceIds)
            {
                var trajectorySequence = trajectorySequencees.FirstOrDefault(o => o.TrackID == trackID);
                if(trajectorySequence!= null)
                {
                    trajectorySequence.CanEdit = false;
                    base.ItemsSource.Add(trajectorySequence);
                }

            }
            this.CycleCount = dispensingCcommandSequence.CycleCount;
            this.SelectedValveNumber = dispensingCcommandSequence.ValveNumber;
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="dispensingCcommandSequence"></param>
        public void CopyTo(DispensingCommandSequence dispensingCcommandSequence)
        {
            if (dispensingCcommandSequence == null)
                return;

            dispensingCcommandSequence.TrajectorySequenceIds.Clear();
            dispensingCcommandSequence.TrajectorySequenceIds.AddRange(base.ItemsSource.Select(o=>o.TrackID));
            dispensingCcommandSequence.CycleCount = (int)this.CycleCount;
            dispensingCcommandSequence.ValveNumber = this.SelectedValveNumber;
        }

        /// <summary>
        ///添加轨迹
        /// </summary>
        private async Task onAddTrajectorySequenceCommand()
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return;
            }

            var editViewModel = new SelectDispensingTrajectoryWindowViewModel(_templateID);
            editViewModel.SetViewReference(_viewReference!);
            var editWindow = new SelectDispensingTrajectoryWindow
            {
                DataContext = editViewModel,
                Title = "选择轨迹",
                Width = 1450,
                Height = 774,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                CanResize = false
            };

            var result = await editWindow.ShowDialog<bool>(parentWindow);
            if ((result))
            {
                foreach (var item in editViewModel.SelectedItems)
                {
                    if (base.ItemsSource.Any(o => o.TrackID == item.TrackID))
                        continue;
                    item.CanEdit = false;
                    base.ItemsSource.Add(item);
                }
                base.IsSelectAll = false;
            }
        }
      
        #region 重载基类方法
        /// <summary>
        /// 
        /// </summary>
        protected override async Task _addItem()
        {
            throw new Exception("不支持该操作");
        }

        #endregion
    }
}