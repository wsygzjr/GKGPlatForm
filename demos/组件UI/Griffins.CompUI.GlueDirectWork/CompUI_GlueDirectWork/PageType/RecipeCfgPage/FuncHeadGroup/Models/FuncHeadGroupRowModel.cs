using System;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Models
{
    [Serializable]
    internal sealed class FuncHeadGroupRowModel
    {
        /// <summary>
        /// 界面显示名称（与 <see cref="FuncHeadGroupRowViewModel.FunctionHeadDisplayName"/> 对应，持久化）。
        /// </summary>
        public string FunctionHeadDisplayName { get; set; } = string.Empty;
    }
}
