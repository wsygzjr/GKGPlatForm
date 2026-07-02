using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Griffins;
using Griffins.Graph;

namespace Griffins.Map.CtlMapCell.Generic.ImageCtl
{
    /// <summary>
    /// 管理点管理节点下直接管理的管理点列表用户控件
    /// </summary>
    internal partial class UCtCellImageCtlView : UserControl
    {
        private bool designTime;
        private IControlMapCellCallBack iMapCellCallBack;
        public UCtCellImageCtlView(bool designTime)
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

        public void SetImage(Image val)
        {
            this.pictureBox1.Image = val;
        }

        public void SetImageSizeMode(PictureBoxSizeMode val)
        {
            this.pictureBox1.SizeMode = val;
        }
        public void SetBackColor(Color val)
        {
            this.pictureBox1.BackColor = val;
        }

        public void SetCursor(CtlCellCursor val)
        {
            this.pictureBox1.Cursor = getCursor(val);
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


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //把坐标转换成屏幕坐标
            Point screenP = this.pictureBox1.PointToScreen(new Point(e.X, e.Y));
            object retVal;
            GraphMouseEventParam graphMouseEventParam = new GraphMouseEventParam(this.iMapCellCallBack.BindingInfo, e.Button, screenP.X, screenP.Y);
            this.iMapCellCallBack.ExecMapCellEvent(MapObjPropEventConst.Event_MouseClick, graphMouseEventParam, out retVal);

        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //把坐标转换成屏幕坐标
            Point screenP = this.pictureBox1.PointToScreen(new Point(e.X, e.Y));
            object retVal;
            GraphMouseEventParam graphMouseEventParam = new GraphMouseEventParam(this.iMapCellCallBack.BindingInfo, e.Button, screenP.X, screenP.Y);
            this.iMapCellCallBack.ExecMapCellEvent(MapObjPropEventConst.Event_MouseDoubleClick, graphMouseEventParam, out retVal);

        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            //把坐标转换成屏幕坐标

            Point screenP = this.pictureBox1.PointToScreen(new Point(0,0));
            object retVal;
            GraphMouseEventParam graphMouseEventParam = new GraphMouseEventParam(this.iMapCellCallBack.BindingInfo, MouseButtons.None, screenP.X, screenP.Y);
            this.iMapCellCallBack.ExecMapCellEvent(MapObjPropEventConst.Event_MouseEnter, graphMouseEventParam, out retVal);

        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            //把坐标转换成屏幕坐标
            Point screenP = this.pictureBox1.PointToScreen(new Point(0, 0));
            object retVal;
            GraphMouseEventParam graphMouseEventParam = new GraphMouseEventParam(this.iMapCellCallBack.BindingInfo, MouseButtons.None, screenP.X, screenP.Y);
            this.iMapCellCallBack.ExecMapCellEvent(MapObjPropEventConst.Event_MouseLeave, graphMouseEventParam, out retVal);

        }
    }

   
}
