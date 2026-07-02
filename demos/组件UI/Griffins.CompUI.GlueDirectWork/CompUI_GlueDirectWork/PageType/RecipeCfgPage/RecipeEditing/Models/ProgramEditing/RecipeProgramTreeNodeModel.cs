using System;
using System.Collections.Generic;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.Models.ProgramEditing
{
    /// <summary>程序编辑的树节点数据模型（用于持久化/传输）。</summary>
    [Serializable]
    internal sealed class RecipeProgramTreeNodeModel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string NodeType { get; set; } = string.Empty;

        public List<RecipeProgramTreeNodeModel> Children { get; set; } = new();
    }
}

