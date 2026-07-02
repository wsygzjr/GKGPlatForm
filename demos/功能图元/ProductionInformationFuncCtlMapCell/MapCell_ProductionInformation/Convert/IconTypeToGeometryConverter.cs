using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using GKG.Map.ProductionInformationFuncCtlMapCell.Models;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using Avalonia.Controls;

namespace GKG.Map.ProductionInformationFuncCtlMapCell.Convert
{
    /// <summary>
    /// 图标类型 → 矢量图形(Geometry) 转换器 (高性能缓存单例版)
    /// </summary>
    public class IconTypeToGeometryConverter : IValueConverter
    {
        /// <summary>
        /// 静态全局单例
        /// </summary>
        public static readonly IconTypeToGeometryConverter Instance = new();

        /// <summary>
        /// 保证字典资源线程安全的懒加载器 (Lazy)，仅在首次真实调用时开销极小地解析一次 AXAML
        /// </summary>
        private static readonly Lazy<ResourceDictionary> _cachedIconDictionary = new(() =>
        {
            var uri = new Uri("avares://GKG.Map.ProductionInformationFuncCtlMapCell/Assets/MyControlResources.axaml");
            return (ResourceDictionary)AvaloniaXamlLoader.Load(uri);
        });

        /// <summary>
        /// 核心提速架构：将已解析出来的 Geometry 物理缓存起来
        /// 消灭高频刷新时反复向 ResourceDictionary 发起 TryGetResource 的检索遍历性能消耗
        /// </summary>
        private static readonly ConcurrentDictionary<string, object?> _geometryCache = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string resourceKey = value switch
            {
                MessageDialogIconType.Alarm => "Icon_Alarm",
                MessageDialogIconType.Tip => "Icon_Tip",
                MessageDialogIconType.Question => "Icon_Question",
                _ => value?.ToString() ?? "Icon_Alarm"
            };

            // 高速缓冲命中机制：O(1) 极速直达
            return _geometryCache.GetOrAdd(resourceKey, key =>
            {
                // 若缓存未命中，才向惰性加载的全局字典中发起真实查询
                if (_cachedIconDictionary.Value.TryGetResource(key, null, out var resource))
                {
                    return resource;
                }
                return null;
            });
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("不支持反向转换");
        }
    }
}