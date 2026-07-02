using System.Text;

namespace GKG
{
    public static class StringConverter
    {
        /// <summary>
        /// 十六进制字符串转换为Int值
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns>Int值</returns>
        public static int HexToInt32Int(string hexStr)
        {
            return Convert.ToInt32(hexStr, 16);
        }

        /// <summary>
        /// byte数组转换为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] bytes)
        {
            return Convert.ToHexString(bytes);
        }

        /// <summary>
        /// int转换为指定长度的Hex字符串
        /// </summary>
        /// <param name="i"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string IntToHexString(int i, int length)
        {
            return i.ToString($"{length}X");
        }

        /// <summary>
        /// 字节数组转换为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 删除字符串中的空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpace(string str)
        {
            return str.Replace(" ", "");
        }

        /// <summary>
        /// 删除字符串中的回车换行符号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveEnter(string str)
        {
            return str.Replace("\r\n", "");
        }
        public static string Reverse(string str)
        {
            return new string(str.Reverse().ToArray());
        }
    }
}
