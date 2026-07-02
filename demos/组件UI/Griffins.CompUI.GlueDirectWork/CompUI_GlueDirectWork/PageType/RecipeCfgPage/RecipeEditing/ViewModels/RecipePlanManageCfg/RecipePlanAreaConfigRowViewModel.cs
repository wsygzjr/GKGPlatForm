using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Griffins.CompUI.GlueDirectWork.PageType.RecipeCfgPage.RecipeEditing.ViewModels
{
    internal sealed class RecipePlanAreaConfigRowViewModel : ReactiveObject, IDisposable
    {
        private readonly AreaEditingRowViewModel _areaRow;
        private bool _isChecked;
        private bool _regionEnabledInPlan = true;
        private int _enableSequence;
        private bool _canMoveUp;
        private bool _canMoveDown;

        public RecipePlanAreaConfigRowViewModel(AreaEditingRowViewModel areaRow, int enableSequence, bool regionEnabledInPlan)
        {
            _areaRow = areaRow;
            _enableSequence = enableSequence;
            _regionEnabledInPlan = regionEnabledInPlan;

            _areaRow.AreaNameViewModel.WhenAnyValue(x => x.Text)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(AreaName));
                })
                .DisposeWith(RowSubscriptions);
        }

        public CompositeDisposable RowSubscriptions { get; } = new();

        public AreaEditingRowViewModel SourceAreaRow => _areaRow;

        public string AreaName => _areaRow.AreaNameViewModel.Text ?? string.Empty;

        public int EnableSequence
        {
            get => _enableSequence;
            set => this.RaiseAndSetIfChanged(ref _enableSequence, value);
        }

        public bool RegionEnabledInPlan
        {
            get => _regionEnabledInPlan;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _regionEnabledInPlan, value))
                {
                    this.RaisePropertyChanged(nameof(EnableStatusText));
                }
            }
        }

        public string EnableStatusText => RegionEnabledInPlan ? "启用" : "禁用";

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }

        public bool CanMoveUp
        {
            get => _canMoveUp;
            set => this.RaiseAndSetIfChanged(ref _canMoveUp, value);
        }

        public bool CanMoveDown
        {
            get => _canMoveDown;
            set => this.RaiseAndSetIfChanged(ref _canMoveDown, value);
        }

        public void DisposeSubscriptions()
        {
            RowSubscriptions.Dispose();
        }

        public void Dispose()
        {
            RowSubscriptions.Dispose();
        }
    }
}

