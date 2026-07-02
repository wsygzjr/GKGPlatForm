using Avalonia.Controls;
using Griffins.CompUI.SL.InitCfgPage.Models;
using Griffins.CompUI.SL.InitCfgPage.ViewModels;
using Griffins.CompUI.SL.InitCfgPage.Views;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.PF;
using System;

namespace Griffins.CompUI.SL.InitCfgPage
{
    internal class TrackStationConfigPageTypeRunTimeCompUIView : IPageTypeRunTimeCompUIView
    {
        private readonly ICompUIRunTimeCallBack callBack;

        private event EventHandler afterModified;

        private TrackStationConfigCompUIView view;

        private readonly TrackStationConfigCompUIViewModel viewModel;

        private TrackStationConfigCompUIModel trackStationConfigCompUIModel;

        object IPageTypeRunTimeCompUIView.View
        {
            get
            {
                RemoveViewFromParent();
                return view;
            }
        }

        public TrackStationConfigPageTypeRunTimeCompUIView(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
            view = new();
            viewModel = new(false, this.callBack);
            view.DataContext = viewModel;
            viewModel.SetViewReference(view);

            viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        void IPageTypeRunTimeCompUIView.SetReadOnly(bool readOnly)
        {
            viewModel.ReadOnly = readOnly;
        }

        public void SetData(TrackStationConfigCompUIModel trackStationConfigCompUIModel)
        {
            this.trackStationConfigCompUIModel = trackStationConfigCompUIModel ?? new TrackStationConfigCompUIModel();
            viewModel.SetData(this.trackStationConfigCompUIModel);
        }

        public TrackStationConfigCompUIModel GetData()
        {
            return viewModel.GetData();
        }

        event EventHandler IPageTypeRunTimeCompUIView.AfterModified
        {
            add
            {
                afterModified += value;
            }
            remove
            {
                afterModified -= value;
            }
        }

        OpMngCellID[] IPageTypeRunTimeCompUIView.EditFuncMngCellIDs
        {
            get
            {
                return null;
            }
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

        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.HasLeftSensor)
                || e.PropertyName == nameof(viewModel.HasRightSensor)
                || e.PropertyName == nameof(viewModel.HasLeftCylinder)
                || e.PropertyName == nameof(viewModel.HasRightCylinder))
            {
                afterModified?.Invoke(sender, EventArgs.Empty);
            }
        }
    }
}
