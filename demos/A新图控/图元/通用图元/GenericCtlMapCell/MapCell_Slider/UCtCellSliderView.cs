using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Griffins;
using Griffins.Graph;

namespace Griffins.Map.CtlMapCell.Generic.Slider
{
    /// <summary>
    /// 管理点管理节点下直接管理的管理点列表用户控件
    /// </summary>
    internal partial class UCtCellSliderView : UserControl
    {
        private bool designTime;
        private IControlMapCellCallBack iMapCellCallBack;
        public UCtCellSliderView(bool designTime)
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
            this.slider1.Maximum = Convert.ToInt32(val);
        }

        public void SetMinimum(decimal val)
        {
            this.slider1.Minimum = Convert.ToInt32(val);
        }

        public void SetIncrement(decimal val)
        {
            this.slider1.Step = Convert.ToInt32(val);
        }

        public void SetOrientation(SliderOrientation val)
        {
            switch (val)
            {
                case SliderOrientation.Horizontal:
                    this.slider1.SliderOrientation = DevComponents.DotNetBar.eOrientation.Horizontal;
                    break;
                case SliderOrientation.Vertical:
                    this.slider1.SliderOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
                    break;
            }
        }

        private bool lockModified = false;
        public void SetValue(decimal val)
        {
            lockModified = true;
            try
            {
                this.slider1.Value = Convert.ToInt32(val);
            }
            finally
            {
                lockModified = false;
            }
        }
       
        public void SetBackColor(Color val)
        {
            this.slider1.BackColor = val;
        }

        public void SetCursor(CtlCellCursor val)
        {
            this.slider1.Cursor = getCursor(val);
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

        private void slider1_ValueChanged(object sender, EventArgs e)
        {
            if (lockModified)
                return;
            object retVal;
            this.iMapCellCallBack.ExecMapCellEvent(MapCellSliderObj.Event_ValueChanged,Convert.ToDecimal(this.slider1.Value), out retVal);
        }

       
    }

   
}
