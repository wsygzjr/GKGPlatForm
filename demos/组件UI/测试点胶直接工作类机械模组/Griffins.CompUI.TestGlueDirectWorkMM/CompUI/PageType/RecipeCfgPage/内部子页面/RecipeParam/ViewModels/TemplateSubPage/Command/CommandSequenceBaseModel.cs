using Griffins.UI;
using Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.Models.TemplateSubPage.Command;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffins.CompUI.TestGlueDirectWorkMM.RecipeCfgPage.ViewModels.TemplateSubPage.Command
{
    /// <summary>
    /// 指令序列基础配置模型
    /// </summary>
    public class CommandSequenceBaseModel : DataGridItemBaseViewModel<CommandSequenceBaseCfgInfo>
    {
        #region 私有字段
       

        #endregion


        #region 响应式控件ViewModel

        /// <summary>
        /// CommandID-数据模型
        /// </summary>
        public TextBlockViewModel CommandIDViewModel { get; }

        /// <summary>
        /// 是否启用-数据模型
        /// </summary>
        public ToggleSwitchViewModel IsEnableViewModel { get; }

        /// <summary>
        /// 指令序列类型-下拉框数据模型
        /// </summary>
        public ComboxViewModel DispensingCommandTypeModel { get; }

        /// <summary>
        /// 描述信息-数据模型
        /// </summary>
        public TextBlockViewModel DescriptionViewModel { get; }
        #endregion

        #region 属性

        /// <summary>
        /// 指令ID
        /// </summary>
        public Guid CommandID
        {
            get => Guid.Parse(CommandIDViewModel.Text);
            set
            {
                CommandIDViewModel.Text = value.ToString();
                this.RaisePropertyChanged(nameof(CommandID));
            }
        }


        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get => IsEnableViewModel.IsChecked;
            set
            {
                IsEnableViewModel.IsChecked = value;
                this.RaisePropertyChanged(nameof(IsEnable));
            }
        }

        /// <summary>
        /// 指令序列类型
        /// </summary>
        public DispensingCommandType DispensingCommandType
        {
            get => (DispensingCommandType)((DispensingCommandTypeModel.SelectedItem as ComBoxItem)?.Value ?? DispensingCommandType.Clean);
            set
            {
                if (DispensingCommandTypeModel.ItemsSource != null)
                {
                    var targetItem = DispensingCommandTypeModel.ItemsSource.Cast<ComBoxItem>()
                    .FirstOrDefault(o => (DispensingCommandType)o.Value == value);
                    if (targetItem != null)
                        DispensingCommandTypeModel.SelectedItem = targetItem;
                    updateTrajectoryByDispensingCommandType();
                    this.RaisePropertyChanged(nameof(DispensingCommandType));
                }
            }
        }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description
        {
            get => DescriptionViewModel.Text;
            set
            {
                DescriptionViewModel.Text = value;
                this.RaisePropertyChanged(nameof(Description));
            }
        }
        #endregion

        #region 事件与构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommandSequenceBaseModel()
        {
            // 初始化基础控件ViewModel
            CommandIDViewModel = new TextBlockViewModel();
            DescriptionViewModel = new TextBlockViewModel();
            IsEnableViewModel = new ToggleSwitchViewModel { IsChecked = true };

            // 初始化指令类型下拉框
            DispensingCommandTypeModel = new ComboxViewModel();
            var dispensingCommandTypeDisplayNames = new Dictionary<DispensingCommandType, string>
            {
                { DispensingCommandType.Clean, "清洁" },
                { DispensingCommandType.Delay, "延时" },
                { DispensingCommandType.NeedleLift, "抬针" },
                { DispensingCommandType.Coating, "涂覆指令" },
                { DispensingCommandType.Dispensing, "点胶指令" },
                { DispensingCommandType.SubDispensing, "放置模板" },
                { DispensingCommandType.QrCode, "扫码" },
                { DispensingCommandType.BadMark, "BadMark设置" },
                { DispensingCommandType.IO, "IO控制" },
                { DispensingCommandType.TwoD, "2D检测" },
                { DispensingCommandType.MeasurementHeight, "测高指令" },
                { DispensingCommandType.MeasurementThickness, "测厚指令" },

                //待实现命令：界面未设计
                //{ DispensingCommandType.Segment, "分段" },
                //{ DispensingCommandType.EdgeGrasping, "抓边指令" },
                //{ DispensingCommandType.ValveOpening, "开阀指令" },

            };

            DispensingCommandTypeModel.ItemsSource = EnumExtensions.ToEnumItems(dispensingCommandTypeDisplayNames);
            DispensingCommandTypeModel.DisplayMemberPath = nameof(ComBoxItem.DisplayName);
            DispensingCommandTypeModel.IsEnabled = false;
            DispensingCommandTypeModel.ValueChanged += (s, e) => this.RaisePropertyChanged(nameof(DispensingCommandType));
        }
        #endregion

        #region 公共方法


        /// <summary>
        /// 从信息对象复制数据
        /// </summary>
        /// <param name="info"></param>
        public override void CopyFrom(CommandSequenceBaseCfgInfo info)
        {
            if (info == null) return;

            base.CopyBasePropertiesFrom(info);
            CommandID = info.CommandID;
            SerialNumber = info.SerialNumber;
            IsEnable = info.Enable;
            DispensingCommandType = info.DispensingCommandType;
            Description = info.Description;
        }

        /// <summary>
        /// 复制到信息对象
        /// </summary>
        /// <param name="targetInfo"></param>
        public override void CopyTo(CommandSequenceBaseCfgInfo targetInfo)
        {
            if (targetInfo == null) return;

            base.CopyBasePropertiesTo(targetInfo);
            targetInfo.CommandID = CommandID;
            targetInfo.SerialNumber = SerialNumber;
            targetInfo.Enable = IsEnable;
            targetInfo.DispensingCommandType = DispensingCommandType;
            targetInfo.Description = Description;

        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 根据指令类型更新相关控件
        /// </summary>
        protected virtual void updateTrajectoryByDispensingCommandType()
        {
        }

        #endregion
    }
}