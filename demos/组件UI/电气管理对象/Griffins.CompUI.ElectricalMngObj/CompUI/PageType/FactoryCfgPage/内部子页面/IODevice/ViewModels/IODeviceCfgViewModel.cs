using Avalonia.Controls;
using GF_Gereric;
using Griffins.UI;
using Griffins.Map.UI;
using ReactiveUI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using System.Collections.Generic;
using System;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// IO设备配置的视图模型
    /// </summary>
    public class IODeviceCfgViewModel : ReactiveObject
    {
        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        private byte[]? _cfgInfo;
        /// <summary>
        /// 页面布局对象
        /// </summary>
        private Control? _viewReference;
        private List<IODeviceCfgInfo> _iODeviceCfgInfos;
        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// IO设备信息列表-视图模型
        /// </summary>
        public IODeviceInfoListViewModel IODeviceInfoListViewModel { get; }

        /// <summary>
        /// 内部子页面配置信息
        /// </summary>
        public byte[]? CfgInfo
        {
            get
            {
                extract(_iODeviceCfgInfos);
                _cfgInfo = JsonObjConvert.ToJSonBytes(_iODeviceCfgInfos);
                return _cfgInfo;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IODeviceCfgViewModel()
        {
            _iODeviceCfgInfos=new List<IODeviceCfgInfo>();
            IODeviceInfoListViewModel = new IODeviceInfoListViewModel();
            // 订阅值变更事件
            subscribeValueChanges();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cfgInfo">内部子页面配置信息</param>
        public void Init( byte[]? cfgInfo)
        {
            _cfgInfo = cfgInfo;
            loadCfgInfo(cfgInfo);
        }

        /// <summary>
        /// 设置视图引用
        /// </summary>
        public void SetViewReference(Control view)
        {
            _viewReference = view ?? throw new Exception("视图引用不能为空");
            IODeviceInfoListViewModel.SetViewReference(view);
        }


        private void extract(List<IODeviceCfgInfo> iODeviceCfgInfos)
        {
            IODeviceInfoListViewModel.CopyTo(iODeviceCfgInfos);
        }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <param name="cfgInfo"></param>
        private void loadCfgInfo(byte[]? cfgInfo)
        {
            if (cfgInfo != null && cfgInfo.Length > 0)
            {
                try
                {
                    _iODeviceCfgInfos = JsonObjConvert.FromJSonBytes<List<IODeviceCfgInfo>>(cfgInfo);
                }
                catch
                {
                    _iODeviceCfgInfos = new List<IODeviceCfgInfo>();
                }
            }

            IODeviceInfoListViewModel.CopyFrom(_iODeviceCfgInfos);
            
        }

        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            IODeviceInfoListViewModel.AfterModified += onAfterModified;
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