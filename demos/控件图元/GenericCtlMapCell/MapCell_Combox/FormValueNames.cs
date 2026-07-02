using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Griffins.Graph;
using Infragistics.Win.UltraWinGrid;

namespace Griffins.Map.CtlMapCell.Generic.ComboBox
{
    partial class FormValueNames : Form
    {
        public FormValueNames()
        {
            InitializeComponent();
            valueNameList = new DecamalValueNameList();
            this.valueNamesGrid.DataSource = valueNameList;
            setDisplayColumns();
        }
        private UltraGridColumn sortColumn;
        private void setDisplayColumns()
        {
            UltraGridColumn valColumn = valueNamesGrid.UpdateColumn("Val", "值", 80, null, 2, Infragistics.Win.HAlign.Left, null, Color.White);
            valueNamesGrid.SetColumnAllowEdit("Val", true);

            UltraGridColumn nameColumn = valueNamesGrid.UpdateColumn("Name", "值含义", 120, null, 3, Infragistics.Win.HAlign.Left, null, Color.White);
            valueNamesGrid.SetColumnAllowEdit("Name", true);
            sortColumn = valColumn;
            sortColumn.SortIndicator = SortIndicator.Ascending;
        }

        private DecamalValueNameList valueNameList;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DecamalValueNameList ValueNameList
        {
            get { return valueNameList; }
            set 
            {
                if (value == null)
                {
                    valueNameList.Clear();
                }
                else
                {
                    valueNameList.Clear();
                    foreach (DecimalValueName valueName in value)
                        valueNameList.Add(valueName);
                }
                fillValueNames();
            }
        }

        private void fillValueNames()
        {
            curGridRow = null;
            this.valueNamesGrid.DataSource = valueNameList;
            this.valueNamesGrid.RefurbishGrid(RefreshRow.RefreshDisplay);
            if (valueNameList.Count > 0)
                this.valueNamesGrid.SetActivatedRow(0);
            SortIndicator s = sortColumn.SortIndicator;
            sortColumn.SortIndicator = s;
        }
        private UltraGridRow curGridRow = null;
        /// <summary>
        /// 当前选择的项
        /// </summary>
        public DecimalValueName CurValueName
        {
            get
            {
                if (curGridRow == null)
                    return null;
                else
                    return (DecimalValueName)curGridRow.ListObject;
            }
        }
        private void valueNamesGrid_AfterRowActivate(object sender, Griffins.UI.RecSelectGrid_Array.AfterSelectChangeEventArgs e)
        {
            curGridRow = e.GridRow;
            if (curGridRow != null)
                this.toolStripButtonDelete.Enabled = true;
            else
                this.toolStripButtonDelete.Enabled = false;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (curGridRow == null)
            {
                this.menuItemDel.Enabled = false;
                this.toolStripButtonDelete.Enabled = false;
            }
            else
            {
                this.menuItemDel.Enabled = true;
                this.toolStripButtonDelete.Enabled = true;
            }
        }
        private void menuItemAdd_Click(object sender, EventArgs e)
        {
            DecimalValueName valueName = new DecimalValueName();
            valueName.Val = 0;
            valueName.Name = "";
            this.valueNameList.Add(valueName);
            this.valueNamesGrid.RefurbishGrid(RefreshRow.RefreshDisplay);
            this.valueNamesGrid.SetActivatedRow(valueName);
        }
        private void menuItemDel_Click(object sender, EventArgs e)
        {
            DecimalValueName valueName = this.CurValueName;
            if (valueName == null)
                return;
            this.valueNameList.Remove(valueName);
            this.valueNamesGrid.RefurbishGrid(RefreshRow.RefreshDisplay);
        }

        private void menuItemDelAll_Click(object sender, EventArgs e)
        {
            this.valueNameList.Clear();
            this.valueNamesGrid.RefurbishGrid(RefreshRow.RefreshDisplay);
        }
    }
}