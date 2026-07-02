using System.Drawing;
using Infragistics.Win;

namespace Griffins.UI
{
	/// <summary>
	/// 外观
	/// </summary>
	public class GriffinsAppearances
	{
		/// <summary>
		/// 数据网格单元格外观
		/// </summary>
		public static readonly Appearance DataGridCell = getDataGridCellAppearance();

		/// <summary>
		/// 数据网格标题外观
		/// </summary>
		public static readonly Appearance DataGridCaption = getDataGridCaptionAppearance();

		/// <summary>
		/// 背景外观
		/// </summary>
		public static readonly Appearance BackAppearance = getBackAppearance();

		/// <summary>
		/// 数据网格列标题外观
		/// </summary>
		public static readonly Appearance DataGridColumnHeaderAppearance = getDataGridColumnHeaderAppearance();

		/// <summary>
		/// 数据网格行选择器外观
		/// </summary>
		public static readonly Appearance DataGridRowSelectorAppearance = getDataGridRowSelectorAppearance();

		/// <summary>
		/// 被激活行外观
		/// </summary>
		public static readonly Appearance ActiveRowAppearance = getActiveRowAppearance();

		/// <summary>
		/// 被激活单元格外观
		/// </summary>
		public static readonly Appearance ActiveCellAppearance = getActiveCellAppearance();

		/// <summary>
		/// 被选中外观
		/// </summary>
		public static readonly Appearance SelectedAppearance = getSelectedAppearance();

		/// <summary>
		/// 按钮外观
		/// </summary>
		public static readonly Appearance ButtonAppearance = getButtonAppearance();

		private static Appearance getDataGridCellAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.White;
			tmpAppearance.BackGradientStyle = GradientStyle.Default;
			tmpAppearance.ForeColor = Color.Black;
			return tmpAppearance;
		}

		private static Appearance getDataGridCaptionAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.White;
			tmpAppearance.BackColor2 = Color.LightGreen;
			tmpAppearance.BackGradientStyle = GradientStyle.Vertical;
			return tmpAppearance;
		}

		private static Appearance getBackAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.White;
			return tmpAppearance;
		}

		private static Appearance getDataGridColumnHeaderAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.WhiteSmoke;
			tmpAppearance.BackColor2 = Color.Gainsboro;
			tmpAppearance.BackGradientStyle = GradientStyle.Vertical;
			return tmpAppearance;
		}

		private static Appearance getDataGridRowSelectorAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.LightGray;
			tmpAppearance.BackGradientStyle = GradientStyle.None;
			return tmpAppearance;
		}

		private static Appearance getActiveRowAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.Blue;
			tmpAppearance.BackGradientStyle = GradientStyle.None;
			tmpAppearance.ForeColor = Color.White;
			return tmpAppearance;
		}

		private static Appearance getActiveCellAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.Blue;
			tmpAppearance.ForeColor = Color.White;
			return tmpAppearance;
		}

		private static Appearance getSelectedAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.Blue;
			tmpAppearance.ForeColor = Color.White;
			return tmpAppearance;
		}

		private static Appearance getButtonAppearance()
		{
			Appearance tmpAppearance = new Appearance();
			tmpAppearance.BackColor = Color.WhiteSmoke;
			tmpAppearance.BackColor2 = Color.Silver;
			tmpAppearance.BackGradientStyle = GradientStyle.Vertical;
			return tmpAppearance;
		}
	}
}
