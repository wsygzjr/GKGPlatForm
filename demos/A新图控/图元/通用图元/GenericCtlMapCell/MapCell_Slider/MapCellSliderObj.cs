using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Griffins.Graph;
using Griffins;
using System.Drawing;
using GF_Gereric;

namespace Griffins.Map.CtlMapCell.Generic.Slider
{
    class MapCellSliderObj : IControlMapCellObject
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
        /// （属性ID）方向
        /// </summary>
        public const string Prop_Orientation = "Orientation";

        /// <summary>
        /// （事件ID）值改变
        /// </summary>
        public const string Event_ValueChanged = "ValueChanged";

        private static MapObjPropertyInfoList propertyInfos;
        private static MapObjEventInfoList eventInfos;
        static MapCellSliderObj()
        {
            propertyInfos = new MapObjPropertyInfoList();
            MapObjPropertyInfo propertyInfo_Maximum = new MapObjPropertyInfo(MapCellSliderObj.Prop_Maximum, ResourceA.Prop_Maximum, typeof(decimal), true, true, false, null,100);
            propertyInfos.Add(propertyInfo_Maximum);
            MapObjPropertyInfo propertyInfo_Minimum = new MapObjPropertyInfo(MapCellSliderObj.Prop_Minimum, ResourceA.Prop_Minimum, typeof(decimal), true, true, false, null,0);
            propertyInfos.Add(propertyInfo_Minimum);
            MapObjPropertyInfo propertyInfo_Increment = new MapObjPropertyInfo(MapCellSliderObj.Prop_Increment, ResourceA.Prop_Increment, typeof(decimal), true, true, false, null,1);
            propertyInfos.Add(propertyInfo_Increment);
            MapObjPropertyInfo propertyInfo_Value = new MapObjPropertyInfo(MapCellSliderObj.Prop_Value, ResourceA.Prop_Value, typeof(decimal), true, true, false, null,0);
            propertyInfos.Add(propertyInfo_Value);
            //MapObjPropertyInfo propertyInfo_Orientation = new MapObjPropertyInfo(MapCellSliderObj.Prop_Orientation, ResourceA.Prop_Orientation, typeof(SliderOrientation), true, false, true, typeof(SliderOrientationUiMapPropertyEditor), SliderOrientation.Horizontal);
            MapObjPropertyInfo propertyInfo_Orientation = new MapObjPropertyInfo(MapCellSliderObj.Prop_Orientation, ResourceA.Prop_Orientation, typeof(bool), true, false, false,null,false);
            propertyInfos.Add(propertyInfo_Orientation);

            MapObjPropertyInfo propertyInfo_BackColor = new MapObjPropertyInfo(MapObjPropEventConst.Prop_BackColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), typeof(Color), true, true, false, null, SystemColors.Control);
            propertyInfos.Add(propertyInfo_BackColor);
            propertyInfos.Add(new MapObjPropertyInfo(MapObjPropEventConst.Prop_Cursor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_Cursor), typeof(CtlCellCursor), true, false, false, null, CtlCellCursor.Default));

            eventInfos = new MapObjEventInfoList();
            eventInfos.Add(new MapObjEventInfo(MapCellSliderObj.Event_ValueChanged,ResourceA.Event_ValueChanged , typeof(decimal), null));
        }
        public MapCellSliderObj(MapObjID mapCellID, string mapCellName)
            :this(mapCellID,mapCellName,false)
        {
        }

        public MapCellSliderObj(MapObjID mapCellID, string mapCellName,bool designTime)
        {
            this.mapCellID = mapCellID;
            this.mapCellName = mapCellName;
    
            view = new UCtCellSliderView(designTime);
        }

        #region IControlMapCellObject 成员

        private IControlMapCellCallBack iMapCellCallBack;
        void IControlMapCellObject.Init(IControlMapCellCallBack iMapCellCallBack)
        {
            this.iMapCellCallBack = iMapCellCallBack;
            view.Init(iMapCellCallBack);
        }

       
        private UCtCellSliderView view;
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
                case MapCellSliderObj.Prop_Maximum:
                    if (val != null)
                        view.SetMaximum(Convert.ToDecimal(val));
                    return true;
                case MapCellSliderObj.Prop_Minimum:
                    if (val != null)
                        view.SetMinimum(Convert.ToDecimal(val));
                    return true;
                case MapCellSliderObj.Prop_Increment:
                    if (val != null)
                        view.SetIncrement(Convert.ToDecimal(val));
                    return true;
                case MapCellSliderObj.Prop_Value:
                    if (val != null)
                        view.SetValue(Convert.ToDecimal(val));
                    return true;
                case MapCellSliderObj.Prop_Orientation:
                    //if (val != null)
                    //    view.SetOrientation((SliderOrientation)val);
                    if (val != null)
                    {
                        if ((bool)val)
                            view.SetOrientation(SliderOrientation.Vertical);
                        else
                            view.SetOrientation(SliderOrientation.Horizontal);
                    }
                    return true;
                case MapObjPropEventConst.Prop_BackColor:
                    if (val == null)
                        view.SetBackColor(Color.Blue);
                    else
                        view.SetBackColor((Color)val);
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
            BytesXmlWriter bw = new BytesXmlWriter("MapCellSlider");
            //bw.WriteSerialObject("TextFont", this.textFont);
            //bw.Write("TextColor", this.textColor.ToArgb());
            return bw.ToBytes();
        }
        /// <summary>
        /// 从字节流中读图控对象单元字段（必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="data">字节流</param>
        void IMapObjCellBase.ReadFromBytes(byte[] data)
        {
            GriffinsXmlReader br = GriffinsXmlReader.Create("MapCellSlider", data);
            br.SkipRootNode();
            //this.textFont = (Font)br.ReadSerialObject("TextFont");
            //if (this.textFont == null)
            //    this.textFont = SystemFonts.DefaultFont;
            //this.textColor = Color.FromArgb(br.ReadInt32("TextColor"));
            //view.SetNumUpDownTextColor(textColor);
            //SetNumUpDownTextFont();
        }

        #endregion
    }


    /// <summary>
    /// 滑块方向
    /// </summary>
    [Serializable]
    public enum SliderOrientation
    {
        /// <summary>
        /// 水平
        /// </summary>
        Horizontal,
        /// <summary>
        /// 垂直
        /// </summary>
        Vertical,
    }

    internal class SliderOrientationUiMapPropertyEditor : UiMapPropertyEditor
    {
        /// <summary>
        /// 值编辑样式（如果不重载，将返回None）
        /// </summary>
        /// <returns>值编辑样式</returns>
        public override UiMapPropertyEditorEditStyle EditStyle()
        {
            return UiMapPropertyEditorEditStyle.Combo;
        }

        /// <summary>
        /// 当EditStyle指示的编辑器为Combo时，提供一个名字和属性值对的集合
        /// </summary>
        /// <returns></returns>
        public override NamePropertyValueList GetNamePropertyValueCollection()
        {
            NamePropertyValueList namePropertyValues = new NamePropertyValueList();
            namePropertyValues.Add(new SliderOrientationNamePropertyValue(SliderOrientation.Horizontal, "水平"));
            namePropertyValues.Add(new SliderOrientationNamePropertyValue(SliderOrientation.Vertical, "垂直"));
            return namePropertyValues;
        }

        #region 内部类型
        /// <summary>
        /// 名称属性值对
        /// </summary>
        private class SliderOrientationNamePropertyValue : NamePropertyValue
        {
            /// <summary>
            /// 创建NamePropertyValue新实例
            /// </summary>
            /// <param name="propertyValue">属性值</param>
            /// <param name="displayName">显示名称</param>
            public SliderOrientationNamePropertyValue(SliderOrientation propertyValue, string displayName)
                : base(propertyValue, displayName)
            {
            }
            /// <summary>
            /// 比较两个属性值是否相等
            /// </summary>
            /// <param name="otherPropertyValue">另一个属性值</param>
            /// <returns>True:相等，False：不等</returns>
            public override bool CompareTo(object otherPropertyValue)
            {
                if (otherPropertyValue == null)
                    return false;
                return (SliderOrientation)this.PropertyValue == (SliderOrientation)otherPropertyValue;
            }
        }
        #endregion
    }
}
