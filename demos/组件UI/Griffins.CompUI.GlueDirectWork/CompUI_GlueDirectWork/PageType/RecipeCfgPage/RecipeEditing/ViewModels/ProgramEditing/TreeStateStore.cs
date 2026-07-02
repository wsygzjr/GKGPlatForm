using System;
using System.Collections.Generic;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    /// <summary>
    /// 树状态仓库：负责保存和恢复展开状态、当前选中节点。
    /// </summary>
    internal sealed class TreeStateStore
    {
        /// <summary>已展开节点键集合。</summary>
        public HashSet<NodeKey> ExpandedKeys { get; } = new();

        /// <summary>当前选中节点键。</summary>
        public NodeKey? SelectedKey { get; private set; }

        /// <summary>记录当前选中节点。</summary>
        public void RememberSelection(ProgramTreeNodeViewModel? node)
        {
            SelectedKey = node?.Key;
        }

        /// <summary>从当前树中采集已展开的节点。</summary>
        public void CaptureExpandedKeys(IEnumerable<ProgramTreeNodeViewModel> nodes)
        {
            ExpandedKeys.Clear();
            CaptureExpandedKeysCore(nodes);
        }

        /// <summary>将已记录的展开状态应用到新树。</summary>
        public void ApplyExpandedKeys(IEnumerable<ProgramTreeNodeViewModel> nodes)
        {
            foreach (var node in nodes)
            {
                node.IsExpanded = ExpandedKeys.Contains(node.Key);
                ApplyExpandedKeys(node.Children);
            }
        }

        /// <summary>按已记录的选中键在当前树中查找节点。</summary>
        public ProgramTreeNodeViewModel? FindSelectedNode(IEnumerable<ProgramTreeNodeViewModel> nodes)
        {
            if (SelectedKey == null)
                return null;
            return FindNode(nodes, n => n.Key == SelectedKey.Value);
        }

        /// <summary>递归采集展开节点键。</summary>
        private void CaptureExpandedKeysCore(IEnumerable<ProgramTreeNodeViewModel> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.IsExpanded)
                    ExpandedKeys.Add(node.Key);
                CaptureExpandedKeysCore(node.Children);
            }
        }

        /// <summary>在树中按条件递归查找节点。</summary>
        private static ProgramTreeNodeViewModel? FindNode(
            IEnumerable<ProgramTreeNodeViewModel> nodes,
            Func<ProgramTreeNodeViewModel, bool> predicate)
        {
            foreach (var node in nodes)
            {
                if (predicate(node))
                    return node;

                var child = FindNode(node.Children, predicate);
                if (child != null)
                    return child;
            }
            return null;
        }
    }
}
