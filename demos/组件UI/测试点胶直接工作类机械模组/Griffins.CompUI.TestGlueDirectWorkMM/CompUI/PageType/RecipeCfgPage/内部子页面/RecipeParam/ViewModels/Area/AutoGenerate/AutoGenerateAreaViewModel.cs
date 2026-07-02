using Avalonia.Controls;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.AutoGenerate;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area.CustomEdit;
using ReactiveUI;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area;

/// <summary>
/// 自动生成参数区域-视图模型
/// </summary>
public class AutoGenerateAreaViewModel : ReactiveObject
{
    /// <summary>
    /// 视图引用（用于弹窗等UI操作）
    /// </summary>
    private Control? _viewReference;
    /// <summary>
    /// 值改变事件
    /// </summary>
    public EventHandler? AfterModified;
    /// <summary>
    /// 一维矩阵参数
    /// </summary>
    private OneDMatrixParamViewModel? _oneDMatrixParamViewModel;
    /// <summary>
    /// 一维矩阵工作区视图实例
    /// </summary>
    private OneDMatrixParamView? _oneDMatrixParamView;

    /// <summary>
    /// 二维矩阵参数
    /// </summary>
    private TwoDMatrixParamViewModel? _twoDMatrixParamViewModel;
    /// <summary>
    /// 二维矩阵工作区视图实例
    /// </summary>
    private TwoDMatrixParamView? _twoDMatrixParamView;
    /// <summary>
    /// 区域自动生成方式下拉框-数据模型
    /// </summary>
    public ComboxViewModel AutoGenerateModeComboBox { get; }

    /// <summary>
    /// 区域自动生成方式
    /// </summary>
    public AutoGenerateMode AutoGenerateMode
    {
        get => (AutoGenerateMode)((AutoGenerateModeComboBox.SelectedItem as ComBoxItem)?.Value ?? AutoGenerateMode.OneDimension);
        set
        {
            if (AutoGenerateModeComboBox.ItemsSource != null)
            {
                var targetItem = AutoGenerateModeComboBox.ItemsSource.Cast<ComBoxItem>()
                .FirstOrDefault(o => (AutoGenerateMode)o.Value == value);
                if (targetItem != null)
                    AutoGenerateModeComboBox.SelectedItem = targetItem;
                onAutoGenerateMode();
                this.RaisePropertyChanged(nameof(AutoGenerateMode));
            }
        }
    }

    /// <summary>
    /// 工作区的视图实例
    /// </summary>
    private object? _workAreaView;
    public object? WorkAreaView
    { 
        get=> _workAreaView; 
        set
        {
            _workAreaView = value;
            this.RaisePropertyChanged(nameof(WorkAreaView));
        }
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public AutoGenerateAreaViewModel()
    {
        // 初始化编辑方式下拉框
        AutoGenerateModeComboBox = new ComboxViewModel();
        AutoGenerateModeComboBox.ValueChanged += onAutoGenerateModeChanged;
        initAutoGenerateModeComboBox();

        AutoGenerateMode = AutoGenerateMode.OneDimension;
        // 订阅值变更事件
        subscribeValueChanges();

    }

