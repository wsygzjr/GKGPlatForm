using Avalonia.Controls;
using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace GKG.UI.General
{
    /// <summary>
    /// Cylinder page type row: label + dropdown + text input
    /// </summary>
    public class CylinderPageTypeRowViewModel : ReactiveObject
    {
        private Control? _viewReference;

        private string _label = string.Empty;
        /// <summary>
        /// Left label text
        /// </summary>
        public string Label
        {
            get => _label;
            set => this.RaiseAndSetIfChanged(ref _label, value);
        }

        /// <summary>
        /// Label ViewModel (public control)
        /// </summary>
        public TextBlockViewModel LabelViewModel { get; }

        /// <summary>
        /// Dropdown ViewModel (public control)
        /// </summary>
        public ComboxViewModel ComboViewModel { get; }

        /// <summary>
        /// Text input ViewModel (public control)
        /// </summary>
        public TextInputViewModel TextInputViewModel { get; }

        /// <summary>
        /// Notify external data changed
        /// </summary>
        public event EventHandler? AfterModified;

        public CylinderPageTypeRowViewModel()
        {
            LabelViewModel = new TextBlockViewModel();

            ComboViewModel = new ComboxViewModel
            {
                DisplayMemberPath = nameof(ComBoxItem.DisplayName),
                PlaceholderText = "请选择卡类型",
            };

            TextInputViewModel = new TextInputViewModel();

            this.WhenAnyValue(x => x.Label)
                .Subscribe(text => LabelViewModel.Text = text);

            var cardTypeNames = new Dictionary<ControlCardType, string>
            {
                { ControlCardType.GaoChuan, "高川" },
                { ControlCardType.GMCMiNi, "GMCMiNi" },
            };
            ComboViewModel.ItemsSource = EnumExtensions.ToEnumItems(cardTypeNames);

            ComboViewModel.ValueChanged += onValueChanged;
            TextInputViewModel.ValueChanged += onValueChanged;
        }

        private void onValueChanged(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        /// <summary>
        /// Set view reference (for dialogs etc.)
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void CopyFrom(CylinderPageTypeRowCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            TextInputViewModel.Text = model.Text;

            if (ComboViewModel.ItemsSource is IEnumerable<ComBoxItem> items)
            {
                foreach (var item in items)
                {
                    var valueString = item.Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(model.ComboValue) && valueString == model.ComboValue)
                    {
                        ComboViewModel.SelectedItem = item;
                        return;
                    }

                    if (!string.IsNullOrEmpty(model.ComboDisplayName) && item.DisplayName == model.ComboDisplayName)
                    {
                        ComboViewModel.SelectedItem = item;
                        return;
                    }
                }
            }
        }

        public void CopyTo(CylinderPageTypeRowCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.Text = TextInputViewModel.Text;

            if (ComboViewModel.SelectedItem is ComBoxItem item)
            {
                model.ComboDisplayName = item.DisplayName;
                model.ComboValue = item.Value?.ToString() ?? string.Empty;
            }
        }
    }
}
