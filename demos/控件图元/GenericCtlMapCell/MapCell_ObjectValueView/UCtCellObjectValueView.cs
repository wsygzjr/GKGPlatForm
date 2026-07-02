using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Griffins;
using Griffins.Graph;
using Griffins.UI;
using Griffins.MpDevFlowMng;
using Griffins.MpDevFlowMng.Client;

namespace Griffins.Map.CtlMapCell.Generic.ObjectValueView
{
    /// <summary>
    /// 管理点管理节点下直接管理的管理点列表用户控件
    /// </summary>
    internal partial class UCtCellObjectValueView : UserControl
    {
        private bool designTime;
        private IControlMapCellCallBack iMapCellCallBack;
        public UCtCellObjectValueView(bool designTime)
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

        private Guid lastObject_ID = Guid.Empty;
        private IObjectValueView_Control curIObjectValueView_Control = null;

        public void SetObjectValue(ObjectValue objectValue)
        {
            Guid object_ID = getObject_ID(objectValue);
            if (object_ID != lastObject_ID)
            {
                //清除原有的对象显示控件
                for (int i = this.Controls.Count - 1; i >= 0; i--)
                {
                    Control tmp = this.Controls[i];
                    this.Controls.RemoveAt(i);
                    tmp.Dispose();
                }
                this.Controls.Clear();

                //得到对象ID对应的对象显示控件
                IObjectValueViewMng iObjectValueViewMng = ObjectValueViewPluginMng.CreateObjectValueView(object_ID);
                curIObjectValueView_Control = iObjectValueViewMng.CreateControl();

                Control view = curIObjectValueView_Control.View;
                this.SuspendLayout();
                view.Dock = DockStyle.Fill;
                this.Controls.Add(view);
                this.ResumeLayout();

                this.lastObject_ID = object_ID;
            }

            if (curIObjectValueView_Control != null)
            {
                if (objectValue != null)
                    curIObjectValueView_Control.SetData(objectValue.XMLVal);
                else
                    curIObjectValueView_Control.SetData(null);
            }
        }

        private Guid getObject_ID(ObjectValue objectValue)
        {
            if (objectValue == null)
                return Guid.Empty;
            else
                return objectValue.Object_ID;
        }
    }
}
