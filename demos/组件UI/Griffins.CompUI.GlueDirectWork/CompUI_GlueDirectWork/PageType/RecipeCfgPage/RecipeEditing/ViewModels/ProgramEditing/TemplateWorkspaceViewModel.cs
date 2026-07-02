using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class TemplateWorkspaceViewModel : ReactiveObject, IDisposable
    {
        /// <summary>当前页面内部订阅集合，释放时统一清理。</summary>
        private readonly CompositeDisposable _d = new();
        /// <summary>模板名称变更后用于通知左侧树重建的回调。</summary>
        private readonly Action _rebuildTree;
        /// <summary>Mark 下拉框的选项列表。</summary>
        private List<ComBoxItem> MarkComboboxItems= new List<ComBoxItem>();

        /// <summary>初始化模板工作区，并建立 UI 与模板模型双向同步。</summary>
        public TemplateWorkspaceViewModel(ProgramTemplateViewModel template,ObservableCollection<ProgramTemplateViewModel> allTemplates,Action rebuildTree)
        {
            Template = template;
            _rebuildTree = rebuildTree;

            TemplateNameViewModel = new TextInputViewModel { Text = template.Name };
            RefPointX = new NumericViewModel { DecimalPlaces = 2, Increment = 0.01m, Maximum = 99999.999m, Value = template.RefBaseX };
            RefPointY = new NumericViewModel { DecimalPlaces = 2, Increment = 0.01m, Maximum = 99999.999m, Value = template.RefBaseY };
            MarkPointX = new NumericViewModel { DecimalPlaces = 2, Increment = 0.01m, Maximum = 99999.999m, Value = template.MarkX };
            MarkPointY = new NumericViewModel { DecimalPlaces = 2, Increment = 0.01m, Maximum = 99999.999m, Value = template.MarkY };

            foreach (var ProgramTemplateViewModel in allTemplates)
            {
                MarkComboboxItems.Add(new ComBoxItem { Value = ProgramTemplateViewModel.Name,DisplayName = ProgramTemplateViewModel.Name });
            }
            MarkComboboxViewModel = new ComboxViewModel { DisplayMemberPath = nameof(ComBoxItem.DisplayName), ItemsSource = MarkComboboxItems };
            MarkComboboxViewModel.SelectedItem =
                allTemplates.FirstOrDefault(x => x.Id == template.MarkTemplateId) ?? template;

            _d.Add(TemplateNameViewModel.WhenAnyValue(x => x.Text).Skip(1).Subscribe(t =>
            {
                if (template.Name == t)
                    return;
                template.Name = t;
                rebuildTree();
            }));
            _d.Add(RefPointX.WhenAnyValue(x => x.Value).Subscribe(v => template.RefBaseX = v));
            _d.Add(RefPointY.WhenAnyValue(x => x.Value).Subscribe(v => template.RefBaseY = v));
            _d.Add(MarkPointX.WhenAnyValue(x => x.Value).Subscribe(v => template.MarkX = v));
            _d.Add(MarkPointY.WhenAnyValue(x => x.Value).Subscribe(v => template.MarkY = v));
            _d.Add(MarkComboboxViewModel.WhenAnyValue(x => x.SelectedItem).Skip(1).Subscribe(o =>
            {
                if (o is ProgramTemplateViewModel m)
                    template.MarkTemplateId = m.Id;
            }));

            TeachCommand = ReactiveCommand.Create(() => { });
            RecognizeCommand = ReactiveCommand.Create(() => { });
        }

        /// <summary>当前编辑的模板模型。</summary>
        public ProgramTemplateViewModel Template { get; }

        /// <summary>模板名称输入控件。</summary>
        public TextInputViewModel TemplateNameViewModel { get; }
        /// <summary>参考点 X 输入控件。</summary>
        public NumericViewModel RefPointX { get; }
        /// <summary>参考点 Y 输入控件。</summary>
        public NumericViewModel RefPointY { get; }
        /// <summary>Mark 参考模板下拉控件。</summary>
        public ComboxViewModel MarkComboboxViewModel { get; }
        /// <summary>Mark 点 X 输入控件。</summary>
        public NumericViewModel MarkPointX { get; }
        /// <summary>Mark 点 Y 输入控件。</summary>
        public NumericViewModel MarkPointY { get; }

        /// <summary>示教命令（预留）。</summary>
        public ReactiveCommand<Unit, Unit> TeachCommand { get; }
        /// <summary>识别命令（预留）。</summary>
        public ReactiveCommand<Unit, Unit> RecognizeCommand { get; }

        /// <summary>释放当前工作区中注册的所有订阅。</summary>
        public void Dispose() => _d.Dispose();
    }
}
