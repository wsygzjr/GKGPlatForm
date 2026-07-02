using Avalonia.Controls;
using GKG.UI;
using ReactiveUI;
using System;

namespace GKG.UI.General
{
    public class StandardCylinderSingleControlDoubleLimitTextViewModel : ReactiveObject
    {
        private Control? _viewReference;

        public TextInputViewModel ControlInputViewModel { get; }

        public TextInputViewModel UpperLimitInputViewModel { get; }

        public TextInputViewModel LowerLimitInputViewModel { get; }

        public event EventHandler? AfterModified;

        public StandardCylinderSingleControlDoubleLimitTextViewModel()
        {
            ControlInputViewModel = new TextInputViewModel();
            UpperLimitInputViewModel = new TextInputViewModel();
            LowerLimitInputViewModel = new TextInputViewModel();

            ControlInputViewModel.ValueChanged += onValueChanged;
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

        public void CopyFrom(StandardCylinderSingleControlDoubleLimitTextCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            ControlInputViewModel.Text = model.ControlInput;
            UpperLimitInputViewModel.Text = model.UpperLimitInput;
            LowerLimitInputViewModel.Text = model.LowerLimitInput;
        }

        public void CopyTo(StandardCylinderSingleControlDoubleLimitTextCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.ControlInput = ControlInputViewModel.Text;
            model.UpperLimitInput = UpperLimitInputViewModel.Text;
            model.LowerLimitInput = LowerLimitInputViewModel.Text;
        }
    }
}
