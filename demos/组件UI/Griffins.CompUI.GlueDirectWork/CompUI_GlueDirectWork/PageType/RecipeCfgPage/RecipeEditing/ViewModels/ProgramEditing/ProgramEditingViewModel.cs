using ReactiveUI;
using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using GKG.UI.General;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class ProgramEditingViewModel : ReactiveObject
    {
        #region 字段
        /// <summary>树根作用域路径。</summary>
        private const string ScopePathRoot = "root";
        /// <summary>XYZ 小步进（1mm，单位按米换算）。</summary>
        private const double XyzSmallStep = 0.001;
        /// <summary>XYZ 大步进（1cm，单位按米换算）。</summary>
        private const double XyzLargeStep = 0.01;
        /// <summary>RU 小步进角度。</summary>
        private const double RuSmallStep = 0.1;
        /// <summary>RU 大步进角度。</summary>
        private const double RuLargeStep = 1.0;

        /// <summary>当前树中选中节点。</summary>
        private ProgramTreeNodeViewModel? _selectedNode;
        /// <summary>当前右侧展示的工作区对象。</summary>
        private object? _selectedWorkspace;
        /// <summary>右侧下方扩展面板是否展开（相机预览下方的空白区域）。</summary>
        private bool _isRightBottomPanelExpanded = true;
        /// <summary>树状态仓库（展开与选中）。</summary>
        private readonly TreeStateStore _treeState = new();
        /// <summary>树重建进行中标志</summary>
        private bool _isRebuildingTree;
        /// <summary>X 轴当前位置。</summary>
        private double _xPosition;
        /// <summary>Y 轴当前位置。</summary>
        private double _yPosition;
        /// <summary>Z 轴当前位置。</summary>
        private double _zPosition;
        /// <summary>R 轴倾角。</summary>
        private double _rAngle;
        /// <summary>U 轴倾角。</summary>
        private double _uAngle;
        /// <summary>
        /// 取消掉旧对象的重画树状图的监听，防止数据改变时旧对象也收到通知
        /// </summary>
        private readonly List<IDisposable> _treeSubscriptions = new();
        #endregion

        #region 公共属性
        /// <summary>左侧树数据源。</summary>
        public ObservableCollection<ProgramTreeNodeViewModel> RootNodes { get; }

        /// <summary>模板列表（由“模板管理”工作区维护）。</summary>
        public ObservableCollection<ProgramTemplateViewModel> Templates { get; }

        /// <summary>外层「模板管理」工作区实例。</summary>
        public TemplateManagementWorkspaceViewModel TemplateManagementWorkspaceViewModel { get; }

        /// <summary>「整板」工作区。</summary>
        public BoardWorkspaceViewModel BoardWorkspaceViewModel { get; }

        /// <summary>「子模板管理」占位工作区。</summary>
        public SubTemplateManagementPlaceholderViewModel SubTemplateManagementPlaceholder { get; }

        /// <summary>「区域批量生成」工作区。</summary>
        public BatchAreaGenerationWorkspaceViewModel BatchAreaGenerationWorkspaceViewModel { get; }

        /// <summary>「区域编辑」工作区。</summary>
        public AreaEditingWorkspaceViewModel AreaEditingWorkspaceViewModel { get; }

        /// <summary>中间工作区当前内容</summary>
        public object? SelectedWorkspace
        {
            get => _selectedWorkspace;
            private set
            {
                if (_selectedWorkspace is TemplateWorkspaceViewModel oldTw)
                    oldTw.Dispose();
                this.RaiseAndSetIfChanged(ref _selectedWorkspace, value);
            }
        }

        /// <summary>当前选中节点（决定右侧工作区内容）。</summary>
        public ProgramTreeNodeViewModel? SelectedNode
        {
            get => _selectedNode;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedNode, value);
                // 树重建期间 TreeView 可能会短暂把选中项回写为 null；此时不应清空选中记忆
                if (_isRebuildingTree && value == null)
                    return;
                _treeState.RememberSelection(value);
            }
        }

        /// <summary>程序编辑页右侧相机下方的扩展区域是否展开（绑定 Expander.IsExpanded）。</summary>
        public bool IsRightBottomPanelExpanded
        {
            get => _isRightBottomPanelExpanded;
            set => this.RaiseAndSetIfChanged(ref _isRightBottomPanelExpanded, value);
        }

        /// <summary>X 轴当前位置。</summary>
        public double XPosition
        {
            get => _xPosition;
            set
            {
                this.RaiseAndSetIfChanged(ref _xPosition, value);
                this.RaisePropertyChanged(nameof(XyzDisplayText));
            }
        }

        /// <summary>Y 轴当前位置。</summary>
        public double YPosition
        {
            get => _yPosition;
            set
            {
                this.RaiseAndSetIfChanged(ref _yPosition, value);
                this.RaisePropertyChanged(nameof(XyzDisplayText));
            }
        }

        /// <summary>Z 轴当前位置。</summary>
        public double ZPosition
        {
            get => _zPosition;
            set
            {
                this.RaiseAndSetIfChanged(ref _zPosition, value);
                this.RaisePropertyChanged(nameof(XyzDisplayText));
            }
        }

        /// <summary>R 轴倾角。</summary>
        public double RAngle
        {
            get => _rAngle;
            set => this.RaiseAndSetIfChanged(ref _rAngle, value);
        }

        /// <summary>U 轴倾角。</summary>
        public double UAngle
        {
            get => _uAngle;
            set => this.RaiseAndSetIfChanged(ref _uAngle, value);
        }

        /// <summary>顶部 XYZ 坐标显示文本。</summary>
        public string XyzDisplayText => $"X: {XPosition:0.000}   Y: {YPosition:0.000}   Z: {ZPosition:0.000}";

        /// <summary>X 轴正向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveXPositiveCommand { get; }
        /// <summary>X 轴负向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveXNegativeCommand { get; }
        /// <summary>X 轴正向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveXLargePositiveCommand { get; }
        /// <summary>X 轴负向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveXLargeNegativeCommand { get; }
        /// <summary>Y 轴正向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveYPositiveCommand { get; }
        /// <summary>Y 轴负向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveYNegativeCommand { get; }
        /// <summary>Y 轴正向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveYLargePositiveCommand { get; }
        /// <summary>Y 轴负向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveYLargeNegativeCommand { get; }
        /// <summary>Z 轴正向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveZPositiveCommand { get; }
        /// <summary>Z 轴负向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveZNegativeCommand { get; }
        /// <summary>Z 轴正向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveZLargePositiveCommand { get; }
        /// <summary>Z 轴负向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveZLargeNegativeCommand { get; }
        /// <summary>R 轴正向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveRPositiveCommand { get; }
        /// <summary>R 轴负向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveRNegativeCommand { get; }
        /// <summary>R 轴正向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveRLargePositiveCommand { get; }
        /// <summary>R 轴负向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveRLargeNegativeCommand { get; }
        /// <summary>U 轴正向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveUPositiveCommand { get; }
        /// <summary>U 轴负向微调命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveUNegativeCommand { get; }
        /// <summary>U 轴正向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveULargePositiveCommand { get; }
        /// <summary>U 轴负向大步进命令。</summary>
        public ReactiveCommand<Unit, Unit> MoveULargeNegativeCommand { get; }
        #endregion

        /// <summary>初始化工作区并建立树与工作区联动</summary>
        public ProgramEditingViewModel()
        {
            //和模板列表共用同一份数据
            Templates = new ObservableCollection<ProgramTemplateViewModel>();
            RootNodes = new ObservableCollection<ProgramTreeNodeViewModel>(BuildTree());
            TemplateManagementWorkspaceViewModel = new TemplateManagementWorkspaceViewModel(Templates);
            BoardWorkspaceViewModel = new BoardWorkspaceViewModel();
            SubTemplateManagementPlaceholder = new SubTemplateManagementPlaceholderViewModel();
            AreaEditingWorkspaceViewModel = new AreaEditingWorkspaceViewModel(Templates);
            BatchAreaGenerationWorkspaceViewModel = new BatchAreaGenerationWorkspaceViewModel(Templates, AreaEditingWorkspaceViewModel);
            MoveXPositiveCommand = ReactiveCommand.Create(() => AdjustXyzAxis("X", +XyzSmallStep));
            MoveXNegativeCommand = ReactiveCommand.Create(() => AdjustXyzAxis("X", -XyzSmallStep));
            MoveXLargePositiveCommand = ReactiveCommand.Create(() => AdjustXyzAxis("X", +XyzLargeStep));
            MoveXLargeNegativeCommand = ReactiveCommand.Create(() => AdjustXyzAxis("X", -XyzLargeStep));
            MoveYPositiveCommand = ReactiveCommand.Create(() => AdjustXyzAxis("Y", +XyzSmallStep));
            MoveYNegativeCommand = ReactiveCommand.Create(() => AdjustXyzAxis("Y", -XyzSmallStep));
            MoveYLargePositiveCommand = ReactiveCommand.Create(() => AdjustXyzAxis("Y", +XyzLargeStep));
            MoveYLargeNegativeCommand = ReactiveCommand.Create(() => AdjustXyzAxis("Y", -XyzLargeStep));
            MoveZPositiveCommand = ReactiveCommand.Create(() => AdjustXyzAxis("Z", +XyzSmallStep));
            MoveZNegativeCommand = ReactiveCommand.Create(() => AdjustXyzAxis("Z", -XyzSmallStep));
            MoveZLargePositiveCommand = ReactiveCommand.Create(() => AdjustXyzAxis("Z", +XyzLargeStep));
            MoveZLargeNegativeCommand = ReactiveCommand.Create(() => AdjustXyzAxis("Z", -XyzLargeStep));
            MoveRPositiveCommand = ReactiveCommand.Create(() => AdjustRuAxis("R", +RuSmallStep));
            MoveRNegativeCommand = ReactiveCommand.Create(() => AdjustRuAxis("R", -RuSmallStep));
            MoveRLargePositiveCommand = ReactiveCommand.Create(() => AdjustRuAxis("R", +RuLargeStep));
            MoveRLargeNegativeCommand = ReactiveCommand.Create(() => AdjustRuAxis("R", -RuLargeStep));
            MoveUPositiveCommand = ReactiveCommand.Create(() => AdjustRuAxis("U", +RuSmallStep));
            MoveUNegativeCommand = ReactiveCommand.Create(() => AdjustRuAxis("U", -RuSmallStep));
            MoveULargePositiveCommand = ReactiveCommand.Create(() => AdjustRuAxis("U", +RuLargeStep));
            MoveULargeNegativeCommand = ReactiveCommand.Create(() => AdjustRuAxis("U", -RuLargeStep));

            Templates.CollectionChanged += OnTemplatesChanged;
            AttachTreeNameSubscriptions();

            // 初始选中：根节点「整板」
            SelectedNode = RootNodes.FirstOrDefault(n => n.NodeType == ProgramNodeTypes.Board);

            // 监听选中的节点来切换对应的工作区
            this.WhenAnyValue(x => x.SelectedNode)
                .Subscribe(_ => UpdateWorkspaceFromSelection());

            UpdateTemplateOrder();
        }

        #region 树构建
        /// <summary>根据当前数据构建左侧树（整板/区域管理/模板管理）。</summary>
        private ProgramTreeNodeViewModel[] BuildTree()
        {
            var areaMgmt = new ProgramTreeNodeViewModel("区域管理", ProgramNodeTypes.AreaManagement, children: new[]
            {
                new ProgramTreeNodeViewModel(
                    "区域批量生成",
                    ProgramNodeTypes.BatchAreaGeneration,
                    key: new NodeKey(ProgramNodeTypes.BatchAreaGeneration, ScopePath: ScopePathRoot)),
                new ProgramTreeNodeViewModel(
                    "区域编辑",
                    ProgramNodeTypes.AreaEditing,
                    key: new NodeKey(ProgramNodeTypes.AreaEditing, ScopePath: ScopePathRoot)),
            }, key: new NodeKey(ProgramNodeTypes.AreaManagement, ScopePath: ScopePathRoot));

            var templateMgmt = new ProgramTreeNodeViewModel(
                "模板管理",
                ProgramNodeTypes.TemplateManagement,
                key: new NodeKey(ProgramNodeTypes.TemplateManagement, ScopePath: ScopePathRoot));
            foreach (var child in BuildTemplateManagementChildren(Templates, ScopePathRoot))
                templateMgmt.Children.Add(child);

            var board = new ProgramTreeNodeViewModel(
                "整板",
                ProgramNodeTypes.Board,
                children: new[] { areaMgmt, templateMgmt },
                key: new NodeKey(ProgramNodeTypes.Board, ScopePath: ScopePathRoot));
            return new[] { board };
        }

        /// <summary>为模板集合构建模板节点及其子节点。</summary>
        private IEnumerable<ProgramTreeNodeViewModel> BuildTemplateManagementChildren(
            ObservableCollection<ProgramTemplateViewModel> source,
            string scopePath,
            string? parentSubAreaBlockId = null)
        {
            //拿到当前模板管理对应的模板列表中的每一个模板
            return source.Select(t =>
            {
                var templateId = parentSubAreaBlockId == null ? t.Id : null;
                var nestedTemplateId = parentSubAreaBlockId != null ? t.Id : null;
                //每一个模板下面只保留区块管理（移除子模板管理节点）
                return new ProgramTreeNodeViewModel(t.Name, ProgramNodeTypes.Template, t, new[]
                {
                    BuildBlockManagementNode(t, scopePath, parentSubAreaBlockId),
                }, key: new NodeKey(ProgramNodeTypes.Template, templateId, parentSubAreaBlockId, nestedTemplateId, scopePath));
            });
        }

        /// <summary>构建区块管理节点及其区块子树。</summary>
        private ProgramTreeNodeViewModel BuildBlockManagementNode(
            ProgramTemplateViewModel template,
            string scopePath,
            string? parentSubAreaBlockId = null)
        {
            var templateId = parentSubAreaBlockId == null ? template.Id : null;
            var nestedTemplateId = parentSubAreaBlockId != null ? template.Id : null;
            var blockChildren = new List<ProgramTreeNodeViewModel>();
            foreach (var b in template.Blocks)
            {
                if (b.BlockType == ProgramBlockType.Trajectory)
                {
                    //轨迹区块下面有轨迹管理和指令内容
                    blockChildren.Add(new ProgramTreeNodeViewModel(b.Name, ProgramNodeTypes.TrajectoryBlock, b,
                        new[]
                        {
                            new ProgramTreeNodeViewModel(
                                "轨迹管理",
                                ProgramNodeTypes.TrajectoryManagement,
                                b,
                                null,
                                new NodeKey(ProgramNodeTypes.TrajectoryManagement, templateId, b.Id, nestedTemplateId, scopePath)),
                            new ProgramTreeNodeViewModel(
                                "指令内容",
                                ProgramNodeTypes.InstructionContent,
                                b,
                                null,
                                new NodeKey(ProgramNodeTypes.InstructionContent, templateId, b.Id, nestedTemplateId, scopePath)),
                        }, key: new NodeKey(ProgramNodeTypes.TrajectoryBlock, templateId, b.Id, nestedTemplateId, scopePath)));
                }
                //子区域区块内容
                else if (b.BlockType == ProgramBlockType.SubArea)
                    blockChildren.Add(BuildSubAreaBlockNode(b, templateId, nestedTemplateId, scopePath));
            }

            return new ProgramTreeNodeViewModel(
                "区块管理",
                ProgramNodeTypes.BlockManagement,
                template,
                blockChildren.ToArray(),
                key: new NodeKey(ProgramNodeTypes.BlockManagement, templateId, parentSubAreaBlockId, nestedTemplateId, scopePath));
        }

        /// <summary>构建子区域区块节点及其内部子树。</summary>
        private ProgramTreeNodeViewModel BuildSubAreaBlockNode(
            ProgramBlockViewModel block,
            string? templateId,
            string? nestedTemplateId,
            string scopePath)
        {
            var nestedScopePath = BuildNestedScopePath(scopePath, block.Id);

            return new ProgramTreeNodeViewModel(block.Name, ProgramNodeTypes.SubAreaBlock, block,
                new[]
                {
                    new ProgramTreeNodeViewModel(
                        "子区域管理",
                        ProgramNodeTypes.SubAreaManagement,
                        block,
                        null,
                        new NodeKey(ProgramNodeTypes.SubAreaManagement, templateId, block.Id, nestedTemplateId, nestedScopePath)),
                    BuildNestedTemplateManagementNode(block, nestedScopePath),
                }, key: new NodeKey(ProgramNodeTypes.SubAreaBlock, templateId, block.Id, nestedTemplateId, nestedScopePath));
        }

        /// <summary>构建子区域内嵌套模板管理节点。</summary>
        private ProgramTreeNodeViewModel BuildNestedTemplateManagementNode(ProgramBlockViewModel parentSubAreaBlock, string nestedScopePath)
        {
            var children = BuildTemplateManagementChildren(parentSubAreaBlock.NestedTemplates, nestedScopePath, parentSubAreaBlock.Id).ToArray();
            return new ProgramTreeNodeViewModel(
                "模板管理",
                ProgramNodeTypes.NestedTemplateManagement,
                parentSubAreaBlock,
                children,
                key: new NodeKey(ProgramNodeTypes.NestedTemplateManagement, BlockId: parentSubAreaBlock.Id, ScopePath: nestedScopePath));
        }

        /// <summary>构建下一层子区域区块作用域路径。</summary>
        private static string BuildNestedScopePath(string currentScopePath, string blockId) =>
            $"{currentScopePath}/subarea:{blockId}";
        #endregion

        #region 树状态同步
        /// <summary>重建整棵树并恢复展开状态与选中节点。</summary>
        private void RebuildTree()
        {
            // 重建开始前先记住当前选中键
            _treeState.RememberSelection(SelectedNode);
            _treeState.CaptureExpandedKeys(RootNodes);

            _isRebuildingTree = true;
            try
            {
                var newNodes = BuildTree();
                _treeState.ApplyExpandedKeys(newNodes);

                RootNodes.Clear();
                foreach (var n in newNodes)
                    RootNodes.Add(n);

                var match = _treeState.FindSelectedNode(RootNodes);
                if (match != null)
                    SelectedNode = match;

                SelectedNode ??= RootNodes.FirstOrDefault(n => n.NodeType == ProgramNodeTypes.Board);
            }
            finally
            {
                _isRebuildingTree = false;
            }
        }
        #endregion

        #region 数据变更监听
        /// <summary>模板集合变化时同步序号、订阅和树结构。</summary>
        private void OnTemplatesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateTemplateOrder();
            AttachTreeNameSubscriptions();
            RebuildTree();
        }

        /// <summary>重排模板及嵌套模板的显示序号。</summary>
        private void UpdateTemplateOrder()
        {
            UpdateTemplateOrderRecursive(Templates);
        }

        /// <summary>递归为任意层模板集合重排显示序号。</summary>
        private static void UpdateTemplateOrderRecursive(ObservableCollection<ProgramTemplateViewModel> templates)
        {
            for (var i = 0; i < templates.Count; i++)
                templates[i].Order = i + 1;

            foreach (var t in templates)
            {
                foreach (var b in t.Blocks)
                {
                    if (b.BlockType != ProgramBlockType.SubArea)
                        continue;
                    UpdateTemplateOrderRecursive(b.NestedTemplates);
                }
            }
        }

        /// <summary>重建树相关的数据监听订阅。</summary>
        private void AttachTreeNameSubscriptions()
        {
            foreach (var d in _treeSubscriptions)
                d.Dispose();
            _treeSubscriptions.Clear();

            foreach (var t in Templates)
                AttachSubscriptionsForTemplate(t);
        }

        /// <summary>为模板及其区块挂接会影响树结构的监听。</summary>
        private void AttachSubscriptionsForTemplate(ProgramTemplateViewModel t)
        {
            _treeSubscriptions.Add(
                t.WhenAnyValue(x => x.Name)
                    .Skip(1)
                    .Subscribe(_ => RebuildTree()));

            _treeSubscriptions.Add(
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => t.Blocks.CollectionChanged += h,
                        h => t.Blocks.CollectionChanged -= h)
                    .Subscribe(_ =>
                    {
                        UpdateTemplateOrder();
                        AttachTreeNameSubscriptions();
                        RebuildTree();
                    }));

            foreach (var b in t.Blocks)
            {
                _treeSubscriptions.Add(
                    b.WhenAnyValue(x => x.Name)
                        .Skip(1)
                        .Subscribe(_ => RebuildTree()));
                if (b.BlockType == ProgramBlockType.SubArea)
                    AttachSubscriptionsForSubAreaBlock(b);
            }
        }

        /// <summary>为子区域区块挂接嵌套模板变化监听。</summary>
        private void AttachSubscriptionsForSubAreaBlock(ProgramBlockViewModel b)
        {
            _treeSubscriptions.Add(
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => b.NestedTemplates.CollectionChanged += h,
                        h => b.NestedTemplates.CollectionChanged -= h)
                    .Subscribe(_ =>
                    {
                        UpdateTemplateOrder();
                        AttachTreeNameSubscriptions();
                        RebuildTree();
                    }));

            foreach (var nt in b.NestedTemplates)
                AttachSubscriptionsForTemplate(nt);
        }
        #endregion

        #region 运动控制
        /// <summary>按轴向微调 XYZ 坐标值。</summary>
        private void AdjustXyzAxis(string axis, double delta)
        {
            switch (axis)
            {
                case "X":
                    XPosition += delta;
                    break;
                case "Y":
                    YPosition += delta;
                    break;
                case "Z":
                    ZPosition += delta;
                    break;
            }

            SyncCameraShowXyzDisplay();
        }

        /// <summary>按轴向微调 RU 倾角值。</summary>
        private void AdjustRuAxis(string axis, double delta)
        {
            switch (axis)
            {
                case "R":
                    RAngle += delta;
                    break;
                case "U":
                    UAngle += delta;
                    break;
            }
        }

        /// <summary>将本地 XYZ 值同步到相机视图模型的显示字段。</summary>
        private void SyncCameraShowXyzDisplay()
        {
            var cameraVm = GlobalVisionViewModel.CameraShowViewModel;
            if (cameraVm == null)
                return;

            var x = (decimal)XPosition;
            var y = (decimal)YPosition;
            var z = (decimal)ZPosition;
            var textX = XPosition.ToString("0.000");
            var textY = YPosition.ToString("0.000");
            var textZ = ZPosition.ToString("0.000");

            cameraVm.AxisViewModel.UpdatePosition(x, y, z);
            cameraVm.AxisViewModel.XViewModel.Text = textX;
            cameraVm.AxisViewModel.YViewModel.Text = textY;
            cameraVm.AxisViewModel.ZViewModel.Text = textZ;

            // 某些流程会重定向全局 AxisViewModel 引用，这里同步一次保证相机底栏实时显示。
            if (!ReferenceEquals(GlobalVisionViewModel.AxisViewModel, cameraVm.AxisViewModel))
            {
                GlobalVisionViewModel.AxisViewModel.UpdatePosition(x, y, z);
                GlobalVisionViewModel.AxisViewModel.XViewModel.Text = textX;
                GlobalVisionViewModel.AxisViewModel.YViewModel.Text = textY;
                GlobalVisionViewModel.AxisViewModel.ZViewModel.Text = textZ;
            }
        }
        #endregion

        #region 工作区选择
        /// <summary>在树中查找目标节点的父节点。</summary>
        private static ProgramTreeNodeViewModel? FindTreeParent(
            IEnumerable<ProgramTreeNodeViewModel> nodes,
            ProgramTreeNodeViewModel target)
        {
            foreach (var n in nodes)
            {
                foreach (var c in n.Children)
                {
                    if (ReferenceEquals(c, target))
                        return n;
                }
                var sub = FindTreeParent(n.Children, target);
                if (sub != null)
                    return sub;
            }
            return null;
        }

        /// <summary>解析当前节点所属的模板作用域（外层/嵌套）。</summary>
        private ObservableCollection<ProgramTemplateViewModel> ResolveTemplateMarkScope()
        {
            for (var n = SelectedNode; n != null; n = FindTreeParent(RootNodes, n))
            {
                if (n.NodeType == ProgramNodeTypes.NestedTemplateManagement && n.Payload is ProgramBlockViewModel sb)
                    return sb.NestedTemplates;
            }
            return Templates;
        }

        /// <summary>根据当前选中节点切换右侧工作区。</summary>
        private void UpdateWorkspaceFromSelection()
        {
            if (SelectedNode == null)
            {
                SelectedWorkspace = TemplateManagementWorkspaceViewModel;
                return;
            }
            switch (SelectedNode.NodeType)
            {
                case ProgramNodeTypes.Board:
                    SelectedWorkspace = BoardWorkspaceViewModel;
                    break;
                case ProgramNodeTypes.BatchAreaGeneration:
                    SelectedWorkspace = BatchAreaGenerationWorkspaceViewModel;
                    break;
                case ProgramNodeTypes.AreaEditing:
                    SelectedWorkspace = AreaEditingWorkspaceViewModel;
                    break;
                case ProgramNodeTypes.BlockManagement:
                    if (SelectedNode.Payload is ProgramTemplateViewModel template)
                        SelectedWorkspace = new BlockManagementWorkspaceViewModel(template, RebuildTree);
                    else
                        SelectedWorkspace = TemplateManagementWorkspaceViewModel;
                    break;
                case ProgramNodeTypes.TemplateManagement:
                    SelectedWorkspace = TemplateManagementWorkspaceViewModel;
                    break;
                case ProgramNodeTypes.NestedTemplateManagement:
                    if (SelectedNode.Payload is ProgramBlockViewModel subBlock)
                        SelectedWorkspace = new TemplateManagementWorkspaceViewModel(subBlock.NestedTemplates);
                    else
                        SelectedWorkspace = TemplateManagementWorkspaceViewModel;
                    break;
                case ProgramNodeTypes.Template:
                    if (SelectedNode.Payload is ProgramTemplateViewModel tmpl)
                        SelectedWorkspace = new TemplateWorkspaceViewModel(tmpl, ResolveTemplateMarkScope(), RebuildTree);
                    else
                        SelectedWorkspace = TemplateManagementWorkspaceViewModel;
                    break;
                case ProgramNodeTypes.SubTemplateManagement:
                    SelectedWorkspace = SubTemplateManagementPlaceholder;
                    break;
                default:
                    SelectedWorkspace = SelectedNode;
                    break;
            }
        }
        #endregion

    }
}
