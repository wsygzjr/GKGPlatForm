namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.DebugPage
{
    public static class AxisDebugAxisNameConstants
    {
        public static readonly (string DisplayName, string AxisKey)[] Default16Axis = new (string DisplayName, string AxisKey)[]
        {
            ("X轴", "X"),
            ("Y轴", "Y"),
            ("Z轴", "Z"),
            ("Vx轴", "Vx"),
            ("Vy轴", "Vy"),
            ("前导轨调宽轴", "FrontGuideWidth"),
            ("中导轨调宽轴", "MiddleGuideWidth"),
            ("后导轨调宽轴", "RearGuideWidth"),
            ("A轨_运输皮带轴", "ATrackBelt"),
            ("B轨_运输皮带轴", "BTrackBelt"),
            ("清洗轴", "CleanAxis1"),
            ("清洗轴2", "CleanAxis2"),
            ("中间顶升平台轴", "MiddleLift"),
            ("倾斜轴", "Tilt"),
            ("回流轨运轴", "ReturnTrack"),
            ("回流调宽轴", "ReturnWidth"),
        };
    }
}
