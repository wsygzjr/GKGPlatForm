using GKG.UI.General;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models
{
    public class PreDispensingCfgModel
    {
        public bool EnablePreDispensing { get; set; }

        public bool EnableRealTimeHeightMeasurement { get; set; }

        public bool Enable2D { get; set; }

        public bool EnableAutoReset { get; set; }

        public int SinglePreDispensingCount { get; set; } = 1;

        public DispensingType PreDispensingType { get; set; } = DispensingType.Point;

        public int XCount { get; set; }

        public int YCount { get; set; }

        public string DispensingStyle { get; set; } = string.Empty;

        public int DispensingPointCount { get; set; }

        public int RemainingCount { get; set; }

        public BasePositionInfo DotMatrix_LeftUpperPositionInfo { get; set; } = new BasePositionInfo();

        public BasePositionInfo DotMatrix_LeftLowerPositionInfo { get; set; } = new BasePositionInfo();

        public BasePositionInfo DotMatrix_RightUpperPositionInfo { get; set; } = new BasePositionInfo();
    }
}
