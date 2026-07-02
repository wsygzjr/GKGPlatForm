    using Griffins.UI2;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    namespace GKG.Map.MapCell.Generic
    {
        internal class CommHelper
        {
            public static PropertyBindInfo CreatePropertyBindInfo(GriffinsBaseDataType griffinsBaseDataType)
            {
                var bindInfo = new PropertyBindInfo(griffinsBaseDataType);
                return bindInfo;
            }

            public static PropertyBindInfo CreatePropertyBindInfo(GriffinsBaseDataType griffinsBaseDataType, Guid objectID)
            {
                var bindInfo = new PropertyBindInfo(griffinsBaseDataType, objectID);
                return bindInfo;
            }

            public static PropertyBindInfo ClonePropertyBindInfo(PropertyBindInfo? source, GriffinsBaseDataType griffinsBaseDataType)
            {
                var bindInfo = CreatePropertyBindInfo(griffinsBaseDataType);
                if (source != null)
                    bindInfo.CopyFrom(source);
                return bindInfo;
            }

            public static PropertyBindInfo ClonePropertyBindInfo(PropertyBindInfo? source, GriffinsBaseDataType griffinsBaseDataType, Guid objectID)
            {
                var bindInfo = CreatePropertyBindInfo(griffinsBaseDataType, objectID);
                if (source != null)
                    bindInfo.CopyFrom(source);
                return bindInfo;
            }

            public static string? GetBindPropertyName(PropertyBindInfo? bindInfo)
            {
                if (bindInfo == null)
                    return null;

                return string.IsNullOrWhiteSpace(bindInfo.BindPropertyName)
                    ? null
                    : bindInfo.BindPropertyName;
            }

            /// <summary>
            /// 获取图元属性ID绑定的管理点属性ID列表
            /// </summary>
            /// <param name="propertyBindEditModelBase">属性绑定编辑模型基类实例</param>
            /// <returns>Key为管理点属性ID，Value为对应的图元属性ID列表</returns>
            public static Dictionary<string, List<string>> GetBindMapCellProperty(PropertyBindEditModelBase propertyBindEditModelBase)
            {
                var valueToPropertyIdsDict = new Dictionary<string, List<string>>(StringComparer.Ordinal);
                if (propertyBindEditModelBase == null)
                    return valueToPropertyIdsDict;

                var modelType = propertyBindEditModelBase.GetType();
                var modelProperties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in modelProperties)
                {
                    if (prop.GetIndexParameters().Length > 0)
                        continue;
                    if (prop.PropertyType != typeof(PropertyBindInfo))
                        continue;

                    try
                    {
                        var hasAttr = prop.GetCustomAttributes(typeof(BindMPPropertyIDAttribute), true)?.Length > 0;
                        if (!hasAttr)
                            continue;
                    }
                    catch
                    {
                        continue;
                    }

                    var propertyId = prop.Name;
                    var propertyValue = GetBindPropertyName(prop.GetValue(propertyBindEditModelBase) as PropertyBindInfo);
                    if (string.IsNullOrWhiteSpace(propertyValue))
                        continue;

                    if (!valueToPropertyIdsDict.TryGetValue(propertyValue, out var list))
                    {
                        list = new List<string>();
                        valueToPropertyIdsDict.Add(propertyValue, list);
                    }
                    list.Add(propertyId);
                }

                return valueToPropertyIdsDict;
            }

            public static GFBaseTypeParamValueList ToEventParamValueList(string? eventCmdParam)
            {
                var list = new GFBaseTypeParamValueList();
                if (string.IsNullOrWhiteSpace(eventCmdParam))
                    return list;

                try
                {
                    list.FromJson(eventCmdParam);
                }
                catch
                {
                }

                return list;
            }

            public static bool TryGetGFBaseTypePropValueList(InformInfoBase? info, out GFBaseTypePropValueList? values)
            {
                values = null;
                if (info == null)
                    return false;

                try
                {
                    const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    var t = info.GetType();

                    foreach (var p in t.GetProperties(flags))
                    {
                        if (!typeof(GFBaseTypePropValueList).IsAssignableFrom(p.PropertyType))
                            continue;
                        if (p.GetIndexParameters().Length != 0)
                            continue;

                        try
                        {
                            values = p.GetValue(info) as GFBaseTypePropValueList;
                            if (values != null)
                                return true;
                        }
                        catch
                        {
                        }
                    }

                    foreach (var f in t.GetFields(flags))
                    {
                        if (!typeof(GFBaseTypePropValueList).IsAssignableFrom(f.FieldType))
                            continue;

                        try
                        {
                            values = f.GetValue(info) as GFBaseTypePropValueList;
                            if (values != null)
                                return true;
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        var json = info.ToJson();
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            values = JsonObjConvert.FromJSon<GFBaseTypePropValueList>(json);
                            if (values != null)
                                return true;
                        }
                    }
                    catch
                    {
                    }
                }
                catch
                {
                }

                return false;
            }

        }
    }
