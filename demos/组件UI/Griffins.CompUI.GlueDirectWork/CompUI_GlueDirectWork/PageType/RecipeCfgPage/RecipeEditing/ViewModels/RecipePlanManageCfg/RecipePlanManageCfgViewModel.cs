using Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.FuncHeadGroup.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class RecipePlanManageCfgViewModel : ReactiveObject
    {
        private FuncHeadGroupModel? _funcHeadGroupData;

        /// <summary>
        /// 方案管理/方案配置入口 ViewModel。
        /// </summary>
        public RecipePlanManageCfgViewModel(
            AreaEditingWorkspaceViewModel areaEditingWorkspaceViewModel,
            ObservableCollection<ProgramTemplateViewModel> programTemplates)
        {
            AreaEditingWorkspaceViewModel = areaEditingWorkspaceViewModel;
            ProgramTemplates = programTemplates;
            PlanManagementWorkspaceViewModel = new RecipePlanManagementWorkspaceViewModel();
            PlanConfigWorkspaceViewModel = new RecipePlanConfigWorkspaceViewModel(
                PlanManagementWorkspaceViewModel,
                this);
        }

        public AreaEditingWorkspaceViewModel AreaEditingWorkspaceViewModel { get; }
        /// <summary>程序编辑中的模板集合（用于方案配置的模板/区块数据源）。</summary>
        public ObservableCollection<ProgramTemplateViewModel> ProgramTemplates { get; }

        public RecipePlanManagementWorkspaceViewModel PlanManagementWorkspaceViewModel { get; }
        public RecipePlanConfigWorkspaceViewModel PlanConfigWorkspaceViewModel { get; }

        /// <summary>当前配方中的功能头组数据（由 <see cref="RecipeCfgPageTypeRunTimeCompUI"/> 注入）。</summary>
        public FuncHeadGroupModel? FuncHeadGroupData
        {
            get => _funcHeadGroupData;
            private set => this.RaiseAndSetIfChanged(ref _funcHeadGroupData, value);
        }

        public void ApplyFuncHeadGroup(FuncHeadGroupModel? model)
        {
            FuncHeadGroupData = model ?? new FuncHeadGroupModel();
        }
    }

    internal sealed class RecipePlanManagementWorkspaceViewModel : ReactiveObject
    {
        private RecipePlanItemViewModel? _selectedPlan;
        private bool _isAllSelected;
        private bool _hasCheckedPlans;
        private int _nextPlanIndex;
        private readonly List<IDisposable> _checkSubscriptions = new();

        public RecipePlanManagementWorkspaceViewModel()
        {
            Plans = new ObservableCollection<RecipePlanItemViewModel>();

            AddPlanCommand = ReactiveCommand.Create(AddPlan);
            BatchDeletePlanCommand = ReactiveCommand.Create(DeleteCheckedPlans, this.WhenAnyValue(x => x.HasCheckedPlans));
            DeletePlanCommand = ReactiveCommand.Create<RecipePlanItemViewModel>(DeletePlan);

            Plans.CollectionChanged += OnPlansChanged;
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedPlans();
        }

        public ObservableCollection<RecipePlanItemViewModel> Plans { get; }

        public RecipePlanItemViewModel? SelectedPlan
        {
            get => _selectedPlan;
            set => this.RaiseAndSetIfChanged(ref _selectedPlan, value);
        }

        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (!this.RaiseAndSetIfChanged(ref _isAllSelected, value))
                    return;

                foreach (var plan in Plans)
                    plan.IsChecked = value;
            }
        }

        public bool HasCheckedPlans
        {
            get => _hasCheckedPlans;
            private set => this.RaiseAndSetIfChanged(ref _hasCheckedPlans, value);
        }

        public ReactiveCommand<Unit, Unit> AddPlanCommand { get; }
        public ReactiveCommand<Unit, Unit> BatchDeletePlanCommand { get; }
        public ReactiveCommand<RecipePlanItemViewModel, Unit> DeletePlanCommand { get; }

        public void ApplySelectAll(bool isChecked)
        {
            foreach (var plan in Plans)
                plan.IsChecked = isChecked;
            IsAllSelected = isChecked;
        }

        private void AddPlan()
        {
            var plan = new RecipePlanItemViewModel
            {
                Order = Plans.Count + 1,
                Name = $"方案{++_nextPlanIndex}"
            };

            Plans.Add(plan);
            SelectedPlan = plan;
            IsAllSelected = false;
        }

        private void DeleteCheckedPlans()
        {
            var targets = Plans.Where(x => x.IsChecked).ToList();
            if (targets.Count == 0)
                return;

            foreach (var target in targets)
                Plans.Remove(target);

            RefreshOrders();
            foreach (var plan in Plans)
                plan.IsChecked = false;
            IsAllSelected = false;
            SelectedPlan = Plans.FirstOrDefault();
        }

        private void DeletePlan(RecipePlanItemViewModel? plan)
        {
            if (plan == null || !Plans.Contains(plan))
                return;

            Plans.Remove(plan);
            RefreshOrders();
            IsAllSelected = Plans.Count > 0 && Plans.All(x => x.IsChecked);

            if (ReferenceEquals(SelectedPlan, plan))
                SelectedPlan = Plans.FirstOrDefault();
        }

        private void OnPlansChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedPlans();
            RefreshOrders();
        }

        private void AttachCheckSubscriptionsForAll()
        {
            foreach (var d in _checkSubscriptions)
                d.Dispose();
            _checkSubscriptions.Clear();

            foreach (var plan in Plans)
            {
                _checkSubscriptions.Add(
                    plan.WhenAnyValue(x => x.IsChecked)
                        .Subscribe(_ =>
                        {
                            UpdateHasCheckedPlans();
                            IsAllSelected = Plans.Count > 0 && Plans.All(x => x.IsChecked);
                        }));
            }
        }

        private void UpdateHasCheckedPlans()
        {
            HasCheckedPlans = Plans.Any(x => x.IsChecked);
        }

        private void RefreshOrders()
        {
            for (var i = 0; i < Plans.Count; i++)
                Plans[i].Order = i + 1;
        }
    }

    internal sealed class RecipePlanConfigWorkspaceViewModel : ReactiveObject
    {
        private RecipePlanConfigTabViewModel? _selectedPlanTab;
        private readonly RecipePlanManagementWorkspaceViewModel _planManagementWorkspaceViewModel;
        private readonly RecipePlanManageCfgViewModel _ownerRecipePlanManageCfg;
        private readonly Dictionary<RecipePlanItemViewModel, RecipePlanConfigTabViewModel> _tabMap = new();

        public RecipePlanConfigWorkspaceViewModel(
            RecipePlanManagementWorkspaceViewModel planManagementWorkspaceViewModel,
            RecipePlanManageCfgViewModel ownerRecipePlanManageCfg)
        {
            _planManagementWorkspaceViewModel = planManagementWorkspaceViewModel;
            _ownerRecipePlanManageCfg = ownerRecipePlanManageCfg;
            PlanTabs = new ObservableCollection<RecipePlanConfigTabViewModel>();

            SyncPlanTabs();
            _planManagementWorkspaceViewModel.Plans.CollectionChanged += OnPlansChanged;
        }

        public ObservableCollection<RecipePlanConfigTabViewModel> PlanTabs { get; }

        public RecipePlanConfigTabViewModel? SelectedPlanTab
        {
            get => _selectedPlanTab;
            set => this.RaiseAndSetIfChanged(ref _selectedPlanTab, value);
        }

        private void OnPlansChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SyncPlanTabs();
        }

        private void SyncPlanTabs()
        {
            var selectedSource = SelectedPlanTab?.SourcePlan;

            var removedSources = _tabMap.Keys.Except(_planManagementWorkspaceViewModel.Plans).ToList();
            foreach (var removedSource in removedSources)
            {
                var tab = _tabMap[removedSource];
                tab.Dispose();
                PlanTabs.Remove(tab);
                _tabMap.Remove(removedSource);
            }

            foreach (var plan in _planManagementWorkspaceViewModel.Plans)
            {
                if (_tabMap.ContainsKey(plan))
                    continue;

                var tab = new RecipePlanConfigTabViewModel(plan, _ownerRecipePlanManageCfg);
                _tabMap.Add(plan, tab);
                PlanTabs.Add(tab);
            }

            SelectedPlanTab = selectedSource != null && _tabMap.TryGetValue(selectedSource, out var selectedTab)
                ? selectedTab
                : PlanTabs.FirstOrDefault();
        }
    }

    internal sealed class RecipePlanItemViewModel : ReactiveObject
    {
        private int _order;
        private string _name = string.Empty;
        private bool _isChecked;

        public int Order
        {
            get => _order;
            set => this.RaiseAndSetIfChanged(ref _order, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }
    }

    internal sealed class RecipePlanConfigTabViewModel : ReactiveObject, IDisposable
    {
        private RecipePlanConfigNodeViewModel? _selectedNode;
        private object? _selectedWorkspace;
        private readonly RecipePlanManageCfgViewModel _ownerRecipePlanManageCfg;
        private readonly List<IDisposable> _subscriptions = new();

        public RecipePlanConfigTabViewModel(RecipePlanItemViewModel sourcePlan, RecipePlanManageCfgViewModel ownerRecipePlanManageCfg)
        {
            _ownerRecipePlanManageCfg = ownerRecipePlanManageCfg;
            SourcePlan = sourcePlan;
            RootNodes = new ObservableCollection<RecipePlanConfigNodeViewModel>(BuildTree());
            SelectedNode = RootNodes.FirstOrDefault();

            _subscriptions.Add(
                SourcePlan.WhenAnyValue(x => x.Name)
                    .Subscribe(_ => this.RaisePropertyChanged(nameof(PlanName))));

            _subscriptions.Add(
                this.WhenAnyValue(x => x.SelectedNode)
                    .Subscribe(_ => UpdateWorkspaceFromSelection()));

            // 程序编辑模板增删改名时，动态刷新“模板配置”子节点
            _ownerRecipePlanManageCfg.ProgramTemplates.CollectionChanged += OnProgramTemplatesChanged;
            _subscriptions.Add(Disposable.Create(() => _ownerRecipePlanManageCfg.ProgramTemplates.CollectionChanged -= OnProgramTemplatesChanged));
            AttachProgramTemplateSubscriptions();

            UpdateWorkspaceFromSelection();
        }

        public RecipePlanItemViewModel SourcePlan { get; }
        public string PlanName => SourcePlan.Name;
        public ObservableCollection<RecipePlanConfigNodeViewModel> RootNodes { get; }

        public RecipePlanConfigNodeViewModel? SelectedNode
        {
            get => _selectedNode;
            set => this.RaiseAndSetIfChanged(ref _selectedNode, value);
        }

        public object? SelectedWorkspace
        {
            get => _selectedWorkspace;
            private set => this.RaiseAndSetIfChanged(ref _selectedWorkspace, value);
        }

        /// <summary>构建方案配置左侧树（整板/区域配置/模板配置...）。</summary>
        private RecipePlanConfigNodeViewModel[] BuildTree()
        {
            var templateNode = BuildTemplateConfigNode("模板配置", _ownerRecipePlanManageCfg.ProgramTemplates, parentSubAreaBlock: null);

            var boardNode = new RecipePlanConfigNodeViewModel(
                "整板",
                RecipePlanConfigNodeType.Board,
                children: new[]
                {
                    new RecipePlanConfigNodeViewModel("区域配置", RecipePlanConfigNodeType.AreaConfig),
                    templateNode,
                });

            return new[] { boardNode };
        }

        /// <summary>构建“模板配置”节点（模板叶子来自程序编辑模板集合）。</summary>
        private static RecipePlanConfigNodeViewModel BuildTemplateConfigNode(
            string name,
            ObservableCollection<ProgramTemplateViewModel> templates,
            ProgramBlockViewModel? parentSubAreaBlock)
        {
            var templateLeaves = new List<RecipePlanConfigNodeViewModel>();
            for (var i = 0; i < templates.Count; i++)
            {
                var tmpl = templates[i];
                var order = tmpl.Order > 0 ? tmpl.Order : i + 1;
                var tmplLeaf = BuildTemplateLeafNode($"模板{order}", tmpl);
                templateLeaves.Add(tmplLeaf);
            }

            return new RecipePlanConfigNodeViewModel(
                name,
                RecipePlanConfigNodeType.TemplateConfig,
                payload: parentSubAreaBlock,
                children: templateLeaves);
        }

        /// <summary>构建“模板叶子”节点（包含区块配置，以及子区域区块的子树入口）。</summary>
        private static RecipePlanConfigNodeViewModel BuildTemplateLeafNode(string name, ProgramTemplateViewModel template)
        {
            var blockConfigNode = BuildBlockConfigNode(template);
            return new RecipePlanConfigNodeViewModel(
                name,
                RecipePlanConfigNodeType.TemplateLeaf,
                payload: template,
                children: new[] { blockConfigNode });
        }

        /// <summary>构建“区块配置”节点：每个子区域区块都挂为其子节点。</summary>
        private static RecipePlanConfigNodeViewModel BuildBlockConfigNode(ProgramTemplateViewModel template)
        {
            var children = new List<RecipePlanConfigNodeViewModel>();
            foreach (var b in template.Blocks)
            {
                if (b.BlockType != ProgramBlockType.SubArea)
                    continue;

                children.Add(BuildSubAreaBlockNode(b));
            }

            return new RecipePlanConfigNodeViewModel(
                "区块配置",
                RecipePlanConfigNodeType.BlockConfig,
                payload: template,
                children: children);
        }

        /// <summary>构建“子区域区块”节点：其下包含“子区域配置”和“模板配置（无限嵌套）”。</summary>
        private static RecipePlanConfigNodeViewModel BuildSubAreaBlockNode(ProgramBlockViewModel subAreaBlock)
        {
            var nestedTemplateConfig = BuildTemplateConfigNode("模板配置", subAreaBlock.NestedTemplates, subAreaBlock);

            return new RecipePlanConfigNodeViewModel(
                subAreaBlock.Name,
                RecipePlanConfigNodeType.SubAreaBlock,
                payload: subAreaBlock,
                children: new[]
                {
                    new RecipePlanConfigNodeViewModel("子区域配置", RecipePlanConfigNodeType.AreaConfig, payload: subAreaBlock),
                    nestedTemplateConfig,
                });
        }

        private void UpdateWorkspaceFromSelection()
        {
            if (SelectedNode == null)
            {
                SelectedWorkspace = new RecipePlanBoardPlaceholderViewModel();
                return;
            }

            SelectedWorkspace = SelectedNode.NodeType switch
            {
                RecipePlanConfigNodeType.Board => new RecipePlanBoardPlaceholderViewModel(),
                RecipePlanConfigNodeType.AreaConfig => SelectedNode.Payload is ProgramBlockViewModel
                    ? new RecipePlanSubAreaConfigPlaceholderViewModel()
                    : new RecipePlanAreaConfigWorkspaceViewModel(_ownerRecipePlanManageCfg),
                RecipePlanConfigNodeType.TemplateConfig => new RecipePlanTemplateConfigPlaceholderViewModel(),
                RecipePlanConfigNodeType.BlockConfig => SelectedNode.Payload is ProgramTemplateViewModel t
                    ? new RecipePlanBlockConfigWorkspaceViewModel(_ownerRecipePlanManageCfg, t)
                    : new RecipePlanBlockConfigPlaceholderViewModel(),
                _ => new RecipePlanNodeFallbackPlaceholderViewModel(SelectedNode.Name)
            };
        }

        /// <summary>程序编辑模板集合变化时重建左侧树。</summary>
        private void OnProgramTemplatesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AttachProgramTemplateSubscriptions();
            RebuildTreeKeepState();
        }

        /// <summary>订阅程序编辑模板/区块/嵌套模板的变化以刷新方案配置树。</summary>
        private void AttachProgramTemplateSubscriptions()
        {
            // 移除旧订阅（保留前面已加入的：PlanName/SelectedNode/CollectionChanged 解绑等）
            // 这里通过 “追加订阅并在 Rebuild 时重建” 的方式避免与其它订阅互相干扰。
            // 为了不重复追加，先清理掉我们在此方法中维护的订阅段。
            const string tag = "ProgramTemplateSubscriptions";
            for (var i = _subscriptions.Count - 1; i >= 0; i--)
            {
                if (_subscriptions[i] is TaggedDisposable td && td.Tag == tag)
                {
                    td.Dispose();
                    _subscriptions.RemoveAt(i);
                }
            }

            foreach (var t in _ownerRecipePlanManageCfg.ProgramTemplates)
                AttachSubscriptionsForTemplate(t, tag);
        }

        private void AttachSubscriptionsForTemplate(ProgramTemplateViewModel t, string tag)
        {
            _subscriptions.Add(new TaggedDisposable(tag,
                t.WhenAnyValue(x => x.Name)
                    .Skip(1)
                    .Subscribe(_ => RebuildTreeKeepState())));

            _subscriptions.Add(new TaggedDisposable(tag,
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => t.Blocks.CollectionChanged += h,
                        h => t.Blocks.CollectionChanged -= h)
                    .Subscribe(_ => RebuildTreeKeepState())));

            foreach (var b in t.Blocks)
            {
                _subscriptions.Add(new TaggedDisposable(tag,
                    b.WhenAnyValue(x => x.Name)
                        .Skip(1)
                        .Subscribe(_ => RebuildTreeKeepState())));

                if (b.BlockType == ProgramBlockType.SubArea)
                    AttachSubscriptionsForSubAreaBlock(b, tag);
            }
        }

        private void AttachSubscriptionsForSubAreaBlock(ProgramBlockViewModel b, string tag)
        {
            _subscriptions.Add(new TaggedDisposable(tag,
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => b.NestedTemplates.CollectionChanged += h,
                        h => b.NestedTemplates.CollectionChanged -= h)
                    .Subscribe(_ =>
                    {
                        // 嵌套模板增删时，同时补齐对新增模板的订阅
                        AttachProgramTemplateSubscriptions();
                        RebuildTreeKeepState();
                    })));

            foreach (var nt in b.NestedTemplates)
                AttachSubscriptionsForTemplate(nt, tag);
        }

        /// <summary>重建树并尽量保持展开/选中状态。</summary>
        private void RebuildTreeKeepState()
        {
            var expandedKeys = SnapshotExpandedKeys(RootNodes);
            var selectedKey = SelectedNode != null ? GetNodeKey(SelectedNode) : null;

            var newRoots = BuildTree();
            ApplyExpandedKeys(newRoots, expandedKeys);

            RootNodes.Clear();
            foreach (var n in newRoots)
                RootNodes.Add(n);

            if (selectedKey != null)
            {
                var match = FindNode(RootNodes, n => GetNodeKey(n) == selectedKey);
                if (match != null)
                    SelectedNode = match;
            }

            SelectedNode ??= RootNodes.FirstOrDefault();
        }

        private static HashSet<string> SnapshotExpandedKeys(IEnumerable<RecipePlanConfigNodeViewModel> nodes)
        {
            var set = new HashSet<string>();
            foreach (var n in nodes)
            {
                if (n.IsExpanded)
                    set.Add(GetNodeKey(n));
                foreach (var k in SnapshotExpandedKeys(n.Children))
                    set.Add(k);
            }
            return set;
        }

        private static void ApplyExpandedKeys(IEnumerable<RecipePlanConfigNodeViewModel> nodes, HashSet<string> expandedKeys)
        {
            foreach (var n in nodes)
            {
                n.IsExpanded = expandedKeys.Contains(GetNodeKey(n));
                ApplyExpandedKeys(n.Children, expandedKeys);
            }
        }

        private static RecipePlanConfigNodeViewModel? FindNode(IEnumerable<RecipePlanConfigNodeViewModel> nodes, Func<RecipePlanConfigNodeViewModel, bool> predicate)
        {
            foreach (var n in nodes)
            {
                if (predicate(n))
                    return n;
                var child = FindNode(n.Children, predicate);
                if (child != null)
                    return child;
            }
            return null;
        }

        private static string GetNodeKey(RecipePlanConfigNodeViewModel node)
        {
            var tmplId = (node.Payload as ProgramTemplateViewModel)?.Id ?? string.Empty;
            var blockId = (node.Payload as ProgramBlockViewModel)?.Id ?? string.Empty;
            var scope = node.NodeType == RecipePlanConfigNodeType.TemplateConfig && node.Payload is ProgramBlockViewModel b
                ? $"subarea:{b.Id}"
                : string.Empty;
            // 兼容：模板节点用模板Id；子区域区块节点用区块Id；模板配置节点用 scope 区分层级
            return $"{node.NodeType}|{tmplId}|{blockId}|{scope}|{node.Name}";
        }

        public void Dispose()
        {
            foreach (var d in _subscriptions)
                d.Dispose();
            _subscriptions.Clear();
        }
    }

    internal sealed class RecipePlanConfigNodeViewModel : ReactiveObject
    {
        private bool _isExpanded;

        public RecipePlanConfigNodeViewModel(
            string name,
            RecipePlanConfigNodeType nodeType,
            IEnumerable<RecipePlanConfigNodeViewModel>? children = null,
            object? payload = null)
        {
            Name = name;
            NodeType = nodeType;
            Payload = payload;
            Children = new ObservableCollection<RecipePlanConfigNodeViewModel>(children ?? Enumerable.Empty<RecipePlanConfigNodeViewModel>());
            // 方案配置树默认全部折叠，避免首次进入时展开过多层级
            IsExpanded = false;
        }

        public string Name { get; }
        public RecipePlanConfigNodeType NodeType { get; }
        /// <summary>节点关联的数据对象（例如 ProgramTemplateViewModel）。</summary>
        public object? Payload { get; }
        public ObservableCollection<RecipePlanConfigNodeViewModel> Children { get; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }
    }

    internal enum RecipePlanConfigNodeType
    {
        Board,
        AreaConfig,
        TemplateConfig,
        BlockConfig,
        TemplateLeaf,
        SubAreaBlock,
    }

    internal sealed class TaggedDisposable : IDisposable
    {
        public TaggedDisposable(string tag, IDisposable inner)
        {
            Tag = tag;
            _inner = inner;
        }

        public string Tag { get; }
        private readonly IDisposable _inner;

        public void Dispose() => _inner.Dispose();
    }

    internal sealed class RecipePlanBoardPlaceholderViewModel : ReactiveObject { }
    internal sealed class RecipePlanTemplateConfigPlaceholderViewModel : ReactiveObject { }
    internal sealed class RecipePlanBlockConfigPlaceholderViewModel : ReactiveObject { }
    internal sealed class RecipePlanSubAreaConfigPlaceholderViewModel : ReactiveObject { }

    internal sealed class RecipePlanNodeFallbackPlaceholderViewModel : ReactiveObject
    {
        public RecipePlanNodeFallbackPlaceholderViewModel(string nodeName)
        {
            NodeName = nodeName;
        }

        public string NodeName { get; }
    }
}
