using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Collections.Generic;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class TemplateManagementWorkspaceViewModel : ReactiveObject
    {
        /// <summary>当前选中的模板项。</summary>
        private ProgramTemplateViewModel? _selectedTemplate;
        /// <summary>下一个默认模板编号。</summary>
        private int _nextTemplateIndex;
        /// <summary>是否存在勾选模板。</summary>
        private bool _hasCheckedTemplates;
        //可释放资源，用于取消上一轮的订阅
        private readonly List<IDisposable> _checkSubscriptions = new();

        /// <summary>初始化模板管理工作区。</summary>
        public TemplateManagementWorkspaceViewModel(ObservableCollection<ProgramTemplateViewModel> templates)
        {
            Templates = templates;
            _nextTemplateIndex = Templates.Select(t => TryGetTemplateIndex(t.Name)).DefaultIfEmpty(0).Max();
            AddTemplateCommand = ReactiveCommand.Create(AddTemplate);
            DeleteTemplateCommand = ReactiveCommand.Create(DeleteCheckedTemplates, this.WhenAnyValue(x => x.HasCheckedTemplates));
            DeleteTemplateItemCommand = ReactiveCommand.Create<ProgramTemplateViewModel>(DeleteTemplateItem);

            Templates.CollectionChanged += OnTemplatesChanged;
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedTemplates();
        }

        /// <summary>新增模板命令。</summary>
        public ReactiveCommand<Unit, Unit> AddTemplateCommand { get; }

        /// <summary>批量删除模板命令。</summary>
        public ReactiveCommand<Unit, Unit> DeleteTemplateCommand { get; }

        /// <summary>删除单个模板命令。</summary>
        public ReactiveCommand<ProgramTemplateViewModel, Unit> DeleteTemplateItemCommand { get; }

        /// <summary>模板集合数据源。</summary>
        public ObservableCollection<ProgramTemplateViewModel> Templates { get; }

        public bool HasCheckedTemplates
        {
            get => _hasCheckedTemplates;
            private set => this.RaiseAndSetIfChanged(ref _hasCheckedTemplates, value);
        }

        /// <summary>当前选中模板。</summary>
        public ProgramTemplateViewModel? SelectedTemplate
        {
            get => _selectedTemplate;
            set => this.RaiseAndSetIfChanged(ref _selectedTemplate, value);
        }

        //添加模板
        private void AddTemplate()
        {
            //添加模板时，以最大数+1添加
            _nextTemplateIndex = Math.Max(_nextTemplateIndex, Templates.Select(t => TryGetTemplateIndex(t.Name)).DefaultIfEmpty(0).Max());
            var template = new ProgramTemplateViewModel($"模板{++_nextTemplateIndex}");
            Templates.Add(template);
            SelectedTemplate = template;
        }

        //获取到最大的模板数字编号
        /// <summary>根据命名规则提取模板序号。</summary>
        private static int TryGetTemplateIndex(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return 0;
            //判断是否以模板开头，不是模板开头的直接返回
            if (!name.StartsWith("模板", StringComparison.Ordinal))
                return 0;
            var suffix = name.Substring(2);  //丢掉前两个字符
            return int.TryParse(suffix, out var i) ? i : 0;  //拿到最后的数字字符串转换成int
        }

        //批量删除模板
        private void DeleteCheckedTemplates()
        {
            //检查模板列表中的每一项是否勾选
            var targets = Templates.Where(t => t.IsChecked).ToList();
            if (targets.Count == 0)
                return;

            //对于勾选的每一项都从列表中移除
            foreach (var t in targets)
                Templates.Remove(t);

            SelectedTemplate = Templates.FirstOrDefault();
        }

        //删除单个模板
        private void DeleteTemplateItem(ProgramTemplateViewModel template)
        {
            var index = Templates.IndexOf(template);  //拿到要删除模板的索引
            //索引小于0表示该模板不存在Templates列表中
            if (index < 0)
                return;

            var wasSelected = ReferenceEquals(SelectedTemplate, template);
            Templates.RemoveAt(index);
            //如果删除的不是选中的模板，直接返回
            if (!wasSelected)
                return;
            //删除的是选中的模板，且只有该模板，则不选中
            if (Templates.Count == 0)
                SelectedTemplate = null;
            //该模板不是最后一个，则选中该模板的后一个
            else if (index < Templates.Count)
                SelectedTemplate = Templates[index];
            //是最后一个，则选中当前列表的最后一个
            else
                SelectedTemplate = Templates[^1];
        }

        //Templates列表变化时的回调，订阅Templates.CollectionChanged
        /// <summary>模板集合变化时刷新勾选状态相关订阅。</summary>
        private void OnTemplatesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AttachCheckSubscriptionsForAll();
            UpdateHasCheckedTemplates();
        }

        //检查模板列表中勾选是否发生变化，HasCheckedTemplates决定是否启用批量删除按钮
        /// <summary>重新订阅每个模板的勾选变化。</summary>
        private void AttachCheckSubscriptionsForAll()
        {
            //去掉上一轮旧对象的订阅
            foreach (var d in _checkSubscriptions)
                d.Dispose();
            _checkSubscriptions.Clear();
            //对于每一行的勾选一旦发生变化就更新HasCheckedTemplates
            foreach (var t in Templates)
            {
                _checkSubscriptions.Add(
                    t.WhenAnyValue(x => x.IsChecked)
                        .Subscribe(_ => UpdateHasCheckedTemplates()));
            }
        }

        //根据列表的勾选情况更新HasCheckedTemplates
        /// <summary>根据模板勾选情况更新批量删除按钮可用性。</summary>
        private void UpdateHasCheckedTemplates()
        {
            HasCheckedTemplates = Templates.Any(t => t.IsChecked);
        }
    }
}
