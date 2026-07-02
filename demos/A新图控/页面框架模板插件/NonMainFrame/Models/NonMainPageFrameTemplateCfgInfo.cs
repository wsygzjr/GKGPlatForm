using Griffins.Map.UI.CustomMenuCmdConfig;
using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Linq;
using NonMainFrameViewModel.ViewModels.Comon;
using System.Text;

namespace NonMainFrameViewModel.Models
{
    /// <summary>
    /// 非主界面框架模板配置信息（包含导航栏、工具栏和顶部菜单的配置）
    /// </summary>
    public class NonMainPageFrameTemplateCfgInfo
    {
        /// <summary>
        /// 通用配置信息
        /// </summary>
        public GeneralConfigInfo GeneralConfigInfo { get; set; } = new();
        /// <summary>
        /// 页面样式ID
        /// </summary>
        public string PageStyleID { get; set; } = "";
        /// <summary>
        /// 工具栏按钮列表
        /// </summary>
        public MenuButtonList ToolbarButtons { get; set; } = new MenuButtonList();

        /// <summary>
        /// 构造函数（初始化默认菜单配置）
        /// </summary>
        public NonMainPageFrameTemplateCfgInfo()
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

                if (jObject.TryGetValue("GeneralConfigInfo", out JToken? generalConfigInfo) && generalConfigInfo is JObject jObjectGeneralConfigInfo)
                {
                    GeneralConfigInfo.FromJObject(jObjectGeneralConfigInfo);
                }
                // 反序列化工具栏配置
                if (jObject.TryGetValue("ToolbarButtons", out JToken? toolbarToken) && toolbarToken is JArray toolbarArray)
                {
                    ToolbarButtons.FromJArray(toolbarArray);
                }
                this.PageStyleID = jObject["PageStyleID"]?.Value<string>() ?? string.Empty;


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
            jObject["GeneralConfigInfo"] = GeneralConfigInfo.ToJObject();

            // 序列化工具栏配置
            jObject["ToolbarButtons"] = ToolbarButtons.ToJArray();

            jObject["PageStyleID"] = this.PageStyleID;

            // 使用缩进格式提升可读性，指定UTF8编码（无BOM）
            string jsonStr = jObject.ToString(Formatting.Indented);
            return Encoding.UTF8.GetBytes(jsonStr);
        }
    }
}
