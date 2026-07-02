using GF_Gereric;
using Griffins.Map.UI;

// =========================================================================
// 注册插件的程序集级别特性 (Assembly Attribute)
// 声明当前程序集属于“功能图元插件 (Functional Map Cell)”
// 指定其全局唯一标识符 (GUID) 和底层描述。
// 这是 Griffins 框架利用反射动态加载和识别此“生产信息”图元模块的唯一凭证。
// =========================================================================
[assembly: Plugin(FunctionalMapCellAttribute.PLUGINKIND_Str, "{B60E6DF8-67A8-41DD-9ED9-A2AE5ED5DA01}", "func map cell productionInformation")]