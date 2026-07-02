namespace GKG.UI
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 枚举字典转成下拉框数据源
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="displayNames"></param>
        /// <returns></returns>
        public static List<ComBoxItem> ToEnumItems<TEnum>(Dictionary<TEnum, string> displayNames)
            where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(value => new ComBoxItem
                {
                    Value = value,
                    DisplayName = displayNames.TryGetValue(value, out var name) ? name : value.ToString()
                }).ToList();
        }

        public static List<ComBoxItem> ToEnumItemsFromDictKeys<TEnum>(Dictionary<TEnum, string> displayNames)
            where TEnum : struct, Enum
        {
            return displayNames
                .Select(kvp => new ComBoxItem
                {
                    Value = kvp.Key,
                    DisplayName = kvp.Value
                })
                .ToList();
        }

    }

    /// <summary>
    /// 下拉框项信息
    /// </summary>
    public class ComBoxItem
    {
        /// <summary>
        /// 原始值
        /// </summary>
        public object Value { get; set; } = default!;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 重载等于方法,在下拉框赋值选中项时需要调用
        /// 同样的,Value对象也需要重载等于方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        public override bool Equals(object? obj)
        {
            // 若为同一实例，直接返回相等
            if (ReferenceEquals(this, obj)) return true;
            // 若类型不同或为空，返回不相等
            if (obj is not ComBoxItem other) return false;
            //是下拉框项
            if ((obj is ComBoxItem objComBoxItem)&& (this is ComBoxItem thisComBoxItem))
            {
                //值为GUID的比较
                if ((objComBoxItem.Value is Guid objComBoxItemGuid) && (thisComBoxItem.Value is Guid thisComBoxItemGuid))
                {
                    var isEqu= Equals(objComBoxItemGuid, thisComBoxItemGuid);
                    return isEqu;
                }
            }
            // 比较 Value（实际业务值）和 DisplayName（显示值）是否都相等
            return Equals(Value, other.Value) && string.Equals(DisplayName, other.DisplayName);
        }

        /// <summary>
        /// 重写哈希码方法：基于相等性的核心字段计算哈希值
        /// 重写 GetHashCode 是为了配合 Equals 方法，确保 ComBoxItem 在哈希表等集合中能被正确处理
        /// </summary>
        public override int GetHashCode()
        {
            // 结合 Value 和 DisplayName 的哈希值，减少哈希冲突
            unchecked // 允许整数溢出，不抛出异常
            {
                int hash = 17; // 初始哈希种子（质数）
                               // 累加 Value 的哈希值（若 Value 为 null 则用 0）
                hash = hash * 31 + (Value?.GetHashCode() ?? 0);
                // 累加 DisplayName 的哈希值（字符串默认实现了 GetHashCode）
                hash = hash * 31 + (DisplayName?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
