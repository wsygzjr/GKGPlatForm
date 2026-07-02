using System;
using System.Collections.Generic;
using System.Text;
using Griffins.Graph;

namespace Griffins.Map.CtlMapCell.Generic.ImageListCtl
{
    internal class ImgKeySelUiMapPropertyEditor : UiMapPropertyEditor
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
        /// 在编辑器为Combo时，进入Combo编辑框
        /// </summary>
        public override void OnComboEnter()
        {
            RaiseNamePropertyValueCollectionChenged();
        }

        /// <summary>
        /// 当EditStyle指示的编辑器为Combo时，提供一个名字和属性值对的集合
        /// </summary>
        /// <returns></returns>
        public override NamePropertyValueList GetNamePropertyValueCollection()
        {
            NamePropertyValueList namePropertyValues = new NamePropertyValueList();
            if ((this.Owner != null) && (this.Owner is MapCellImageListCtlObj))
            {
                MapCellImageListCtlObj drawImageList = (this.Owner as MapCellImageListCtlObj);
                if (drawImageList.ImageGroup != null)
                {
                    foreach (GirffinsImageGroupItem imageGroupItem in drawImageList.ImageGroup)
                        namePropertyValues.Add(new ImgKeySelNamePropertyValue(imageGroupItem.ImgKey, imageGroupItem.ImgKey));
                }
            }
            return namePropertyValues;
        }

        #region 内部类型
        /// <summary>
        /// 名称属性值对
        /// </summary>
        private class ImgKeySelNamePropertyValue : NamePropertyValue
        {
            /// <summary>
            /// 创建NamePropertyValue新实例
            /// </summary>
            /// <param name="propertyValue">属性值</param>
            /// <param name="displayName">显示名称</param>
            public ImgKeySelNamePropertyValue(string propertyValue, string displayName)
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
                return (string)this.PropertyValue == (string)otherPropertyValue;
            }
        }
        #endregion
    }
}
