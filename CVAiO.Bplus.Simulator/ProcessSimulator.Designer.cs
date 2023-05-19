
namespace CVAiO.Bplus.Simulator
{
    partial class ProcessSimulator
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
            MainGUI.Instance.Stop();
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
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSave = new CVAiO.Bplus.Simulator.ButtonExtension();
            this.btnLoad = new CVAiO.Bplus.Simulator.ButtonExtension();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnStop = new CVAiO.Bplus.Simulator.ButtonExtension();
            this.btnStart = new CVAiO.Bplus.Simulator.ButtonExtension();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lbAlive = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbReady = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbReset = new CVAiO.Bplus.Simulator.LabelStatus();
            this.numSchedulerCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMainGUI = new CVAiO.Bplus.Simulator.ButtonExtension();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.btnScheduler3 = new CVAiO.Bplus.Simulator.ButtonExtension();
            this.btnScheduler2 = new CVAiO.Bplus.Simulator.ButtonExtension();
            this.btnScheduler1 = new CVAiO.Bplus.Simulator.ButtonExtension();
            this.schedulerEdit1 = new CVAiO.Bplus.Simulator.SchedulerEdit();
            this.schedulerEdit2 = new CVAiO.Bplus.Simulator.SchedulerEdit();
            this.schedulerEdit3 = new CVAiO.Bplus.Simulator.SchedulerEdit();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSchedulerCount)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 200;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.btnSave, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnLoad, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(244, 44);
            this.tableLayoutPanel3.TabIndex = 40;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(125, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(116, 38);
            this.btnSave.TabIndex = 1;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoad.FlatAppearance.BorderSize = 0;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.Location = new System.Drawing.Point(3, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(116, 38);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.TabStop = false;
            this.btnLoad.Text = "LOAD";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel7, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1041, 538);
            this.tableLayoutPanel2.TabIndex = 45;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.btnStop, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnStart, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(255, 4);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(267, 44);
            this.tableLayoutPanel4.TabIndex = 46;
            // 
            // btnStop
            // 
            this.btnStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStop.Enabled = false;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(136, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(128, 38);
            this.btnStop.TabIndex = 1;
            this.btnStop.TabStop = false;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(3, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(127, 38);
            this.btnStart.TabIndex = 0;
            this.btnStart.TabStop = false;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(255, 55);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.54697F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.45303F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(782, 479);
            this.tableLayoutPanel1.TabIndex = 45;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.lbAlive, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.lbReady, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.lbReset, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.numSchedulerCount, 1, 2);
            this.tableLayoutPanel5.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.btnMainGUI, 0, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(264, 101);
            this.tableLayoutPanel5.TabIndex = 45;
            // 
            // lbAlive
            // 
            this.lbAlive.AutoSize = true;
            this.lbAlive.BlinkStatus = false;
            this.lbAlive.BlinkTime = 1000;
            this.lbAlive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbAlive.Location = new System.Drawing.Point(135, 0);
            this.lbAlive.Name = "lbAlive";
            this.lbAlive.Size = new System.Drawing.Size(126, 38);
            this.lbAlive.SizeStatus = 6;
            this.lbAlive.StateStatus = false;
            this.lbAlive.TabIndex = 1;
            this.lbAlive.Text = "Alive";
            this.lbAlive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbAlive.VisibleStatus = true;
            // 
            // lbReady
            // 
            this.lbReady.AutoSize = true;
            this.lbReady.BlinkStatus = false;
            this.lbReady.BlinkTime = 1000;
            this.lbReady.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbReady.Location = new System.Drawing.Point(135, 38);
            this.lbReady.Name = "lbReady";
            this.lbReady.Size = new System.Drawing.Size(126, 38);
            this.lbReady.SizeStatus = 6;
            this.lbReady.StateStatus = false;
            this.lbReady.TabIndex = 2;
            this.lbReady.Text = "Ready";
            this.lbReady.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbReady.VisibleStatus = true;
            // 
            // lbReset
            // 
            this.lbReset.AutoSize = true;
            this.lbReset.BlinkStatus = false;
            this.lbReset.BlinkTime = 1000;
            this.lbReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbReset.Location = new System.Drawing.Point(3, 0);
            this.lbReset.Name = "lbReset";
            this.lbReset.Size = new System.Drawing.Size(126, 38);
            this.lbReset.SizeStatus = 6;
            this.lbReset.StateStatus = false;
            this.lbReset.TabIndex = 4;
            this.lbReset.Text = "Reset";
            this.lbReset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbReset.VisibleStatus = true;
            this.lbReset.Click += new System.EventHandler(this.lbReset_Click);
            // 
            // numSchedulerCount
            // 
            this.numSchedulerCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numSchedulerCount.Location = new System.Drawing.Point(135, 79);
            this.numSchedulerCount.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numSchedulerCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSchedulerCount.Name = "numSchedulerCount";
            this.numSchedulerCount.Size = new System.Drawing.Size(126, 20);
            this.numSchedulerCount.TabIndex = 5;
            this.numSchedulerCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSchedulerCount.ValueChanged += new System.EventHandler(this.numSchedulerCount_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Scheduler Count:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnMainGUI
            // 
            this.btnMainGUI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMainGUI.FlatAppearance.BorderSize = 0;
            this.btnMainGUI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMainGUI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMainGUI.Location = new System.Drawing.Point(3, 41);
            this.btnMainGUI.Name = "btnMainGUI";
            this.btnMainGUI.Size = new System.Drawing.Size(126, 32);
            this.btnMainGUI.TabIndex = 7;
            this.btnMainGUI.TabStop = false;
            this.btnMainGUI.Text = "VIEW";
            this.btnMainGUI.UseVisualStyleBackColor = true;
            this.btnMainGUI.Click += new System.EventHandler(this.btnMainGUI_Click);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel6.Controls.Add(this.btnScheduler3, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.btnScheduler2, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.btnScheduler1, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.schedulerEdit1, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.schedulerEdit2, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.schedulerEdit3, 2, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 110);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.668509F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.33149F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(776, 366);
            this.tableLayoutPanel6.TabIndex = 46;
            // 
            // btnScheduler3
            // 
            this.btnScheduler3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnScheduler3.FlatAppearance.BorderSize = 0;
            this.btnScheduler3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScheduler3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScheduler3.Location = new System.Drawing.Point(519, 3);
            this.btnScheduler3.Name = "btnScheduler3";
            this.btnScheduler3.Size = new System.Drawing.Size(254, 29);
            this.btnScheduler3.TabIndex = 6;
            this.btnScheduler3.TabStop = false;
            this.btnScheduler3.Text = "SCHEDULER 3";
            this.btnScheduler3.UseVisualStyleBackColor = true;
            this.btnScheduler3.Click += new System.EventHandler(this.btnScheduler3_Click);
            // 
            // btnScheduler2
            // 
            this.btnScheduler2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnScheduler2.FlatAppearance.BorderSize = 0;
            this.btnScheduler2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScheduler2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScheduler2.Location = new System.Drawing.Point(261, 3);
            this.btnScheduler2.Name = "btnScheduler2";
            this.btnScheduler2.Size = new System.Drawing.Size(252, 29);
            this.btnScheduler2.TabIndex = 5;
            this.btnScheduler2.TabStop = false;
            this.btnScheduler2.Text = "SCHEDULER 2";
            this.btnScheduler2.UseVisualStyleBackColor = true;
            this.btnScheduler2.Click += new System.EventHandler(this.btnScheduler2_Click);
            // 
            // btnScheduler1
            // 
            this.btnScheduler1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnScheduler1.FlatAppearance.BorderSize = 0;
            this.btnScheduler1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScheduler1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScheduler1.Location = new System.Drawing.Point(3, 3);
            this.btnScheduler1.Name = "btnScheduler1";
            this.btnScheduler1.Size = new System.Drawing.Size(252, 29);
            this.btnScheduler1.TabIndex = 4;
            this.btnScheduler1.TabStop = false;
            this.btnScheduler1.Text = "SCHEDULER 1";
            this.btnScheduler1.UseVisualStyleBackColor = true;
            this.btnScheduler1.Click += new System.EventHandler(this.btnScheduler1_Click);
            // 
            // schedulerEdit1
            // 
            this.schedulerEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schedulerEdit1.Location = new System.Drawing.Point(3, 38);
            this.schedulerEdit1.Name = "schedulerEdit1";
            this.schedulerEdit1.Size = new System.Drawing.Size(252, 325);
            this.schedulerEdit1.TabIndex = 0;
            this.schedulerEdit1.TriggerCount = 0;
            // 
            // schedulerEdit2
            // 
            this.schedulerEdit2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schedulerEdit2.Location = new System.Drawing.Point(261, 38);
            this.schedulerEdit2.Name = "schedulerEdit2";
            this.schedulerEdit2.Size = new System.Drawing.Size(252, 325);
            this.schedulerEdit2.TabIndex = 1;
            this.schedulerEdit2.TriggerCount = 0;
            // 
            // schedulerEdit3
            // 
            this.schedulerEdit3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schedulerEdit3.Location = new System.Drawing.Point(519, 38);
            this.schedulerEdit3.Name = "schedulerEdit3";
            this.schedulerEdit3.Size = new System.Drawing.Size(254, 325);
            this.schedulerEdit3.TabIndex = 2;
            this.schedulerEdit3.TriggerCount = 0;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.PropertyGrid, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.tbName, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(4, 55);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.636743F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.36326F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(244, 479);
            this.tableLayoutPanel7.TabIndex = 47;
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyGrid.Location = new System.Drawing.Point(3, 29);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(238, 447);
            this.PropertyGrid.TabIndex = 1;
            // 
            // tbName
            // 
            this.tbName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbName.Location = new System.Drawing.Point(3, 3);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(238, 20);
            this.tbName.TabIndex = 2;
            this.tbName.Text = "Simulator Name";
            // 
            // ProcessSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "ProcessSimulator";
            this.Size = new System.Drawing.Size(1041, 538);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSchedulerCount)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer;
        private LabelStatus lbStart;
        private LabelStatus lbStop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private LabelStatus lbSave;
        private LabelStatus lbLoad;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private ButtonExtension btnSave;
        private ButtonExtension btnLoad;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private ButtonExtension btnStop;
        private ButtonExtension btnStart;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private LabelStatus lbReset;
        private LabelStatus lbReady;
        private LabelStatus lbAlive;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private ButtonExtension btnScheduler3;
        private ButtonExtension btnScheduler2;
        private ButtonExtension btnScheduler1;
        private SchedulerEdit schedulerEdit1;
        private SchedulerEdit schedulerEdit2;
        private SchedulerEdit schedulerEdit3;
        private System.Windows.Forms.NumericUpDown numSchedulerCount;
        private System.Windows.Forms.Label label1;
        private ButtonExtension btnMainGUI;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private System.Windows.Forms.TextBox tbName;
    }
}

