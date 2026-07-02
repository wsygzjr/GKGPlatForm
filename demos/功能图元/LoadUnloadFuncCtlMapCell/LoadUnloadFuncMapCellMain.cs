using GF_Gereric;
using Griffins.Map.UI;

// 注册插件的程序集级别特性：
// 声明当前程序集属于功能图元插件 (Functional Map Cell)，并指定其全局唯一标识符 (GUID) 和底层描述。
[assembly: Plugin(FunctionalMapCellAttribute.PLUGINKIND_Str, "{4A7D7E07-25D5-4D7C-B65F-F6B7790D5F8C}", "func map cell LoadUnload")]