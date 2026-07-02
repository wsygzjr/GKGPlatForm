using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// 运控卡基础配置
    /// </summary>
    public class ControlCardBaseCfgViewModel : ReactiveObject
    {
        private string _cardNo = "";
        private string _cfgFilePath="";

        /// <summary>
        /// 值改变事件
        /// </summary>
        public EventHandler? AfterModified;
        /// <summary>
        /// 卡号-数据模型
        /// </summary>
        public TextInputViewModel CardNoViewModel { get; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo
        {
            get
            {
                _cardNo = CardNoViewModel.Text;
                return _cardNo;
            }
            set
            {
                _cardNo = value;
                CardNoViewModel.Text = _cardNo;
            }
        }

        /// <summary>
        /// 运控卡配置文件路径-数据模型
        /// </summary>
        public TextInputViewModel CfgFilePathoViewModel { get; }
        /// <summary>
        /// 运控卡配置文件路径
        /// </summary>
        public string CfgFilePath
        {
            get
            {
                _cfgFilePath = CfgFilePathoViewModel.Text;
                return _cfgFilePath;
            }
            set
            {
                _cfgFilePath = value;
                CfgFilePathoViewModel.Text = _cfgFilePath;
            }
        }
       
        /// <summary>
        /// 运控卡轴配置列表
        /// </summary>
        private ObservableCollection<ControlCardAxisCfgViewModel> _controlCardAxisCfgViewModels = new();
        public ObservableCollection<ControlCardAxisCfgViewModel> ControlCardAxisCfgViewModels
        {
            get => _controlCardAxisCfgViewModels;
            set => this.RaiseAndSetIfChanged(ref _controlCardAxisCfgViewModels, value);
        }

        /// <summary>
        /// 当前选中的运控卡轴模型
        /// </summary>
        private ControlCardAxisCfgViewModel? _selectedControlCardAxisCfgViewModel;
        public ControlCardAxisCfgViewModel? SelectedControlCardAxisCfgViewModel
        {
            get => _selectedControlCardAxisCfgViewModel;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedControlCardAxisCfgViewModel, value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardBaseCfgViewModel()
        {
            CardNoViewModel = new TextInputViewModel();
            CfgFilePathoViewModel = new TextInputViewModel();
            ControlCardAxisCfgViewModels = new ObservableCollection<ControlCardAxisCfgViewModel>();
            // 订阅值变更事件
            subscribeValueChanges();
        }

        /// <summary>
        /// 从数据模型复制数据到ViewModel
        /// </summary>
        /// <param name="controlCardBaseCfg"></param>
        /// <param name="axisCount"></param>
        public void CopyFrom(ControlCardBaseCfg controlCardBaseCfg, int axisCount)
        {
            this.CardNo = controlCardBaseCfg.CardNo;
            this.CfgFilePath = controlCardBaseCfg.CfgFilePath;

            ControlCardAxisCfgViewModels.Clear();
            //如果配置中的为空则新增
            if (controlCardBaseCfg.ControlCardAxisCfgInfoes.Count==0)
            {
                for (int index = 0; index < axisCount; index++)
                {
                    controlCardBaseCfg.ControlCardAxisCfgInfoes.Add(new ControlCardAxisCfgInfo());
                }
            }
            int axisNoIndex = 0;
            foreach (var controlCardAxisCfgInfo in controlCardBaseCfg.ControlCardAxisCfgInfoes)
            {
                axisNoIndex++;
                controlCardAxisCfgInfo.AxisNo=axisNoIndex;
                controlCardAxisCfgInfo.AxisName="轴"+ axisNoIndex;
                var controlCardAxisCfgViewModel = new ControlCardAxisCfgViewModel();
                controlCardAxisCfgViewModel.AfterModified += onAfterModified;
                controlCardAxisCfgViewModel.CopyFrom(controlCardAxisCfgInfo);
                ControlCardAxisCfgViewModels.Add(controlCardAxisCfgViewModel);
            }
        }

        public void UpdateAaxisCount( int axisCount)
        {
            if(axisCount>ControlCardAxisCfgViewModels.Count)
            {
                int axisNoIndex = 0;
                for (int index = 0; index < axisCount- ControlCardAxisCfgViewModels.Count; index++)
                {
                    var controlCardAxisCfgInfo = new ControlCardAxisCfgInfo();
                    controlCardAxisCfgInfo.AxisNo = axisNoIndex;
                    controlCardAxisCfgInfo.AxisName = "轴" + axisNoIndex;
                    var controlCardAxisCfgViewModel = new ControlCardAxisCfgViewModel();
                    controlCardAxisCfgViewModel.AfterModified += onAfterModified;
                    controlCardAxisCfgViewModel.CopyFrom(controlCardAxisCfgInfo);
                    ControlCardAxisCfgViewModels.Add(controlCardAxisCfgViewModel);
                    axisNoIndex++;
                }
            }
            else if (axisCount < ControlCardAxisCfgViewModels.Count)
            {
                for (int index = ControlCardAxisCfgViewModels.Count-1; index >= axisCount; index--)
                {
                    ControlCardAxisCfgViewModels.RemoveAt(index);
                }
            }

        }
        /// <summary>
        /// 提取参数
        /// </summary>
        /// <param name="controlCardBaseCfg"></param>
        public void CopyTo(ControlCardBaseCfg controlCardBaseCfg)
        {
            controlCardBaseCfg.CardNo = this.CardNo;
            controlCardBaseCfg.CfgFilePath = this.CfgFilePath;
            controlCardBaseCfg.ControlCardAxisCfgInfoes.Clear();
            foreach (var controlCardAxisCfgViewModel in this.ControlCardAxisCfgViewModels)
            {
                var controlCardAxisCfgInfo = new ControlCardAxisCfgInfo();
                controlCardAxisCfgViewModel.CopyTo(controlCardAxisCfgInfo);
                controlCardBaseCfg.ControlCardAxisCfgInfoes.Add(controlCardAxisCfgInfo);
            }
        }
        #region 值改变事件
        /// <summary>
        /// 订阅值改变事件
        /// </summary>
        private void subscribeValueChanges()
        {
            CardNoViewModel.ValueChanged += onValueChanged;
            CfgFilePathoViewModel.ValueChanged += onValueChanged;
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