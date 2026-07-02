using Avalonia.Controls;
using Avalonia.VisualTree;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Views;
using Griffins.UI;
using Griffins.UI.General;
using MsBox.Avalonia.Enums;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// IO设备信息列表-视图模型
    /// </summary>
    public class IODeviceInfoListViewModel : ReactiveObject
    {
        private ObservableCollection<IODeviceInfoViewModel> _iODeviceInfoList;
        private Control? _viewReference;
        private bool _canAdd;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// IO设备信息列表
        /// </summary>
        public ObservableCollection<IODeviceInfoViewModel> IODeviceInfoList
        {
            get => _iODeviceInfoList;
            set => this.RaiseAndSetIfChanged(ref _iODeviceInfoList, value);
        } 

        /// <summary>
        /// 当前选中的运控卡模型（绑定到DataGrid的SelectedItem）
        /// </summary>
        private IODeviceInfoViewModel? _selectedIODevice;
        public IODeviceInfoViewModel? SelectedIODevice
        {
            get => _selectedIODevice;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedIODevice, value);
                //onSelectedItemChanged(value);
            }
        }

        /// <summary>
        /// 是否支持增加IO设备信息
        /// </summary>
        public bool CanAdd
        {
            get => _canAdd;
            set => this.RaiseAndSetIfChanged(ref _canAdd, value);
        }


        /// <summary>
        /// 添加IO设备信息
        /// </summary>
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        /// <summary>
        /// 修改IO设备信息
        /// </summary>
        public ReactiveCommand<IODeviceInfoViewModel, Unit> EditCommand { get; }
        /// <summary>
        /// 删除IO设备信息
        /// </summary>
        public ReactiveCommand<IODeviceInfoViewModel, Unit> DeleteCommand { get; }

        /// <summary>
        /// 设置视图引用（用于文件选择器/消息框等UI操作）
        /// </summary>
        public void SetViewReference(Control view) => _viewReference = view;

        public IODeviceInfoListViewModel()
        {
            // 初始化集合
            _iODeviceInfoList = new ObservableCollection<IODeviceInfoViewModel>();

            // 初始化命令
            AddCommand = ReactiveCommand.CreateFromTask(addIODeviceInfo);
            EditCommand = ReactiveCommand.CreateFromTask<IODeviceInfoViewModel>(editIODeviceInfo);
            DeleteCommand = ReactiveCommand.CreateFromTask<IODeviceInfoViewModel>(deleteIODeviceInfo);
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="iODeviceCfgInfoes">IO设备信息信息</param>
        public void CopyFrom(List<IODeviceCfgInfo> iODeviceCfgInfoes)
        {
            IODeviceInfoList.Clear();
            foreach (var item in iODeviceCfgInfoes)
            {
                var data = new IODeviceInfoViewModel();
                data.CopyFrom(item);
                IODeviceInfoList.Add(data);
            }
        }
        /// <summary>
        /// 从ViewModel复制数据到数据模型
        /// </summary>
        /// <param name="iODeviceInfoModeles"></param>
        public void CopyTo(List<IODeviceCfgInfo> iODeviceInfoModeles)
        {
            if (iODeviceInfoModeles == null) return;

            iODeviceInfoModeles.Clear();
            foreach (var iODeviceInfo in IODeviceInfoList)
            {
                var data = new IODeviceCfgInfo();
                iODeviceInfo.CopyTo(data);
                iODeviceInfoModeles.Add(data);
            }
        }
        
        /// <summary>
        /// 添加IO设备信息（打开编辑窗口，创建新页面）
        /// </summary>
        private async Task addIODeviceInfo()
        {
            var iODeviceInfoModel = new IODeviceInfoViewModel
            {
            };

            var result = await openEditWindow(iODeviceInfoModel, isNew: true);
            if (result)
            {
               IODeviceInfoList.Add(iODeviceInfoModel);
                onAfterModified(this, new EventArgs());
            }
        }

        /// <summary>
        /// 编辑IO设备信息
        /// </summary>
        /// <param name="iodeviceinfomodel"></param>
        private async Task editIODeviceInfo(IODeviceInfoViewModel? iodeviceinfomodel)
        {
            if (iodeviceinfomodel != null)
            {
                var result = await openEditWindow(iodeviceinfomodel, isNew: false);
                if (result)
                {
                    onAfterModified(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// 删除IO设备信息
        /// </summary>
        /// <param name="iodeviceinfomodel"></param>
        private async Task deleteIODeviceInfo(IODeviceInfoViewModel? iodeviceinfomodel)
        {
            if (iodeviceinfomodel == null) return;

            var result = await MessageBox.ShowConfirmDialog(
                  "删除确认",
                  $"确定要删除IO设备信息吗？\n删除后不可恢复。",
                  _viewReference
              );

            if (result != ButtonResult.Yes) return;

            IODeviceInfoList.Remove(iodeviceinfomodel);

            onAfterModified(this, new EventArgs());
        }

        /// <summary>
        /// 打开编辑窗口
        /// </summary>
        /// <param name="iodeviceinfomodel"></param>
        /// <param name="isNew"></param>
        private async Task<bool> openEditWindow(IODeviceInfoViewModel iodeviceinfomodel, bool isNew)
        {
            var parentWindow = _viewReference?.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                await MessageBox.ShowErrorDialog("错误", "无法获取窗口上下文，操作失败", null);
                return false;
            }

            var editViewModel = new IODeviceEditViewModel(iodeviceinfomodel, isNew);
            var editWindow = new IODeviceEditWindow
            {
                DataContext = editViewModel,
                Title = isNew ? "添加IO设备信息" : "编辑IO设备信息",
                Width = 490,
                Height = 210,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            // 异步显示对话框并等待结果
            return await editWindow.ShowDialog<bool>(parentWindow);

        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
           
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onValueChanged(object? sender, ValueChangedEventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        /// <summary>
        /// 值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAfterModified(object? sender, EventArgs e)
        {
            AfterModified?.Invoke(sender, e);
        }
        #endregion
    }
}