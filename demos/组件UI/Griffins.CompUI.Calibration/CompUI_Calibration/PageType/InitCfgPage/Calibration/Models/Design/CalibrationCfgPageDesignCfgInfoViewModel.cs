using Griffins.CompUI.Calibration.ViewModels;
using GKG.UI;
using Griffins.CompUI.Calibration.Models;
using ReactiveUI;

namespace Griffins.CompUI.Calibration.Models;

/// <summary>
/// 标定配置页面设计时配置-视图模型
/// </summary>
public class CalibrationCfgPageDesignCfgInfoViewModel : ReactiveObject
{
    /// <summary>
    /// 模组别名-数据模型
    /// </summary>
    public TextInputViewModel ModulesAliasViewModel { get; }
    /// <summary>
    /// 模组别名
    /// </summary>
    public string CalibrationTechnicalModulesAlias
    {
        get => ModulesAliasViewModel.Text;
        set
        {
            if (ModulesAliasViewModel.Text == value) return;
            ModulesAliasViewModel.Text = value;
            this.RaisePropertyChanged(nameof(CalibrationTechnicalModulesAlias));
        }
    }
    public CalibrationCfgPageDesignCfgInfoViewModel()
    {
        ModulesAliasViewModel = new TextInputViewModel();
        ModulesAliasViewModel.ValueChanged += ModulesAliasViewModel_ValueChanged;
    }
    /// <summary>
    /// 从源复制
    /// </summary>
    /// <param name="calibrationCfgPageDesignCfgInfo"></param>
    public void CopyFrom(CalibrationCfgPageDesignCfgInfo calibrationCfgPageDesignCfgInfo)
    {
        CalibrationTechnicalModulesAlias = calibrationCfgPageDesignCfgInfo.ModulesAlias;
    }
    /// <summary>
    /// 复制到
    /// </summary>
    /// <param name="calibrationCfgPageDesignCfgInfo"></param>
    public void CopyTo(CalibrationCfgPageDesignCfgInfo calibrationCfgPageDesignCfgInfo)
    {
        calibrationCfgPageDesignCfgInfo.ModulesAlias = CalibrationTechnicalModulesAlias;
    }
    private void ModulesAliasViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(CalibrationTechnicalModulesAlias));
    }
}