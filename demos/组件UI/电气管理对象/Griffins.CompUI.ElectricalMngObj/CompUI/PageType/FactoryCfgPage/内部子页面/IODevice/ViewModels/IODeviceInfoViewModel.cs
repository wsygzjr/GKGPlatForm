using ReactiveUI;
using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using System.Collections.Generic;
using System.Linq;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// IO设备信息模型
    /// </summary>
    public class IODeviceInfoViewModel : ReactiveObject
    {
        private string _serialNumber="";

        /// <summary>
        /// IO设备型号-下拉框数据模型
        /// </summary>
        public ComboxViewModel IODeviceModelModel { get; }
        /// <summary>
        /// IO设备ID-下拉框数据模型
        /// </summary>
        public ComboxViewModel IODeviceIDModel { get; }

        /// <summary>
        /// 选中的IO设备型号
        /// </summary>
        public string SelectedIODeviceModel
        {
          
            get => (string)((IODeviceModelModel.SelectedItem as ComBoxItem)?.Value ?? "");
            set
            {
                if (IODeviceModelModel.ItemsSource != null)
                {
                    var targetItem = IODeviceModelModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        IODeviceModelModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedIODeviceModel));
                }
            }
        }


        /// <summary>
        /// 选中的IO设备ID
        /// </summary>
        public string SelectedIODeviceID
        {
            get => (string)((IODeviceIDModel.SelectedItem as ComBoxItem)?.Value ?? "");
            set
            {
                if (IODeviceIDModel.ItemsSource != null)
                {
                    var targetItem = IODeviceIDModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        IODeviceIDModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedIODeviceID));
                }
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public string SerialNumber
        {
            get => _serialNumber;
            set => this.RaiseAndSetIfChanged(ref _serialNumber, value); 
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IODeviceInfoViewModel()
        {
            IODeviceModelModel = new ComboxViewModel();
            List<ComBoxItem> cardTypeItems = new List<ComBoxItem>();
            foreach (var item in getIODeviceModelSource())
            {
                cardTypeItems.Add(new ComBoxItem()
                {
                    Value = item.Value,
                    DisplayName = item.DispName
                });
            }
            IODeviceModelModel.ItemsSource = cardTypeItems;
            IODeviceModelModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            IODeviceModelModel.ValueChanged += iODeviceModelModel_ValueChanged;

            IODeviceIDModel = new ComboxViewModel();
            List<ComBoxItem> cardidItems = new List<ComBoxItem>();
            foreach (var item in getIODeviceIDSource())
            {
                cardidItems.Add(new ComBoxItem()
                {
                    Value = item.Value,
                    DisplayName = item.DispName
                });
            }
            IODeviceIDModel.ItemsSource = cardidItems;
            IODeviceIDModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            IODeviceIDModel.ValueChanged += iODeviceIDModel_ValueChanged;
        }

        private void iODeviceIDModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            //通知UI刷新,保存按钮是否可用的绑定
            this.RaisePropertyChanged(nameof(SelectedIODeviceID));
        }

        private void iODeviceModelModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            //通知UI刷新
            this.RaisePropertyChanged(nameof(SelectedIODeviceModel));
        }

        public void CopyFrom(IODeviceCfgInfo iODeviceCfgInfo)
        {
            if (iODeviceCfgInfo == null)
                return;
            this.SelectedIODeviceModel = iODeviceCfgInfo.IODeviceModel;
            this.SelectedIODeviceID = iODeviceCfgInfo.IODeviceID;
            this.SerialNumber = iODeviceCfgInfo.SerialNumber;
        }
        public void CopyFrom(IODeviceInfoViewModel iODeviceInfoModel)
        {
            if (iODeviceInfoModel == null)
                return;
            this.SelectedIODeviceModel = iODeviceInfoModel.SelectedIODeviceModel;
            this.SelectedIODeviceID = iODeviceInfoModel.SelectedIODeviceID;
            this.SerialNumber = iODeviceInfoModel.SerialNumber;
        }
        public void CopyTo(IODeviceCfgInfo targetInfo)
        {
            if (targetInfo == null) return;

            targetInfo.IODeviceModel = this.SelectedIODeviceModel;
            targetInfo.IODeviceID = this.SelectedIODeviceID;
            targetInfo.SerialNumber = this.SerialNumber;
        }
        private List<IODeviceModelInfo> getIODeviceModelSource()
        {
            List<IODeviceModelInfo> iODeviceModels = new List<IODeviceModelInfo>();
            iODeviceModels.Add(new IODeviceModelInfo()
            {
                DispName = "雷赛",
                Value = "LeiSai"
            });
            iODeviceModels.Add(new IODeviceModelInfo()
            {
                DispName = "高川",
                Value = "GaoChuan"
            });
            return iODeviceModels;
        }
        private List<IODeviceModelInfo> getIODeviceIDSource()
        {
            List<IODeviceModelInfo> iODeviceIDs = new List<IODeviceModelInfo>();
            iODeviceIDs.Add(new IODeviceModelInfo()
            {
                DispName = "型号1",
                Value = "1000B"
            });
            iODeviceIDs.Add(new IODeviceModelInfo()
            {
                DispName = "型号2",
                Value = "1000C"
            });
            return iODeviceIDs;
        }
        /// <summary>
        /// 
        /// </summary>
        public class IODeviceModelInfo
        {
            /// <summary>
            /// 值
            /// </summary>
            public string Value { set; get; } = "";
            /// <summary>
            /// 显示名称
            /// </summary>
            public string DispName { set; get; } = "";
        }
        
    }
}
