using Avalonia.Controls;
using GKG.UI;
using ReactiveUI;
using System;

namespace GKG.UI.General
{
    public class StandardCylinderDoubleControlSingleLimitTextViewModel : ReactiveObject
    {
        private Control? _viewReference;

        public TextInputViewModel OpenControlInputViewModel { get; }

        public TextInputViewModel CloseControlInputViewModel { get; }

        public event EventHandler? AfterModified;
        
        public StandardCylinderDoubleControlSingleLimitTextViewModel()
        {
            OpenControlInputViewModel = new TextInputViewModel();
            CloseControlInputViewModel = new TextInputViewModel();

            OpenControlInputViewModel.ValueChanged += onValueChanged;
            CloseControlInputViewModel.ValueChanged += onValueChanged;
        }

        private void onValueChanged(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }

        public void SetViewReference(Control view)
        {
            _viewReference = view;
        }

        public void CopyFrom(StandardCylinderDoubleControlSingleLimitTextCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            OpenControlInputViewModel.Text = model.OpenControlInput;
            CloseControlInputViewModel.Text = model.CloseControlInput;
        }

        public void CopyTo(StandardCylinderDoubleControlSingleLimitTextCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.OpenControlInput = OpenControlInputViewModel.Text;
            model.CloseControlInput = CloseControlInputViewModel.Text;
        }
    }
}
