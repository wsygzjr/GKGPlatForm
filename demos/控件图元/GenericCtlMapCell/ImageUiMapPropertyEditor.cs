using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Griffins.Graph;

namespace Griffins.Map.CtlMapCell.Generic
{
    class ImageUiMapPropertyEditor : UiMapPropertyEditor
    {
        /// <summary>
        /// 值编辑样式（如果不重载，将返回None）
        /// </summary>
        /// <returns>值编辑样式</returns>
        public override UiMapPropertyEditorEditStyle EditStyle()
        {
            return UiMapPropertyEditorEditStyle.Modal;
        }

        /// <summary>
        /// 使用 EditStyle 所指示的编辑器样式编辑指定对象的值，（在EditStyle为None时，不用重载）
        /// </summary>
        /// <param name="value">要编辑的对象</param>
        /// <returns>新的对象值</returns>
        public override Object EditPropertyValue(Object value)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                Image image = Image.FromFile(fileName);
                return image;
            }
            else
                return value;
        }
        /// <summary>
        /// 把属性值转换为字符串表示（在PaintPropertyValueSupported为True时，该方法不用实现）
        /// </summary>
        /// <param name="propertyValue">属性值</param>
        /// <returns>属性值的字符串表示</returns>
        public override string ObjectToString(Object propertyValue)
        {
            if (propertyValue == null)
                return string.Empty;
            else
                return propertyValue.ToString();
        }
    }
}
