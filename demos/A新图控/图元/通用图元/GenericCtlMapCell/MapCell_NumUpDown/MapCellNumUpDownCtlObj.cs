using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Griffins.Graph;
using Griffins;
using System.Drawing;
using GF_Gereric;

namespace Griffins.Map.CtlMapCell.Generic.NumUpDown
{
    class MapCellNumUpDownCtlObj : IControlMapCellObject
    {
        /// <summary>
        /// （属性ID）最大值
        /// </summary>
        public const string Prop_Maximum = "Maximum";
        /// <summary>
        /// （属性ID）最小值
        /// </summary>
        public const string Prop_Minimum = "Minimum";
        /// <summary>
        /// （属性ID）步长
        /// </summary>
        public const string Prop_Increment = "Increment";
        /// <summary>
        /// （属性ID）值
        /// </summary>
        public const string Prop_Value = "Value";

        /// <summary>
        /// （事件ID）值改变
        /// </summary>
        public const string Event_ValueChanged = "ValueChanged";

        private static MapObjPropertyInfoList propertyInfos;
        private static MapObjEventInfoList eventInfos;
        static MapCellNumUpDownCtlObj()
        {
            propertyInfos = new MapObjPropertyInfoList();
            MapObjPropertyInfo propertyInfo_Maximum = new MapObjPropertyInfo(MapCellNumUpDownCtlObj.Prop_Maximum, ResourceA.Prop_Maximum, typeof(decimal), true, true, false, null,100);
            propertyInfos.Add(propertyInfo_Maximum);
            MapObjPropertyInfo propertyInfo_Minimum = new MapObjPropertyInfo(MapCellNumUpDownCtlObj.Prop_Minimum, ResourceA.Prop_Minimum, typeof(decimal), true, true, false, null,0);
            propertyInfos.Add(propertyInfo_Minimum);
            MapObjPropertyInfo propertyInfo_Increment = new MapObjPropertyInfo(MapCellNumUpDownCtlObj.Prop_Increment, ResourceA.Prop_Increment, typeof(decimal), true, true, false, null,1);
            propertyInfos.Add(propertyInfo_Increment);
            MapObjPropertyInfo propertyInfo_Value = new MapObjPropertyInfo(MapCellNumUpDownCtlObj.Prop_Value, ResourceA.Prop_Value, typeof(decimal), true, true, false, null,0);
            propertyInfos.Add(propertyInfo_Value);

            MapObjPropertyInfo propertyInfo_BackColor = new MapObjPropertyInfo(MapObjPropEventConst.Prop_BackColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), typeof(Color), true, true, false, null, SystemColors.Control);
            propertyInfos.Add(propertyInfo_BackColor);
            MapObjPropertyInfo propertyInfo_TextColor = new MapObjPropertyInfo(MapObjPropEventConst.Prop_TextColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_TextColor), typeof(Color), true, true, false, null, SystemColors.InfoText);
            propertyInfos.Add(propertyInfo_TextColor);
            MapObjPropertyInfo propertyInfo_FontStyle = new MapObjPropertyInfo(MapObjPropEventConst.Prop_FontStyle, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_FontStyle), typeof(Font), true, false, false, null, SystemFonts.DefaultFont);
            propertyInfos.Add(propertyInfo_FontStyle);
            propertyInfos.Add(new MapObjPropertyInfo(MapObjPropEventConst.Prop_Cursor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_Cursor), typeof(CtlCellCursor), true, false, false, null, CtlCellCursor.Default));

            eventInfos = new MapObjEventInfoList();
            eventInfos.Add(new MapObjEventInfo(MapCellNumUpDownCtlObj.Event_ValueChanged,ResourceA.Event_ValueChanged , typeof(decimal), null));
        }
        public MapCellNumUpDownCtlObj(MapObjID mapCellID, string mapCellName)
            :this(mapCellID,mapCellName,false)
        {
        }

        public MapCellNumUpDownCtlObj(MapObjID mapCellID, string mapCellName,bool designTime)
        {
            this.mapCellID = mapCellID;
            this.mapCellName = mapCellName;
            this.textFont = SystemFonts.DefaultFont;
            this.textColor = Color.Black;
            view = new UCtCellNumUpDownCtlView(designTime);
        }

        #region IControlMapCellObject 成员

        private IControlMapCellCallBack iMapCellCallBack;
        void IControlMapCellObject.Init(IControlMapCellCallBack iMapCellCallBack)
        {
            this.iMapCellCallBack = iMapCellCallBack;
            view.Init(iMapCellCallBack);

            view.SetNumUpDownTextColor(textColor);
            SetNumUpDownTextFont();
        }

       
        private UCtCellNumUpDownCtlView view;
        Control IControlMapCellObject.View
        {
            get { return view; }
        }

        #endregion

        #region IMapCellObjectBase 成员

        MapObjPropertyInfoList IMapObjCellBase.PropertyInfos
        {
            get { return propertyInfos; }
        }

        MapObjEventInfoList IMapObjCellBase.EventInfos
        {
            get { return eventInfos; }
        }


        private MapObjID mapCellID;
        /// <summary>
        /// 图元ID
        /// </summary>
        MapObjID IMapObjCellBase.ID
        {
            get { return mapCellID; }
        }
         /// <summary>
        /// 设置图元ID
        /// </summary>
        /// <param name="mapCellID">图元ID</param>
        internal void SetMapCellID(MapObjID mapCellID)
        {
            this.mapCellID = mapCellID;
        }

        private string mapCellName;
        /// <summary>
        /// 图元名称
        /// </summary>
        string IMapObjCellBase.Name
        {
            get { return mapCellName; }
            set 
            {
                SetMapCellName(value);
            }
        }

        internal void SetMapCellName(string mapCellName)
        {
            this.mapCellName = mapCellName;
        }

        private Font textFont;
        /// <summary>
        /// 文本字体
        /// </summary>
        public Font TextFont
        {
            get { return textFont; }
            set 
            {
                textFont = value;
                SetNumUpDownTextFont();
            }
        }
        private Color textColor;
        /// <summary>
        /// 文本颜色
        /// </summary>
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                view.SetNumUpDownTextColor(textColor);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyID">属性ID</param>
        /// <param name="val">属性值</param>
        /// <returns>True:已经找到该属性并设置，false:没有该属性</returns>
        bool IMapObjCellBase.SetPropertyValue(string propertyID, object val)
        {
            switch (propertyID)
            {
                case MapCellNumUpDownCtlObj.Prop_Maximum:
                    if (val != null)
                        view.SetMaximum(Convert.ToDecimal(val));
                    return true;
                case MapCellNumUpDownCtlObj.Prop_Minimum:
                    if (val != null)
                        view.SetMinimum(Convert.ToDecimal(val));
                    return true;
                case MapCellNumUpDownCtlObj.Prop_Increment:
                    if (val != null)
                        view.SetIncrement(Convert.ToDecimal(val));
                    return true;
                case MapCellNumUpDownCtlObj.Prop_Value:
                    if (val != null)
                        view.SetValue(Convert.ToDecimal(val));
                    return true;
                case MapObjPropEventConst.Prop_BackColor:
                    if (val == null)
                        view.SetBackColor(Color.Blue);
                    else
                        view.SetBackColor((Color)val);
                    return true;
                case MapObjPropEventConst.Prop_TextColor:
                    if (val == null)
                        view.SetNumUpDownTextColor(SystemColors.InfoText);
                    else
                        view.SetNumUpDownTextColor((Color)val);
                    return true;
                case MapObjPropEventConst.Prop_FontStyle:
                    if (val == null)
                        textFont = SystemFonts.DefaultFont;
                    else
                        textFont = (Font)val;
                    SetNumUpDownTextFont();
                    return true;

                case MapObjPropEventConst.Prop_Cursor:
                    if (val == null)
                        view.SetCursor(CtlCellCursor.Default);
                    else
                        view.SetCursor((CtlCellCursor)val);
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 当图缩放参数调整时调用
        /// </summary>
        void IMapObjCellBase.OnZoomChanged()
        {
            SetNumUpDownTextFont();
        }

        internal void SetNumUpDownTextFont()
        {
            float size = iMapCellCallBack.Calc.CalcZoomVal((int)this.textFont.Size);
            if (size < 2)
                size = 2;
            Font font = new Font(this.textFont.FontFamily, size, this.textFont.Style, this.textFont.Unit);
            view.SetNumUpDownTextFont(font);
        }

        /// <summary>
        /// 从来源实例复制字段到本实例
        /// </summary>
        /// <param name="source">来源实例</param>
        void IMapObjCellBase.CopyFrom(IMapObjCellBase source)
        {
        }
        /// <summary>
        /// 把图控对象单元字段写入到字节流中
        /// </summary>
        /// <param name="bw">字节流写入对象</param>
        byte[]  IMapObjCellBase.WriteToBytes()
        {
            BytesXmlWriter bw = new BytesXmlWriter("MapCellNumUpDownCtl");
            bw.WriteSerialObject("TextFont", this.textFont);
            bw.Write("TextColor", this.textColor.ToArgb());
            return bw.ToBytes();
        }
        /// <summary>
        /// 从字节流中读图控对象单元字段（必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="data">字节流</param>
        void IMapObjCellBase.ReadFromBytes(byte[] data)
        {
            GriffinsXmlReader br = GriffinsXmlReader.Create("MapCellNumUpDownCtl", data);
            br.SkipRootNode();
            this.textFont = (Font)br.ReadSerialObject("TextFont");
            if (this.textFont == null)
                this.textFont = SystemFonts.DefaultFont;
            this.textColor = Color.FromArgb(br.ReadInt32("TextColor"));
            view.SetNumUpDownTextColor(textColor);
            SetNumUpDownTextFont();
        }

        #endregion
    }
}
