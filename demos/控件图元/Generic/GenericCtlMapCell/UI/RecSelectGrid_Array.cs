using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace Griffins.UI
{
	/// <summary>
	/// GriffinsSelectGrid_Array 的摘要说明。
	/// </summary>
	public class RecSelectGrid_Array : UserControl
	{
		/// <summary>
		/// 选择改变事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void AfterSelectChangeEventHandler(object sender, AfterSelectChangeEventArgs e);

		/// <summary>
		/// 选择改变事件参数
		/// </summary>
		public class AfterSelectChangeEventArgs : EventArgs
		{
			private UltraGridRow gridRow;

			/// <summary>
			/// 当前选择的行
			/// </summary>
			public UltraGridRow GridRow => gridRow;

			/// <summary>
			/// 创建AfterSelectChangeEventArgs新实例
			/// </summary>
			/// <param name="gridRow">当前选择的行</param>
			public AfterSelectChangeEventArgs(UltraGridRow gridRow)
			{
				this.gridRow = gridRow;
			}
		}

		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private Container components = null;

		private UltraGrid ultraGrid1;

		private bool rowSelectors;

		private bool readOnly;

		private bool mutiRowSelect;

		private bool canSelectRow;

		private bool lockSelectChange = false;

		/// <summary>
		/// 显示行选择器
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool RowSelectors
		{
			get
			{
				return rowSelectors;
			}
			set
			{
				rowSelectors = value;
				if (rowSelectors)
				{
					ultraGrid1.DisplayLayout.Override.RowSelectors = DefaultableBoolean.True;
				}
				else
				{
					ultraGrid1.DisplayLayout.Override.RowSelectors = DefaultableBoolean.False;
				}
			}
		}

		/// <summary>
		/// 只读
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool ReadOnly
		{
			get
			{
				return readOnly;
			}
			set
			{
				readOnly = value;
				if (readOnly)
				{
					ultraGrid1.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
					ultraGrid1.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
				}
				else
				{
					ultraGrid1.DisplayLayout.Override.CellClickAction = CellClickAction.Default;
					ultraGrid1.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
				}
			}
		}

		/// <summary>
		/// 选择多行
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool MutiRowSelect
		{
			get
			{
				return mutiRowSelect;
			}
			set
			{
				mutiRowSelect = value;
				if (mutiRowSelect)
				{
					ultraGrid1.DisplayLayout.Override.MaxSelectedRows = -1;
				}
				else
				{
					ultraGrid1.DisplayLayout.Override.MaxSelectedRows = 1;
				}
			}
		}

		/// <summary>
		/// 是否可以选择行
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool CanSelectRow
		{
			get
			{
				return canSelectRow;
			}
			set
			{
				canSelectRow = value;
				if (!readOnly)
				{
					canSelectRow = true;
				}
				if (canSelectRow)
				{
					ultraGrid1.DisplayLayout.Override.SelectedRowAppearance = GriffinsAppearances.SelectedAppearance;
					ultraGrid1.DisplayLayout.Override.ActiveRowAppearance = GriffinsAppearances.ActiveRowAppearance;
					ultraGrid1.DisplayLayout.Override.ActiveCellAppearance = GriffinsAppearances.ActiveCellAppearance;
				}
				else
				{
					ultraGrid1.DisplayLayout.Override.SelectedRowAppearance = GriffinsAppearances.DataGridCell;
					ultraGrid1.DisplayLayout.Override.ActiveRowAppearance = GriffinsAppearances.DataGridCell;
					ultraGrid1.DisplayLayout.Override.ActiveCellAppearance = GriffinsAppearances.DataGridCell;
				}
			}
		}

		/// <summary>
		/// 标题
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Caption
		{
			get
			{
				return ultraGrid1.Text;
			}
			set
			{
				ultraGrid1.Text = value;
			}
		}

		/// <summary>
		/// 列宽自动适应显示区域宽度
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool AutoFitColumns
		{
			get
			{
				return ultraGrid1.DisplayLayout.AutoFitColumns;
			}
			set
			{
				ultraGrid1.DisplayLayout.AutoFitColumns = value;
			}
		}

		/// <summary>
		/// 列宽自动适应显示区域宽度
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public AutoFitStyle AutoFitStyle
		{
			get
			{
				return ultraGrid1.DisplayLayout.AutoFitStyle;
			}
			set
			{
				ultraGrid1.DisplayLayout.AutoFitStyle = value;
			}
		}

		/// <summary>
		/// 边框
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new UIElementBorderStyle BorderStyle
		{
			get
			{
				return ultraGrid1.DisplayLayout.BorderStyle;
			}
			set
			{
				ultraGrid1.DisplayLayout.BorderStyle = value;
			}
		}

		/// <summary>
		/// 数据行
		/// </summary>
		public RowsCollection Rows => ultraGrid1.Rows;

		/// <summary>
		/// UltraGrid对象
		/// </summary>
		public UltraGrid Grid => ultraGrid1;

		/// <summary>
		/// 数据源
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public object DataSource
		{
			get
			{
				return ultraGrid1.DataSource;
			}
			set
			{
				ultraGrid1.DataSource = value;
				ultraGrid1.ActiveRow = null;
			}
		}

		/// <summary>
		/// 初始化打印和预览事件
		/// </summary>
		public event InitializePrintEventHandler InitializePrint_Preview;

		/// <summary>
		/// 初始化逻辑页事件
		/// </summary>
		public event InitializeLogicalPrintPageEventHandler InitializeLogicalPrintPage;

		/// <summary>
		/// 选择行改变动作触发器
		/// </summary>
		public event AfterSelectChangeEventHandler AfterRowActivate;

		/// <summary>
		/// 创建GriffinsSelectGrid_Array新实例
		/// </summary>
		public RecSelectGrid_Array()
		{
			InitializeComponent();
			initGrid();
		}

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary> 
		/// 设计器支持所需的方法 - 不要使用代码编辑器 
		/// 修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
			((System.ComponentModel.ISupportInitialize)ultraGrid1).BeginInit();
			SuspendLayout();
			ultraGrid1.DisplayLayout.MaxBandDepth = 1;
			ultraGrid1.DisplayLayout.Override.MaxSelectedCells = 1;
			ultraGrid1.DisplayLayout.Override.MaxSelectedRows = 1;
			ultraGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			ultraGrid1.Location = new System.Drawing.Point(0, 0);
			ultraGrid1.Name = "ultraGrid1";
			ultraGrid1.Size = new System.Drawing.Size(272, 248);
			ultraGrid1.TabIndex = 11;
			ultraGrid1.Text = "选择";
			ultraGrid1.InitializeLogicalPrintPage += new Infragistics.Win.UltraWinGrid.InitializeLogicalPrintPageEventHandler(ultraGrid1_InitializeLogicalPrintPage);
			ultraGrid1.InitializePrintPreview += new Infragistics.Win.UltraWinGrid.InitializePrintPreviewEventHandler(ultraGrid1_InitializePrintPreview);
			ultraGrid1.InitializePrint += new Infragistics.Win.UltraWinGrid.InitializePrintEventHandler(ultraGrid1_InitializePrint);
			ultraGrid1.Click += new System.EventHandler(ultraGrid1_Click);
			ultraGrid1.AfterRowActivate += new System.EventHandler(ultraGrid1_AfterRowActivate);
			//ultraGrid1.ContextMenuChanged += new System.EventHandler(RecSelectGrid_ContextMenuChanged);
			ultraGrid1.DoubleClick += new System.EventHandler(ultraGrid1_DoubleClick);
			base.Controls.Add(ultraGrid1);
			base.Name = "RecSelectGrid_Array";
			base.Size = new System.Drawing.Size(272, 248);
			//base.ContextMenuChanged += new System.EventHandler(RecSelectGrid_ContextMenuChanged);
			((System.ComponentModel.ISupportInitialize)ultraGrid1).EndInit();
			ResumeLayout(false);
		}

		private void initGrid()
		{
			setAppearances();
			CanSelectRow = true;
			removeFuncKeys();
			ultraGrid1.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
			ultraGrid1.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
			ultraGrid1.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
			ultraGrid1.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
			ultraGrid1.DisplayLayout.Override.MaxSelectedRows = -1;
			ReadOnly = true;
			RowSelectors = false;
			MutiRowSelect = false;
		}

		private void setAppearances()
		{
			ultraGrid1.DisplayLayout.CaptionAppearance = GriffinsAppearances.DataGridCaption;
			ultraGrid1.DisplayLayout.Appearance = GriffinsAppearances.BackAppearance;
			ultraGrid1.DisplayLayout.Override.HeaderAppearance = GriffinsAppearances.DataGridColumnHeaderAppearance;
			ultraGrid1.DisplayLayout.Override.RowSelectorAppearance = GriffinsAppearances.DataGridRowSelectorAppearance;
		}

		private void removeFuncKeys()
		{
			foreach (GridKeyActionMapping keyMap in ultraGrid1.KeyActionMappings)
			{
				if (keyMap.KeyCode == Keys.Left || keyMap.KeyCode == Keys.Right || keyMap.KeyCode == Keys.Delete || keyMap.KeyCode == Keys.Return)
				{
					ultraGrid1.KeyActionMappings.Remove(keyMap);
				}
			}
		}

		/// <summary>
		/// 取字段（或属性）的网格列
		/// </summary>
		/// <param name="fieldName">字段（或属性）名</param>
		/// <returns>字段（或属性）的网格列</returns>
		public UltraGridColumn GetColumn(string fieldName)
		{
			return ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName];
		}

		/// <summary>
		/// 设置列的显示标题
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="caption">显示标题</param>
		public void SetColumnCaption(string fieldName, string caption)
		{
			ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName].Header.Caption = caption;
		}

		/// <summary>
		/// 设置列宽
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="width">宽度</param>
		public void SetColumnWidth(string fieldName, int width)
		{
			ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName].Width = width;
		}

		/// <summary>
		/// 设置列编辑控制对象
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="editorControl">编辑控制对象</param>
		public void SetColumnEditorControl(string fieldName, Control editorControl)
		{
			UltraGridColumn ultraGridColumn = ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName];
			ultraGridColumn.EditorComponent = editorControl;
			if (ultraGridColumn.DataType == typeof(bool))
			{
				ultraGridColumn.ValueList = null;
				ultraGridColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
			}
		}

		/// <summary>
		/// 设置列显示位置
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="visiblePosition">显示位置</param>
		public void SetColumnVisiblePosition(string fieldName, int visiblePosition)
		{
			ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName].Header.VisiblePosition = visiblePosition;
		}

		/// <summary>
		/// 设置列对齐方式
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="hAlign">对齐方式</param>
		public void SetColumnVisiblePosition(string fieldName, HAlign hAlign)
		{
			ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName].CellAppearance.TextHAlign = hAlign;
		}

		/// <summary>
		/// 设置列格式信息
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="format">格式信息</param>
		public void SetColumnVisiblePosition(string fieldName, string format)
		{
			if (format != null)
			{
				ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName].Format = format;
			}
		}

		/// <summary>
		/// 设置列背景色
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="backColor">背景色</param>
		public void SetColumnVisiblePosition(string fieldName, Color backColor)
		{
			if (backColor != Color.Empty)
			{
				ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName].CellAppearance.BackColor = backColor;
			}
		}

		/// <summary>
		/// 设置列的可见性
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="visible">可见性</param>
		public void SetColumnVisible(string fieldName, bool visible)
		{
			ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName].Hidden = !visible;
		}

		/// <summary>
		/// 设置列是否可编辑
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="allowEdit">是否允许编辑</param>
		public void SetColumnAllowEdit(string fieldName, bool allowEdit)
		{
			UltraGridColumn ultraGridColumn = ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName];
			ultraGridColumn.AutoEdit = allowEdit;
			if (allowEdit)
			{
				ultraGridColumn.CellActivation = Activation.AllowEdit;
				if (ultraGridColumn.DataType == typeof(bool))
				{
					ultraGridColumn.ValueList = null;
					ultraGridColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
				}
				return;
			}
			ultraGridColumn.CellActivation = Activation.NoEdit;
			if (ultraGridColumn.DataType == typeof(bool) && ultraGridColumn.EditorControl == null)
			{
				ValueList boolPeriodList = ultraGrid1.DisplayLayout.ValueLists.Add();
				boolPeriodList.ValueListItems.Add(true, "√");
				boolPeriodList.ValueListItems.Add(false, "x");
				boolPeriodList.DisplayStyle = ValueListDisplayStyle.DisplayText;
				ultraGridColumn.ValueList = boolPeriodList;
				ultraGridColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
				ultraGridColumn.CellAppearance.TextHAlign = HAlign.Center;
			}
		}

		/// <summary>
		/// 设置显示为空白的值（比如日期时间DateTime.MinValue显示为空白）
		/// </summary>
		/// <param name="fieldName">字段名</param>
		/// <param name="emptyValue">显示为空白的值</param>
		public void SetDisplayEmptyValue(string fieldName, object emptyValue)
		{
			UltraGridColumn ultraGridColumn = ultraGrid1.DisplayLayout.Bands[0].Columns[fieldName];
			ValueList valueList = ultraGrid1.DisplayLayout.ValueLists.Add(fieldName + "_ValueList");
			valueList.DisplayStyle = ValueListDisplayStyle.DisplayText;
			valueList.ValueListItems.Add(emptyValue, " ");
			ultraGridColumn.ValueList = valueList;
		}

		/// <summary>
		/// 修改显示列属性
		/// </summary>
		/// <param name="fieldName">子段名</param>
		/// <param name="caption">显示标题</param>
		/// <param name="width">宽度</param>
		/// <param name="editorControl">编辑控制对象</param>
		/// <param name="visiblePosition">显示位置</param>
		/// <param name="hAlign">对齐方式</param>
		/// <param name="format">格式信息</param>
		/// <param name="backColor">背景色</param>
		public UltraGridColumn UpdateColumn(string fieldName, string caption, int width, Control editorControl, int visiblePosition, HAlign hAlign, string format, Color backColor)
		{
			UltraGridColumn ultraGridColumn = GetColumn(fieldName);
			if (ultraGridColumn == null)
			{
				return null;
			}
			ultraGridColumn.Header.Caption = caption;
			ultraGridColumn.Width = width;
			ultraGridColumn.Header.VisiblePosition = visiblePosition;
			if (format != null)
			{
				ultraGridColumn.Format = format;
			}
			ultraGridColumn.CellAppearance.TextHAlign = hAlign;
			if (backColor != Color.Empty)
			{
				ultraGridColumn.CellAppearance.BackColor = backColor;
			}
			if (editorControl != null)
			{
				if (editorControl is IValueList)
				{
					ultraGridColumn.ValueList = (IValueList)editorControl;
				}
				ultraGridColumn.EditorControl = editorControl;
			}
			if (editorControl == null && ultraGridColumn.DataType == typeof(bool))
			{
				ValueList boolPeriodList = ultraGrid1.DisplayLayout.ValueLists.Add();
				boolPeriodList.ValueListItems.Add(true, "√");
				boolPeriodList.ValueListItems.Add(false, "x");
				boolPeriodList.DisplayStyle = ValueListDisplayStyle.DisplayText;
				ultraGridColumn.ValueList = boolPeriodList;
				ultraGridColumn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
				ultraGridColumn.CellAppearance.TextHAlign = HAlign.Center;
			}
			return ultraGridColumn;
		}

		/// <summary>
		/// 刷新
		/// </summary>
		public void RefurbishGrid()
		{
			ultraGrid1.Rows.Refresh(RefreshRow.RefreshDisplay);
		}

		/// <summary>
		/// 刷新
		/// </summary>
		/// <param name="refreshRow">刷新行方式</param>
		public void RefurbishGrid(RefreshRow refreshRow)
		{
			ultraGrid1.Rows.Refresh(refreshRow);
		}

		/// <summary>
		/// 设置执行行为激活状态
		/// </summary>
		/// <param name="index">行号</param>
		public void SetActivatedRow(int index)
		{
			if (index < ultraGrid1.Rows.Count && index >= 0)
			{
				ultraGrid1.Rows[index].Activated = true;
			}
			else if (index < 0)
			{
				for (int i = 0; i < ultraGrid1.Rows.Count; i++)
				{
					ultraGrid1.Rows[i].Activated = false;
				}
			}
		}

		/// <summary>
		/// 设置指定项对应的行为激活状态
		/// </summary>
		/// <param name="listItem">指定项</param>
		public void SetActivatedRow(object listItem)
		{
			UltraGridRow gridRow = GetRowOfListItem(listItem);
			if (gridRow != null)
			{
				gridRow.Activated = true;
				return;
			}
			for (int i = 0; i < ultraGrid1.Rows.Count; i++)
			{
				ultraGrid1.Rows[i].Activated = false;
			}
		}

		/// <summary>
		/// 设置执行行为激活状态
		/// </summary>
		/// <param name="index">行号</param>
		/// <param name="backColor">指定项对应行的背景颜色</param>
		/// <param name="foreColor">指定项对应行的前景颜色</param>
		public void SetRowColor(int index, Color backColor, Color foreColor)
		{
			if (index < ultraGrid1.Rows.Count && index >= 0)
			{
				ultraGrid1.Rows[index].Appearance.BackColor = backColor;
				ultraGrid1.Rows[index].Appearance.ForeColor = foreColor;
			}
		}

		/// <summary>
		/// 设置指定项对应的行颜色
		/// </summary>
		/// <param name="listItem">指定项</param>
		/// <param name="backColor">指定项对应行的背景颜色</param>
		/// <param name="foreColor">指定项对应行的前景颜色</param>
		public void SetRowColor(object listItem, Color backColor, Color foreColor)
		{
			UltraGridRow gridRow = GetRowOfListItem(listItem);
			if (gridRow != null)
			{
				gridRow.Appearance.BackColor = backColor;
				gridRow.Appearance.ForeColor = foreColor;
			}
		}

		/// <summary>
		/// 取指定项对应的行
		/// </summary>
		/// <param name="listItem">指定项</param>
		public UltraGridRow GetRowOfListItem(object listItem)
		{
			foreach (UltraGridRow gridRow in ultraGrid1.Rows)
			{
				if (gridRow.ListObject.Equals(listItem))
				{
					return gridRow;
				}
			}
			return null;
		}

		/// <summary>
		/// 打印
		/// </summary>
		public void Print()
		{
			ultraGrid1.Print();
		}

		/// <summary>
		/// 打印
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="printDocument"></param>
		public void Print(UltraGridLayout layout, PrintDocument printDocument)
		{
			ultraGrid1.Print(layout, printDocument);
		}

		private void ultraGrid1_InitializePrintPreview(object sender, CancelablePrintPreviewEventArgs e)
		{
			if (this.InitializePrint_Preview != null)
			{
				this.InitializePrint_Preview(sender, e);
			}
		}

		private void ultraGrid1_InitializePrint(object sender, CancelablePrintEventArgs e)
		{
			if (this.InitializePrint_Preview != null)
			{
				this.InitializePrint_Preview(sender, e);
			}
		}

		private void ultraGrid1_InitializeLogicalPrintPage(object sender, CancelableLogicalPrintPageEventArgs e)
		{
			if (this.InitializeLogicalPrintPage != null)
			{
				this.InitializeLogicalPrintPage(sender, e);
			}
		}

		/// <summary>
		/// 预览
		/// </summary>
		public void PrintPreview()
		{
			ultraGrid1.PrintPreview();
		}

		/// <summary>
		/// 预览
		/// </summary>
		/// <param name="layout"></param>
		/// <param name="printDocument"></param>
		public void PrintPreview(UltraGridLayout layout, PrintDocument printDocument)
		{
			ultraGrid1.PrintPreview(layout, printDocument);
		}

		/// <summary>
		/// 保存UltraGrid的数据到EXCEL文件中
		/// 引用:
		/// 	using Infragistics.Excel;
		/// 	using Infragistics.Win.UltraWinGrid;
		/// </summary>
		/// <param name="sheetName">电子表格页名称</param>
		/// <param name="pathName">文件名称</param>
		public void SaveToExcel(string sheetName, string pathName)
		{
			UltraGrid uGrid = ultraGrid1;
			//Workbook wb = new Workbook();
			//Worksheet ws = wb.Worksheets.Add(sheetName);
			//save_RowTitles(uGrid, ws);
			//save_RowDatas(uGrid, ws);
			//BIFF8Writer.WriteWorkbookToFile(wb, pathName);
		}

		//private void save_RowTitles(UltraGrid uGrid, Worksheet ws)
		//{
		//	int i = 0;
		//	ws.Rows[0].Height = 300;
		//	foreach (UltraGridColumn col in uGrid.Rows.Band.Columns)
		//	{
		//		if (!col.Hidden)
		//		{
		//			ws.Rows[0].Cells[i].CellFormat.Font.Color = Color.Blue;
		//			ws.Rows[0].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
		//			ws.Rows[0].Cells[i].Value = col.Header.Caption;
		//			i++;
		//		}
		//	}
		//}

		//private void save_RowDatas(UltraGrid uGrid, Worksheet ws)
		//{
		//	int i = 1;
		//	foreach (UltraGridRow row in uGrid.Rows)
		//	{
		//		save_OneRowData(uGrid, row, ws, i++);
		//	}
		//}

		//private void save_OneRowData(UltraGrid uGrid, UltraGridRow row, Worksheet ws, int rowIndex)
		//{
		//	int i = 0;
		//	int colIndex = 0;
		//	foreach (UltraGridColumn col in uGrid.Rows.Band.Columns)
		//	{
		//		if (!col.Hidden)
		//		{
		//			ws.Rows[rowIndex].Cells[i].Value = row.Cells[colIndex].Text;
		//			i++;
		//		}
		//		colIndex++;
		//	}
		//}

		private void ultraGrid1_AfterRowActivate(object sender, EventArgs e)
		{
			if (!lockSelectChange)
			{
				doSelectRow(ultraGrid1.ActiveRow);
			}
		}

		private void doSelectRow(UltraGridRow gridRow)
		{
			if (this.AfterRowActivate != null)
			{
				this.AfterRowActivate(this, new AfterSelectChangeEventArgs(gridRow));
			}
		}

		private void ultraGrid1_Click(object sender, EventArgs e)
		{
			OnClick(e);
		}

		private void ultraGrid1_DoubleClick(object sender, EventArgs e)
		{
			OnDoubleClick(e);
		}

		private void RecSelectGrid_ContextMenuChanged(object sender, EventArgs e)
		{
			//ultraGrid1.ContextMenu = ContextMenuStrip;
		}
	}
}
