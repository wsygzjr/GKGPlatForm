using Griffins.Map.UI;
using Griffins.Map.UI.CustomMenuCmdConfig;
using MainFrame.ViewModels;
using Newtonsoft.JsonG;
using Newtonsoft.JsonG.Linq;
using System.Text;

namespace MainFrame.Models
{
    /// <summary>
    /// 主页面框架模板配置信息（包含导航栏、工具栏和顶部菜单的配置）
    /// </summary>
    public class MainPageFrameTemplateCfgInfo
    {
        /// <summary>
        /// 通用配置信息
        /// </summary>
        public GeneralConfigInfo GeneralConfigInfo { get; set; } = new();
        /// <summary>
        /// 导航栏菜单配置信息列表
        /// </summary>
        public WorkAreaInfo NavigationMenu { get; set; }
        /// <summary>
        /// 快捷工具栏按钮
        /// </summary>
        public WorkAreaInfo ToolbarButton { get; set; }
        /// <summary>
        /// 顶部菜单配置信息
        /// </summary>
        public WorkAreaInfo TopMenuCfgInfo { get; set; }
        /// <summary>
        /// 底部菜单配置信息
        /// </summary>
        public WorkAreaInfo ButtomColumn { get; set; }
        /// <summary>
        /// 左下信息块配置信息
        /// </summary>
        public WorkAreaInfo LeftBottomInfoBlock { get; set; }

        /// <summary>
        /// 页面样式ID
        /// </summary>
        public string PageStyleID { get; set; } = "";
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainPageFrameTemplateCfgInfo()
        {
            NavigationMenu = new WorkAreaInfo()
            {
                WorkAreaID = Guid.NewGuid().ToString(),
                WorkAreaName = "导航栏工作区",
            };
            ToolbarButton = new WorkAreaInfo()
            {
                WorkAreaID = Guid.NewGuid().ToString(),
                WorkAreaName = "右侧快捷栏工作区",
            };
            TopMenuCfgInfo = new WorkAreaInfo()
            {
                WorkAreaID = Guid.NewGuid().ToString(),
                WorkAreaName = "顶部菜单栏工作区",
            };
            ButtomColumn = new WorkAreaInfo()
            {
                WorkAreaID = Guid.NewGuid().ToString(),
                WorkAreaName = "底部信息栏工作区",
            };
            LeftBottomInfoBlock = new WorkAreaInfo()
            {
                WorkAreaID = Guid.NewGuid().ToString(),
                WorkAreaName = "左下信息块工作区",
            };
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

            string jsonStr = Encoding.UTF8.GetString(jsonBytes);
            var jObject = JObject.Parse(jsonStr);

            if (jObject.TryGetValue("GeneralConfigInfo", out JToken? generalConfigInfo) && generalConfigInfo is JObject jObjectGeneralConfigInfo)
            {
                GeneralConfigInfo.FromJObject(jObjectGeneralConfigInfo);
            }
            
            if (jObject.TryGetValue("NavigationMenu", out JToken? navigationMenu) && navigationMenu is JObject jObjectNavigationMenu)
            {
                NavigationMenu.FromJObject(jObjectNavigationMenu);
            }

            if (jObject.TryGetValue("ToolbarButton", out JToken? toolbarButton) && toolbarButton is JObject jObjecttoolbarButton)
            {
                ToolbarButton.FromJObject(jObjecttoolbarButton);
            }
            if (jObject.TryGetValue("TopMenuCfgInfo", out JToken? topMenuCfgInfo) && topMenuCfgInfo is JObject jObjecttopMenuCfgInfo)
            {
                TopMenuCfgInfo.FromJObject(jObjecttopMenuCfgInfo);
            }
            if (jObject.TryGetValue("ButtomColumn", out JToken? buttomColumn) && buttomColumn is JObject jObjectbuttomColumn)
            {
                ButtomColumn.FromJObject(jObjectbuttomColumn);
            }
            if (jObject.TryGetValue("LeftBottomInfoBlock", out JToken? leftBottomInfoBlock) && leftBottomInfoBlock is JObject jObjectleftBottomInfoBlock)
            {
                LeftBottomInfoBlock.FromJObject(jObjectleftBottomInfoBlock);
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

            // 序列化导航栏配置
            jObject["NavigationMenu"] = NavigationMenu.ToJObject();

            // 序列化右侧工具栏配置
            jObject["ToolbarButton"] = ToolbarButton.ToJObject();

            // 序列化顶部菜单栏配置
            jObject["TopMenuCfgInfo"] = TopMenuCfgInfo.ToJObject();

            // 序列化低部信息栏配置
            jObject["ButtomColumn"] = ButtomColumn.ToJObject();

            // 序列化左下信息块配置
            jObject["LeftBottomInfoBlock"] = LeftBottomInfoBlock.ToJObject();

            // 使用缩进格式提升可读性，指定UTF8编码（无BOM）
            string jsonStr = jObject.ToString(Formatting.Indented);
            return Encoding.UTF8.GetBytes(jsonStr);
        }
    }
}
