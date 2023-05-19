
namespace CVAiO.Bplus.Simulator
{
    partial class SchedulerEdit
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lbOK = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbComp = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbNG = new CVAiO.Bplus.Simulator.LabelStatus();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lbReady = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbStart = new CVAiO.Bplus.Simulator.LabelStatus();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lbCalcAck = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbTrigger3Ack = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbCalc = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbTrigger2Ack = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbTrigger4 = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbTrigger1Ack = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbTrigger1 = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbTrigger3 = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbTrigger2 = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbTrigger4Ack = new CVAiO.Bplus.Simulator.LabelStatus();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbMode = new CVAiO.Bplus.Simulator.LabelStatus();
            this.lbCurrentMode = new System.Windows.Forms.Label();
            this.pnMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.lbOK, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lbComp, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lbNG, 1, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(14, 258);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(311, 60);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // lbOK
            // 
            this.lbOK.AutoSize = true;
            this.lbOK.BlinkStatus = false;
            this.lbOK.BlinkTime = 1000;
            this.lbOK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOK.Location = new System.Drawing.Point(158, 0);
            this.lbOK.Name = "lbOK";
            this.lbOK.Size = new System.Drawing.Size(150, 30);
            this.lbOK.SizeStatus = 6;
            this.lbOK.StateStatus = false;
            this.lbOK.TabIndex = 1;
            this.lbOK.Text = "Ok";
            this.lbOK.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbOK.VisibleStatus = true;
            // 
            // lbComp
            // 
            this.lbComp.AutoSize = true;
            this.lbComp.BlinkStatus = false;
            this.lbComp.BlinkTime = 1000;
            this.lbComp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbComp.Location = new System.Drawing.Point(3, 30);
            this.lbComp.Name = "lbComp";
            this.lbComp.Size = new System.Drawing.Size(149, 30);
            this.lbComp.SizeStatus = 6;
            this.lbComp.StateStatus = false;
            this.lbComp.TabIndex = 4;
            this.lbComp.Text = "Complete";
            this.lbComp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbComp.VisibleStatus = true;
            this.lbComp.Click += new System.EventHandler(this.lbComp_Click);
            // 
            // lbNG
            // 
            this.lbNG.AutoSize = true;
            this.lbNG.BlinkStatus = false;
            this.lbNG.BlinkTime = 1000;
            this.lbNG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbNG.Location = new System.Drawing.Point(158, 30);
            this.lbNG.Name = "lbNG";
            this.lbNG.Size = new System.Drawing.Size(150, 30);
            this.lbNG.SizeStatus = 6;
            this.lbNG.StateStatus = false;
            this.lbNG.TabIndex = 2;
            this.lbNG.Text = "Ng";
            this.lbNG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbNG.VisibleStatus = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.lbReady, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.lbStart, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(14, 54);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(311, 30);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // lbReady
            // 
            this.lbReady.AutoSize = true;
            this.lbReady.BlinkStatus = false;
            this.lbReady.BlinkTime = 1000;
            this.lbReady.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbReady.Location = new System.Drawing.Point(158, 0);
            this.lbReady.Name = "lbReady";
            this.lbReady.Size = new System.Drawing.Size(150, 30);
            this.lbReady.SizeStatus = 6;
            this.lbReady.StateStatus = false;
            this.lbReady.TabIndex = 0;
            this.lbReady.Text = "Ready";
            this.lbReady.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbReady.VisibleStatus = true;
            // 
            // lbStart
            // 
            this.lbStart.AutoSize = true;
            this.lbStart.BlinkStatus = false;
            this.lbStart.BlinkTime = 1000;
            this.lbStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbStart.Location = new System.Drawing.Point(3, 0);
            this.lbStart.Name = "lbStart";
            this.lbStart.Size = new System.Drawing.Size(149, 30);
            this.lbStart.SizeStatus = 6;
            this.lbStart.StateStatus = false;
            this.lbStart.TabIndex = 3;
            this.lbStart.Text = "Start";
            this.lbStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbStart.VisibleStatus = true;
            this.lbStart.Click += new System.EventHandler(this.lbStart_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.lbCalcAck, 1, 4);
            this.tableLayoutPanel4.Controls.Add(this.lbTrigger3Ack, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.lbCalc, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.lbTrigger2Ack, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.lbTrigger4, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.lbTrigger1Ack, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.lbTrigger1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lbTrigger3, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.lbTrigger2, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.lbTrigger4Ack, 1, 3);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(14, 97);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 5;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(311, 150);
            this.tableLayoutPanel4.TabIndex = 7;
            // 
            // lbCalcAck
            // 
            this.lbCalcAck.AutoSize = true;
            this.lbCalcAck.BlinkStatus = false;
            this.lbCalcAck.BlinkTime = 1000;
            this.lbCalcAck.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCalcAck.Location = new System.Drawing.Point(158, 120);
            this.lbCalcAck.Name = "lbCalcAck";
            this.lbCalcAck.Size = new System.Drawing.Size(150, 30);
            this.lbCalcAck.SizeStatus = 6;
            this.lbCalcAck.StateStatus = false;
            this.lbCalcAck.TabIndex = 4;
            this.lbCalcAck.Text = "Calc Ack";
            this.lbCalcAck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbCalcAck.VisibleStatus = true;
            // 
            // lbTrigger3Ack
            // 
            this.lbTrigger3Ack.AutoSize = true;
            this.lbTrigger3Ack.BlinkStatus = false;
            this.lbTrigger3Ack.BlinkTime = 1000;
            this.lbTrigger3Ack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTrigger3Ack.Location = new System.Drawing.Point(158, 60);
            this.lbTrigger3Ack.Name = "lbTrigger3Ack";
            this.lbTrigger3Ack.Size = new System.Drawing.Size(150, 30);
            this.lbTrigger3Ack.SizeStatus = 6;
            this.lbTrigger3Ack.StateStatus = false;
            this.lbTrigger3Ack.TabIndex = 2;
            this.lbTrigger3Ack.Text = "Trigger 3 Ack";
            this.lbTrigger3Ack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTrigger3Ack.VisibleStatus = true;
            // 
            // lbCalc
            // 
            this.lbCalc.AutoSize = true;
            this.lbCalc.BlinkStatus = false;
            this.lbCalc.BlinkTime = 1000;
            this.lbCalc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCalc.Location = new System.Drawing.Point(3, 120);
            this.lbCalc.Name = "lbCalc";
            this.lbCalc.Size = new System.Drawing.Size(149, 30);
            this.lbCalc.SizeStatus = 6;
            this.lbCalc.StateStatus = false;
            this.lbCalc.TabIndex = 10;
            this.lbCalc.Text = "Calc";
            this.lbCalc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbCalc.VisibleStatus = true;
            this.lbCalc.Click += new System.EventHandler(this.lbCalc_Click);
            // 
            // lbTrigger2Ack
            // 
            this.lbTrigger2Ack.AutoSize = true;
            this.lbTrigger2Ack.BlinkStatus = false;
            this.lbTrigger2Ack.BlinkTime = 1000;
            this.lbTrigger2Ack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTrigger2Ack.Location = new System.Drawing.Point(158, 30);
            this.lbTrigger2Ack.Name = "lbTrigger2Ack";
            this.lbTrigger2Ack.Size = new System.Drawing.Size(150, 30);
            this.lbTrigger2Ack.SizeStatus = 6;
            this.lbTrigger2Ack.StateStatus = false;
            this.lbTrigger2Ack.TabIndex = 1;
            this.lbTrigger2Ack.Text = "Trigger 2 Ack";
            this.lbTrigger2Ack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTrigger2Ack.VisibleStatus = true;
            // 
            // lbTrigger4
            // 
            this.lbTrigger4.AutoSize = true;
            this.lbTrigger4.BlinkStatus = false;
            this.lbTrigger4.BlinkTime = 1000;
            this.lbTrigger4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTrigger4.Location = new System.Drawing.Point(3, 90);
            this.lbTrigger4.Name = "lbTrigger4";
            this.lbTrigger4.Size = new System.Drawing.Size(149, 30);
            this.lbTrigger4.SizeStatus = 6;
            this.lbTrigger4.StateStatus = false;
            this.lbTrigger4.TabIndex = 9;
            this.lbTrigger4.Text = "Trigger 4";
            this.lbTrigger4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTrigger4.VisibleStatus = true;
            this.lbTrigger4.Click += new System.EventHandler(this.lbTrigger4_Click);
            // 
            // lbTrigger1Ack
            // 
            this.lbTrigger1Ack.AutoSize = true;
            this.lbTrigger1Ack.BlinkStatus = false;
            this.lbTrigger1Ack.BlinkTime = 1000;
            this.lbTrigger1Ack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTrigger1Ack.Location = new System.Drawing.Point(158, 0);
            this.lbTrigger1Ack.Name = "lbTrigger1Ack";
            this.lbTrigger1Ack.Size = new System.Drawing.Size(150, 30);
            this.lbTrigger1Ack.SizeStatus = 6;
            this.lbTrigger1Ack.StateStatus = false;
            this.lbTrigger1Ack.TabIndex = 0;
            this.lbTrigger1Ack.Text = "Trigger 1 Ack";
            this.lbTrigger1Ack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTrigger1Ack.VisibleStatus = true;
            // 
            // lbTrigger1
            // 
            this.lbTrigger1.AutoSize = true;
            this.lbTrigger1.BlinkStatus = false;
            this.lbTrigger1.BlinkTime = 1000;
            this.lbTrigger1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTrigger1.Location = new System.Drawing.Point(3, 0);
            this.lbTrigger1.Name = "lbTrigger1";
            this.lbTrigger1.Size = new System.Drawing.Size(149, 30);
            this.lbTrigger1.SizeStatus = 6;
            this.lbTrigger1.StateStatus = false;
            this.lbTrigger1.TabIndex = 6;
            this.lbTrigger1.Text = "Trigger 1";
            this.lbTrigger1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTrigger1.VisibleStatus = true;
            this.lbTrigger1.Click += new System.EventHandler(this.lbTrigger1_Click);
            // 
            // lbTrigger3
            // 
            this.lbTrigger3.AutoSize = true;
            this.lbTrigger3.BlinkStatus = false;
            this.lbTrigger3.BlinkTime = 1000;
            this.lbTrigger3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTrigger3.Location = new System.Drawing.Point(3, 60);
            this.lbTrigger3.Name = "lbTrigger3";
            this.lbTrigger3.Size = new System.Drawing.Size(149, 30);
            this.lbTrigger3.SizeStatus = 6;
            this.lbTrigger3.StateStatus = false;
            this.lbTrigger3.TabIndex = 8;
            this.lbTrigger3.Text = "Trigger 3";
            this.lbTrigger3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTrigger3.VisibleStatus = true;
            this.lbTrigger3.Click += new System.EventHandler(this.lbTrigger3_Click);
            // 
            // lbTrigger2
            // 
            this.lbTrigger2.AutoSize = true;
            this.lbTrigger2.BlinkStatus = false;
            this.lbTrigger2.BlinkTime = 1000;
            this.lbTrigger2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTrigger2.Location = new System.Drawing.Point(3, 30);
            this.lbTrigger2.Name = "lbTrigger2";
            this.lbTrigger2.Size = new System.Drawing.Size(149, 30);
            this.lbTrigger2.SizeStatus = 6;
            this.lbTrigger2.StateStatus = false;
            this.lbTrigger2.TabIndex = 7;
            this.lbTrigger2.Text = "Trigger 2";
            this.lbTrigger2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTrigger2.VisibleStatus = true;
            this.lbTrigger2.Click += new System.EventHandler(this.lbTrigger2_Click);
            // 
            // lbTrigger4Ack
            // 
            this.lbTrigger4Ack.AutoSize = true;
            this.lbTrigger4Ack.BlinkStatus = false;
            this.lbTrigger4Ack.BlinkTime = 1000;
            this.lbTrigger4Ack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbTrigger4Ack.Location = new System.Drawing.Point(158, 90);
            this.lbTrigger4Ack.Name = "lbTrigger4Ack";
            this.lbTrigger4Ack.Size = new System.Drawing.Size(150, 30);
            this.lbTrigger4Ack.SizeStatus = 6;
            this.lbTrigger4Ack.StateStatus = false;
            this.lbTrigger4Ack.TabIndex = 3;
            this.lbTrigger4Ack.Text = "Trigger 4 Ack";
            this.lbTrigger4Ack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbTrigger4Ack.VisibleStatus = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lbMode, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbCurrentMode, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 9);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(312, 30);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // lbMode
            // 
            this.lbMode.BlinkStatus = false;
            this.lbMode.BlinkTime = 1000;
            this.lbMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbMode.Location = new System.Drawing.Point(3, 0);
            this.lbMode.Name = "lbMode";
            this.lbMode.Size = new System.Drawing.Size(150, 30);
            this.lbMode.SizeStatus = 6;
            this.lbMode.StateStatus = false;
            this.lbMode.TabIndex = 5;
            this.lbMode.Text = "MODE";
            this.lbMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbMode.VisibleStatus = false;
            this.lbMode.Click += new System.EventHandler(this.lbMode_Click);
            // 
            // lbCurrentMode
            // 
            this.lbCurrentMode.AutoSize = true;
            this.lbCurrentMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCurrentMode.Location = new System.Drawing.Point(159, 0);
            this.lbCurrentMode.Name = "lbCurrentMode";
            this.lbCurrentMode.Size = new System.Drawing.Size(150, 30);
            this.lbCurrentMode.TabIndex = 6;
            this.lbCurrentMode.Text = "Auto";
            this.lbCurrentMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnMain
            // 
            this.pnMain.Controls.Add(this.tableLayoutPanel3);
            this.pnMain.Controls.Add(this.tableLayoutPanel1);
            this.pnMain.Controls.Add(this.tableLayoutPanel2);
            this.pnMain.Controls.Add(this.tableLayoutPanel4);
            this.pnMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnMain.Location = new System.Drawing.Point(0, 0);
            this.pnMain.Name = "pnMain";
            this.pnMain.Size = new System.Drawing.Size(335, 328);
            this.pnMain.TabIndex = 9;
            this.pnMain.Paint += new System.Windows.Forms.PaintEventHandler(this.pnMain_Paint);
            // 
            // SchedulerEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnMain);
            this.Name = "SchedulerEdit";
            this.Size = new System.Drawing.Size(335, 328);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private LabelStatus lbTrigger1Ack;
        private LabelStatus lbTrigger2Ack;
        private LabelStatus lbTrigger3Ack;
        private LabelStatus lbTrigger4Ack;
        private LabelStatus lbCalcAck;
        private LabelStatus lbTrigger1;
        private LabelStatus lbTrigger2;
        private LabelStatus lbTrigger3;
        private LabelStatus lbTrigger4;
        private LabelStatus lbCalc;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private LabelStatus lbReady;
        private LabelStatus lbOK;
        private LabelStatus lbNG;
        private LabelStatus lbStart;
        private LabelStatus lbComp;
        private LabelStatus lbMode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnMain;
        private System.Windows.Forms.Label lbCurrentMode;
    }
}
