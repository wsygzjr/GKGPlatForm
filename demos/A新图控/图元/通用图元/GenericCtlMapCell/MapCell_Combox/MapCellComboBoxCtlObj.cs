using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Griffins.Graph;
using Griffins;
using System.Drawing;
using GF_Gereric;
using System.IO;
using Griffins.IOT;
using System.ComponentModel;
using System.Drawing.Design;
using Newtonsoft.JsonG;
using System.Dynamic;

namespace Griffins.Map.CtlMapCell.Generic.ComboBox
{
    class MapCellComboBoxCtlObj : ControlCellBase
    {
        /// <summary>
        /// （属性ID）值列表
        /// </summary>
        public const string Prop_ValueList = "ValueList";
        /// <summary>
        /// （属性ID）值
        /// </summary>
        public const string Prop_Value = "Value";
        /// <summary>
        /// （事件ID）值改变
        /// </summary>
        public const string Event_ValueChanged = "ValueChanged";

        private UCtCellComboBoxCtlView view;

        public MapCellComboBoxCtlObj(MapObjID mapCellID, string mapCellName)
            : this(mapCellID, mapCellName, false)
        {
        }

        public MapCellComboBoxCtlObj(MapObjID mapCellID, string mapCellName, bool designTime)
        {
            base.SetID(mapCellID);
            base.SetName(mapCellName);
            this.textFont = SystemFonts.DefaultFont;
            this.textColor = Color.Black;
            view = new UCtCellComboBoxCtlView(designTime);
			RegisterProperty(new MapObjPropertyInfo(Prop_ValueList, ResourceA.ValueList, GriffinsBaseDataType.Object_Bytes, Guid.Empty, MapPropertyEditorID.Custom, false, true, null, null));
			RegisterProperty(new MapObjPropertyInfo(Prop_Value, ResourceA.Prop_Value, GriffinsBaseDataType.Decimal, Guid.Empty, MapPropertyEditorID.Text, false, true, null, 0));
			RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_BackColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), GriffinsBaseDataType.Integer, Guid.Empty, MapPropertyEditorID.Color, true, true, null, SystemColors.Control.ToArgb()));
			RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_TextColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), GriffinsBaseDataType.Integer, Guid.Empty, MapPropertyEditorID.Color, true, true, null, SystemColors.Control.ToArgb()));
            dynamic fontStyle = new ExpandoObject();
            fontStyle.Name = SystemFonts.DefaultFont.FontFamily.Name;
            fontStyle.Size = SystemFonts.DefaultFont.Size;
            fontStyle.Style = (int)SystemFonts.DefaultFont.Style;
            fontStyle.Uint = (int)SystemFonts.DefaultFont.Unit;
            RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_FontStyle, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_FontStyle), GriffinsBaseDataType.Object_Bytes, Guid.Empty, MapPropertyEditorID.Font, true, true, null, fontStyle));
			RegisterProperty(new MapObjPropertyInfo(MapObjPropEventConst.Prop_Cursor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_Cursor), GriffinsBaseDataType.Integer, Guid.Empty, MapPropertyEditorID.GriffinsCursor, true, true, null, (int)CtlCellCursor.Default));
			RegisterEvent(new MapObjEventInfo(Event_ValueChanged, ResourceA.Event_ValueChanged, GriffinsBaseDataType.Object_Bytes, Guid.Empty));
		}

        /// <summary>
        /// 初始化时
        /// </summary>
        protected override void OnInit()
        {
            view.Init(base.CallBack);
            view.SetComboBoxTextColor(textColor);
            SetComboBoxTextFont();
        }

        protected override object OnGetView()
        {
            return view;
        }

        public override void OnZoomChanged()
        {
            SetComboBoxTextFont();
        }

        internal void SetComboBoxTextFont()
        {
            int size = base.CallBack?.Calc?.CalcZoomVal(this.textFont.Size) ?? 2;
            if (size < 2)
                size = 2;
            Font font = new Font(this.textFont.FontFamily, size, this.textFont.Style, this.textFont.Unit);
            view.SetComboBoxTextFont(font);
        }

        public override bool SetPropertyValue(string propertyID, object propertyVal)
        {
            try
            {
                switch (propertyID)
                {
                    case Prop_ValueList:
                        if (propertyVal == null)
                            this.valueNameList = null;
                        else
                            this.valueNameList = JsonObjConvert.FromJSon<DecamalValueNameList>(propertyVal.ToString());
                        if (this.valueNameList != null)
                            view.SetValueNameList(this.valueNameList);
                        else
                            view.SetValueNameList(null);
                        return true;
                    case Prop_Value:
                        if (propertyVal != null)
                            view.SetValue(Convert.ToDecimal(propertyVal));
                        return true;
                    case MapObjPropEventConst.Prop_BackColor:
                        if (propertyVal == null || !int.TryParse(propertyVal.ToString(),out int backColoRargb))
                            view.SetBackColor(Color.Blue);
                        else
                            view.SetBackColor(Color.FromArgb(backColoRargb));
                        return true;
                    case MapObjPropEventConst.Prop_TextColor:
                        if (propertyVal == null || !int.TryParse(propertyVal.ToString(), out int inttTextColoRargb))
                            view.SetComboBoxTextColor(SystemColors.InfoText);
                        else
                            view.SetComboBoxTextColor(Color.FromArgb(inttTextColoRargb));
                        return true;
                    case MapObjPropEventConst.Prop_FontStyle:
                        if (propertyVal == null || string.IsNullOrWhiteSpace(propertyVal.ToString()))
                            textFont = SystemFonts.DefaultFont;
                        else
                        {
                            dynamic fontStyle = GF_Gereric.JsonObjConvert.FromJSon<dynamic>(propertyVal.ToString());
                            textFont = new Font((string)fontStyle.Name, (float)fontStyle.Size, (FontStyle)fontStyle.Style, (GraphicsUnit)fontStyle.Uint);
                        }
                        SetComboBoxTextFont();
                        return true;
                    case MapObjPropEventConst.Prop_Cursor:
                        if (propertyVal == null || !int.TryParse(propertyVal.ToString(), out int cursor))
                            view.SetCursor(CtlCellCursor.Default);
                        else
                            view.SetCursor((CtlCellCursor)cursor);
                        return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return base.SetPropertyValue(propertyID, propertyVal);
        }

        /// <summary>
        /// 从字节流中读画图信息（必须先调用基类的OnReadDrawInfoFromBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="br">字节流读取对象</param>
        protected override void OnReadDrawInfoFromBytes(GriffinsXmlReader br)
        {
            base.OnReadDrawInfoFromBytes(br);
        }

        /// <summary>
        /// 当把画图信息写入到字节流中（必须先调用基类的OnWriteDrawInfoToBytes，必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="bw">字节流写入对象</param>
        protected override void OnWriteDrawInfoToBytes(GriffinsXmlWriter bw)
        {
            base.OnWriteDrawInfoToBytes(bw);
        }

        private Font textFont;
        /// <summary>
        /// 文本字体
        /// </summary>
        [DisplayName("文字字体")]
        [Category("图元信息")]
        public Font TextFont
        {
            get { return textFont; }
            set
            {
                textFont = value;
                SetComboBoxTextFont();
                dynamic fontStyle = new ExpandoObject();
                fontStyle.Name = textFont.FontFamily.Name;
                fontStyle.Size = textFont.Size;
                fontStyle.Style = (int)SystemFonts.DefaultFont.Style;
                fontStyle.Uint = (int)SystemFonts.DefaultFont.Unit;
                UpdateProperty(MapObjPropEventConst.Prop_FontStyle, fontStyle);
            }
        }
        private Color textColor;
        /// <summary>
        /// 文本颜色
        /// </summary>
        [DisplayName("文本颜色")]
        [Category("图元信息")]
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                if (textColor == null)
                {
                    view.SetComboBoxTextColor(SystemColors.InfoText);
                    UpdateProperty(MapObjPropEventConst.Prop_TextColor, SystemColors.InfoText.ToArgb());
                }
                else
                {
                    view.SetComboBoxTextColor(textColor);
                    UpdateProperty(MapObjPropEventConst.Prop_TextColor, textColor.ToArgb());
                }
            }
        }

        private Color backColor;
        /// <summary>
        /// 文本颜色
        /// </summary>
        [DisplayName("背景颜色")]
        [Category("图元信息")]
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                if (backColor == null)
                {
                    view.SetBackColor(Color.Blue);
                    UpdateProperty(MapObjPropEventConst.Prop_BackColor, Color.Blue.ToArgb());
                }
                else
                {
                    view.SetBackColor(backColor);
                    UpdateProperty(MapObjPropEventConst.Prop_BackColor, backColor.ToArgb());
                }
            }
        }

        private DecamalValueNameList valueNameList;
        /// <summary>
        /// 下拉框绑定
        /// </summary>
        [DisplayName("集合")]
        [Category("图元信息")]
        [Editor(typeof(ComboBoxValuesEditor), typeof(UITypeEditor))]
        public DecamalValueNameList ValueNameList
        {
            get 
            { 
                return valueNameList; 
            }
            set 
            { 
                valueNameList = value;
                view.SetValueNameList(valueNameList);
                UpdateProperty(Prop_ValueList, valueNameList);
            }
        }

        private decimal val;
        /// <summary>
        /// 下拉框绑定
        /// </summary>
        [DisplayName("值")]
        [Category("图元信息")]
        public decimal Val
        {
            get
            {
                return val;
            }
            set
            {
                val = value;
                view.SetValue(val);
                UpdateProperty(Prop_Value, val);
            }
        }

        protected override void OnCopyFrom(GraphObjectBase source)
        {

        }

        public override string ToString()
        {
            return "下拉框";
        }
    }
}
