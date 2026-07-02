using Avalonia.Controls;
using GKG.UI;
using ReactiveUI;
using System;

namespace GKG.UI.General
{
    public class StandardCylinderSingleControlSingleLimitTextViewModel : ReactiveObject
    {
        private Control? _viewReference;

        public TextInputViewModel ControlInputViewModel { get; }

        public TextInputViewModel LimitInputViewModel { get; }

        public event EventHandler? AfterModified;

        public StandardCylinderSingleControlSingleLimitTextViewModel()
        {
            ControlInputViewModel = new TextInputViewModel();
            LimitInputViewModel = new TextInputViewModel();

            ControlInputViewModel.ValueChanged += onValueChanged;
            LimitInputViewModel.ValueChanged += onValueChanged;
        }

        private void onValueChanged(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void CopyFrom(StandardCylinderSingleControlSingleLimitTextCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            ControlInputViewModel.Text = model.ControlInput;
            LimitInputViewModel.Text = model.LimitInput;
        }

        public void CopyTo(StandardCylinderSingleControlSingleLimitTextCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.ControlInput = ControlInputViewModel.Text;
            model.LimitInput = LimitInputViewModel.Text;
        }
    }
}
