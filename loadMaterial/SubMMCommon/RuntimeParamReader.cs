using GF_Gereric;
using Griffins;
using System.Globalization;

namespace GKG.SubMM
{
    /// <summary>运行时参数读取助手：统一从命令参数中做弱类型解析，减少执行器里的重复 try/catch。</summary>
    internal static class RuntimeParamReader
    {
        /// <summary>读取整型参数；不存在、为空或格式错误时返回 false。</summary>
        public static bool TryGetInt(GFBaseTypeParamValueList cmdParam, string key, out int value)
        {
            value = 0;
            if (cmdParam == null || string.IsNullOrEmpty(key))
                return false;
            try
            {
                GriffinsBaseValue v = cmdParam[key];
                if (v == null)
                    return false;
                value = (int)v.ToInteger();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>读取浮点参数；优先按 invariant culture 解析，兼容运行环境差异。</summary>
        public static bool TryGetDouble(GFBaseTypeParamValueList cmdParam, string key, out double value)
        {
            value = 0;
            if (cmdParam == null || string.IsNullOrEmpty(key))
                return false;
            try
            {
                GriffinsBaseValue v = cmdParam[key];
                if (v == null)
                    return false;
                string s = v.ToString();
                return !string.IsNullOrWhiteSpace(s)
                    && (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value)
                        || double.TryParse(s, out value));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>读取字符串参数，并做基础 Trim。</summary>
        public static bool TryGetString(GFBaseTypeParamValueList cmdParam, string key, out string value)
        {
            value = null;
            if (cmdParam == null || string.IsNullOrEmpty(key))
                return false;
            try
            {
                GriffinsBaseValue v = cmdParam[key];
                string s = v?.ToString();
                if (string.IsNullOrWhiteSpace(s))
                    return false;
                value = s.Trim();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>读取布尔参数；兼容 true/false、0/1 以及底层值对象转换。</summary>
        public static bool TryGetBool(GFBaseTypeParamValueList cmdParam, string key, out bool value)
        {
            value = false;
            if (cmdParam == null || string.IsNullOrEmpty(key))
                return false;
            try
            {
                GriffinsBaseValue v = cmdParam[key];
                if (v == null)
                    return false;
                string s = v.ToString();
                if (bool.TryParse(s, out value))
                    return true;
                if (int.TryParse(s, out int iv))
                {
                    value = iv != 0;
                    return true;
                }
                value = v.ToBool();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
