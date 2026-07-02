using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Griffins.Graph;

namespace GKG.Map.MapCell.Generic
{
    class ImageUiMapPropertyEditor : UiMapPropertyEditor
    {
        /// <summary>
        /// 值编辑器样式，返回类型见枚举，默认返回None即可
        /// </summary>
        /// <returns>值编辑器样式</returns>
        public override UiMapPropertyEditorEditStyle EditStyle()
        {
            return UiMapPropertyEditorEditStyle.Modal;
        }

        /// <summary>
        /// 使用EditStyle所指定的编辑窗口样式编辑指定的属性值，当EditStyle为None时，此函数不会被调用
        /// </summary>
        /// <param name="value">要编辑的属性</param>
        /// <returns>新的属性值</returns>
        public override Object EditPropertyValue(Object value)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            // 设置支持的图片类型后缀
            openFileDialog1.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.ico;*.svg;*.tif;*.tiff;*.webp;*.heic;*.heif;*.avif;*.tga;*.wmf;*.emf|JPEG图片|*.jpg;*.jpeg|PNG图片|*.png|BMP图片|*.bmp|GIF图片|*.gif|图标文件|*.ico|SVG图片|*.svg|TIFF图片|*.tif;*.tiff|WebP图片|*.webp|HEIC/HEIF图片|*.heic;*.heif|AVIF图片|*.avif|TGA图片|*.tga|WMF/EMF图片|*.wmf;*.emf|所有文件|*.*";
            openFileDialog1.FilterIndex = 1;
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
        /// 将属性值转换为字符串显示，当PaintPropertyValueSupported为True时，需实现此函数
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
