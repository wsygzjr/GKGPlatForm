using Avalonia.Media.Imaging;
using Newtonsoft.JsonG.Linq;
using System.Reflection;

namespace MainFrame.Models
{
    /// <summary>
    /// 通用配置信息
    /// </summary>
    public class GeneralConfigInfo
    {
        // 默认图标资源路径
        private const string defaultCompanyLogoPath = "Assets/Icons/DefaultCompanyLogo.png";
        private const string defaultMenuCmdIcon = "Assets/Icons/DefaultMenuCmdIcon.png";
        private const string defaultMinIconPath = "Assets/Icons/DefaultMinIcon.png";
        private const string defaultCloseIconPath = "Assets/Icons/DefaultCloseIcon.png";

        /// <summary>
        /// 公司Logo位图
        /// </summary>
        public Bitmap? CompanyLogo { get; set; }
        /// <summary>
        /// 默认的菜单命令图标
        /// </summary>
        public Bitmap? DefaultMenuCmdIcon { get; set; }
        /// <summary>
        /// 最小化按钮图标位图
        /// </summary>
        public Bitmap? MinimizeIcon { get; set; }

        /// <summary>
        /// 关闭按钮图标位图
        /// </summary>
        public Bitmap? CloseIcon { get; set; }

        /// <summary>
        /// 最小化按钮启用状态
        /// </summary>
        public bool IsMinimizeButtonEnabled { get; set; } = true;


        public GeneralConfigInfo()
        {
            loadDefaultIcons();
        }
        /// <summary>
        /// 加载默认图标资源
        /// </summary>
        private void loadDefaultIcons()
        {
            try
            {
                CompanyLogo = loadBitmapFromResource(defaultCompanyLogoPath);
                DefaultMenuCmdIcon = loadBitmapFromResource(defaultMenuCmdIcon);
                MinimizeIcon = loadBitmapFromResource(defaultMinIconPath);
                CloseIcon = loadBitmapFromResource(defaultCloseIconPath);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 从嵌入式资源加载Bitmap
        /// </summary>
        /// <param name="resourcePath">资源路径</param>
        /// <returns>Bitmap实例（失败返回null）</returns>
        private Bitmap? loadBitmapFromResource(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
                return null;

            try
            {

                var assembly = Assembly.GetExecutingAssembly();
                string fullResourceName = $"{assembly.GetName().Name}.{resourcePath.Replace("/", ".")}";
                using (var stream = assembly.GetManifestResourceStream(fullResourceName))
                {
                    if (stream != null)
                    {
                        return new Bitmap(stream);
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null; 
            }
        }
        /// <summary>
        /// 从JObject反序列化配置数据
        /// </summary>
        /// <param name="jObject">JObject实例（可为null）</param>
        /// <exception cref="InvalidDataException">反序列化失败时抛出</exception>
        public void FromJObject(JObject? jObject)
        {
            if (jObject == null)
                return;

            IsMinimizeButtonEnabled = jObject["IsMinimizeButtonEnabled"]?.Value<bool>() ?? true;

            CompanyLogo = deserializeBitmapFromJObject(jObject, "CompanyLogo") ?? CompanyLogo;
            DefaultMenuCmdIcon = deserializeBitmapFromJObject(jObject, "DefaultMenuCmdIcon") ?? DefaultMenuCmdIcon;
            MinimizeIcon = deserializeBitmapFromJObject(jObject, "MinimizeIcon")?? MinimizeIcon;
            CloseIcon = deserializeBitmapFromJObject(jObject, "CloseIcon") ?? CloseIcon;
        }

        /// <summary>
        /// 序列化为JObject
        /// </summary>
        /// <returns>包含配置数据的JObject实例</returns>
        public JObject ToJObject()
        {
            var jObject = new JObject
            {
                ["IsMinimizeButtonEnabled"] = IsMinimizeButtonEnabled
            };

            serializeBitmapToJObject(jObject, "CompanyLogo", CompanyLogo);
            serializeBitmapToJObject(jObject, "DefaultMenuCmdIcon", DefaultMenuCmdIcon);
            serializeBitmapToJObject(jObject, "MinimizeIcon", MinimizeIcon);
            serializeBitmapToJObject(jObject, "CloseIcon", CloseIcon);

            return jObject;
        }

        /// <summary>
        /// 从JObject反序列化Bitmap（通用方法）
        /// </summary>
        /// <param name="jObject">JObject实例</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>Bitmap实例（null表示无数据）</returns>
        /// <exception cref="InvalidDataException">Base64格式错误时抛出</exception>
        private Bitmap? deserializeBitmapFromJObject(JObject jObject, string propertyName)
        {
            try
            {
                string base64Str = jObject[propertyName]?.Value<string>() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(base64Str))
                {
                    return null;
                }

                // Base64字符串转字节数组
                byte[] bytes = Convert.FromBase64String(base64Str);
                using (var stream = new MemoryStream(bytes))
                {
                    return new Bitmap(stream);
                }
            }
            catch (FormatException ex)
            {
                throw new InvalidDataException($"{propertyName} Base64格式无效", ex);
            }
            catch (Exception ex) when (ex is not InvalidDataException)
            {
                throw new InvalidDataException($"反序列化{propertyName}失败", ex);
            }
        }

        /// <summary>
        /// 将Bitmap序列化为JObject的Base64属性（通用方法）
        /// </summary>
        /// <param name="jObject">目标JObject</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="bitmap">Bitmap实例（可为null）</param>
        private void serializeBitmapToJObject(JObject jObject, string propertyName, Bitmap? bitmap)
        {
            if (bitmap == null)
            {
                jObject[propertyName] = null;
                return;
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    // Avalonia Bitmap的Save方法（需要确保使用正确的格式）
                    bitmap.Save(stream);
                    byte[] bytes = stream.ToArray();
                    // 字节数组转Base64字符串
                    jObject[propertyName] = Convert.ToBase64String(bytes);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"序列化{propertyName}失败", ex);
            }
        }
    }
}