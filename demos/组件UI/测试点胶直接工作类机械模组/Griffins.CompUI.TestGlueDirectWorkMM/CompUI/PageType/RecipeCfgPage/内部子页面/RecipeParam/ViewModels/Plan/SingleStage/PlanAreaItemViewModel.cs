using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.Plan;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.Plan;

/// <summary>
/// 方案中区域项-视图模型
/// </summary>
public class PlanAreaItemViewModel : DataGridItemBaseViewModel<PlanAreaInfo>
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
    /// 构造函数
    /// </summary>
    public PlanAreaItemViewModel()
    {
        // 初始化标签控件
        AreaIDLabel = new TextBlockViewModel();
        AreaNameViewModel = new TextInputViewModel();
    }
    /// <summary>
    /// 从实体复制数据
    /// </summary>
    public override void CopyFrom(PlanAreaInfo entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        base.CopyBasePropertiesFrom(entity);

        AreaID = entity.AreaID;
        //赋值区域名称
        var areaInfos = CacheDataExchange.GetAreaListes();
        this.AreaName= areaInfos.FirstOrDefault(o=>o.AreaID==AreaID)?.AreaName??"";
    }

    /// <summary>
    /// 复制数据到实体
    /// </summary>
    public override void CopyTo(PlanAreaInfo entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        base.CopyBasePropertiesTo(entity);

        entity.AreaID = AreaID;
    }

}