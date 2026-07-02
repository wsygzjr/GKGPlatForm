using Avalonia.Controls;
using Avalonia.VisualTree;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.TrajectorySequence;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 轨迹序列配置列表-视图模型
    /// </summary>
    public class TrajectorySequenceListViewModel :
        DataGridListBaseViewModel<TrajectorySequenceModel, TrajectorySequenceCfgInfo>
    {

        /// <summary>
        /// 当前选中点（点或线中的点）的编辑视图模型
        /// </summary>
        public CamreaPositionViewModel PointEditViewModel { get; }
        #region 命令定义
        /// <summary>
        /// 添加点轨迹序列命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddPointCommand { get; }

        /// <summary>
        /// 添加线轨迹序列命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddLineCommand { get; }

        /// <summary>
        /// 平移命令
        /// </summary>
        public ReactiveCommand<TrajectorySequenceModel, Unit> TranslationCommand { get; }


        /// <summary>
        /// 批量平移命令
        /// </summary>
        public ReactiveCommand<Unit, bool> BatchTranslationCommand { get; }
        
        /// <summary>
        /// 轨迹序列项改变通知
        /// </summary>
        public EventHandler<EventArgs>? TrajectoryItemChanged { get; internal set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public TrajectorySequenceListViewModel() : base()
        {
            AddPointCommand = ReactiveCommand.Create(addPointTrajectorySequenceCfgInfo, Observable.Return(true));
            AddLineCommand = ReactiveCommand.Create(addLineTrajectorySequenceCfgInfo, Observable.Return(true));
            TranslationCommand = ReactiveCommand.Create<TrajectorySequenceModel>(translationCommand, Observable.Return(true));
            BatchTranslationCommand = ReactiveCommand.CreateFromTask(batchTranslationCommand, Observable.Return(true));
          
            //点坐标编辑
            PointEditViewModel = new CamreaPositionViewModel();
            //当点坐标编辑视图的点改变时，通知点线轨迹中的点信息
            this.WhenAnyValue(
                    vm => vm.PointEditViewModel.X,
                    vm => vm.PointEditViewModel.Y,
                    vm => vm.PointEditViewModel.Z
                )
                .Subscribe(_ =>
                {
                    onPointEditChanged(PointEditViewModel.X, PointEditViewModel.Y, PointEditViewModel.Z);
                });

           
            //当选中的点轨迹时，同步更新点轨迹下的点信息到点编辑视图中
            this.WhenAnyValue(
                  vm => vm.SelectedItem
              )
              .Subscribe(_ =>
              {
                  if(SelectedItem!=null)
                  {
                      if (SelectedItem.CalculateTrajectoryType == CalculateTrajectoryType.Point)
                      {
                          if(SelectedItem.PointTrajectoryCfgViewModel!=null)
                          {
                              onPointChanged(this, new PointChangedEventArgs()
                              {
                                  X = SelectedItem.PointTrajectoryCfgViewModel.X,
                                  Y = SelectedItem.PointTrajectoryCfgViewModel.Y,
                                  Z = SelectedItem.PointTrajectoryCfgViewModel.Z
                              });
                          }
                      }
                  }
                  
              });

            //轨迹项改变通知
            this.ItemsSource.CollectionChanged += (s, e) =>
            {
                TrajectoryItemChanged?.Invoke(this, new EventArgs());
            };

        }

        /// <summary>
        /// 点和线中的点信息改变
        /// 通知更新到点编辑视图的点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onPointChanged(object? sender, PointChangedEventArgs e)
        {
            PointEditViewModel.X = e.X;
            PointEditViewModel.Y = e.Y;
            PointEditViewModel.Z = e.Z;
        }

        /// <summary>
        /// 编辑的点坐标改变事件
        /// </summary>
        private void onPointEditChanged( decimal x ,decimal y,decimal z)
        {
            //更新到当前选中的点或是线中选中的点
            if (this.SelectedItem == null)
                return;
            this.SelectedItem.SetPointWhenEditChanged(x,y,z);
            
        }
      
        /// <summary>
        /// 添加点轨迹序列
        /// </summary>
        private void addPointTrajectorySequenceCfgInfo()
        {
            var newTrajectorySequenceModel = new TrajectorySequenceModel();
            newTrajectorySequenceModel.SubscribPointChanged(onPointChanged);
            newTrajectorySequenceModel.SetViewReference(_viewReference!);
            var trajectorysequenceCfgInfo = new TrajectorySequenceCfgInfo();
            trajectorysequenceCfgInfo.TrackID = Guid.NewGuid();
            trajectorysequenceCfgInfo.SerialNumber = ItemsSource.Count + 1;
            trajectorysequenceCfgInfo.CalculateTrajectoryType = CalculateTrajectoryType.Point;
            trajectorysequenceCfgInfo.CalculateTrajectory = new PointCalculateTrajectory();
            newTrajectorySequenceModel.CopyFrom(trajectorysequenceCfgInfo);
            newTrajectorySequenceModel.AfterModified += onAfterModified;
            ItemsSource.Add(newTrajectorySequenceModel);
            SelectedItem = newTrajectorySequenceModel;
        }

       
        /// <summary>
        /// 添加线轨迹序列
        /// </summary>
        private void addLineTrajectorySequenceCfgInfo()
        {
            var newTrajectorySequenceModel = new TrajectorySequenceModel();
            newTrajectorySequenceModel.SubscribPointChanged(onPointChanged);

            newTrajectorySequenceModel.SetViewReference(_viewReference!);
            var trajectorysequenceCfgInfo = new TrajectorySequenceCfgInfo();
            trajectorysequenceCfgInfo.TrackID = Guid.NewGuid();
            trajectorysequenceCfgInfo.SerialNumber = ItemsSource.Count + 1;
            trajectorysequenceCfgInfo.CalculateTrajectoryType = CalculateTrajectoryType.Line;
            trajectorysequenceCfgInfo.CalculateTrajectory = new LineCalculateTrajectory();
            newTrajectorySequenceModel.CopyFrom(trajectorysequenceCfgInfo);
            newTrajectorySequenceModel.AfterModified += onAfterModified;
            ItemsSource.Add(newTrajectorySequenceModel);
            SelectedItem = newTrajectorySequenceModel;
        }

        
        /// <summary>
        /// 平移
        /// </summary>
        /// <param name="trajectorySequenceModel">待删除的轨迹序列模型</param>
        private async void translationCommand(TrajectorySequenceModel trajectorySequenceModel)
        {
            TranslationParam translationParam = new TranslationParam();
            var result = await showTranslationPositionWindow(translationParam);
            if (result)
            {
                trajectorySequenceModel.Translation(translationParam);
            }
        }

        /// <summary>
        /// 批量平移
        /// </summary>
        /// <returns></returns>
        private async Task<bool> batchTranslationCommand()
        {
            TranslationParam translationParam = new TranslationParam();
            var result = await showTranslationPositionWindow(translationParam);
            if (result)
            {
                foreach (var trajectorySequenceModel in SelectedItems)
                {
                    trajectorySequenceModel.Translation(translationParam);
                }
            }
            return result;
        }
        /// <summary>
        /// 打开平移窗口
        /// </summary>
        /// <returns></returns>
        private async Task<bool> showTranslationPositionWindow(TranslationParam translationParam)
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", _viewReference);
                return false;
            }

            var editViewModel = new TranslationPositionViewModel();
            var editWindow = new TranslationPositionWindow
            {
                DataContext = editViewModel,
                Title = "平移",
                Width = 400,
                Height = 230,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                CanResize = false 
            };

            var result = await editWindow.ShowDialog<bool>(parentWindow);
            if ((result))
            {
                translationParam.X = editViewModel.X;
                translationParam.Y = editViewModel.Y;
                translationParam.Z = editViewModel.Z;
            }
            return result;
        }
        #region 重载基类方法
        /// <summary>
        /// 
        /// </summary>
        protected override async Task _addItem()
        {
            throw new Exception("不支持的命令");
        }
        #endregion

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

    }
}