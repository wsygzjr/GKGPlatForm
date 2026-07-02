using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Griffins;
using Griffins.Graph;

namespace Griffins.Map.CtlMapCell.Generic.NumUpDown
{
    /// <summary>
    /// 管理点管理节点下直接管理的管理点列表用户控件
    /// </summary>
    internal partial class UCtCellNumUpDownCtlView : UserControl
    {
        private bool designTime;
        private IControlMapCellCallBack iMapCellCallBack;
        public UCtCellNumUpDownCtlView(bool designTime)
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

        public void SetMaximum(decimal val)
        {
            this.numericUpDown1.Maximum = val;
        }

        public void SetMinimum(decimal val)
        {
            this.numericUpDown1.Minimum = val;
        }

        public void SetIncrement(decimal val)
        {
            this.numericUpDown1.Increment = val;
        }

        private bool lockModified = false;
        public void SetValue(decimal val)
        {
            lockModified = true;
            try
            {
                this.numericUpDown1.Value = val;
            }
            finally
            {
                lockModified = false;
            }
        }
       
        public void SetBackColor(Color val)
        {
            this.numericUpDown1.BackColor= val;
        }

      

        public void SetNumUpDownTextFont(Font val)
        {
            this.numericUpDown1.Font = val;
        }

        public void SetNumUpDownTextColor(Color val)
        {
            this.numericUpDown1.ForeColor = val;
        }

        public void SetCursor(CtlCellCursor val)
        {
            this.numericUpDown1.Cursor = getCursor(val);
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (lockModified)
                return;
            object retVal;
            this.iMapCellCallBack.ExecMapCellEvent(MapCellNumUpDownCtlObj.Event_ValueChanged, this.numericUpDown1.Value, out retVal);
        }

       
    }

   
}
