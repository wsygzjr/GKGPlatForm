using GF_Gereric;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Griffins.CompUI.AxisDebug.CompUI_AxisDebug.PageType.InitCfgPage.IOOutDebugging.ViewModels
{
    public class IOOutDebuggingCompUIViewModel : ReactiveObject
    {
        public event EventHandler AfterModified;

        private readonly ObservableCollection<IOOutItemViewModel> _column1Items = new();
        private readonly ObservableCollection<IOOutItemViewModel> _column2Items = new();
        private readonly ObservableCollection<IOOutItemViewModel> _column3Items = new();

        private IOOutItemViewModel _selectedItem;

        public ObservableCollection<IOOutItemViewModel> Column1Items => _column1Items;

        public ObservableCollection<IOOutItemViewModel> Column2Items => _column2Items;

        public ObservableCollection<IOOutItemViewModel> Column3Items => _column3Items;

        public IOOutItemViewModel SelectedItem
        {
            get => _selectedItem;
            private set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        private byte[] _cfgInfo;

        public byte[] CfgInfo
        {
            get
            {
                var cfgList = _column1Items
                    .Concat(_column2Items)
                    .Concat(_column3Items)
                    .Select(x => new IOOutItemCfg
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsOn = x.IsOn,
                        IsEnabled = x.IsEnabled
                    })
                    .ToList();

                _cfgInfo = JsonObjConvert.ToJSonBytes(cfgList);
                return _cfgInfo;
            }
        }

        public IOOutDebuggingCompUIViewModel()
        {
            loadCfgInfo(null);
        }

        public void Init(byte[] cfgInfo)
        {
            _cfgInfo = cfgInfo;
            loadCfgInfo(cfgInfo);
        }

        private void loadCfgInfo(byte[] cfgInfo)
        {
            List<IOOutItemCfg> cfgList = null;
            if (cfgInfo != null && cfgInfo.Length > 0)
            {
                try
                {
                    cfgList = JsonObjConvert.FromJSonBytes<List<IOOutItemCfg>>(cfgInfo);
                }
                catch
                {
                    cfgList = null;
                }
            }

            cfgList ??= getDefaultItems();

            rebuildColumns(cfgList);
        }

        private void rebuildColumns(List<IOOutItemCfg> cfgList)
        {
            _column1Items.Clear();
            _column2Items.Clear();
            _column3Items.Clear();

            SelectedItem = null;

            for (int i = 0; i < cfgList.Count; i++)
            {
                var cfg = cfgList[i];
                var itemVm = new IOOutItemViewModel(cfg.Id, cfg.Name, cfg.IsOn, cfg.IsEnabled);
                itemVm.AfterToggled += onAfterToggled;

                if (!string.IsNullOrWhiteSpace(cfg.Name) && cfg.Name.StartsWith("(A轨)", StringComparison.OrdinalIgnoreCase))
                {
                    _column2Items.Add(itemVm);
                }
                else if (!string.IsNullOrWhiteSpace(cfg.Name) && cfg.Name.StartsWith("(B轨)", StringComparison.OrdinalIgnoreCase))
                {
                    _column3Items.Add(itemVm);
                }
                else
                {
                    _column1Items.Add(itemVm);
                }
            }
        }

        public void SelectItem(IOOutItemViewModel item)
        {
            if (ReferenceEquals(SelectedItem, item))
                return;

            if (SelectedItem != null)
                SelectedItem.IsSelected = false;

            SelectedItem = item;

            if (SelectedItem != null)
                SelectedItem.IsSelected = true;
        }

        private void onAfterToggled(object sender, EventArgs e)
        {
            AfterModified?.Invoke(this, EventArgs.Empty);
        }

        private static List<IOOutItemCfg> getDefaultItems()
        {
            var names = new List<string>
            {
                "红灯",
                "蜂鸣器",
                "黄灯",
                "绿灯",
                "(前门)安全门气缸锁",
                "(后门)安全门气缸锁",
                "驱动器电源",
                "胶水气压",
                "右阀喷胶",
                "左阀喷胶",
                "胶阀气压",
                "真空",
                "右阀",
                "左阀",
                "机器运行灯",

                "(A轨)左挡板气缸",
                "(A轨)右挡板气缸",
                "(A轨)出料口挡板气缸",
                "(A轨)顶板气缸",
                "(A轨)顶板气缸2",
                "(A轨)顶板气缸3",
                "(A轨)本机真空板",
                "(A轨)本机右板",
                "(A轨)左加热吹气",
                "(A轨)中加热吹气",
                "(A轨)右加热吹气",
                "(A轨)侧夹气缸",
                "(A轨)X向盖板保护信号",
                "(A轨)Z向盖板保护信号",
                "(A轨)平台真空吸",
                "(A轨)等待位顶板气缸",

                "(B轨)左挡板气缸",
                "(B轨)右挡板气缸",
                "(B轨)出料口挡板气缸",
                "(B轨)顶板气缸",
                "(B轨)顶板气缸2",
                "(B轨)顶板气缸3",
                "(B轨)本机真空板",
                "(B轨)本机右板",
                "(B轨)左加热吹气",
                "(B轨)中加热吹气",
                "(B轨)右加热吹气",
                "(B轨)侧夹气缸",
                "(B轨)X向盖板保护信号",
                "(B轨)Z向盖板保护信号",
                "(B轨)平台真空吸",
                "(B轨)等待位顶板气缸",
            };

            return names
                .Select((n, idx) => new IOOutItemCfg
                {
                    Id = $"DO{idx + 1}",
                    Name = n,
                    IsOn = false,
                    IsEnabled = true
                })
                .ToList();
        }
    }

    public class IOOutItemCfg
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public bool IsOn { get; set; }
        public bool IsEnabled { get; set; } = true;
    }

    public class IOOutItemViewModel : ReactiveObject
    {
        private bool _isOn;
        private bool _isEnabled = true;
        private bool _isSelected;

        public string Id { get; }

        public string Name { get; }

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn == value)
                    return;

                _isOn = value;
                this.RaisePropertyChanged(nameof(IsOn));
                AfterToggled?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public event EventHandler AfterToggled;

        public IOOutItemViewModel(string id, string name, bool isOn, bool isEnabled)
        {
            Id = id;
            Name = name;
            _isOn = isOn;
            _isEnabled = isEnabled;
        }
    }
}
