using System.Text;
using Avalonia.Media;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.MapOprtCellParamCfgView;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.Objects;
using GKG.Map.MapCell.Generic.Control.MapCell_Calendar.ViewModels;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar
{
    internal partial class MapCellCalendarCtlObj
    {
        private class BrushInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not CalendarViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    try
                    {
                        BrushInfoMapOprtCellParamViewModel? param = JsonSerializer.Deserialize<BrushInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                        if (param != null)
                        {
                            PostToUI(() =>
                            {
                                vm.BrushInfo.BackgroundColor = ParseColorOrDefault(param.BackgroundColor, CalendarBrushInfo.Default.BackgroundColor);
                                vm.BrushInfo.BorderColor = ParseColorOrDefault(param.BorderColor, CalendarBrushInfo.Default.BorderColor);
                                vm.BrushInfo.ForegroundColor = ParseColorOrDefault(param.ForegroundColor, CalendarBrushInfo.Default.ForegroundColor);
                            });
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                MapCellPropValue? val = callBack.GetMapCellPropValue(nameof(CalendarPropertyModelEdit.BrushInfo));
                if (val == null)
                    return;

                try
                {
                    CalendarBrushInfo info = DeserializeObject<CalendarBrushInfo>(val);
                    PostToUI(() =>
                    {
                        vm.BrushInfo.BackgroundColor = info.BackgroundColor;
                        vm.BrushInfo.BorderColor = info.BorderColor;
                        vm.BrushInfo.ForegroundColor = info.ForegroundColor;
                    });
                }
                catch
                {
                }
            }
        }

        private class AppearanceInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not CalendarViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    try
                    {
                        AppearanceInfoMapOprtCellParamViewModel? param = JsonSerializer.Deserialize<AppearanceInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                        if (param != null)
                        {
                            int.TryParse(param.Opacity, out int opacity);
                            int.TryParse(param.BorderThicknessLeft, out int left);
                            int.TryParse(param.BorderThicknessTop, out int top);
                            int.TryParse(param.BorderThicknessRight, out int right);
                            int.TryParse(param.BorderThicknessBottom, out int bottom);

                            PostToUI(() =>
                            {
                                vm.AppearanceInfo.Opacity = opacity;
                                vm.AppearanceInfo.BorderThicknessLeft = left;
                                vm.AppearanceInfo.BorderThicknessTop = top;
                                vm.AppearanceInfo.BorderThicknessRight = right;
                                vm.AppearanceInfo.BorderThicknessBottom = bottom;
                            });
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                MapCellPropValue? val = callBack.GetMapCellPropValue(nameof(CalendarPropertyModelEdit.AppearanceInfo));
                if (val == null)
                    return;

                try
                {
                    CalendarAppearanceInfo info = DeserializeObject<CalendarAppearanceInfo>(val);
                    PostToUI(() =>
                    {
                        vm.AppearanceInfo.Opacity = info.Opacity;
                        vm.AppearanceInfo.BorderThicknessLeft = info.BorderThicknessLeft;
                        vm.AppearanceInfo.BorderThicknessTop = info.BorderThicknessTop;
                        vm.AppearanceInfo.BorderThicknessRight = info.BorderThicknessRight;
                        vm.AppearanceInfo.BorderThicknessBottom = info.BorderThicknessBottom;
                    });
                }
                catch
                {
                }
            }
        }

        private class CommonInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not CalendarViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    try
                    {
                        CommonInfoMapOprtCellParamViewModel? param = JsonSerializer.Deserialize<CommonInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                        if (param != null)
                        {
                            PostToUI(() =>
                            {
                                vm.CommonInfo.HoverCursor = param.HoverCursor;
                                vm.CommonInfo.Enabled = param.Enabled;
                                vm.CommonInfo.ShowButtonPanel = param.ShowButtonPanel;
                            });
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                MapCellPropValue? val = callBack.GetMapCellPropValue(nameof(CalendarPropertyModelEdit.CommonInfo));
                if (val == null)
                    return;

                try
                {
                    CalendarCommonInfo info = DeserializeObject<CalendarCommonInfo>(val);
                    PostToUI(() =>
                    {
                        vm.CommonInfo.HoverCursor = info.HoverCursor;
                        vm.CommonInfo.Enabled = info.Enabled;
                        vm.CommonInfo.ShowButtonPanel = info.ShowButtonPanel;
                    });
                }
                catch
                {
                }
            }
        }

        private class LayoutInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not CalendarViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    try
                    {
                        LayoutInfoMapOprtCellParamViewModel? param = JsonSerializer.Deserialize<LayoutInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                        if (param != null)
                        {
                            int.TryParse(param.MarginLeft, out int marginLeft);
                            int.TryParse(param.MarginTop, out int marginTop);
                            int.TryParse(param.MarginRight, out int marginRight);
                            int.TryParse(param.MarginBottom, out int marginBottom);
                            int.TryParse(param.MinWidth, out int minWidth);
                            int.TryParse(param.MaxWidth, out int maxWidth);
                            int.TryParse(param.MinHeight, out int minHeight);
                            int.TryParse(param.MaxHeight, out int maxHeight);

                            PostToUI(() =>
                            {
                                vm.LayoutInfo.HorizontalAlignment = param.HorizontalAlignment;
                                vm.LayoutInfo.VerticalAlignment = param.VerticalAlignment;
                                vm.LayoutInfo.MarginLeft = marginLeft;
                                vm.LayoutInfo.MarginTop = marginTop;
                                vm.LayoutInfo.MarginRight = marginRight;
                                vm.LayoutInfo.MarginBottom = marginBottom;
                                vm.LayoutInfo.MinWidth = minWidth;
                                vm.LayoutInfo.MaxWidth = maxWidth;
                                vm.LayoutInfo.MinHeight = minHeight;
                                vm.LayoutInfo.MaxHeight = maxHeight;
                            });
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                MapCellPropValue? val = callBack.GetMapCellPropValue(nameof(CalendarPropertyModelEdit.LayoutInfo));
                if (val == null)
                    return;

                try
                {
                    CalendarLayoutInfo info = DeserializeObject<CalendarLayoutInfo>(val);
                    PostToUI(() =>
                    {
                        vm.LayoutInfo.HorizontalAlignment = info.HorizontalAlignment;
                        vm.LayoutInfo.VerticalAlignment = info.VerticalAlignment;
                        vm.LayoutInfo.MarginLeft = info.MarginLeft;
                        vm.LayoutInfo.MarginTop = info.MarginTop;
                        vm.LayoutInfo.MarginRight = info.MarginRight;
                        vm.LayoutInfo.MarginBottom = info.MarginBottom;
                        vm.LayoutInfo.MinWidth = info.MinWidth;
                        vm.LayoutInfo.MaxWidth = info.MaxWidth;
                        vm.LayoutInfo.MinHeight = info.MinHeight;
                        vm.LayoutInfo.MaxHeight = info.MaxHeight;
                    });
                }
                catch
                {
                }
            }
        }

        private class TextInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not CalendarViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    try
                    {
                        TextInfoMapOprtCellParamViewModel? param = JsonSerializer.Deserialize<TextInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                        if (param != null)
                        {
                            int.TryParse(param.FontSize, out int fontSize);
                            PostToUI(() =>
                            {
                                vm.TextInfo.FontFamily = param.FontFamily;
                                vm.TextInfo.FontColor = ParseColorOrDefault(param.FontColor, CalendarTextInfo.Default.FontColor);
                                vm.TextInfo.FontSize = fontSize;
                                vm.TextInfo.IsItalic = param.IsItalic;
                                vm.TextInfo.IsBold = param.IsBold;
                            });
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                MapCellPropValue? val = callBack.GetMapCellPropValue(nameof(CalendarPropertyModelEdit.TextInfo));
                if (val == null)
                    return;

                try
                {
                    CalendarTextInfo info = DeserializeObject<CalendarTextInfo>(val);
                    PostToUI(() =>
                    {
                        vm.TextInfo.FontFamily = info.FontFamily;
                        vm.TextInfo.FontColor = info.FontColor;
                        vm.TextInfo.FontSize = info.FontSize;
                        vm.TextInfo.IsItalic = info.IsItalic;
                        vm.TextInfo.IsBold = info.IsBold;
                    });
                }
                catch
                {
                }
            }
        }

        private class MiscInfoMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack) => this.callBack = callBack;

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is not CalendarViewModel vm)
                    return;

                if (cfg != null && cfg.Length > 0)
                {
                    try
                    {
                        MiscInfoMapOprtCellParamViewModel? param = JsonSerializer.Deserialize<MiscInfoMapOprtCellParamViewModel>(Encoding.UTF8.GetString(cfg));
                        if (param != null)
                        {
                            PostToUI(() =>
                            {
                                vm.MiscInfo.SelectedDate = param.SelectedDate ?? string.Empty;
                                vm.MiscInfo.SelectedDates = param.SelectedDates ?? "[]";
                                vm.MiscInfo.DisplayDate = param.DisplayDate ?? CalendarValueHelpers.FormatDate(System.DateTime.Today);
                                vm.MiscInfo.DisplayDateStart = param.DisplayDateStart ?? string.Empty;
                                vm.MiscInfo.DisplayDateEnd = param.DisplayDateEnd ?? string.Empty;
                                vm.MiscInfo.FirstDayOfWeek = param.FirstDayOfWeek;
                                vm.MiscInfo.SelectionMode = param.SelectionMode;
                                vm.MiscInfo.IsTodayHighlighted = param.IsTodayHighlighted;
                                vm.MiscInfo.DisplayMode = param.DisplayMode;
                                vm.MiscInfo.BlackoutDates = param.BlackoutDates ?? "[]";
                            });
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                MapCellPropValue? val = callBack.GetMapCellPropValue(nameof(CalendarPropertyModelEdit.MiscInfo));
                if (val == null)
                    return;

                try
                {
                    CalendarMiscInfo info = DeserializeObject<CalendarMiscInfo>(val);
                    PostToUI(() =>
                    {
                        vm.MiscInfo.SelectedDate = info.SelectedDate;
                        vm.MiscInfo.SelectedDates = info.SelectedDates;
                        vm.MiscInfo.DisplayDate = info.DisplayDate;
                        vm.MiscInfo.DisplayDateStart = info.DisplayDateStart;
                        vm.MiscInfo.DisplayDateEnd = info.DisplayDateEnd;
                        vm.MiscInfo.FirstDayOfWeek = info.FirstDayOfWeek;
                        vm.MiscInfo.SelectionMode = info.SelectionMode;
                        vm.MiscInfo.IsTodayHighlighted = info.IsTodayHighlighted;
                        vm.MiscInfo.DisplayMode = info.DisplayMode;
                        vm.MiscInfo.BlackoutDates = info.BlackoutDates;
                    });
                }
                catch
                {
                }
            }
        }

        private static Color ParseColorOrDefault(string? colorText, Color defaultColor)
        {
            if (string.IsNullOrWhiteSpace(colorText))
                return defaultColor;

            try
            {
                return Color.Parse(colorText);
            }
            catch
            {
                return defaultColor;
            }
        }
    }
}
