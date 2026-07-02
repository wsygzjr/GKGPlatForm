using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area
{
    /// <summary>
    /// 矩阵参数-视图模型
    /// </summary>
    public class DMatrixParamViewModel : ReactiveObject
    {
        #region 私有字段

        /// <summary>
        /// 视图引用（用于弹窗等UI操作）
        /// </summary>
        private Control? _viewReference;
        #endregion

        #region UI组件模型

        /// <summary>
        /// 列数-数字输入框模型
        /// </summary>
        public NumericViewModel ColumnCountViewModel { get; }

        /// <summary>
        /// 行数-数字输入框模型
        /// </summary>
        public NumericViewModel RowCountViewModel { get; }

        /// <summary>
        /// 起始点-下拉框模型
        /// </summary>
        public ComboxViewModel StartPointModel { get; }

        /// <summary>
        /// 矩阵方向-下拉框模型
        /// </summary>
        public ComboxViewModel MatrixDirectionModel { get; }

        /// <summary>
        /// 路径形状-下拉框模型
        /// </summary>
        public ComboxViewModel PathShapeModel { get; }
        #endregion

        #region 响应式属性
        
        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount
        {
            get => (int)ColumnCountViewModel.Value;
            set => ColumnCountViewModel.Value = value;
        }

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount
        {
            get => (int)RowCountViewModel.Value;
            set => RowCountViewModel.Value = value;
        }

        /// <summary>
        /// 起始点
        /// </summary>
        public StartPointType SelectedStartPoint
        {
            get => (StartPointType)((StartPointModel.SelectedItem as ComBoxItem)?.Value ?? StartPointType.TopLeft);
            set
            {
                if (StartPointModel.ItemsSource != null)
                {
                    var targetItem = StartPointModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (StartPointType)o.Value == value);
                    if (targetItem != null)
                        StartPointModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedStartPoint));
                }
            }
        }

        /// <summary>
        /// 矩阵方向
        /// </summary>
        public MatrixDirectionType SelectedMatrixDirection
        {
            get => (MatrixDirectionType)((MatrixDirectionModel.SelectedItem as ComBoxItem)?.Value ?? MatrixDirectionType.X);
            set
            {
                if (MatrixDirectionModel.ItemsSource != null)
                {
                    var targetItem = MatrixDirectionModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (MatrixDirectionType)o.Value == value);
                    if (targetItem != null)
                        MatrixDirectionModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedMatrixDirection));
                }
            }
        }

        /// <summary>
        /// 路径形状
        /// </summary>
        public PathShapeType SelectedPathShape
        {
            get => (PathShapeType)((PathShapeModel.SelectedItem as ComBoxItem)?.Value ?? PathShapeType.Line);
            set
            {
                if (PathShapeModel.ItemsSource != null)
                {
                    var targetItem = PathShapeModel.ItemsSource.Cast<ComBoxItem>()
                        .FirstOrDefault(o => (PathShapeType)o.Value == value);
                    if (targetItem != null)
                        PathShapeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedPathShape));
                }
            }
        }


        /// <summary>
        /// 点阵左上位置-视图模型
        /// </summary>

        public CamreaPositionViewModel DotMatrix_LeftUpperPositionViewModel { get; }
        /// <summary>
        /// 点阵右上位置-视图模型
        /// </summary>

        public CamreaPositionViewModel DotMatrix_RightUpperPositionViewModel { get; }
        /// <summary>
        /// 点阵右下位置-视图模型
        /// </summary>

        public CamreaPositionViewModel DotMatrix_RightLowerPositionViewModel { get; }
        #endregion


        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;

        /// <summary>
        /// 构造函数（初始化组件模型、数据源、事件订阅）
        /// </summary>
        public DMatrixParamViewModel()
        {
            StartPointModel = new ComboxViewModel();
            initStartPointDataSource();

            MatrixDirectionModel = new ComboxViewModel();
            initMatrixDirectionDataSource();

            PathShapeModel = new ComboxViewModel();
            initPathShapeDataSource();

            ColumnCountViewModel = new NumericViewModel
            {
                Minimum = 1,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 5,
                Increment = 1
            };

            RowCountViewModel = new NumericViewModel
            {
                Minimum = 1,
                Maximum = 100,
                DecimalPlaces = 0,
                Value = 1,
                Increment = 1
            };

            DotMatrix_LeftUpperPositionViewModel = new CamreaPositionViewModel();
            DotMatrix_RightUpperPositionViewModel = new CamreaPositionViewModel();
            DotMatrix_RightLowerPositionViewModel = new CamreaPositionViewModel();

            // 订阅值变更事件
            subscribeValueChanges();

        }

        #region 数据同步方法
        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        public void CopyFrom(DMatrixParamCfgInfo matrixParamCfgInfo)
        {
            if (matrixParamCfgInfo == null)
            {
                resetToDefault();
                return;
            }

            // 映射所有字段到视图模型
            ColumnCount = matrixParamCfgInfo.ColumnCount;
            RowCount = matrixParamCfgInfo.RowCount;
            SelectedStartPoint = matrixParamCfgInfo.StartPoint;
            SelectedMatrixDirection = matrixParamCfgInfo.MatrixDirection;
            SelectedPathShape = matrixParamCfgInfo.PathShape;

            DotMatrix_LeftUpperPositionViewModel.CopyFrom(matrixParamCfgInfo.DotMatrix_LeftUpperPositionInfo);
            DotMatrix_RightUpperPositionViewModel.CopyFrom(matrixParamCfgInfo.DotMatrix_RightUpperPositionInfo);
            DotMatrix_RightLowerPositionViewModel.CopyFrom(matrixParamCfgInfo.DotMatrix_RightLowerPositionInfo);
        }

        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        public void CopyTo(DMatrixParamCfgInfo matrixParamCfgInfo)
        {
            if (matrixParamCfgInfo == null) return;

            matrixParamCfgInfo.ColumnCount = ColumnCount;
            matrixParamCfgInfo.RowCount = RowCount;
            matrixParamCfgInfo.StartPoint = SelectedStartPoint;
            matrixParamCfgInfo.MatrixDirection = SelectedMatrixDirection;
            matrixParamCfgInfo.PathShape = SelectedPathShape;

            DotMatrix_LeftUpperPositionViewModel.CopyTo(matrixParamCfgInfo.DotMatrix_LeftUpperPositionInfo);
            DotMatrix_RightUpperPositionViewModel.CopyTo(matrixParamCfgInfo.DotMatrix_RightUpperPositionInfo);
            DotMatrix_RightLowerPositionViewModel.CopyTo(matrixParamCfgInfo.DotMatrix_RightLowerPositionInfo);
        }

        /// <summary>
        /// 设置视图引用（用于弹窗等UI操作）
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
            DotMatrix_LeftUpperPositionViewModel.SetViewReference(view);
            DotMatrix_RightUpperPositionViewModel.SetViewReference(view);
            DotMatrix_RightLowerPositionViewModel.SetViewReference(view);
        }
        #endregion

        #region 数据源初始化
        
        /// <summary>
        /// 初始化起始点下拉框数据源
        /// </summary>
        private void initStartPointDataSource()
        {
            var startPointDisplayNames = new Dictionary<StartPointType, string>
            {
                { StartPointType.TopLeft, "左上" },
                { StartPointType.TopRight, "右上" },
                { StartPointType.BottomLeft, "左下" },
                { StartPointType.BottomRight, "右下" }
            };

            StartPointModel.ItemsSource = EnumExtensions.ToEnumItems(startPointDisplayNames);
            StartPointModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        }

        /// <summary>
        /// 初始化矩阵方向下拉框数据源
        /// </summary>
        private void initMatrixDirectionDataSource()
        {
            var directionDisplayNames = new Dictionary<MatrixDirectionType, string>
            {
                { MatrixDirectionType.X, "X方向" },
                { MatrixDirectionType.Y, "Y方向" },
                { MatrixDirectionType.Z, "Z方向" },
            };

            MatrixDirectionModel.ItemsSource = EnumExtensions.ToEnumItems(directionDisplayNames);
            MatrixDirectionModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        }

        /// <summary>
        /// 初始化路径形状下拉框数据源
        /// </summary>
        private void initPathShapeDataSource()
        {
            var shapeDisplayNames = new Dictionary<PathShapeType, string>
            {
                { PathShapeType.S, "S型" },
                { PathShapeType.Line, "直线" },
                { PathShapeType.Zigzag, "锯齿形" },
                { PathShapeType.Circle, "圆形" }
            };

            PathShapeModel.ItemsSource = EnumExtensions.ToEnumItems(shapeDisplayNames);
            PathShapeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void resetToDefault()
        {
            ColumnCount = 5;
            RowCount = 1;
            SelectedStartPoint = StartPointType.TopLeft;
            SelectedMatrixDirection = MatrixDirectionType.X;
            SelectedPathShape = PathShapeType.Line;
        }
       
        #endregion

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            DotMatrix_LeftUpperPositionViewModel.AfterModified += onAfterModified;
            DotMatrix_RightUpperPositionViewModel.AfterModified += onAfterModified;
            DotMatrix_RightLowerPositionViewModel.AfterModified += onAfterModified;

            StartPointModel.ValueChanged += onValueChanged;
            MatrixDirectionModel.ValueChanged += onValueChanged;
            PathShapeModel.ValueChanged += onValueChanged;
            ColumnCountViewModel.ValueChanged += onValueChanged;
            RowCountViewModel.ValueChanged += onValueChanged;
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