    /// <summary>
    /// 从实体复制数据
    /// </summary>
    public  void CopyFrom(AutoGenerateAreaInfo entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        AutoGenerateMode = entity.AutoGenerateMode;
        switch (AutoGenerateMode)
        {
            case AutoGenerateMode.OneDimension:
                if (_oneDMatrixParamViewModel == null)
                    throw new Exception("一维矩阵参数视图模型为空");
                _oneDMatrixParamViewModel.CopyFrom(entity.OneDMatrixParamCfgInfo);
                break;
            case AutoGenerateMode.TwoDimension:
                if (_twoDMatrixParamViewModel == null)
                    throw new Exception("二维矩阵参数视图模型为空");
                _twoDMatrixParamViewModel.CopyFrom(entity.TwoDMatrixParamCfgInfo);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 复制数据到实体
    /// </summary>
    public  void CopyTo(AutoGenerateAreaInfo entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.AutoGenerateMode = AutoGenerateMode;
        switch (entity.AutoGenerateMode)
        {
            case AutoGenerateMode.OneDimension:
                if (_oneDMatrixParamViewModel == null)
                    throw new Exception("一维矩阵参数视图模型为空");
                _oneDMatrixParamViewModel.CopyTo(entity.OneDMatrixParamCfgInfo);
                break;
            case AutoGenerateMode.TwoDimension:
                if (_twoDMatrixParamViewModel == null)
                    throw new Exception("二维矩阵参数视图模型为空");
                _twoDMatrixParamViewModel.CopyTo(entity.TwoDMatrixParamCfgInfo);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 设置视图引用
    /// </summary>
    public void SetViewReference(Control view)
    {
        _viewReference= view;   
        //if (_oneDMatrixParamViewModel != null)
        //    _oneDMatrixParamViewModel.SetViewReference(view);
        //if (_twoDMatrixParamViewModel != null)
        //    _twoDMatrixParamViewModel.SetViewReference(view);
    }
    /// <summary>
    /// 获取模板实例列表
    /// </summary>
    /// <returns></returns>
    public List<DispensingTemplateInstanceExecutionObject> GetCommandTemplateInstance()
    {
        switch (AutoGenerateMode)
        {
            case AutoGenerateMode.OneDimension:
                if (_oneDMatrixParamViewModel == null)
                    throw new Exception("一维矩阵参数不能为空");
               return _oneDMatrixParamViewModel.GetCommandTemplateInstance();
            case AutoGenerateMode.TwoDimension:
                if (_twoDMatrixParamViewModel == null)
                    throw new Exception("二维矩阵参数不能为空");
                List<DispensingTemplateInstanceExecutionObject> instances = new List<DispensingTemplateInstanceExecutionObject>();
                for (int row = 0; row < _twoDMatrixParamViewModel.TowOfDMatrixParamViewModel.RowCount; row++)
                {
                    for (int column = 0; column < _twoDMatrixParamViewModel.TowOfDMatrixParamViewModel.ColumnCount; column++)
                    {
                        //获取二维中一维矩阵的模板实例列表
                        instances.AddRange(_twoDMatrixParamViewModel.GetCommandTemplateInstance());
                    }
                }
                return instances;
            default:
                throw new Exception("不支持的区域自动生成方式");
        }
    }
    /// <summary>
    /// 区域自动生成方式选项改变后
    /// </summary>
    private void onAutoGenerateMode()
    {
        switch (AutoGenerateMode)
        {
            case AutoGenerateMode.OneDimension:
                if (_oneDMatrixParamViewModel == null)
                {
                    _oneDMatrixParamViewModel = new OneDMatrixParamViewModel();
                    _oneDMatrixParamViewModel.CopyFrom(new OneDMatrixParamCfgInfo());
                    _oneDMatrixParamViewModel.AfterModified += onAfterModified;
                }
                if (_viewReference != null)
                    _oneDMatrixParamViewModel.SetViewReference(_viewReference);

                if (_oneDMatrixParamView == null)
                    _oneDMatrixParamView = new OneDMatrixParamView() { DataContext = _oneDMatrixParamViewModel };

                WorkAreaView = _oneDMatrixParamView;
                break;
            case AutoGenerateMode.TwoDimension:

                if (_twoDMatrixParamViewModel == null)
                {
                    _twoDMatrixParamViewModel = new TwoDMatrixParamViewModel();
                    _twoDMatrixParamViewModel.CopyFrom(new TwoDMatrixParamCfgInfo());
                    _twoDMatrixParamViewModel.AfterModified += onAfterModified;
                }
                if (_viewReference != null)
                    _twoDMatrixParamViewModel.SetViewReference(_viewReference);

                if (_twoDMatrixParamView == null)
                    _twoDMatrixParamView = new TwoDMatrixParamView() { DataContext = _twoDMatrixParamViewModel };

                WorkAreaView = _twoDMatrixParamView;
                break;
        }
    }
    /// <summary>
    /// 初始化编辑方式下拉框数据源
    /// </summary>
    private void initAutoGenerateModeComboBox()
    {
        var autoGenerateModeItems = new List<ComBoxItem>
            {
                new ComBoxItem { Value = AutoGenerateMode.OneDimension, DisplayName = "一维" },
                new ComBoxItem { Value = AutoGenerateMode.TwoDimension, DisplayName = "二维" }
            };

        AutoGenerateModeComboBox.ItemsSource = autoGenerateModeItems;
        AutoGenerateModeComboBox.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        AutoGenerateModeComboBox.SelectedItem = autoGenerateModeItems.FirstOrDefault();
    }

    /// <summary>
    /// 编辑方式下拉框值变更事件处理
    /// </summary>
    private void onAutoGenerateModeChanged(object? sender, EventArgs e)
    {
        if (AutoGenerateModeComboBox.SelectedItem is ComBoxItem selectedItem)
        {
            AutoGenerateMode = (AutoGenerateMode)selectedItem.Value;
        }
    }
    #region 值改变事件
    /// <summary>
    /// 订阅值改变事件
    /// </summary>
    private void subscribeValueChanges()
    {
        AutoGenerateModeComboBox.ValueChanged += onValueChanged;
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