using Avalonia.Controls;
using DispensingPageType.Views.RecipeParamCfgPage.ParametersAndCalculation;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan;

/// <summary>
/// 点胶流程项-视图模型
/// </summary>
public class DispensingProcessItemViewModel : DataGridItemBaseViewModel<DispensingProcessItemInfo>
{

    /// <summary>
    /// 流程ID标签-数据模型
    /// </summary>
    public TextBlockViewModel ProcessIDLabel { get; }

    /// <summary>
    /// 流程名称标签-数据模型
    /// </summary>
    public TextInputViewModel ProcessNameVieMode { get; }
    /// <summary>
    /// 点胶流程类型方式下拉框-数据模型
    /// </summary>
    public ComboxViewModel DispensingProcessTypeComboBox { get; }

    /// <summary>
    /// 单段流程方案配置参数
    /// </summary>
    private SingleStagePlanConfigViewModel? _singleStagePlanConfigViewModel;
    private SingleStagePlanConfigView? _singleStagePlanConfigView;

    /// <summary>
    /// 分段模式设置配置参数
    /// </summary>
    private SegmentationStageModeViewModel? _segmentationStageModeViewModel;
    private SegmentationStageModeView? _segmentationStageModeView;

    /// <summary>
    /// 当前工作区视图
    /// 创建对象时就决定是工作区的view，所以不需要通知UI处理
    /// </summary>
    public object? WorkAreaView { get; set; }
    /// <summary>
    /// 流程ID
    /// </summary>
    public Guid ProcessID
    {
        get => Guid.Parse(ProcessIDLabel.Text);
        set
        {
            ProcessIDLabel.Text = value.ToString();
            this.RaisePropertyChanged(nameof(ProcessID));
        }
    }
    /// <summary>
    /// 流程名称
    /// </summary>
    public string ProcessName
    {
        get => ProcessNameVieMode.Text;
        set
        {
            ProcessNameVieMode.Text = value;
            this.RaisePropertyChanged(nameof(ProcessName));
        }
    }

    /// <summary>
    /// 点胶流程类型方式
    /// </summary>
    public DispensingProcessType DispensingProcessType
    {
        get => (DispensingProcessType)((DispensingProcessTypeComboBox.SelectedItem as ComBoxItem)?.Value ?? DispensingProcessType.SingleStage);
        set
        {
            if (DispensingProcessTypeComboBox.ItemsSource != null)
            {
                var targetItem = DispensingProcessTypeComboBox.ItemsSource.Cast<ComBoxItem>()
                .FirstOrDefault(o => (DispensingProcessType)o.Value == value);
                if (targetItem != null)
                    DispensingProcessTypeComboBox.SelectedItem = targetItem;
                this.RaisePropertyChanged(nameof(DispensingProcessType));
                onDispensingProcessTypeChanged();
            }
        }
    }
   

