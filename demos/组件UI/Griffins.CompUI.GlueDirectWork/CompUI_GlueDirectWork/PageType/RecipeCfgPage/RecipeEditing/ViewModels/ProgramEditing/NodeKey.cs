namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 树节点强类型身份键（用于展开状态和选中状态恢复）。
    /// </summary>
    internal readonly record struct NodeKey(
        string NodeType,
        string? TemplateId = null,
        string? BlockId = null,
        string? NestedTemplateId = null,
        string? ScopePath = null);
}
