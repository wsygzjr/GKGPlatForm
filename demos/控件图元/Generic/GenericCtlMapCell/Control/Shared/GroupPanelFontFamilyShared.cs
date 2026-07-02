using System;
using System.ComponentModel;

namespace GKG.Map.MapCell.Generic.GroupPanel
{
    public enum FontFamilyType
    {
        [Description("微软雅黑")] MicrosoftYaHei = 0,
        [Description("宋体")] SimSun = 1,
        [Description("黑体")] SimHei = 2,
        [Description("楷体")] KaiTi = 3,
        [Description("仿宋")] FangSong = 4,
        [Description("新宋体")] NSimSun = 5,
        [Description("Arial")] Arial = 6,
        [Description("Times New Roman")] TimesNewRoman = 7,
        [Description("Consolas")] Consolas = 8,
        [Description("Segoe UI")] SegoeUI = 9,
    }

    public static class GroupPanelTextInfo
    {
        public static string GetFontFamilyName(FontFamilyType fontFamilyType)
        {
            return fontFamilyType switch
            {
                FontFamilyType.MicrosoftYaHei => "Microsoft YaHei",
                FontFamilyType.SimSun => "SimSun",
                FontFamilyType.SimHei => "SimHei",
                FontFamilyType.KaiTi => "KaiTi",
                FontFamilyType.FangSong => "FangSong",
                FontFamilyType.NSimSun => "NSimSun",
                FontFamilyType.Arial => "Arial",
                FontFamilyType.TimesNewRoman => "Times New Roman",
                FontFamilyType.Consolas => "Consolas",
                FontFamilyType.SegoeUI => "Segoe UI",
                _ => "Microsoft YaHei"
            };
        }

        public static FontFamilyType GetFontFamilyType(string? fontFamilyName)
        {
            if (string.IsNullOrWhiteSpace(fontFamilyName))
                return FontFamilyType.MicrosoftYaHei;

            return fontFamilyName.Trim() switch
            {
                "Microsoft YaHei" => FontFamilyType.MicrosoftYaHei,
                "SimSun" => FontFamilyType.SimSun,
                "SimHei" => FontFamilyType.SimHei,
                "KaiTi" => FontFamilyType.KaiTi,
                "FangSong" => FontFamilyType.FangSong,
                "NSimSun" => FontFamilyType.NSimSun,
                "Arial" => FontFamilyType.Arial,
                "Times New Roman" => FontFamilyType.TimesNewRoman,
                "Consolas" => FontFamilyType.Consolas,
                "Segoe UI" => FontFamilyType.SegoeUI,
                _ => Enum.TryParse<FontFamilyType>(fontFamilyName, out var result)
                    ? result
                    : FontFamilyType.MicrosoftYaHei
            };
        }
    }
}
