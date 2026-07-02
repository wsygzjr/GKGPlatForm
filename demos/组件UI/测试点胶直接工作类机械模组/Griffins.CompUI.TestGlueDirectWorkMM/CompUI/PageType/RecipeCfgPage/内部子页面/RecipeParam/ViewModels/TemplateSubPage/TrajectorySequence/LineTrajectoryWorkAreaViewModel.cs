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

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.TrajectorySequence
{
    /// <summary>
    /// 线计算轨迹工作区-视图模型
    /// </summary>
    public class LineWorkAreaViewModel :
        DataGridListBaseViewModel<LineCalculateTrajectoryItemViewModel, CalculateTrajectoryItem>
    {
        #region 线轨迹列表相关
      
        /// <summary>
        /// 添加直线命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddStraightLineCommand { get; }

        /// <summary>
        /// 添加圆弧A命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCircularArcACommand { get; }

        /// <summary>
        /// 添加圆弧B命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCircularArcBCommand { get; }

        /// <summary>
        /// 添加圆命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCircleCommand { get; }

        /// <summary>
        /// 平移命令
        /// </summary>
        public ReactiveCommand<LineCalculateTrajectoryItemViewModel, Unit> TranslationCommand { get; }

        /// <summary>
        /// 批量平移命令
        /// </summary>
        public ReactiveCommand<Unit, bool> BatchTranslationCommand { get; }
        #endregion

        private EventHandler<PointChangedEventArgs>? _pointChanged;

        /// <summary>
        /// tabcontrol 选中的索引
        /// 用于：线列表选中行改变事件：默认将选项卡指定为第1个
        /// </summary>
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LineWorkAreaViewModel()
        {
            AddStraightLineCommand = ReactiveCommand.Create(() => addSpecificTrajectory(LineCalculateTrajectoryType.StraightLine), Observable.Return(true));
            AddCircularArcACommand = ReactiveCommand.Create(() => addSpecificTrajectory(LineCalculateTrajectoryType.CircularArcA), Observable.Return(true));
            AddCircularArcBCommand = ReactiveCommand.Create(() => addSpecificTrajectory(LineCalculateTrajectoryType.CircularArcB));
            AddCircleCommand = ReactiveCommand.Create(() => addSpecificTrajectory(LineCalculateTrajectoryType.Circle));

            TranslationCommand = ReactiveCommand.Create<LineCalculateTrajectoryItemViewModel>(translationCommand, Observable.Return(true));
            BatchTranslationCommand = ReactiveCommand.CreateFromTask(batchTranslationCommand, Observable.Return(true));

            //线列表选中行改变事件：默认将选项卡指定为第1个
            this.WhenAnyValue(vm => vm.SelectedItem )
            .Subscribe( selectedItem =>
            {
                SelectedTabIndex = 0;
            });  
        }

        /// <summary>
        /// 添加特定类型的轨迹
        /// </summary>
        private void addSpecificTrajectory(LineCalculateTrajectoryType type)
        { 
            CalculateTrajectoryItem item = new CalculateTrajectoryItem();
           
            switch (type)
            {
                case LineCalculateTrajectoryType.StraightLine:
                    item.CalculateTrajectory = new StraightLineACalculateTrajectory();
                    break;
                case LineCalculateTrajectoryType.CircularArcA:
                    item.CalculateTrajectory = new CircularArcACalculateTrajectory();
                    break;
                case LineCalculateTrajectoryType.CircularArcB:
                    item.CalculateTrajectory = new CircularArcBCalculateTrajectory();
                    break;
                case LineCalculateTrajectoryType.Circle:
                    item.CalculateTrajectory = new CircleCalculateTrajectory();
                   
                    break;
                default:
                    break;
            }
            item.LineCalculateTrajectoryType = type;
            item.CalculateTrajectory.TrackID = Guid.NewGuid();
            item.CalculateTrajectory.SerialNumber = ItemsSource.Count + 1;

            var newItem = new LineCalculateTrajectoryItemViewModel();
            newItem.SubscribPointChanged(_pointChanged);


            //轨迹中的第一条线才会自动增加一条起点
            if (ItemsSource.Count == 0)
            {
                PointInfo startPoint = new PointInfo();
                addStartPoint(startPoint, newItem);
            }

            newItem.CopyFrom(item);
            newItem.AfterModified += onAfterModified;
            ItemsSource.Add(newItem);
            SelectedItem = newItem;
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="lineCalculateTrajectory"></param>
        public void CopyFrom(LineCalculateTrajectory lineCalculateTrajectory)
        {
            if (lineCalculateTrajectory == null)
                return;

            // 初始化轨迹项
            ItemsSource.Clear();
            bool fisrtPoint = true;
            if (lineCalculateTrajectory.CalculateTrajectoryItem != null)
            {
               
                foreach (var item in lineCalculateTrajectory.CalculateTrajectoryItem)
                {
                    var itemVm = new LineCalculateTrajectoryItemViewModel();
                    itemVm.SubscribPointChanged(_pointChanged);
                    //轨迹中的第一条线才会自动增加一条起点
                    if (fisrtPoint)
                        addStartPoint(lineCalculateTrajectory.StartPoint, itemVm);
                    itemVm.CopyFrom(item);
                    itemVm.AfterModified += onAfterModified;
                    ItemsSource.Add(itemVm);
                    fisrtPoint = false;
                }
            }
        }
        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="lineCalculateTrajectory"></param>
        /// <exception cref="Exception"></exception>
        public void CopyTo(LineCalculateTrajectory lineCalculateTrajectory)
        {
            if (lineCalculateTrajectory == null)
                return;

            lineCalculateTrajectory.CalculateTrajectoryItem = new List<CalculateTrajectoryItem>();
            //轨迹中的第一条线才会自动增加一条起点
            if (ItemsSource.Count!=0)
            {
               var pointInfoItem= ItemsSource[0].PointInfoItems.FirstOrDefault(o => o.LinePointKind == LinePointKind.Start);
                if (pointInfoItem == null)
                    throw new Exception("不存在起点");
                PointInfo startPoint = new PointInfo();
                pointInfoItem.CopyTo(startPoint);
                lineCalculateTrajectory.StartPoint= startPoint;
            }
            foreach (var itemVm in ItemsSource)
            {
                var item = new CalculateTrajectoryItem();
                itemVm.CopyTo(item);
                lineCalculateTrajectory.CalculateTrajectoryItem.Add(item);
            }
           
        }
        private void addStartPoint(PointInfo startPoint, LineCalculateTrajectoryItemViewModel lineCalculateTrajectoryItemViewModel)
        {
            var startPointViewModel = new LineTrajectoryItemPointInfoViewModel();
            startPointViewModel.LinePointKind = LinePointKind.Start;
            startPointViewModel.CopyFrom(startPoint);
            lineCalculateTrajectoryItemViewModel.PointInfoItems.Insert(0, startPointViewModel);
        }

        /// <summary>
        /// 设置点坐标
        /// </summary>
        public void SetPointWhenEditChanged(decimal x, decimal y, decimal z)
        {
            if (this.SelectedItem == null)
                return;
            this.SelectedItem.SetPointWhenEditChanged(x,  y,  z);

        }
        /// <summary>
        /// 订阅点改变事件
        /// </summary>
        /// <param name="styleItemChanged"></param>
        public void SubscribPointChanged(EventHandler<PointChangedEventArgs>? pointChanged)
        {
            _pointChanged= pointChanged;
        }
        /// <summary>
        /// 平移
        /// </summary>
        /// <param name="translationParam"></param>
        public void Translation(TranslationParam translationParam)
        {
            foreach (LineCalculateTrajectoryItemViewModel itemVm in ItemsSource)
            {
                itemVm.Translation(translationParam);
            }
        }

        /// <summary>
        /// 平移
        /// </summary>
        /// <param name="trajectorySequenceModel"></param>
        private async void translationCommand(LineCalculateTrajectoryItemViewModel trajectorySequenceModel)
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
                WindowStartupLocation = WindowStartupLocation.CenterScreen
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
      
        /// <summary>
        /// 单条删除
        /// </summary>
        protected override async Task<bool> _deleteItem(LineCalculateTrajectoryItemViewModel item)
        {
           bool nret= await base._deleteItem(item);
            if(nret)
            {
                if (ItemsSource.Count > 0)
                {
                    resetStartPoint(item);
                    SelectedItem = ItemsSource[0];
                }
            }
            return nret;
        }
        /// <summary>
        /// 批量删除项
        /// </summary>
        protected override async Task<bool> _batchDeleteItems()
        {
            LineCalculateTrajectoryItemViewModel? startVM=null;
            foreach (var item in SelectedItems)
            {
                var startPointInfoItem = item.PointInfoItems.FirstOrDefault(o => o.LinePointKind == LinePointKind.Start);
                if (startPointInfoItem != null)
                {
                    startVM = item;
                    break;
                }
            }
            bool success = await base._batchDeleteItems();
            if (!success)
                return success;
            if(startVM!=null)
                resetStartPoint(startVM);
            return true;
        }

        /// <summary>
        /// 执行上移操作
        /// </summary>
        protected override void executeUpMove(LineCalculateTrajectoryItemViewModel trajectorySequenceModel)
        {
            if (SelectedItem == null) return;

            // 获取选中项当前索引
            int currentIndex = ItemsSource.IndexOf(trajectorySequenceModel);
            if (currentIndex <= 0) return; // 不能上移第1项
            // 获取前一项
            var prevItem = ItemsSource[currentIndex - 1];

            base.executeUpMove(trajectorySequenceModel);

            //是第二项上移时，则重置起点
            if (currentIndex == 1)
            {
                resetStartPoint(prevItem);
            }
        }

        /// <summary>
        /// 执行下移操作
        /// </summary>
        protected override void executeDownMove(LineCalculateTrajectoryItemViewModel trajectorySequenceModel)
        {
            if (SelectedItem == null) return;

            // 获取选中项当前索引
            int currentIndex = ItemsSource.IndexOf(trajectorySequenceModel);
            if (currentIndex >= ItemsSource.Count - 1) return; //不能下移最后1项

            // 获取后一项
            var nextItem = ItemsSource[currentIndex + 1];

            base.executeDownMove(trajectorySequenceModel);

            //是第一项时则需要重置起点
            if (currentIndex == 0)
                resetStartPoint(trajectorySequenceModel);
        }

        #endregion
        /// <summary>
        /// 将起点设置到第一条线上
        /// </summary>
        /// <param name="currentVm">线计算轨迹项</param>
        private void resetStartPoint(LineCalculateTrajectoryItemViewModel currentVm)
        {
            if (ItemsSource.Count == 0)
                return;
            //找到有起点的线的起点，即第一条线
            var startPointInfoItem = currentVm.PointInfoItems.FirstOrDefault(o => o.LinePointKind == LinePointKind.Start);
            if (startPointInfoItem != null)
            {
                PointInfo startPoint = new PointInfo();
                startPointInfoItem.CopyTo(startPoint);
                //将起点加到第一条线
                addStartPoint(startPoint, ItemsSource[0]);
                currentVm.PointInfoItems.Remove(startPointInfoItem);
            }
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
    }

}