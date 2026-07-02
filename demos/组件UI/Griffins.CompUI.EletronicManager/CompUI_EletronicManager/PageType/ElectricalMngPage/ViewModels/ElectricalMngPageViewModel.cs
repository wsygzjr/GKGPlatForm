using Avalonia.Controls;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ControlCard.ViewModels;
using Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.IODevice.ViewModels;
using Griffins.Map.UI;
using ReactiveUI;
using System;

namespace Griffins.CompUI.EletronicManager.CompUI_EletronicManager.PageType.ElectricalMngPage.ViewModels
{
    public class ElectricalMngPageViewModel : ReactiveObject
    {
        private readonly IODeviceCfgViewModel _analogIODeviceCfgViewModel = new();

        public ControlCardCfgViewModel ControlCardCfgViewModel { get; } = new();

        public AxisConfigViewModel AxisConfigViewModel => ControlCardCfgViewModel.AxisConfigViewModel;

        public IODeviceCfgViewModel StateIODeviceCfgViewModel => ControlCardCfgViewModel.IODeviceCfgViewModel;

        public IODeviceCfgViewModel AnalogIODeviceCfgViewModel => _analogIODeviceCfgViewModel;

        public event EventHandler? AfterModified
        {
            add => ControlCardCfgViewModel.AfterModified += value;
            remove => ControlCardCfgViewModel.AfterModified -= value;
        }

        public void SetCallBack(ICompUIRunTimeCallBack? callBack)
        {
            ControlCardCfgViewModel.SetCallBack(callBack);
        }

        public void SetViewReference(Control view)
        {
            ControlCardCfgViewModel.SetViewReference(view);
        }

        public void Init(byte[] data)
        {
            ControlCardCfgViewModel.Init(data);
            _analogIODeviceCfgViewModel.SetControlCardsProvider(() => ControlCardCfgViewModel.ControlCardListViewModel.ControlCardList);
            _analogIODeviceCfgViewModel.Init(data);
        }

        public byte[] GetData()
        {
            return ControlCardCfgViewModel.GetData();
        }

    }
}
