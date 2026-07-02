using Griffins.UI;
using Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.Models;
using ReactiveUI;
using System.Reactive.Linq;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Griffins.CompUI.ElectricalMngObj.FactoryCfgPage.ViewModels
{
    /// <summary>
    /// 运控卡信息模型
    /// </summary>
    public class ControlCardViewModel : ReactiveObject
    {

        //用于跟踪已存在的CadID，避免重复（实际项目中可能从外部数据源获取）
        private HashSet<string> _existingCadIds = new HashSet<string>();

        //当前生成序号的计数器
        private int _currentSequence = 1;

        /// <summary>
        /// 控制卡唯一ID
        /// </summary>
        public Guid UniqueID { set; get; }
        /// <summary>
        /// 控制卡种类-下拉框数据模型
        /// </summary>
        public ComboxViewModel ControlCardKindModel { get; }
        /// <summary>
        /// 控制卡类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel ControlCardTypeModel { get; }
        /// <summary>
        /// 卡ID-数据模型
        /// </summary>
        public TextInputViewModel CadIDViewModel { get; }
        /// <summary>
        /// 序号-数据模型
        /// </summary>
        public NumericViewModel SerialNumberViewModel { get; }
        /// <summary>
        /// 轴数量-数据模型
        /// </summary>
        public NumericViewModel AxisCountViewModel { get; }
        /// <summary>
        /// 模拟量通道数量-数据模型
        /// </summary>
        public NumericViewModel AnalogChannelCountViewModel { get; }
        /// <summary>
        /// 状态量通道数量-数据模型
        /// </summary>
        public NumericViewModel StateChannelCountViewModel { get; }
        /// <summary>
        /// 选中的控制卡种类
        /// </summary>
        public string SelectedControlCardKind
        {
          
            get => (string)((ControlCardKindModel.SelectedItem as ComBoxItem)?.Value ?? "");
            set
            {
                if (ControlCardKindModel.ItemsSource != null)
                {
                    var targetItem = ControlCardKindModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        ControlCardKindModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedControlCardKind));
                }
            }
        }

        /// <summary>
        /// 选中的控制卡类型
        /// </summary>
        public string SelectedControlCardType
        {
          
            get => (string)((ControlCardTypeModel.SelectedItem as ComBoxItem)?.Value ?? "");
            set
            {
                if (ControlCardTypeModel.ItemsSource != null)
                {
                    var targetItem = ControlCardTypeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (string)o.Value == value);
                    if (targetItem != null)
                        ControlCardTypeModel.SelectedItem = targetItem;
                    this.RaisePropertyChanged(nameof(SelectedControlCardType));
                }
            }
        }
       
        /// <summary>
        /// 控制卡ID
        /// </summary>
        public string CadID
        {
            get=> CadIDViewModel.Text;
            set
            {
                if (CadIDViewModel.Text == value) return;
                CadIDViewModel.Text = value;
                this.RaisePropertyChanged(nameof(CadID));

            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber
        {
            get=> (int)SerialNumberViewModel.Value;
            set=> SerialNumberViewModel.Value = value;
        }


        /// <summary>
        /// 轴数量
        /// </summary>
      
        public int AxisCount
        {
            get=> (int)AxisCountViewModel.Value;
            set => AxisCountViewModel.Value = value;
        }

        /// <summary>
        /// 模拟量通道数量
        /// </summary>
        public int AnalogChannelCount
        {
            get => (int)AnalogChannelCountViewModel.Value;
            set => AnalogChannelCountViewModel.Value = value;
        }

        /// <summary>
        /// 状态量通道数量
        /// </summary>
        public int StateChannelCount
        {
            get => (int)StateChannelCountViewModel.Value;
            set => StateChannelCountViewModel.Value = value;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ControlCardViewModel()
        {
            UniqueID = Guid.NewGuid();
            SerialNumberViewModel = new NumericViewModel();
            CadIDViewModel = new TextInputViewModel();
            CadIDViewModel.ValueChanged += CadIDViewModel_ValueChanged;
            AxisCountViewModel =new NumericViewModel();
            AxisCountViewModel.IsEnabled = false;

            AnalogChannelCountViewModel = new NumericViewModel();
            AnalogChannelCountViewModel.IsEnabled = false;

            StateChannelCountViewModel = new NumericViewModel();
            StateChannelCountViewModel.IsEnabled = false;

            ControlCardKindModel = new ComboxViewModel();
            ControlCardKindModel.IsEnabled = true;
            List<ComBoxItem> comBoxItems = new List<ComBoxItem>();
            foreach (var item in getControlCardKindSource())
            {
                comBoxItems.Add(new ComBoxItem()
                {
                    Value = item.CardKind,
                    DisplayName = item.CardKindName
                });
            }
            ControlCardKindModel.ItemsSource = comBoxItems;
            ControlCardKindModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ControlCardKindModel.ValueChanged += controlCardKindModel_ValueChanged; ;

            ControlCardTypeModel = new ComboxViewModel();
            ControlCardTypeModel.IsEnabled = true;
            
            ControlCardTypeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            ControlCardTypeModel.ValueChanged += controlCardTypeModel_ValueChanged;
        }

        private void CadIDViewModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            //通知UI更新
            this.RaisePropertyChanged(nameof(CadID));
        }

        /// <summary>
        /// 运控卡种类选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controlCardKindModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(SelectedControlCardKind));
            rereshControlCardTypeItemsSource(SelectedControlCardKind);
        }

        /// <summary>
        /// 运控卡类型选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void controlCardTypeModel_ValueChanged(object? sender, ValueChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(SelectedControlCardType));
            autoGenerateCadID(SelectedControlCardKind, SelectedControlCardType, CadID);
        }
        /// <summary>
        /// 更新运控卡型号选择源
        /// </summary>
        /// <param name="controlCardKind"></param>
        private void rereshControlCardTypeItemsSource(string controlCardKind)
        {
            List<ComBoxItem> cardTypeItems = new List<ComBoxItem>();
            foreach (var item in getControlCardTypeSource(controlCardKind))
            {
                cardTypeItems.Add(new ComBoxItem()
                {
                    Value = item.CardType,
                    DisplayName = item.CardTypeName
                });
            }
            ControlCardTypeModel.ItemsSource = cardTypeItems;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlCardInfo"></param>
        public void CopyFrom(ControlCardInfo controlCardInfo)
        {
            if (controlCardInfo == null)
                return;
            this.UniqueID = controlCardInfo.UniqueID;
            this.SelectedControlCardKind = controlCardInfo.ControlCardKind;
            this.SelectedControlCardType = controlCardInfo.ControlCardType;
            this.CadID = controlCardInfo.CadID;
            this.SerialNumber = controlCardInfo.SerialNumber;
            //this.AxisCount = controlCardInfo.AxisCount;
            //this.AnalogChannelCount = controlCardInfo.AnalogChannelCount;
            //this.StateChannelCount = controlCardInfo.StateChannelCount;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlCardModel"></param>
        public void CopyFrom(ControlCardViewModel controlCardModel)
        {
            if (controlCardModel == null)
                return;
            this.UniqueID = controlCardModel.UniqueID;
            this.SelectedControlCardKind = controlCardModel.SelectedControlCardKind;
            this.SelectedControlCardType = controlCardModel.SelectedControlCardType;
            this.CadID = controlCardModel.CadID;
            this.SerialNumber = controlCardModel.SerialNumber;
            this.AxisCount = controlCardModel.AxisCount;
            this.AnalogChannelCount = controlCardModel.AnalogChannelCount;
            this.StateChannelCount = controlCardModel.StateChannelCount;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetInfo"></param>
        public void CopyTo(ControlCardInfo targetInfo)
        {
            if (targetInfo == null) return;

            targetInfo.UniqueID = this.UniqueID;
            targetInfo.ControlCardKind = this.SelectedControlCardKind;
            targetInfo.ControlCardType = this.SelectedControlCardType;
            targetInfo.SerialNumber = this.SerialNumber;
            targetInfo.AnalogChannelCount = this.AnalogChannelCount;
            targetInfo.StateChannelCount = this.StateChannelCount;

            if (targetInfo.ControlCardBaseCfg == null)
            {
                targetInfo.ControlCardBaseCfg = new ControlCardBaseCfg();
            }
        }
        /// <summary>
        /// 设置已存在的卡ID
        /// </summary>
        /// <param name="existingCadIds"></param>
        public void SetExistingCadIds(List<string> existingCadIds)
        {
            _existingCadIds=existingCadIds.ToHashSet();
        }

        #region 运控卡种类和类型
        /// <summary>
        /// 模拟数据,后期从电气绑定对象中获取（模拟）
        /// </summary>
        /// <returns></returns>
        private List<ControlCardKindInfo> getControlCardKindSource()
        {
            //运控卡种类和型号决定了轴数量和通道数量先模拟
            this.AxisCount = 10;
            this.AnalogChannelCount = 15;
            this.StateChannelCount = 15;

            List<ControlCardKindInfo> controlCardKinds = new List<ControlCardKindInfo>();
            controlCardKinds.Add(new ControlCardKindInfo()
            {
                CardKindName = "运动控制卡基础",
                CardKind = "MotionControlBase"
            });
            controlCardKinds.Add(new ControlCardKindInfo()
            {
                CardKindName = "A类运动控制卡",
                CardKind = "MotionControlCategoryA"
            });
            return controlCardKinds;
        }
        /// <summary>
        /// 获取运控卡类型选择源（模拟）
        /// </summary>
        /// <param name="controlCardKind"></param>
        /// <returns></returns>
        private List<ControlCardTypeInfo> getControlCardTypeSource(string controlCardKind)
        {
            List<ControlCardTypeInfo> controlCardTypes = new List<ControlCardTypeInfo>();
            switch (controlCardKind)
            {
                case "MotionControlBase":
                    controlCardTypes.Add(new ControlCardTypeInfo()
                    {
                        CardTypeName = "雷赛1000B",
                        CardType = "1000B"
                    });
                    break;
                case "MotionControlCategoryA":
                    controlCardTypes.Add(new ControlCardTypeInfo()
                    {
                        CardTypeName = "高川1000C",
                        CardType = "1000C"
                    });
                    break;
                default:
                    break;
            }
            return controlCardTypes;
        }
        /// <summary>
        /// 自动生成CadID（当CadID为空且种类/类型变化时）
        /// </summary>
        private void autoGenerateCadID(string kind, string type, string currentCadId)
        {
            if (!string.IsNullOrWhiteSpace(kind)
                && !string.IsNullOrWhiteSpace(type))
            {
                string baseFormat = $"{kind}_{type}_";

                int sequence = _currentSequence;
                while (true)
                {
                    string newCadId = $"{baseFormat}{sequence}";
                    if (!_existingCadIds.Contains(newCadId))
                    {
                        CadID = newCadId;
                        break;
                    }
                    sequence++;
                }
            }
        }
        /// <summary>
        /// 运控卡种类信息
        /// </summary>
        public class ControlCardKindInfo
        {
            /// <summary>
            /// 运控卡种类
            /// </summary>
            public string CardKind { set; get; } = "";
            /// <summary>
            /// 运控卡种类名称
            /// </summary>
            public string CardKindName { set; get; } = "";
        }
        /// <summary>
        /// 运控卡类型信息
        /// </summary>
        public class ControlCardTypeInfo
        {
            /// <summary>
            /// 运控卡种类
            /// </summary>
            public string CardType { set; get; } = "";
            /// <summary>
            /// 运控卡种类名称
            /// </summary>
            public string CardTypeName { set; get; } = "";
        } 
        #endregion
    }

    public enum ControlCardChangedType
    {
        Add = 0,
        Edit,
        Delete,
    }
}
