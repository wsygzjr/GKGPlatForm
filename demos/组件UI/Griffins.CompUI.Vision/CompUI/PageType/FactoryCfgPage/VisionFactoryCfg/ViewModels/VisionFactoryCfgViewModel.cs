using GKG.SubMM;
using GKG.UI;
using GKG.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffins.CompUI.Vision.CompUI.PageType.FactoryCfgPage.VisionFactoryCfg.ViewModels
{
    public class VisionFactoryCfgViewModel
    {
        public ComboxViewModel VisionComboxViewModel { get; }

        public event EventHandler? AfterModified;

        public List<ComBoxItem> VisionItems = new List<ComBoxItem>();

        private VisionSubMachineModulesFactoryCfg factoryCfg;
        public VisionFactoryCfgViewModel()
        {
            VisionComboxViewModel = new ComboxViewModel();
            foreach (var item in VisionPluginManager.GetAllVisionDriverNames())
            {
                VisionItems.Add(new ComBoxItem()
                {
                    Value = item,
                    DisplayName= item
                });
            }
            VisionComboxViewModel.ItemsSource = VisionItems;
            VisionComboxViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            VisionComboxViewModel.ValueChanged += OnValueChanged;
        }

        internal VisionSubMachineModulesFactoryCfg GetData()
        {
            return factoryCfg;
        }

        internal void SetData(VisionSubMachineModulesFactoryCfg model)
        {
            factoryCfg = model;
            foreach(ComBoxItem item in VisionComboxViewModel.ItemsSource)
            {
                if (item.DisplayName == factoryCfg.VisionDiverName)
                {
                    VisionComboxViewModel.SelectedItem = item;
                    break;
                }
            }
        }

        private void OnValueChanged(object? oldValue, object? newValue)
        {
            factoryCfg.VisionDiverName = ((ComBoxItem)((ValueChangedEventArgs)newValue).NewValue).DisplayName;
        }
    }
}
