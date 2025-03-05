namespace POCControlCenter
{
    partial class FenceRuleDef
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FenceRuleDef));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridViewRule = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvFenceAlarm = new System.Windows.Forms.DataGridView();
            this.fenceName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userName2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.alarmTypeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startActionTimeStr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endActionTimeStr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notifyTimeStr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stayTimeMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fenceId2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userId2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endLatitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endLongitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fencePoints = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip_Queryalarm = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.cbRule = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cbFencelist = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemDel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fenceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ruleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ruleAssign = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRule)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFenceAlarm)).BeginInit();
            this.contextMenuStrip_Queryalarm.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 278);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1195, 75);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(377, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(354, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "规则新增或变更会立即生效，记录行点右键进行变更";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(199, 18);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 36);
            this.button2.TabIndex = 1;
            this.button2.Text = "关闭";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(117)))), ((int)(((byte)(232)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(38, 18);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "新增规则";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1209, 447);
            this.panel2.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 54);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1209, 393);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridViewRule);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(1201, 357);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "围栏所含人员维护";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridViewRule
            // 
            this.dataGridViewRule.AllowUserToAddRows = false;
            this.dataGridViewRule.AllowUserToDeleteRows = false;
            this.dataGridViewRule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.fenceName,
            this.userName,
            this.userId,
            this.ruleType,
            this.ruleAssign,
            this.startTime,
            this.endTime});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(123)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewRule.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewRule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRule.Location = new System.Drawing.Point(3, 4);
            this.dataGridViewRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridViewRule.Name = "dataGridViewRule";
            this.dataGridViewRule.RowHeadersVisible = false;
            this.dataGridViewRule.RowTemplate.Height = 23;
            this.dataGridViewRule.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRule.Size = new System.Drawing.Size(1195, 274);
            this.dataGridViewRule.TabIndex = 0;
            this.dataGridViewRule.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRule_CellContentClick);
            this.dataGridViewRule.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewRule_CellMouseClick);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvFenceAlarm);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 32);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(1201, 357);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "警告记录查询";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvFenceAlarm
            // 
            this.dgvFenceAlarm.AllowUserToAddRows = false;
            this.dgvFenceAlarm.AllowUserToDeleteRows = false;
            this.dgvFenceAlarm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFenceAlarm.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fenceName2,
            this.userName2,
            this.alarmTypeName,
            this.startActionTimeStr,
            this.endActionTimeStr,
            this.notifyTimeStr,
            this.stayTimeMin,
            this.id2,
            this.fenceId2,
            this.userId2,
            this.endLatitude,
            this.endLongitude,
            this.fencePoints});
            this.dgvFenceAlarm.ContextMenuStrip = this.contextMenuStrip_Queryalarm;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(123)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFenceAlarm.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFenceAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFenceAlarm.Location = new System.Drawing.Point(3, 59);
            this.dgvFenceAlarm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvFenceAlarm.MultiSelect = false;
            this.dgvFenceAlarm.Name = "dgvFenceAlarm";
            this.dgvFenceAlarm.RowHeadersVisible = false;
            this.dgvFenceAlarm.RowTemplate.Height = 23;
            this.dgvFenceAlarm.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFenceAlarm.Size = new System.Drawing.Size(1195, 294);
            this.dgvFenceAlarm.TabIndex = 9;
            // 
            // fenceName2
            // 
            this.fenceName2.DataPropertyName = "fenceName";
            this.fenceName2.HeaderText = "围栏名称";
            this.fenceName2.Name = "fenceName2";
            this.fenceName2.ReadOnly = true;
            // 
            // userName2
            // 
            this.userName2.DataPropertyName = "userName";
            this.userName2.HeaderText = "用户名称";
            this.userName2.Name = "userName2";
            this.userName2.ReadOnly = true;
            // 
            // alarmTypeName
            // 
            this.alarmTypeName.DataPropertyName = "alarmTypeName";
            this.alarmTypeName.HeaderText = "警告类型";
            this.alarmTypeName.Name = "alarmTypeName";
            this.alarmTypeName.ReadOnly = true;
            // 
            // startActionTimeStr
            // 
            this.startActionTimeStr.DataPropertyName = "startActionTimeStr";
            this.startActionTimeStr.HeaderText = "开始时间";
            this.startActionTimeStr.Name = "startActionTimeStr";
            this.startActionTimeStr.ReadOnly = true;
            // 
            // endActionTimeStr
            // 
            this.endActionTimeStr.DataPropertyName = "endActionTimeStr";
            this.endActionTimeStr.HeaderText = "结束时间";
            this.endActionTimeStr.Name = "endActionTimeStr";
            this.endActionTimeStr.ReadOnly = true;
            // 
            // notifyTimeStr
            // 
            this.notifyTimeStr.DataPropertyName = "notifyTimeStr";
            this.notifyTimeStr.HeaderText = "触发警告时间";
            this.notifyTimeStr.Name = "notifyTimeStr";
            this.notifyTimeStr.ReadOnly = true;
            // 
            // stayTimeMin
            // 
            this.stayTimeMin.DataPropertyName = "stayTimeMin";
            this.stayTimeMin.HeaderText = "停留时间(分)";
            this.stayTimeMin.Name = "stayTimeMin";
            this.stayTimeMin.ReadOnly = true;
            this.stayTimeMin.ToolTipText = "单位:分钟";
            // 
            // id2
            // 
            this.id2.DataPropertyName = "id";
            this.id2.HeaderText = "id";
            this.id2.Name = "id2";
            this.id2.ReadOnly = true;
            this.id2.Visible = false;
            // 
            // fenceId2
            // 
            this.fenceId2.DataPropertyName = "fenceId";
            this.fenceId2.HeaderText = "fenceId";
            this.fenceId2.Name = "fenceId2";
            this.fenceId2.ReadOnly = true;
            this.fenceId2.Visible = false;
            // 
            // userId2
            // 
            this.userId2.DataPropertyName = "userId";
            this.userId2.HeaderText = "userId";
            this.userId2.Name = "userId2";
            this.userId2.ReadOnly = true;
            this.userId2.Visible = false;
            // 
            // endLatitude
            // 
            this.endLatitude.DataPropertyName = "endLatitude";
            this.endLatitude.HeaderText = "endLatitude";
            this.endLatitude.Name = "endLatitude";
            this.endLatitude.ReadOnly = true;
            this.endLatitude.Visible = false;
            // 
            // endLongitude
            // 
            this.endLongitude.DataPropertyName = "endLongitude";
            this.endLongitude.HeaderText = "endLongitude";
            this.endLongitude.Name = "endLongitude";
            this.endLongitude.ReadOnly = true;
            this.endLongitude.Visible = false;
            // 
            // fencePoints
            // 
            this.fencePoints.DataPropertyName = "fencePoints";
            this.fencePoints.HeaderText = "fencePoints";
            this.fencePoints.Name = "fencePoints";
            // 
            // contextMenuStrip_Queryalarm
            // 
            this.contextMenuStrip_Queryalarm.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.contextMenuStrip_Queryalarm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.contextMenuStrip_Queryalarm.Name = "contextMenuStrip1";
            this.contextMenuStrip_Queryalarm.Size = new System.Drawing.Size(147, 52);
            this.contextMenuStrip_Queryalarm.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_Queryalarm_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(146, 24);
            this.toolStripMenuItem1.Text = "导出excel";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(146, 24);
            this.toolStripMenuItem2.Text = "导出pdf";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button4);
            this.panel3.Controls.Add(this.button3);
            this.panel3.Controls.Add(this.dateTimePickerEnd);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.dateTimePickerStart);
            this.panel3.Controls.Add(this.cbRule);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 4);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1195, 55);
            this.panel3.TabIndex = 0;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(756, 11);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(159, 33);
            this.button4.TabIndex = 5;
            this.button4.Text = "地图显示结果";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(96)))), ((int)(((byte)(144)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(190)))), ((int)(((byte)(156)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(654, 10);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(82, 34);
            this.button3.TabIndex = 4;
            this.button3.Text = "查询";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.CustomFormat = "yyyy-MM-dd";
            this.dateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEnd.Location = new System.Drawing.Point(450, 14);
            this.dateTimePickerEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(178, 27);
            this.dateTimePickerEnd.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(394, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "到";
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CustomFormat = "yyyy-MM-dd";
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Location = new System.Drawing.Point(174, 14);
            this.dateTimePickerStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(181, 27);
            this.dateTimePickerStart.TabIndex = 1;
            // 
            // cbRule
            // 
            this.cbRule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRule.FormattingEnabled = true;
            this.cbRule.Items.AddRange(new object[] {
            "所有警告",
            "进入警告",
            "出去警告"});
            this.cbRule.Location = new System.Drawing.Point(12, 14);
            this.cbRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbRule.Name = "cbRule";
            this.cbRule.Size = new System.Drawing.Size(140, 28);
            this.cbRule.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.cbFencelist);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1209, 54);
            this.panel4.TabIndex = 2;
            // 
            // cbFencelist
            // 
            this.cbFencelist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFencelist.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbFencelist.FormattingEnabled = true;
            this.cbFencelist.Location = new System.Drawing.Point(111, 13);
            this.cbFencelist.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbFencelist.Name = "cbFencelist";
            this.cbFencelist.Size = new System.Drawing.Size(296, 29);
            this.cbFencelist.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(14, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "围栏名称";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemDel,
            this.toolStripMenuItemEdit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(109, 52);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // toolStripMenuItemDel
            // 
            this.toolStripMenuItemDel.Name = "toolStripMenuItemDel";
            this.toolStripMenuItemDel.Size = new System.Drawing.Size(108, 24);
            this.toolStripMenuItemDel.Text = "删除";
            // 
            // toolStripMenuItemEdit
            // 
            this.toolStripMenuItemEdit.Name = "toolStripMenuItemEdit";
            this.toolStripMenuItemEdit.Size = new System.Drawing.Size(108, 24);
            this.toolStripMenuItemEdit.Text = "编辑";
            // 
            // id
            // 
            this.id.DataPropertyName = "id";
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            // 
            // fenceName
            // 
            this.fenceName.DataPropertyName = "fenceName";
            this.fenceName.HeaderText = "围栏名称";
            this.fenceName.Name = "fenceName";
            this.fenceName.ReadOnly = true;
            // 
            // userName
            // 
            this.userName.DataPropertyName = "userName";
            this.userName.HeaderText = "用户名称";
            this.userName.Name = "userName";
            this.userName.ReadOnly = true;
            // 
            // userId
            // 
            this.userId.DataPropertyName = "userId";
            this.userId.HeaderText = "用户ID";
            this.userId.Name = "userId";
            this.userId.ReadOnly = true;
            this.userId.Visible = false;
            // 
            // ruleType
            // 
            this.ruleType.DataPropertyName = "ruleType";
            this.ruleType.HeaderText = "围栏规则";
            this.ruleType.Name = "ruleType";
            this.ruleType.ReadOnly = true;
            // 
            // ruleAssign
            // 
            this.ruleAssign.DataPropertyName = "ruleAssign";
            this.ruleAssign.HeaderText = "黑白名单";
            this.ruleAssign.Name = "ruleAssign";
            this.ruleAssign.ReadOnly = true;
            // 
            // startTime
            // 
            this.startTime.DataPropertyName = "startTime";
            this.startTime.HeaderText = "开始时间";
            this.startTime.Name = "startTime";
            this.startTime.ReadOnly = true;
            // 
            // endTime
            // 
            this.endTime.DataPropertyName = "endTime";
            this.endTime.HeaderText = "结束时间";
            this.endTime.Name = "endTime";
            this.endTime.ReadOnly = true;
            // 
            // FenceRuleDef
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 447);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FenceRuleDef";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "围栏规则及警告查询";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FenceRuleDef_FormClosed);
            this.Load += new System.EventHandler(this.FenceRuleDef_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRule)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFenceAlarm)).EndInit();
            this.contextMenuStrip_Queryalarm.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.DataGridView dataGridViewRule;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEdit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Queryalarm;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.ComboBox cbRule;
        private System.Windows.Forms.DataGridView dgvFenceAlarm;
        private System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.ComboBox cbFencelist;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DataGridViewTextBoxColumn fenceName2;
        private System.Windows.Forms.DataGridViewTextBoxColumn userName2;
        private System.Windows.Forms.DataGridViewTextBoxColumn alarmTypeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn startActionTimeStr;
        private System.Windows.Forms.DataGridViewTextBoxColumn endActionTimeStr;
        private System.Windows.Forms.DataGridViewTextBoxColumn notifyTimeStr;
        private System.Windows.Forms.DataGridViewTextBoxColumn stayTimeMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn id2;
        private System.Windows.Forms.DataGridViewTextBoxColumn fenceId2;
        private System.Windows.Forms.DataGridViewTextBoxColumn userId2;
        private System.Windows.Forms.DataGridViewTextBoxColumn endLatitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn endLongitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn fencePoints;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn fenceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn userName;
        private System.Windows.Forms.DataGridViewTextBoxColumn userId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ruleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ruleAssign;
        private System.Windows.Forms.DataGridViewTextBoxColumn startTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn endTime;
    }
}