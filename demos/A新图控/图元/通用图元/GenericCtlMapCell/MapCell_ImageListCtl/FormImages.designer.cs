namespace Griffins.Map.CtlMapCell.Generic
{
    partial class FormImages
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImages));
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraButtonOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraButtonCancel = new Infragistics.Win.Misc.UltraButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxKey = new System.Windows.Forms.GroupBox();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.ultraButtonUpdate = new Infragistics.Win.Misc.UltraButton();
            this.ultraButtonRemove = new Infragistics.Win.Misc.UltraButton();
            this.ultraButtonAdd = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxKey.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraGroupBox2
            // 
            appearance1.BackColor = System.Drawing.Color.WhiteSmoke;
            appearance1.BackColor2 = System.Drawing.Color.Silver;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.ultraGroupBox2.Appearance = appearance1;
            this.ultraGroupBox2.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.ultraGroupBox2.Controls.Add(this.ultraButtonOK);
            this.ultraGroupBox2.Controls.Add(this.ultraButtonCancel);
            this.ultraGroupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraGroupBox2.Location = new System.Drawing.Point(0, 250);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(335, 45);
            this.ultraGroupBox2.SupportThemes = false;
            this.ultraGroupBox2.TabIndex = 127;
            // 
            // ultraButtonOK
            // 
            this.ultraButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.Color.White;
            appearance2.BackColor2 = System.Drawing.Color.Silver;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            this.ultraButtonOK.Appearance = appearance2;
            this.ultraButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ultraButtonOK.HotTracking = true;
            this.ultraButtonOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ultraButtonOK.Location = new System.Drawing.Point(160, 9);
            this.ultraButtonOK.Name = "ultraButtonOK";
            this.ultraButtonOK.Size = new System.Drawing.Size(80, 27);
            this.ultraButtonOK.TabIndex = 16;
            this.ultraButtonOK.Text = "确认";
            // 
            // ultraButtonCancel
            // 
            this.ultraButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.BackColor = System.Drawing.Color.White;
            appearance3.BackColor2 = System.Drawing.Color.Silver;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            this.ultraButtonCancel.Appearance = appearance3;
            this.ultraButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ultraButtonCancel.HotTracking = true;
            this.ultraButtonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ultraButtonCancel.Location = new System.Drawing.Point(248, 9);
            this.ultraButtonCancel.Name = "ultraButtonCancel";
            this.ultraButtonCancel.Size = new System.Drawing.Size(80, 27);
            this.ultraButtonCancel.TabIndex = 17;
            this.ultraButtonCancel.Text = "取消";
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(242, 248);
            this.listView1.TabIndex = 128;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(64, 64);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.groupBoxKey);
            this.panel1.Controls.Add(this.ultraButtonUpdate);
            this.panel1.Controls.Add(this.ultraButtonRemove);
            this.panel1.Controls.Add(this.ultraButtonAdd);
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(335, 250);
            this.panel1.TabIndex = 129;
            // 
            // groupBoxKey
            // 
            this.groupBoxKey.Controls.Add(this.textBoxKey);
            this.groupBoxKey.Location = new System.Drawing.Point(248, 3);
            this.groupBoxKey.Name = "groupBoxKey";
            this.groupBoxKey.Size = new System.Drawing.Size(77, 48);
            this.groupBoxKey.TabIndex = 132;
            this.groupBoxKey.TabStop = false;
            this.groupBoxKey.Text = "主键";
            // 
            // textBoxKey
            // 
            this.textBoxKey.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxKey.Location = new System.Drawing.Point(3, 17);
            this.textBoxKey.Name = "textBoxKey";
            this.textBoxKey.Size = new System.Drawing.Size(71, 21);
            this.textBoxKey.TabIndex = 0;
            this.textBoxKey.TextChanged += new System.EventHandler(this.textBoxKey_TextChanged);
            // 
            // ultraButtonUpdate
            // 
            appearance4.BackColor = System.Drawing.Color.White;
            appearance4.BackColor2 = System.Drawing.Color.Silver;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.ultraButtonUpdate.Appearance = appearance4;
            this.ultraButtonUpdate.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.ultraButtonUpdate.HotTracking = true;
            this.ultraButtonUpdate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ultraButtonUpdate.Location = new System.Drawing.Point(248, 121);
            this.ultraButtonUpdate.Name = "ultraButtonUpdate";
            this.ultraButtonUpdate.Size = new System.Drawing.Size(77, 27);
            this.ultraButtonUpdate.TabIndex = 131;
            this.ultraButtonUpdate.Text = "修改图片";
            this.ultraButtonUpdate.Click += new System.EventHandler(this.ultraButtonUpdate_Click);
            // 
            // ultraButtonRemove
            // 
            appearance5.BackColor = System.Drawing.Color.White;
            appearance5.BackColor2 = System.Drawing.Color.Silver;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.ultraButtonRemove.Appearance = appearance5;
            this.ultraButtonRemove.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.ultraButtonRemove.HotTracking = true;
            this.ultraButtonRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ultraButtonRemove.Location = new System.Drawing.Point(248, 88);
            this.ultraButtonRemove.Name = "ultraButtonRemove";
            this.ultraButtonRemove.Size = new System.Drawing.Size(77, 27);
            this.ultraButtonRemove.TabIndex = 130;
            this.ultraButtonRemove.Text = "移除";
            this.ultraButtonRemove.Click += new System.EventHandler(this.ultraButtonRemove_Click);
            // 
            // ultraButtonAdd
            // 
            appearance6.BackColor = System.Drawing.Color.White;
            appearance6.BackColor2 = System.Drawing.Color.Silver;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.ultraButtonAdd.Appearance = appearance6;
            this.ultraButtonAdd.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.ultraButtonAdd.HotTracking = true;
            this.ultraButtonAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ultraButtonAdd.Location = new System.Drawing.Point(248, 57);
            this.ultraButtonAdd.Name = "ultraButtonAdd";
            this.ultraButtonAdd.Size = new System.Drawing.Size(77, 27);
            this.ultraButtonAdd.TabIndex = 129;
            this.ultraButtonAdd.Text = "添加";
            this.ultraButtonAdd.Click += new System.EventHandler(this.ultraButtonAdd_Click);
            // 
            // FormImages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 295);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ultraGroupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormImages";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "图形列表";
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBoxKey.ResumeLayout(false);
            this.groupBoxKey.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.Misc.UltraButton ultraButtonOK;
        private Infragistics.Win.Misc.UltraButton ultraButtonCancel;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel1;
        private Infragistics.Win.Misc.UltraButton ultraButtonRemove;
        private Infragistics.Win.Misc.UltraButton ultraButtonAdd;
        private System.Windows.Forms.GroupBox groupBoxKey;
        private System.Windows.Forms.TextBox textBoxKey;
        private Infragistics.Win.Misc.UltraButton ultraButtonUpdate;
    }
}