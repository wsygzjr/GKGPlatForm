using Avalonia.Controls;
using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Area;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Area;

/// <summary>
/// 区域项-视图模型
/// </summary>
public class AreaItemViewModel : DataGridItemBaseViewModel<AreaInfo>
{
    /// <summary>
    /// 区域ID标签-数据模型
    /// </summary>
    public TextBlockViewModel AreaIDLabel { get; }
    /// <summary>
    ///区域名称
    /// </summary>
    public TextInputViewModel AreaNameViewModel { get; }
    /// <summary>
    /// 区域编辑方式下拉框-数据模型
    /// </summary>
    public ComboxViewModel EditModeComboBox { get; }

    /// <summary>
    /// 区域ID
    /// </summary>
    public Guid AreaID
    {
        get => Guid.Parse(AreaIDLabel.Text);
        set
        {
            AreaIDLabel.Text = value.ToString();
            this.RaisePropertyChanged(nameof(AreaID));
        }
    }
    /// <summary>
    /// 区域名称
    /// </summary>
    public string AreaName
    {
        get => AreaNameViewModel.Text;
        set
        {
            AreaNameViewModel.Text = value;
            this.RaisePropertyChanged(nameof(AreaName));
        }
    }

    /// <summary>
    /// 区域编辑方式
    /// </summary>
    public AreaEditMode EditMode
    {
        get => (AreaEditMode)((EditModeComboBox.SelectedItem as ComBoxItem)?.Value ?? AreaEditMode.CustomEdit);
        set
        {
            if (EditModeComboBox.ItemsSource != null)
            {
                var targetItem = EditModeComboBox.ItemsSource.Cast<ComBoxItem>()
                .FirstOrDefault(o => (AreaEditMode)o.Value == value);
                if (targetItem != null)
                    EditModeComboBox.SelectedItem = targetItem;
                this.RaisePropertyChanged(nameof(EditMode));
            }
        }
    }
    /// <summary>
    /// 自动生成参数区域
    /// </summary>
    public AutoGenerateAreaViewModel AutoGenerateAreaViewModel { get; }
    /// <summary>
    /// 区域编辑-点胶执行对象列表视图模型
    /// </summary>
    public DispensingExecutionListViewModel DispensingExecutionListViewModel { get; }

    /// <summary>
    /// 获取模板实例列表
    /// </summary>
    /// <returns></returns>
    public List<DispensingTemplateInstanceExecutionObject> GetCommandTemplateInstance()
    {
        switch (EditMode)
        {
            case AreaEditMode.AutoGenerate:
                return AutoGenerateAreaViewModel.GetCommandTemplateInstance();
            case AreaEditMode.CustomEdit:
                return DispensingExecutionListViewModel.GetCommandTemplateInstance();
            default:
                throw new Exception("不支持的区域编辑方式");
        }
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public AreaItemViewModel()
    {
        // 初始化标签控件
        AreaIDLabel = new TextBlockViewModel();
        AreaNameViewModel = new TextInputViewModel();
        // 初始化编辑方式下拉框
        EditModeComboBox = new ComboxViewModel();
        EditModeComboBox.ValueChanged += onEditModeChanged;
        initEditModeComboBox();

        EditMode = AreaEditMode.AutoGenerate;

        AutoGenerateAreaViewModel=new AutoGenerateAreaViewModel();
        DispensingExecutionListViewModel=new DispensingExecutionListViewModel();
        // 订阅值变更事件
        subscribeValueChanges();
    }

    /// <summary>
    /// 初始化编辑方式下拉框数据源
    /// </summary>
    private void initEditModeComboBox()
    {
        var editModeItems = new List<ComBoxItem>
            {
                new ComBoxItem { Value = AreaEditMode.AutoGenerate, DisplayName = "采用规则自动生成" },
                new ComBoxItem { Value = AreaEditMode.CustomEdit, DisplayName = "自定义编辑" }
            };

        EditModeComboBox.ItemsSource = editModeItems;
        EditModeComboBox.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        EditModeComboBox.SelectedItem = editModeItems.FirstOrDefault();
    }

    /// <summary>
    /// 编辑方式下拉框值变更事件处理
    /// </summary>
    private void onEditModeChanged(object? sender, EventArgs e)
    {
        if (EditModeComboBox.SelectedItem is ComBoxItem selectedItem)
        {
            EditMode = (AreaEditMode)selectedItem.Value;
        }
    }

    /// <summary>
    /// 从实体复制数据
    /// </summary>
    public override void CopyFrom(AreaInfo entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        base.CopyBasePropertiesFrom(entity);

        AreaID = entity.AreaID;
        AreaName = entity.AreaName;
        EditMode = entity.EditMode;

        AutoGenerateAreaViewModel.CopyFrom(entity.AutoGenerateAreaInfo);
        DispensingExecutionListViewModel.CopyFrom(entity.DispensingExecutionObjects);
    }

    /// <summary>
    /// 复制数据到实体
    /// </summary>
    public override void CopyTo(AreaInfo entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        base.CopyBasePropertiesTo(entity);

        entity.AreaID = AreaID;
        entity.AreaName = AreaName;
        entity.EditMode = EditMode;
        AutoGenerateAreaViewModel.CopyTo(entity.AutoGenerateAreaInfo);
        DispensingExecutionListViewModel.CopyFrom(entity.DispensingExecutionObjects);
    }


    /// <summary>
    /// 设置视图引用
    /// </summary>
    public override void SetViewReference(Control view)
    {
        base.SetViewReference(view);
        AutoGenerateAreaViewModel.SetViewReference(view);
        DispensingExecutionListViewModel.SetViewReference(view);
        
    }
    #region 值改变事件
    /// <summary>
    /// 订阅值改变事件
    /// </summary>
    private void subscribeValueChanges()
    {
        AutoGenerateAreaViewModel.AfterModified += onAfterModified;
        DispensingExecutionListViewModel.AfterModified += onAfterModified;

        AreaNameViewModel.ValueChanged += onValueChanged;
        EditModeComboBox.ValueChanged += onValueChanged;
       
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