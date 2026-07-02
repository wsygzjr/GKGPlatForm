using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Griffins.Graph;
using Griffins;
using System.Drawing;

namespace Griffins.Map.CtlMapCell.Generic.ImageCtl
{
    class MapCellImageCtlObj : IControlMapCellObject
    {
        /// <summary>
        /// （属性ID）图像
        /// </summary>
        public const string Prop_Image = "Image";

        private static MapObjPropertyInfoList propertyInfos;
        private static MapObjEventInfoList eventInfos;
        static MapCellImageCtlObj()
        {
            propertyInfos = new MapObjPropertyInfoList();
            MapObjPropertyInfo propertyInfo_Image = new MapObjPropertyInfo(MapCellImageCtlObj.Prop_Image, ResourceA.Image, typeof(Image), false, false, true, typeof(ImageUiMapPropertyEditor));
            propertyInfos.Add(propertyInfo_Image);
            MapObjPropertyInfo propertyInfo_ImageSizeMode = new MapObjPropertyInfo(MapObjPropEventConst.Prop_ImageSizeMode, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_ImageSizeMode), typeof(ImageSizeMode), false, false, false, null, ImageSizeMode.Normal);
            propertyInfos.Add(propertyInfo_ImageSizeMode);
            MapObjPropertyInfo propertyInfo_BackColor = new MapObjPropertyInfo(MapObjPropEventConst.Prop_BackColor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_BackColor), typeof(Color), true, true, false, null, Color.Blue);
            propertyInfos.Add(propertyInfo_BackColor);
            propertyInfos.Add(new MapObjPropertyInfo(MapObjPropEventConst.Prop_Cursor, MapObjPropEventConst.GetName(MapObjPropEventConst.Prop_Cursor), typeof(CtlCellCursor), true, false, false, null, CtlCellCursor.Default));


            eventInfos = new MapObjEventInfoList();
            eventInfos.Add(new MapObjEventInfo(MapObjPropEventConst.Event_MouseClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseClick), typeof(GraphMouseEventParam), null));
            eventInfos.Add(new MapObjEventInfo(MapObjPropEventConst.Event_MouseDoubleClick, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseDoubleClick), typeof(GraphMouseEventParam), null));
            eventInfos.Add(new MapObjEventInfo(MapObjPropEventConst.Event_MouseEnter, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseEnter), typeof(GraphMouseEventParam), null));
            eventInfos.Add(new MapObjEventInfo(MapObjPropEventConst.Event_MouseLeave, MapObjPropEventConst.GetName(MapObjPropEventConst.Event_MouseLeave), typeof(GraphMouseEventParam), null));

        }
        public MapCellImageCtlObj(MapObjID mapCellID, string mapCellName)
            :this(mapCellID,mapCellName,false)
        {
        }

        public MapCellImageCtlObj(MapObjID mapCellID, string mapCellName,bool designTime)
        {
            this.mapCellID = mapCellID;
            this.mapCellName = mapCellName;
            view = new UCtCellImageCtlView(designTime);
        }

        #region IControlMapCellObject 成员

        void IControlMapCellObject.Init(IControlMapCellCallBack iMapCellCallBack)
        {
            view.Init(iMapCellCallBack);
        }

       
        private UCtCellImageCtlView view;
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
                case MapCellImageCtlObj.Prop_Image:
                    if (val != null)
                    {
                        try
                        {
                            view.SetImage((Image)val);
                        }
                        catch
                        {
                            view.SetImage(null);
                        }
                    }
                    else
                        view.SetImage(null);
                    return true;
                case MapObjPropEventConst.Prop_ImageSizeMode:
                    if (val == null)
                        view.SetImageSizeMode(PictureBoxSizeMode.Normal);
                    else
                    {
                        switch ((ImageSizeMode)val)
                        {
                            case ImageSizeMode.Normal:
                                view.SetImageSizeMode(PictureBoxSizeMode.Normal);
                                break;
                            case ImageSizeMode.CenterImage:
                                view.SetImageSizeMode(PictureBoxSizeMode.CenterImage);
                                break;
                            case ImageSizeMode.StretchImage:
                                view.SetImageSizeMode(PictureBoxSizeMode.StretchImage);
                                break;
                            case ImageSizeMode.Tile:
                                view.SetImageSizeMode(PictureBoxSizeMode.Normal);
                                break;
                            case ImageSizeMode.Zoom:
                                view.SetImageSizeMode(PictureBoxSizeMode.Zoom);
                                break;
                        }
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
            return null;
        }
        /// <summary>
        /// 从字节流中读图控对象单元字段（必须保证写入数据和读出数据的顺序一致）
        /// </summary>
        /// <param name="data">字节流</param>
        void IMapObjCellBase.ReadFromBytes(byte[] data)
        {
        }

        #endregion
    }
}
