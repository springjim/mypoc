namespace POCControlCenter.Agora.AVLive
{
    partial class AVLiveCenterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AVLiveCenterForm));
            this.panelTitle = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMinX = new System.Windows.Forms.Button();
            this.btnMaxNorm = new System.Windows.Forms.Button();
            this.btnCloseX = new System.Windows.Forms.Button();
            this.toolStripMenuItemStopLive = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStopMoni = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemVolume = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemResize = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemVideoRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripVideo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_rec = new System.Windows.Forms.Button();
            this.tableLayoutPanelVideo = new System.Windows.Forms.TableLayoutPanel();
            this.panelTitle.SuspendLayout();
            this.contextMenuStripVideo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTitle
            // 
            this.panelTitle.AllowDrop = true;
            this.panelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.panelTitle.Controls.Add(this.label1);
            this.panelTitle.Controls.Add(this.btnMinX);
            this.panelTitle.Controls.Add(this.btnMaxNorm);
            this.panelTitle.Controls.Add(this.btnCloseX);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Margin = new System.Windows.Forms.Padding(0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(705, 48);
            this.panelTitle.TabIndex = 3;
            this.panelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitle_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.label1.MinimumSize = new System.Drawing.Size(0, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 38);
            this.label1.TabIndex = 7;
            this.label1.Text = "图文直播中心";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnMinX
            // 
            this.btnMinX.AutoSize = true;
            this.btnMinX.BackColor = System.Drawing.Color.Transparent;
            this.btnMinX.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMinX.BackgroundImage")));
            this.btnMinX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnMinX.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnMinX.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnMinX.FlatAppearance.BorderSize = 0;
            this.btnMinX.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.HotTrack;
            this.btnMinX.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.HotTrack;
            this.btnMinX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinX.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnMinX.ForeColor = System.Drawing.Color.White;
            this.btnMinX.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnMinX.Location = new System.Drawing.Point(561, 0);
            this.btnMinX.Margin = new System.Windows.Forms.Padding(4);
            this.btnMinX.Name = "btnMinX";
            this.btnMinX.Size = new System.Drawing.Size(48, 48);
            this.btnMinX.TabIndex = 6;
            this.btnMinX.UseVisualStyleBackColor = true;
            this.btnMinX.Click += new System.EventHandler(this.btnMinX_Click);
            // 
            // btnMaxNorm
            // 
            this.btnMaxNorm.AutoSize = true;
            this.btnMaxNorm.BackColor = System.Drawing.Color.Transparent;
            this.btnMaxNorm.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMaxNorm.BackgroundImage")));
            this.btnMaxNorm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnMaxNorm.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnMaxNorm.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnMaxNorm.FlatAppearance.BorderSize = 0;
            this.btnMaxNorm.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.HotTrack;
            this.btnMaxNorm.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.HotTrack;
            this.btnMaxNorm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaxNorm.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnMaxNorm.ForeColor = System.Drawing.Color.White;
            this.btnMaxNorm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnMaxNorm.Location = new System.Drawing.Point(609, 0);
            this.btnMaxNorm.Margin = new System.Windows.Forms.Padding(4);
            this.btnMaxNorm.Name = "btnMaxNorm";
            this.btnMaxNorm.Size = new System.Drawing.Size(48, 48);
            this.btnMaxNorm.TabIndex = 5;
            this.btnMaxNorm.UseVisualStyleBackColor = true;
            this.btnMaxNorm.Click += new System.EventHandler(this.btnMaxNorm_Click);
            // 
            // btnCloseX
            // 
            this.btnCloseX.AutoSize = true;
            this.btnCloseX.BackColor = System.Drawing.Color.Transparent;
            this.btnCloseX.BackgroundImage = global::POCControlCenter.Properties.Resources.mainwnd_close;
            this.btnCloseX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCloseX.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCloseX.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCloseX.FlatAppearance.BorderSize = 0;
            this.btnCloseX.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.HotTrack;
            this.btnCloseX.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.HotTrack;
            this.btnCloseX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseX.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnCloseX.ForeColor = System.Drawing.Color.White;
            this.btnCloseX.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCloseX.Location = new System.Drawing.Point(657, 0);
            this.btnCloseX.Margin = new System.Windows.Forms.Padding(4);
            this.btnCloseX.Name = "btnCloseX";
            this.btnCloseX.Size = new System.Drawing.Size(48, 48);
            this.btnCloseX.TabIndex = 4;
            this.btnCloseX.UseVisualStyleBackColor = true;
            this.btnCloseX.Click += new System.EventHandler(this.btnCloseX_Click);
            // 
            // toolStripMenuItemStopLive
            // 
            this.toolStripMenuItemStopLive.Name = "toolStripMenuItemStopLive";
            this.toolStripMenuItemStopLive.Size = new System.Drawing.Size(159, 24);
            this.toolStripMenuItemStopLive.Text = "停止直播";
            // 
            // toolStripMenuItemStopMoni
            // 
            this.toolStripMenuItemStopMoni.Name = "toolStripMenuItemStopMoni";
            this.toolStripMenuItemStopMoni.Size = new System.Drawing.Size(159, 24);
            this.toolStripMenuItemStopMoni.Text = "停止监控";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(156, 6);
            // 
            // toolStripMenuItemVolume
            // 
            this.toolStripMenuItemVolume.Name = "toolStripMenuItemVolume";
            this.toolStripMenuItemVolume.Size = new System.Drawing.Size(159, 24);
            this.toolStripMenuItemVolume.Text = "禁音/放音";
            // 
            // toolStripMenuItemResize
            // 
            this.toolStripMenuItemResize.Name = "toolStripMenuItemResize";
            this.toolStripMenuItemResize.Size = new System.Drawing.Size(159, 24);
            this.toolStripMenuItemResize.Text = "最大化/还原";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(156, 6);
            // 
            // toolStripMenuItemVideoRefresh
            // 
            this.toolStripMenuItemVideoRefresh.Name = "toolStripMenuItemVideoRefresh";
            this.toolStripMenuItemVideoRefresh.Size = new System.Drawing.Size(159, 24);
            this.toolStripMenuItemVideoRefresh.Text = "画面刷新";
            // 
            // toolStripMenuItemCapture
            // 
            this.toolStripMenuItemCapture.Name = "toolStripMenuItemCapture";
            this.toolStripMenuItemCapture.Size = new System.Drawing.Size(159, 24);
            this.toolStripMenuItemCapture.Text = "抓拍";
            // 
            // contextMenuStripVideo
            // 
            this.contextMenuStripVideo.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.contextMenuStripVideo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCapture,
            this.toolStripMenuItemVideoRefresh,
            this.toolStripSeparator3,
            this.toolStripMenuItemResize,
            this.toolStripMenuItemVolume,
            this.toolStripSeparator2,
            this.toolStripMenuItemStopMoni,
            this.toolStripMenuItemStopLive});
            this.contextMenuStripVideo.Name = "contextMenuStripVideo";
            this.contextMenuStripVideo.Size = new System.Drawing.Size(160, 160);
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel1.Controls.Add(this.button7);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btn_rec);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 588);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(705, 52);
            this.panel1.TabIndex = 4;
            // 
            // button7
            // 
            this.button7.AutoSize = true;
            this.button7.BackColor = System.Drawing.Color.Transparent;
            this.button7.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button7.FlatAppearance.BorderSize = 0;
            this.button7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button7.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button7.ForeColor = System.Drawing.Color.White;
            this.button7.Image = global::POCControlCenter.Properties.Resources.layout_g20;
            this.button7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button7.Location = new System.Drawing.Point(590, 0);
            this.button7.Margin = new System.Windows.Forms.Padding(4);
            this.button7.Name = "button7";
            this.button7.Padding = new System.Windows.Forms.Padding(4);
            this.button7.Size = new System.Drawing.Size(52, 52);
            this.button7.TabIndex = 24;
            this.button7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.AutoSize = true;
            this.button6.BackColor = System.Drawing.Color.Transparent;
            this.button6.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button6.FlatAppearance.BorderSize = 0;
            this.button6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button6.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button6.ForeColor = System.Drawing.Color.White;
            this.button6.Image = global::POCControlCenter.Properties.Resources.layout_g16;
            this.button6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button6.Location = new System.Drawing.Point(510, 0);
            this.button6.Margin = new System.Windows.Forms.Padding(4);
            this.button6.Name = "button6";
            this.button6.Padding = new System.Windows.Forms.Padding(4);
            this.button6.Size = new System.Drawing.Size(52, 52);
            this.button6.TabIndex = 23;
            this.button6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.AutoSize = true;
            this.button5.BackColor = System.Drawing.Color.Transparent;
            this.button5.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Image = global::POCControlCenter.Properties.Resources.layout_g12;
            this.button5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button5.Location = new System.Drawing.Point(430, 0);
            this.button5.Margin = new System.Windows.Forms.Padding(4);
            this.button5.Name = "button5";
            this.button5.Padding = new System.Windows.Forms.Padding(4);
            this.button5.Size = new System.Drawing.Size(52, 52);
            this.button5.TabIndex = 22;
            this.button5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.AutoSize = true;
            this.button4.BackColor = System.Drawing.Color.Transparent;
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Image = global::POCControlCenter.Properties.Resources.layout_g9x;
            this.button4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button4.Location = new System.Drawing.Point(350, 0);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Padding = new System.Windows.Forms.Padding(4);
            this.button4.Size = new System.Drawing.Size(52, 52);
            this.button4.TabIndex = 21;
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.AutoSize = true;
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Image = global::POCControlCenter.Properties.Resources.layout_g8;
            this.button3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button3.Location = new System.Drawing.Point(270, 0);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Padding = new System.Windows.Forms.Padding(4);
            this.button3.Size = new System.Drawing.Size(52, 52);
            this.button3.TabIndex = 20;
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Image = global::POCControlCenter.Properties.Resources.layout_g6_x2;
            this.button2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button2.Location = new System.Drawing.Point(190, 0);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Padding = new System.Windows.Forms.Padding(4);
            this.button2.Size = new System.Drawing.Size(52, 52);
            this.button2.TabIndex = 19;
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Image = global::POCControlCenter.Properties.Resources.layout_g6;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(110, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Padding = new System.Windows.Forms.Padding(4);
            this.button1.Size = new System.Drawing.Size(52, 52);
            this.button1.TabIndex = 18;
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_rec
            // 
            this.btn_rec.AutoSize = true;
            this.btn_rec.BackColor = System.Drawing.Color.Transparent;
            this.btn_rec.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_rec.FlatAppearance.BorderSize = 0;
            this.btn_rec.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_rec.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
            this.btn_rec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_rec.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_rec.ForeColor = System.Drawing.Color.White;
            this.btn_rec.Image = global::POCControlCenter.Properties.Resources.layout_g4;
            this.btn_rec.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_rec.Location = new System.Drawing.Point(30, 0);
            this.btn_rec.Margin = new System.Windows.Forms.Padding(4);
            this.btn_rec.Name = "btn_rec";
            this.btn_rec.Padding = new System.Windows.Forms.Padding(4);
            this.btn_rec.Size = new System.Drawing.Size(52, 52);
            this.btn_rec.TabIndex = 17;
            this.btn_rec.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_rec.UseVisualStyleBackColor = true;
            this.btn_rec.Click += new System.EventHandler(this.btn_rec_Click);
            // 
            // tableLayoutPanelVideo
            // 
            this.tableLayoutPanelVideo.AllowDrop = true;
            this.tableLayoutPanelVideo.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanelVideo.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
            this.tableLayoutPanelVideo.ColumnCount = 3;
            this.tableLayoutPanelVideo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelVideo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelVideo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelVideo.Location = new System.Drawing.Point(0, 48);
            this.tableLayoutPanelVideo.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelVideo.Name = "tableLayoutPanelVideo";
            this.tableLayoutPanelVideo.RowCount = 2;
            this.tableLayoutPanelVideo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelVideo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelVideo.Size = new System.Drawing.Size(705, 540);
            this.tableLayoutPanelVideo.TabIndex = 6;
            this.tableLayoutPanelVideo.Tag = "999";
            // 
            // AVLiveCenterForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 640);
            this.Controls.Add(this.tableLayoutPanelVideo);
            this.Controls.Add(this.panelTitle);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AVLiveCenterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AVLiveCenterForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AVLiveCenterForm_FormClosed);
            this.Load += new System.EventHandler(this.AVLiveCenterForm_Load);
            this.panelTitle.ResumeLayout(false);
            this.panelTitle.PerformLayout();
            this.contextMenuStripVideo.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMinX;
        private System.Windows.Forms.Button btnCloseX;
        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Button btnMaxNorm;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStopLive;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemStopMoni;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemVolume;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemResize;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemVideoRefresh;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCapture;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripVideo;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_rec;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelVideo;
        private System.Windows.Forms.Label label1;
    }
}