using System.Collections.Generic;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Tools;

/// <summary>
/// 序号生成工具类
/// </summary>
public static class SerialNumberGenerator
{
    /// <summary>
    /// 获取最小可用序号（从1开始，不重复）
    /// </summary>
    /// <param name="prefix">名称前缀（可为空；为空时existingNames需为纯数字字符串）</param>
    /// <param name="existingNames">已有的名称列表：
    /// 1. prefix不为空时：格式为「前缀+数字」（如"线-点胶前后样式1"）；
    /// 2. prefix为空时：需为纯数字字符串（如"1"、"2"）
    /// </param>
    /// <returns>最小可用序号（int）</returns>
    public static int GetMinUnusedSerialNumber(string prefix, IEnumerable<string> existingNames)
    {
        // 入参校验：名称列表不能为空
        if (existingNames == null)
            throw new System.ArgumentNullException(nameof(existingNames), "已有名称列表不能为空");

        // 收集所有已使用的序号
        var usedSerials = new HashSet<int>();
        foreach (var name in existingNames)
        {
            // 过滤空名称，避免解析异常
            if (string.IsNullOrEmpty(name))
                continue;

            int serial = 0;
            // 分场景解析序号
            if (string.IsNullOrEmpty(prefix))
            {
                // 场景1：无前缀，名称为纯数字字符串
                if (int.TryParse(name, out serial))
                {
                    usedSerials.Add(serial);
                }
            }
            else
            {
                // 场景2：有前缀，解析前缀后的数字部分
                if (name.StartsWith(prefix) &&
                    int.TryParse(name.Substring(prefix.Length), out serial))
                {
                    usedSerials.Add(serial);
                }
            }
        }

        // 查找从1开始的最小未使用序号
        int newSerialNumber = 1;
        while (usedSerials.Contains(newSerialNumber))
        {
            newSerialNumber++;
        }

        return newSerialNumber;
    }
}