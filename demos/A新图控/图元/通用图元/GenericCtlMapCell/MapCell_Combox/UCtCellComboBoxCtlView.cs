using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Griffins;
using Griffins.Graph;

namespace Griffins.Map.CtlMapCell.Generic.ComboBox
{
    /// <summary>
    /// 管理点管理节点下直接管理的管理点列表用户控件
    /// </summary>
    internal partial class UCtCellComboBoxCtlView : UserControl
    {
        private bool designTime;
        private DecamalValueNameList valueNameList;
        private IControlMapCellCallBack iMapCellCallBack;
        public UCtCellComboBoxCtlView(bool designTime)
        {
            iMapCellCallBack = null;
            this.designTime = designTime;
            InitializeComponent();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(IControlMapCellCallBack iMapCellCallBack)
        {
            this.iMapCellCallBack = iMapCellCallBack;
        }

        public void SetValueNameList(DecamalValueNameList valueNameList)
        {
            this.valueNameList = valueNameList;
            this.comboBox1.Items.Clear();
            if (valueNameList != null)
            {
                foreach (DecimalValueName valueName in valueNameList)
                {
                    this.comboBox1.Items.Add(valueName.Name);
                }
            }

            SetValue(this.val);
        }

        private int indexOfValue(decimal val)
        {
            if (this.valueNameList == null)
                return -1;
            for(int i=0;i< this.valueNameList.Count;i++)
            {
                if (this.valueNameList[i].Val == val)
                    return i;
            }
            return -1;
        }


        private decimal val = 0;
        private bool lockModified = false;
        public void SetValue(decimal val)
        {
            this.val = val;
            lockModified = true;
            try
            {
                if (this.comboBox1.Items.Count > 0)
                {
                    this.comboBox1.SelectedIndex = indexOfValue(val);
                }
            }
            finally
            {
                lockModified = false;
            }
        }
       
        public void SetBackColor(Color val)
        {
            this.comboBox1.BackColor = val;
        }

        public void SetComboBoxTextFont(Font val)
        {
            this.comboBox1.Font = val;
        }

        public void SetComboBoxTextColor(Color val)
        {
            this.comboBox1.ForeColor = val;
        }

        public void SetCursor(CtlCellCursor val)
        {
            this.comboBox1.Cursor = getCursor(val);
        }

        private Cursor getCursor(CtlCellCursor val)
        {
            switch (val)
            {
                case CtlCellCursor.Default:
                    return Cursors.Default;
                case CtlCellCursor.Arrow:
                    return Cursors.Arrow;
                case CtlCellCursor.Cross:
                    return Cursors.Cross;
                case CtlCellCursor.Hand:
                    return Cursors.Hand;
                case CtlCellCursor.Help:
                    return Cursors.Help;
                case CtlCellCursor.IBeam:
                    return Cursors.IBeam;
                case CtlCellCursor.No:
                    return Cursors.No;
                default:
                    return Cursors.Default;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lockModified)
                return;            
            if (this.comboBox1.SelectedIndex < 0)
                return;
            if (this.valueNameList == null)
                return;
            if (this.comboBox1.SelectedIndex >= this.valueNameList.Count)
                return;
            object retVal;
            this.iMapCellCallBack.ExecMapCellEvent(MapCellComboBoxCtlObj.Event_ValueChanged, this.valueNameList[this.comboBox1.SelectedIndex].Val, out retVal);

        }
    }
}
