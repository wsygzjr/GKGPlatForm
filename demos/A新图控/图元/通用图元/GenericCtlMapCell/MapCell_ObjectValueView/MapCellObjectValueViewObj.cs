using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Griffins.Graph;
using Griffins;
using System.Drawing;

namespace Griffins.Map.CtlMapCell.Generic.ObjectValueView
{
    class MapCellObjectValueViewObj : IControlMapCellObject
    {
        /// <summary>
        /// （属性ID）对象值
        /// </summary>
        public const string Prop_XMLVal = "XMLVal";

        private static MapObjPropertyInfoList propertyInfos;
        private static MapObjEventInfoList eventInfos;
        static MapCellObjectValueViewObj()
        {
            propertyInfos = new MapObjPropertyInfoList();
            MapObjPropertyInfo propertyInfo_XMLVal = new MapObjPropertyInfo(MapCellObjectValueViewObj.Prop_XMLVal, ResourceA.ObjectValueView, typeof(ObjectValue), false, true);
            propertyInfos.Add(propertyInfo_XMLVal);

            eventInfos = new MapObjEventInfoList();
        }
        public MapCellObjectValueViewObj(MapObjID mapCellID, string mapCellName)
            :this(mapCellID,mapCellName,false)
        {
        }

        public MapCellObjectValueViewObj(MapObjID mapCellID, string mapCellName,bool designTime)
        {
            this.mapCellID = mapCellID;
            this.mapCellName = mapCellName;
            view = new UCtCellObjectValueView(designTime);
        }

        #region IControlMapCellObject 成员

        void IControlMapCellObject.Init(IControlMapCellCallBack iMapCellCallBack)
        {
            view.Init(iMapCellCallBack);
        }

       
        private UCtCellObjectValueView view;
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
                case MapCellObjectValueViewObj.Prop_XMLVal:
                    if (val != null)
                    {
                        try
                        {
                            view.SetObjectValue((ObjectValue)val);
                        }
                        catch
                        {
                            view.SetObjectValue(null);
                        }
                    }
                    else
                        view.SetObjectValue(null);
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
