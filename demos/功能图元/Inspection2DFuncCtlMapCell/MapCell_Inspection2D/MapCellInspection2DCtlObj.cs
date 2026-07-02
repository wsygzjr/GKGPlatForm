using Avalonia.Controls;
using Avalonia.Media;
using GF_Gereric;
using GKG.Map.Inspection2DFuncCtlMapCell.ViewModels;
using GKG.Map.Inspection2DFuncCtlMapCell.Views;
using Griffins;
using Griffins.Map;
using Griffins.Map.UI;
using Griffins.UI2;
using Newtonsoft.JsonG;
using PropertyModels.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GKG.Map.Inspection2DFuncCtlMapCell
{
    /// <summary>
    /// 2D检测图元对象
    /// </summary>
    public class MapCellInspection2DCtlObj : FunctionalCellBase
    {
        #region 字段与属性

        private Inspection2DView view;
        private Inspection2DViewModel viewModel;

        [Browsable(false)] 
        public Inspection2DPropertyModelEdit Inspection2DPropertyModelEdit => (PropertyEditModelBase as Inspection2DPropertyModelEdit)!;

        #endregion

        #region 构造与初始化

        public MapCellInspection2DCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellInspection2DCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();


            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new Inspection2DView();

            (this as IMapCellTypeBase).Name = ResourceA.Inspection2D;

            viewModel = new Inspection2DViewModel();
            view.DataContext = viewModel;
        }

        #endregion

        #region 数据同步与更新

        protected override bool SetUIDataObjPropValues(GFBaseTypePropValueList gFBaseTypePropValues)
        {
            foreach (GFBaseTypePropValue gFBaseTypePropValue in gFBaseTypePropValues)
            {
                if (gFBaseTypePropValue == null)
                    continue;

                string propId = gFBaseTypePropValue.PropertyID.ToString();
                if (string.IsNullOrWhiteSpace(propId))
                    continue;
            }
            return true;
        }

        protected override void AfterPropertyChanged(string propertyID, GriffinsBaseValue propertyValue)
        {
            base.AfterPropertyChanged(propertyID, propertyValue);

            if (string.Compare(propertyID, nameof(Inspection2DPropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(Inspection2DPropertyModelEdit.TextColor));
            }
            if (string.Compare(propertyID, nameof(Inspection2DPropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(Inspection2DPropertyModelEdit.BackColor));
            }
            if (string.Compare(propertyID, nameof(Inspection2DPropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(Inspection2DPropertyModelEdit.TextFont));
            }

            if (!Inspection2DPropertyModelEdit.IsRuning)
            {
                //CallBack?.UpdateUIDataObjPropValues(BuildUIDataObjPropValues());
            }
        }

        public override GriffinsBaseValue ConvertPropertyValue(string propertyID, object propertyValue)
        {
            return null!;
        }

        #endregion

        #region 操作原子定义与执行

        protected override bool ExecOprtCell(MapOprtCellInstInfo mapOprtCellInstInfo)
        {
            #region 处理基础属性操作
            if (mapOprtCellInstInfo.OprtCellID == Inspection2DMapOprtCellConst.TextColor_MapOprtCellID)
            {
                if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out var exector))
                {
                    exector = new TextColorMapOprtCellExector();
                    exector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, exector);
                }
                exector.Exec(mapOprtCellInstInfo.CfgInfo);
                return true;
            }

            if (mapOprtCellInstInfo.OprtCellID == Inspection2DMapOprtCellConst.BackColor_MapOprtCellID)
            {
                if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out var exector))
                {
                    exector = new BackColorMapOprtCellExector();
                    exector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, exector);
                }
                exector.Exec(mapOprtCellInstInfo.CfgInfo);
                return true;
            }

            if (mapOprtCellInstInfo.OprtCellID == Inspection2DMapOprtCellConst.TextFont_MapOprtCellID)
            {
                if (!MapOprtCellExectorDict.TryGetValue(mapOprtCellInstInfo.InstanceID, out var exector))
                {
                    exector = new TextFontMapOprtCellExector();
                    exector.Init(IMapOprtCellExectorCallBack);
                    MapOprtCellExectorDict.TryAdd(mapOprtCellInstInfo.InstanceID, exector);
                }
                exector.Exec(mapOprtCellInstInfo.CfgInfo);
                return true;
            }

            #endregion

            return base.ExecOprtCell(mapOprtCellInstInfo);
        }

        /// <summary>
        /// 文本颜色操作原子执行对象
        /// </summary>
        private class TextColorMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is Inspection2DViewModel vm)
                {
                    GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(Inspection2DPropertyModelEdit.TextColor));
                    if (val != null)
                    {
                        //var color = val.ToPrimitiveValue<string>();
                        //vm.TextColor = Color.Parse(color);
                    }
                }
            }
        }

        /// <summary>
        /// 背景颜色操作原子执行对象
        /// </summary>
        private class BackColorMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is Inspection2DViewModel vm)
                {
                    GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(Inspection2DPropertyModelEdit.BackColor));
                    if (val != null)
                    {
                        //var color = val.ToPrimitiveValue<string>();
                        //vm.BackColor = Color.Parse(color);
                    }
                }
            }
        }

        /// <summary>
        /// 文本字体操作原子执行对象
        /// </summary>
        private class TextFontMapOprtCellExector : IMapOprtCellExector
        {
            private IMapOprtCellExectorCallBack callBack;

            void IMapOprtCellExector.Init(IMapOprtCellExectorCallBack callBack)
            {
                this.callBack = callBack;
            }

            void IMapOprtCellExector.Exec(byte[] cfg)
            {
                if (callBack.GetMapCellVMObjInstance() is Inspection2DViewModel vm)
                {
                    GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(Inspection2DPropertyModelEdit.TextFont));
                    if (val != null)
                    {
                        //ObjectValue_Json objectValue_Json = val.ToObjectValue_Json();
                        //GriffinsBaseValue griffinsBaseValue = GriffinsBaseValue.Create(objectValue_Json);
                        //IGriffinsBaseValue iMPPropObjectValue = new FontInfo();
                        //iMPPropObjectValue.PopulateFromBaseValue(griffinsBaseValue);
                        //vm.TextFont = (FontInfo)iMPPropObjectValue;
                    }
                }
            }
        }

        #endregion

        #region 生命周期与数据持久化

        protected override void OnInit() => base.OnInit();

        public override void OnDispose()
        {
            view.DataContext = null;
            viewModel?.Dispose();
            base.OnDispose();
        }

        protected override void OnReadDrawInfoFromBytes(GF_Gereric.GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
            var propertyEditModelBase = JsonObjConvert.FromJSon<Inspection2DPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as Inspection2DPropertyModelEdit)!.CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<Inspection2DPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as Inspection2DPropertyBindEditModel)!.CopyFrom(propertyBindEditModelBase);
            var eventBindEditModel = System.Text.Json.JsonSerializer.Deserialize<EventBindEditModel>(br.ReadString("EventBindEditModel"));
            EventBindEditModel.CopyFrom(eventBindEditModel!);
        }

        protected override void OnWriteDrawInfoToBytes(GF_Gereric.GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
            bw.Write("PropertyEditModelBase", JsonObjConvert.ToJSon(PropertyEditModelBase));
            bw.Write("PropertyBindEditModelBase", JsonObjConvert.ToJSon(PropertyBindEditModelBase));
            bw.Write("EventBindEditModel", System.Text.Json.JsonSerializer.Serialize(EventBindEditModel));
        }

        protected override void OnCopyFrom(FunctionalCellBase source)
        {
            var src = source as MapCellInspection2DCtlObj;
            base._CopyFrom(src);
            PropertyEditModelBase.CopyFrom(source.PropertyEditModelBase);
            PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel?.CopyFrom(source.EventBindEditModel);
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => viewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new Inspection2DPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new Inspection2DPropertyBindEditModel();

        public override EventBindEditModel CreateEventBindEditModel()
        {
            return new EventBindEditModel()
            {
                EventCmdInfos = new BindingList<EventCmdInfo>()
                {
                    new EventCmdInfo() { },
                }
            };
        }

        public override string ToString() => ResourceA.Inspection2D;

        #endregion

    }

    /// <summary>
    /// 2D检测图元属性模型
    /// </summary>
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("反馈信息", 2)]
    public class Inspection2DPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        #region 构造函数
        /// <summary>
        /// 初始化2D检测属性模型编辑
        /// </summary>
        public Inspection2DPropertyModelEdit()
        {
            TextFont.PropertyChanged += textFont_PropertyChanged;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 是否正在设置反馈信息标志
        /// </summary>
        internal bool _isSettingFeedbackInfo = false;

        /// <summary>
        /// 文体属性变更事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">属性变更事件参数</param>
        private void textFont_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TextFont));
        }

        /// <summary>
        /// 反馈信息属性变更事件处理
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">属性变更事件参数</param>
        private void feedbackInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isSettingFeedbackInfo)
                return;

            // 防止Items变化时重复触发同步 - 使用更精确的过滤
            if (e?.PropertyName == "Items")
                return;

            if (!string.IsNullOrWhiteSpace(e?.PropertyName))
                RaisePropertyChanged(e.PropertyName);
        }

        #endregion

        #region 属性

        private FontInfo _textFont = new FontInfo(FontInfo.DefaultFont.FontFamily, 16, FontInfo.DefaultFont.FontWeight, FontInfo.DefaultFont.FontStyle);
        [DisplayName("文字字体")]
        [Category("图元信息")]
        [PropertySortOrder(8)]
        public FontInfo TextFont
        {
            get { return _textFont; }
            set { SetProperty(ref _textFont, value); }
        }

        private Color _textColor = Colors.Black;
        [DisplayName("文本颜色")]
        [Category("图元信息")]
        [PropertySortOrder(9)]
        [JsonConverter(typeof(ColorConvert))]
        public Color TextColor
        {
            get { return _textColor; }
            set { SetProperty(ref _textColor, value); }
        }

        private Color _backColor = Colors.White;
        [DisplayName("背景颜色")]
        [Category("图元信息")]
        [PropertySortOrder(10)]
        [JsonConverter(typeof(ColorConvert))]
        public Color BackColor
        {
            get { return _backColor; }
            set { SetProperty(ref _backColor, value); }
        }

        /// <summary>
        /// 从其他实例复制属性值
        /// </summary>
        /// <param name="source">源实例</param>
        public void CopyFrom(PropertyEditModelBase source)
        {
            if (source is Inspection2DPropertyModelEdit sourceModel)
            {
                BackColor = sourceModel.BackColor;
                TextColor = sourceModel.TextColor;
                TextFont = sourceModel.TextFont;
            }
        }

        #endregion
    }

    /// <summary>
    /// 2D检测图元属性绑定模型
    /// </summary>
    [MapPropertyOrder]
    [CategoryPriority("点位信息", 1)]
    [CategoryPriority("绑定信息", 2)]
    public class Inspection2DPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        public void CopyFrom(Inspection2DPropertyBindEditModel source)
        {
            base.CopyFrom(source);
           
        }

    }

    /// <summary>
    /// 2D检测历史记录项
    /// </summary>
    [Serializable]
    public class Inspection2DHistoryItem

    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string DetectionType { get; set; }
        public bool IsSuccess { get; set; }
        public string AdditionalInfo { get; set; }

        public Inspection2DHistoryItem()
        {
            Timestamp = DateTime.Now;
            Message = "";
            DetectionType = "未知";
            IsSuccess = true;
            AdditionalInfo = "";
        }

        public Inspection2DHistoryItem(string message, string detectionType = "2D检测", bool isSuccess = true, string additionalInfo = "")
        {
            Timestamp = DateTime.Now;
            Message = message;
            DetectionType = detectionType;
            IsSuccess = isSuccess;
            AdditionalInfo = additionalInfo;
        }
    }

    
}
