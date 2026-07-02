using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Linq;
using System.Text;

namespace GKG.Map.Page.UIContainer.GridContainer.Models
{
    /// <summary>
    /// 网格子页面容器模板配置信息（包含导航栏、工具栏和顶部菜单的配置）
    /// </summary>
    public class GridContainerTemplateCfgInfo
    {
        /// <summary>
        /// 页面样式ID
        /// </summary>
        public string PageStyleID { get; set; } = string.Empty;

        /// <summary>
        /// 网格行数
        /// </summary>
        public int GridRows { get; set; } = 1;

        /// <summary>
        /// 网格列数
        /// </summary>
        public int GridColumns { get; set; } = 1;

        /// <summary>
        /// 构造函数（初始化默认菜单配置）
        /// </summary>
        public GridContainerTemplateCfgInfo()
        {

        }

        /// <summary>
        /// 从JSON字节数组反序列化配置信息
        /// </summary>
        /// <param name="jsonBytes">JSON字节数组（可为null或空）</param>
        public void FromJsonBytes(byte[]? jsonBytes)
        {
            // 处理空输入
            if (jsonBytes == null || jsonBytes.Length == 0)
                return;

            try
            {
                string jsonStr = Encoding.UTF8.GetString(jsonBytes);
                var jObject = JObject.Parse(jsonStr);

                // 页面样式ID
                this.PageStyleID = jObject["PageStyleID"]?.Value<string>() ?? string.Empty;
                // 网格行列数
                this.GridRows = jObject["GridRows"]?.Value<int>() ?? 1;
                this.GridColumns = jObject["GridColumns"]?.Value<int>() ?? 1;
            }
            catch (Newtonsoft.JsonG.JsonException ex)
            {
                throw new InvalidOperationException("JSON格式错误，无法反序列化配置信息", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("反序列化配置信息失败", ex);
            }
        }

        /// <summary>
        /// 序列化为JSON字节数组
        /// </summary>
        /// <returns>包含配置信息的JSON字节数组</returns>
        public byte[] ToJsonBytes()
        {
            var jObject = new JObject();

            jObject["PageStyleID"] = PageStyleID;
            jObject["GridRows"] = GridRows;
            jObject["GridColumns"] = GridColumns;

            // 使用缩进格式提升可读性，指定UTF8编码（无BOM）
            string jsonStr = jObject.ToString(Formatting.Indented);
            return Encoding.UTF8.GetBytes(jsonStr);
        }
    }
}
