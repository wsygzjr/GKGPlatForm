using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Griffins.Map.CtlMapCell.Generic.ComboBox
{
	/// <summary>
	/// 下拉框可选值编辑
	/// </summary>
	public class ComboBoxValuesEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			// 编辑属性值时，在右侧显示...更多按钮  
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc)
			{
				FormValueNames formValueNames = new FormValueNames();
				if (value != null)
					formValueNames.ValueNameList = (DecamalValueNameList)value;
				else
					formValueNames.ValueNameList = null;

				if (formValueNames.ShowDialog() == DialogResult.OK)
					return formValueNames.ValueNameList;
				else
					return value;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
