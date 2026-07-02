using GKG.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.SL.InitCfgPage.ViewModels
{
    internal class SensorConfigViewModel : ReactiveObject
    {
        private readonly List<ComBoxItem> _cardTypeItems;

        public ComboxViewModel CardTypeViewModel { get; }

        public TextInputViewModel TextViewModel { get; }

        public SensorConfigViewModel()
        {
            CardTypeViewModel = new ComboxViewModel();
            TextViewModel = new TextInputViewModel();

            _cardTypeItems = new List<ComBoxItem>
            {
                new ComBoxItem { Value = Models.SensorCardType.GaoChuan, DisplayName = "高川" },
                new ComBoxItem { Value = Models.SensorCardType.GMCMiNi, DisplayName = "GMCMiNi" },
            };

            CardTypeViewModel.ItemsSource = _cardTypeItems;
            CardTypeViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            CardTypeViewModel.SelectedItem = _cardTypeItems.FirstOrDefault();
        }

        public void CopyFrom(Models.SensorConfigCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            var target = _cardTypeItems.FirstOrDefault(o => (Models.SensorCardType)o.Value == model.CardType);
            if (target != null)
            {
                CardTypeViewModel.SelectedItem = target;
            }

            TextViewModel.Text = model.Text ?? string.Empty;
        }

        public void CopyTo(Models.SensorConfigCfgInfo model)
        {
            if (model == null)
            {
                return;
            }

            model.CardType = (Models.SensorCardType)((CardTypeViewModel.SelectedItem as ComBoxItem)?.Value ?? Models.SensorCardType.GaoChuan);
            model.Text = TextViewModel.Text ?? string.Empty;
        }
    }
}
