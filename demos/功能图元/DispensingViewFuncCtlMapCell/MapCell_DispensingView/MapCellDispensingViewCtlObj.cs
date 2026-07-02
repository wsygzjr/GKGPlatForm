using Avalonia.Controls;
using Avalonia.Media;
using GF_Gereric;
using GKG.Map.DispensingViewFuncCtlMapCell.ViewModels;
using GKG.Map.DispensingViewFuncCtlMapCell.Views;
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

namespace GKG.Map.DispensingViewFuncCtlMapCell
{
    /// <summary>
    /// 2D检测图元对象
    /// </summary>
    public class MapCellDispensingViewCtlObj : FunctionalCellBase
    {
        #region 字段与属性

        private DispensingView view;
        private DispensingViewModel viewModel;

        [Browsable(false)]
        public DispensingViewPropertyModelEdit DispensingViewPropertyModelEdit => (PropertyEditModelBase as DispensingViewPropertyModelEdit)!;

        #endregion

        #region 构造与初始化

        public MapCellDispensingViewCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellDispensingViewCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            PropertyEditModelBase = CreatePropertyModelEditBase();
            PropertyBindEditModelBase = CreatePropertyBindEditModelBase();
            EventBindEditModel = CreateEventBindEditModel();


            base.SetID(mapCellID);
            base.SetName(mapCellName);

            view = new DispensingView();

            (this as IMapCellTypeBase).Name = ResourceA.DispensingView;

            viewModel = new DispensingViewModel();
            view.DataContext = viewModel;
        }

        #endregion

        #region 数据同步与更新

        public override bool SetPropertyValue(string propertyID, GriffinsBaseValue propertyVal, bool isRuning)
        {
            DispensingViewPropertyModelEdit.IsRuning = isRuning;
            return DispensingViewPropertyModelEdit.SetPropertyValue(propertyID, propertyVal);
        }

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

            if (string.Compare(propertyID, nameof(DispensingViewPropertyModelEdit.TextColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(DispensingViewPropertyModelEdit.TextColor));
            }
            if (string.Compare(propertyID, nameof(DispensingViewPropertyModelEdit.BackColor)) == 0)
            {
                CallBack?.ExecOprt(nameof(DispensingViewPropertyModelEdit.BackColor));
            }
            if (string.Compare(propertyID, nameof(DispensingViewPropertyModelEdit.TextFont)) == 0)
            {
                CallBack?.ExecOprt(nameof(DispensingViewPropertyModelEdit.TextFont));
            }

            if (!DispensingViewPropertyModelEdit.IsRuning)
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
            if (mapOprtCellInstInfo.OprtCellID == DispensingViewMapOprtCellConst.TextColor_MapOprtCellID)
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

            if (mapOprtCellInstInfo.OprtCellID == DispensingViewMapOprtCellConst.BackColor_MapOprtCellID)
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

            if (mapOprtCellInstInfo.OprtCellID == DispensingViewMapOprtCellConst.TextFont_MapOprtCellID)
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
                if (callBack.GetMapCellVMObjInstance() is DispensingViewModel vm)
                {
                    GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(DispensingViewPropertyModelEdit.TextColor));
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
                if (callBack.GetMapCellVMObjInstance() is DispensingViewModel vm)
                {
                    GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(DispensingViewPropertyModelEdit.BackColor));
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
                if (callBack.GetMapCellVMObjInstance() is DispensingViewModel vm)
                {
                    GriffinsBaseValue val = callBack.GetMapCellPropValue(nameof(DispensingViewPropertyModelEdit.TextFont));
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
            var propertyEditModelBase = JsonObjConvert.FromJSon<DispensingViewPropertyModelEdit>(br.ReadString("PropertyEditModelBase"));
            (PropertyEditModelBase as DispensingViewPropertyModelEdit)!.CopyFrom(propertyEditModelBase);
            var propertyBindEditModelBase = JsonObjConvert.FromJSon<DispensingViewPropertyBindEditModel>(br.ReadString("PropertyBindEditModelBase"));
            (PropertyBindEditModelBase as DispensingViewPropertyBindEditModel)!.CopyFrom(propertyBindEditModelBase);
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
            var src = source as MapCellDispensingViewCtlObj;
            base._CopyFrom(src);
            PropertyEditModelBase.CopyFrom(source.PropertyEditModelBase);
            PropertyBindEditModelBase.CopyFrom(source.PropertyBindEditModelBase);
            EventBindEditModel?.CopyFrom(source.EventBindEditModel);
        }

        protected override object OnGetView() => view;

        protected override object OnGetViewModel() => viewModel;

        public override PropertyEditModelBase CreatePropertyModelEditBase() => new DispensingViewPropertyModelEdit();

        public override PropertyBindEditModelBase CreatePropertyBindEditModelBase() => new DispensingViewPropertyBindEditModel();

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

        public override string ToString() => ResourceA.DispensingView;

        #endregion

    }

    /// <summary>
    /// 2D检测图元属性模型
    /// </summary>
    [MapPropertyOrder]
    [CategoryPriority("图元信息", 1)]
    [CategoryPriority("反馈信息", 2)]
    public class DispensingViewPropertyModelEdit : FunctionalCellPropertyModelEdit
    {
        public DispensingViewPropertyModelEdit()
        {
         
        }

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
            if (source is DispensingViewPropertyModelEdit sourceModel)
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
    public class DispensingViewPropertyBindEditModel : FunctionalCellPropertyBindEditModel
    {
        public void CopyFrom(DispensingViewPropertyBindEditModel source)
        {
            base.CopyFrom(source);

        }

    }
}
