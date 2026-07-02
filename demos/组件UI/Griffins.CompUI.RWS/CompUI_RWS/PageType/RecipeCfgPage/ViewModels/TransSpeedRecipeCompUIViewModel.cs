using GF_Gereric;
using GKG.SubMM;
using GKG.UI;
using Griffins.Map.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Griffins.CompUI.RWS.CompUI_RWS.PageType.RecipeCfgPage.ViewModels
{
    internal class TransSpeedRecipeCompUIViewModel : ReactiveObject
    {
        private const string RunTimeCtlGetSpeedList = RailWorkStationSubMachineModulesConst.GetSpeedListCtlCmdID;

        private readonly ICompUIRunTimeCallBack callBack;
        private bool readOnly;
        private int selectedGearId;
        private bool isApplyingData;

        public ComboxViewModel TransSpeedGearComboViewModel { get; } = new ComboxViewModel();

        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                this.RaiseAndSetIfChanged(ref readOnly, value);
                TransSpeedGearComboViewModel.IsEnabled = !readOnly;
            }
        }

        public int TransSpeedGearID
        {
            get
            {
                if (TransSpeedGearComboViewModel.SelectedItem is ComBoxItem item && item.Value is int gearId)
                {
                    return gearId;
                }

                return selectedGearId;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedGearId, value);
                ApplySelectedGear(value);
            }
        }

        public TransSpeedRecipeCompUIViewModel()
        {
            InitComboPresentation();
        }

        public TransSpeedRecipeCompUIViewModel(ICompUIRunTimeCallBack callBack)
        {
            this.callBack = callBack;
            InitComboPresentation();
            TransSpeedGearComboViewModel.ValueChanged += (_, __) => OnGearSelectionChanged();
        }

        public void LoadSpeedGearOptions()
        {
            var previousGearId = selectedGearId;
            var newItems = new List<ComBoxItem>();

            if (callBack != null)
            {
                try
                {
                    var result = callBack.ExecConfigSvrCtlCmd(RunTimeCtlGetSpeedList, new GFBaseTypeParamValueList());
                    var raw = result?["Result"]?.ToStringVal();
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        raw = result?["data"]?.ToStringVal();
                    }

                    var list = string.IsNullOrWhiteSpace(raw)
                        ? null
                        : JsonObjConvert.FromJSon<WorkStationTransSpeedGearList>(raw);

                    if (list != null)
                    {
                        var addedGearIds = new HashSet<int>();
                        foreach (var gear in list)
                        {
                            if (gear == null || !addedGearIds.Add(gear.TransSpeedGear))
                            {
                                continue;
                            }

                            newItems.Add(new ComBoxItem
                            {
                                Value = gear.TransSpeedGear,
                                DisplayName = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "档位 {0}（速度 {1}）",
                                    gear.TransSpeedGear,
                                    gear.TransSpeed),
                            });
                        }
                    }
                }
                catch
                {
                }
            }

            TransSpeedGearComboViewModel.ItemsSource = null;
            TransSpeedGearComboViewModel.ItemsSource = newItems;
            ApplySelectedGear(previousGearId);
        }

        public void SetData(RailWorkStationSubMachineModulesPPCfg data)
        {
            isApplyingData = true;
            try
            {
                var model = data?.WorkStationTransSpeed ?? new WorkStationTransSpeed();
                selectedGearId = model.TransSpeedGearID;
                LoadSpeedGearOptions();
                this.RaisePropertyChanged(nameof(TransSpeedGearID));
            }
            finally
            {
                isApplyingData = false;
            }
        }

        public WorkStationTransSpeed GetData()
        {
            return new WorkStationTransSpeed
            {
                TransSpeedGearID = TransSpeedGearID,
            };
        }

        private void InitComboPresentation()
        {
            TransSpeedGearComboViewModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
        }

        private void ApplySelectedGear(int gearId)
        {
            selectedGearId = gearId;
            var items = TransSpeedGearComboViewModel.ItemsSource as IEnumerable<ComBoxItem>
                ?? Enumerable.Empty<ComBoxItem>();
            var target = items.FirstOrDefault(x => x.Value is int id && id == gearId);
            if (target != null)
            {
                TransSpeedGearComboViewModel.SelectedItem = target;
                return;
            }

            TransSpeedGearComboViewModel.SelectedItem = items.FirstOrDefault();
            if (TransSpeedGearComboViewModel.SelectedItem is ComBoxItem firstItem && firstItem.Value is int firstGearId)
            {
                selectedGearId = firstGearId;
            }
        }

        private void OnGearSelectionChanged()
        {
            if (isApplyingData)
            {
                return;
            }

            var newGearId = TransSpeedGearID;
            if (selectedGearId == newGearId)
            {
                return;
            }

            selectedGearId = newGearId;
            this.RaisePropertyChanged(nameof(TransSpeedGearID));
        }
    }
}