    /// <summary>
    /// 构造函数
    /// </summary>
    public DispensingProcessItemViewModel()
    {
        // 初始化标签控件
        ProcessIDLabel = new TextBlockViewModel();
        ProcessNameVieMode = new TextInputViewModel();
        // 初始化编辑方式下拉框
        DispensingProcessTypeComboBox = new ComboxViewModel();
        DispensingProcessTypeComboBox.ValueChanged += onDispensingProcessTypeChanged;
        initDispensingProcessTypeComboBox();
        // 订阅值变更事件
        subscribeValueChanges();


    }
    /// <summary>
    /// 从实体复制数据
    /// </summary>
    public override void CopyFrom(DispensingProcessItemInfo entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        base.CopyBasePropertiesFrom(entity);

        ProcessID = entity.ID;
        ProcessName = entity.ProcessName;
        SerialNumber = entity.SerialNumber;
        DispensingProcessType = entity.DispensingProcessType;
        switch (DispensingProcessType)
        {
            case DispensingProcessType.SingleStage:
                if (_singleStagePlanConfigViewModel == null)
                    throw new Exception("单段流程方案视图模型为空");
                SingleStagePlanInfo singleStagePlanInfo = (SingleStagePlanInfo)entity.DispensingProcessInfo;
                _singleStagePlanConfigViewModel.CopyFrom(singleStagePlanInfo);
                break;
            case DispensingProcessType.SegmentationStage:
                if (_segmentationStageModeViewModel == null)
                    throw new Exception("分段模式方案视图模型为空");
                SegmentationStagePlanInfo segmentationStagePlanInfo = (SegmentationStagePlanInfo)entity.DispensingProcessInfo;
                _segmentationStageModeViewModel.CopyFrom(segmentationStagePlanInfo);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 复制数据到实体
    /// </summary>
    public override void CopyTo(DispensingProcessItemInfo entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        base.CopyBasePropertiesTo(entity);

        entity.ID = ProcessID;
        entity.SerialNumber = SerialNumber;
        entity.ProcessName = ProcessName;
        entity.DispensingProcessType = DispensingProcessType;

        switch (DispensingProcessType)
        {
            case DispensingProcessType.SingleStage:
                if (_singleStagePlanConfigViewModel == null)
                    throw new Exception("单段流程方案视图模型为空");
                SingleStagePlanInfo singleStagePlanInfo =new SingleStagePlanInfo();
                _singleStagePlanConfigViewModel.CopyTo(singleStagePlanInfo);
                entity.DispensingProcessInfo = singleStagePlanInfo;
                break;
            case DispensingProcessType.SegmentationStage:
                if (_segmentationStageModeViewModel == null)
                    throw new Exception("分段模式方案视图模型为空");
                SegmentationStagePlanInfo segmentationStagePlanInfo = new SegmentationStagePlanInfo();
                _segmentationStageModeViewModel.CopyTo(segmentationStagePlanInfo);
                entity.DispensingProcessInfo = segmentationStagePlanInfo;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 点胶流程类型改变
    /// </summary>
    private void onDispensingProcessTypeChanged()
    {
        switch (DispensingProcessType)
        {
            case DispensingProcessType.SingleStage:
                _singleStagePlanConfigViewModel = new SingleStagePlanConfigViewModel();
                _singleStagePlanConfigViewModel.SetViewReference(_viewReference!);
                _singleStagePlanConfigViewModel.AfterModified += onAfterModified;
                _singleStagePlanConfigView = new SingleStagePlanConfigView { DataContext = _singleStagePlanConfigViewModel };
                WorkAreaView = _singleStagePlanConfigView;
                break;
            case DispensingProcessType.SegmentationStage:
                _segmentationStageModeViewModel = new SegmentationStageModeViewModel();
                _segmentationStageModeViewModel.SetViewReference(_viewReference!);
                _segmentationStageModeViewModel.AfterModified += onAfterModified;
                _segmentationStageModeView = new SegmentationStageModeView { DataContext = _segmentationStageModeViewModel };

                WorkAreaView = _segmentationStageModeView;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 初始化编辑方式下拉框数据源
    /// </summary>
    private void initDispensingProcessTypeComboBox()
    {
        var dispensingProcessTypeItems = new List<ComBoxItem>
            {
                new ComBoxItem { Value = DispensingProcessType.SingleStage, DisplayName = "单段流程" },
                new ComBoxItem { Value = DispensingProcessType.SegmentationStage, DisplayName = "分段流程" }
            };

        DispensingProcessTypeComboBox.ItemsSource = dispensingProcessTypeItems;
        DispensingProcessTypeComboBox.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
    }

    /// <summary>
    /// 编辑方式下拉框值变更事件处理
    /// </summary>
    private void onDispensingProcessTypeChanged(object? sender, EventArgs e)
    {
        if (DispensingProcessTypeComboBox.SelectedItem is ComBoxItem selectedItem)
        {
            DispensingProcessType = (DispensingProcessType)selectedItem.Value;
        }
    }
    /// <summary>
    /// 设置视图引用
    /// </summary>
    public new void SetViewReference(Control view)
    {
        _viewReference = view;
    }
    #region 值改变事件
    /// <summary>
    /// 订阅值改变事件
    /// </summary>
    private void subscribeValueChanges()
    {

        ProcessIDLabel.ValueChanged += onValueChanged;
        ProcessNameVieMode.ValueChanged += onValueChanged;
        DispensingProcessTypeComboBox.ValueChanged += onValueChanged;
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