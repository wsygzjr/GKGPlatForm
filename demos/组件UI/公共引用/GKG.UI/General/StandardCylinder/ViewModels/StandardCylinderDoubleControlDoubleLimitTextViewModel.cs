using Avalonia.Controls;
using GKG.UI;
using ReactiveUI;
using System;

namespace GKG.UI.General
{
    public class StandardCylinderDoubleControlDoubleLimitTextViewModel : ReactiveObject
    {
        private Control? _viewReference;

        public TextInputViewModel OpenControlInputViewModel { get; }

        public TextInputViewModel CloseControlInputViewModel { get; }

        public TextInputViewModel UpperLimitInputViewModel { get; }

        public TextInputViewModel LowerLimitInputViewModel { get; }

        public event EventHandler? AfterModified;

        public StandardCylinderDoubleControlDoubleLimitTextViewModel()
        {
            OpenControlInputViewModel = new TextInputViewModel();
            CloseControlInputViewModel = new TextInputViewModel();
            UpperLimitInputViewModel = new TextInputViewModel();
            LowerLimitInputViewModel = new TextInputViewModel();

            OpenControlInputViewModel.ValueChanged += onValueChanged;
            CloseControlInputViewModel.ValueChanged += onValueChanged;
            UpperLimitInputViewModel.ValueChanged += onValueChanged;
            LowerLimitInputViewModel.ValueChanged += onValueChanged;
        }

        private void onValueChanged(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void CopyFrom(StandardCylinderDoubleControlDoubleLimitTextCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            OpenControlInputViewModel.Text = model.OpenControlInput;
            CloseControlInputViewModel.Text = model.CloseControlInput;
            UpperLimitInputViewModel.Text = model.UpperLimitInput;
            LowerLimitInputViewModel.Text = model.LowerLimitInput;
        }

        public void CopyTo(StandardCylinderDoubleControlDoubleLimitTextCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.OpenControlInput = OpenControlInputViewModel.Text;
            model.CloseControlInput = CloseControlInputViewModel.Text;
            model.UpperLimitInput = UpperLimitInputViewModel.Text;
            model.LowerLimitInput = LowerLimitInputViewModel.Text;
        }
    }
}
