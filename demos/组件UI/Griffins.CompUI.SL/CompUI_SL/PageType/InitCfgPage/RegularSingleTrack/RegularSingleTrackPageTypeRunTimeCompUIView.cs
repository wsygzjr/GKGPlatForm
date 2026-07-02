using System;
using Avalonia.Controls;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;
using Griffins.CompUI.SL.InitCfgPage.Views;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;

namespace Griffins.CompUI.SL.InitCfgPage
{
    internal class RegularSingleTrackPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private ICompUIRunTimeCallBack callBack;
        private event EventHandler afterModified;
        private RegularSingleTrackCompUIView view;
        private readonly RegularSingleTrackCompUIViewModel viewModel;
        private RegularSingleTrackCompUIModel regularSingleTrackCompUIModel;

        object IPageTypeRunTimeCompUIView.View
        {
            get
            {
                RemoveViewFromParent();
                return view;
            }
        }

        public RegularSingleTrackPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
            view = new();
            viewModel = new(false, this.callBack);
            view.DataContext = viewModel;

            viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        void IPageTypeRunTimeCompUIView.SetReadOnly(bool readOnly)
        {
            viewModel.ReadOnly = readOnly;
        }

        public void SetData(RegularSingleTrackCompUIModel model)
        {
            regularSingleTrackCompUIModel = model ?? new RegularSingleTrackCompUIModel();
            viewModel.SetData(regularSingleTrackCompUIModel);
        }

        public RegularSingleTrackCompUIModel GetData()
        {
            return viewModel.GetData();
        }

        private void RemoveViewFromParent()
        {
            if (view == null) return;

            if (view.Parent is Panel panelParent)
            {
                if (panelParent.Children.Contains(view))
                {
                    panelParent.Children.Remove(view);
                }
            }
            else if (view.Parent is ContentControl contentParent)
            {
                if (contentParent.Content == view)
                {
                    contentParent.Content = null;
                }
            }
        }

        event EventHandler IPageTypeRunTimeCompUIView.AfterModified
        {
            add { afterModified += value; }
            remove { afterModified -= value; }
        }

        OpMngCellID[] IPageTypeRunTimeCompUIView.EditFuncMngCellIDs => null;

        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.RegularSingleTrack))
            {
                afterModified?.Invoke(sender, EventArgs.Empty);
            }
        }
    }
}
