using System;
using System.Collections.Generic;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Models
{
    [Serializable]
    internal sealed class FuncHeadGroupModel
    {
        public List<FuncHeadGroupRowModel> Rows { get; set; } = new();
    }
}
