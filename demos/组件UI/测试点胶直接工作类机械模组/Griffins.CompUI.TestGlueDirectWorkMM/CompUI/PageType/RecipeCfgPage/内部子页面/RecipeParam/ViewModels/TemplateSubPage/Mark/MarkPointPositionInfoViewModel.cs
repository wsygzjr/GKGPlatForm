using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins.UI;
using Griffins.UI.General;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Mark;
using ReactiveUI;
using System.Reactive;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Mark
{
    /// <summary>
    /// Mark点位置信息-
    /// </summary>
    public class MarkPointPositionInfoViewModel : DataGridItemBaseViewModel<MarkPointPositionInfo>
    {
        /// <summary>
        /// Mark识别参数 
        /// </summary>
        private MarkPointRecognizeCfgInfo _markPointRecognizeCfgInfo;

        /// <summary>
        /// 相机教导移动
        /// </summary>
        public CamreaPositionViewModel CamreaPositionViewModel { get; }
        /// <summary>
        /// Mark操作类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel MarkOpKindModel { get; }

        /// <summary>
        /// 选中的Mark操作类型
        /// </summary>
        public MarkOpKind SelectedMarkOpKind
        {
            get => (MarkOpKind)((MarkOpKindModel.SelectedItem as ComBoxItem)?.Value ?? MarkOpKind.Standard);
            set
            {
                if (MarkOpKindModel.ItemsSource != null)
                {
                    var targetItem = MarkOpKindModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (MarkOpKind)o.Value == value);
                    if (targetItem != null)
                        MarkOpKindModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedMarkOpKind));
                }
            }
        }


        /// <summary>
        /// 创建模板命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateTemplateCommand { get; }
        /// <summary>
        /// 测试模板命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> TestTemplateCommand { get; }
        /// <summary>
        /// 搜索定位命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> SearchLocationCommand { get; }

        public MarkPointPositionInfoViewModel()
        {
            CamreaPositionViewModel = new CamreaPositionViewModel();
            _markPointRecognizeCfgInfo = new MarkPointRecognizeCfgInfo();
            MarkOpKindModel = new ComboxViewModel();


            CreateTemplateCommand = ReactiveCommand.CreateFromTask(onCreateTemplateCommand);
            TestTemplateCommand = ReactiveCommand.CreateFromTask(onTestTemplateCommand);
            SearchLocationCommand = ReactiveCommand.CreateFromTask(onSearchLocationCommand);

            var markOpKindDisplayNames = new Dictionary<MarkOpKind, string>
            {
                { MarkOpKind.Standard, "标准" },
                { MarkOpKind.Camera, "摄像头模组" },
                { MarkOpKind.CornerCenter, "边角中心" },
                { MarkOpKind.Custom, "自定义" },
            };
            MarkOpKindModel.ItemsSource = EnumExtensions.ToEnumItems<MarkOpKind>(markOpKindDisplayNames);
            MarkOpKindModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            MarkOpKindModel.ValueChanged += onChildValueChanged;

            // 订阅值变更事件
            subscribeValueChanges();
        }


        /// <summary>
        /// 设置视图引用
        /// </summary>
        public override void SetViewReference(Control view)
        {
            base.SetViewReference(view);
            CamreaPositionViewModel.SetViewReference(view);
        }
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="markPointPositionInfo"></param>
        public override void CopyFrom(MarkPointPositionInfo markPointPositionInfo)
        {
            base.CopyBasePropertiesFrom(markPointPositionInfo);
            foreach (var axisValue in markPointPositionInfo.CoordinateAxisValues)
            {
                switch (axisValue.Axis)
                {
                    case CoordinateAxisConstant.X:
                        CamreaPositionViewModel.X = axisValue.AxisValue;
                        break;
                    case CoordinateAxisConstant.Y:
                        CamreaPositionViewModel.Y = axisValue.AxisValue;
                        break;
                    case CoordinateAxisConstant.Z:
                        CamreaPositionViewModel.Z = axisValue.AxisValue;
                        break;
                }
            }
            SelectedMarkOpKind = markPointPositionInfo.MarkOpKind;
            _markPointRecognizeCfgInfo.CopyFrom(markPointPositionInfo.MarkPointRecognizeCfgInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="markPointPositionInfo"></param>
        public override void CopyTo(MarkPointPositionInfo markPointPositionInfo)
        {
            base.CopyBasePropertiesTo(markPointPositionInfo);
            //markPointPositionInfo.CoordinateAxisValues = new CoordinateAxisValueList
            //{
            //    new CoordinateAxisValue { Axis = CoordinateAxisConstant.X, AxisValue = CamreaPositionViewModel.X },
            //    new CoordinateAxisValue { Axis = CoordinateAxisConstant.Y, AxisValue = CamreaPositionViewModel.Y },
            //    new CoordinateAxisValue { Axis = CoordinateAxisConstant.Z, AxisValue = CamreaPositionViewModel.Z }
            //};
            markPointPositionInfo.MarkOpKind = SelectedMarkOpKind;
            markPointPositionInfo.MarkPointRecognizeCfgInfo.CopyFrom(_markPointRecognizeCfgInfo);
        }
        /// <summary>
        /// 子组件值变更触发事件
        /// </summary>
        private void onChildValueChanged(object? sender, ValueChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(SelectedMarkOpKind));
        }

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            CamreaPositionViewModel.AfterModified += onAfterModified;

            MarkOpKindModel.ValueChanged += onValueChanged;
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

        #region 执行命令

        /// <summary>
        ///创建模板
        /// </summary>
        private async Task onCreateTemplateCommand()
        {
            try
            {
                var parentWindow = _viewReference?.GetVisualRoot() as Window;
                if (parentWindow == null)
                    throw new Exception("无法获取窗口上下文");

                //var editViewModel = new MarkTemplateEditWindowViewModel();
                //editViewModel.CopyFrom(_markPointRecognizeCfgInfo);
                //editViewModel.SetViewReference(_viewReference!);
                //var editWindow = new MarkTemplateEditWindow
                //{
                //    DataContext = editViewModel,
                //    Title = "Mark模板编辑",
                //    Width = 1920,
                //    Height = 1020,
                //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //};
                //editViewModel.SetViewReference(editWindow);

                //var result = await editWindow.ShowDialog<bool>(parentWindow);
                //if ((result))
                //{
                //    editViewModel.CopyTo(_markPointRecognizeCfgInfo);
                //    onAfterModified(this, new EventArgs());
                //}
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", $"{ex.Message}", _viewReference);
            }

        }
        /// <summary>
        ///测试模板
        /// </summary>
        private async Task onTestTemplateCommand()
        {
            try
            {
                var parentWindow = _viewReference?.GetVisualRoot() as Window;
                if (parentWindow == null)
                    throw new Exception("无法获取窗口上下文");

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", $"{ex.Message}", _viewReference);
            }
        }
        /// <summary>
        ///搜索定位
        /// </summary>
        private async Task onSearchLocationCommand()
        {
            try
            {
                var parentWindow = _viewReference?.GetVisualRoot() as Window;
                if (parentWindow == null)
                    throw new Exception("无法获取窗口上下文");

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                await MessageBox.ShowErrorDialog("错误", $"{ex.Message}", _viewReference);
            }
        }

        #endregion
    }


}