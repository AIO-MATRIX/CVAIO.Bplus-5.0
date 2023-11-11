
namespace CVAiO.Bplus.ToolByUser.Controls
{
    partial class ConnectorInspectionEdit
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
            this.HistogramBoxRed = new CVAiO.Bplus.ImageProcessing.Controls.HistogramBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.HistogramBoxGreen = new CVAiO.Bplus.ImageProcessing.Controls.HistogramBox();
            this.HistogramBoxBlue = new CVAiO.Bplus.ImageProcessing.Controls.HistogramBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnNextWire = new CVAiO.Bplus.Core.ButtonExtension();
            this.btnPreviousWire = new CVAiO.Bplus.Core.ButtonExtension();
            ((System.ComponentModel.ISupportInitialize)(this.splitPropertyControl)).BeginInit();
            this.splitPropertyControl.Panel1.SuspendLayout();
            this.splitPropertyControl.Panel2.SuspendLayout();
            this.splitPropertyControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnControl.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolProperty
            // 
            this.ToolProperty.Size = new System.Drawing.Size(345, 465);
            // 
            // splitPropertyControl
            // 
            this.splitPropertyControl.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitPropertyControl.IsSplitterFixed = true;
            this.splitPropertyControl.SplitterDistance = 498;
            // 
            // splitContainer
            // 
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.tableLayoutPanel2);
            this.pnControl.Size = new System.Drawing.Size(348, 268);
            // 
            // HistogramBoxRed
            // 
            this.HistogramBoxRed.BackColor = System.Drawing.SystemColors.Control;
            this.HistogramBoxRed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.HistogramBoxRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HistogramBoxRed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HistogramBoxRed.EnableRangeDrag = true;
            this.HistogramBoxRed.Location = new System.Drawing.Point(23, 3);
            this.HistogramBoxRed.Name = "HistogramBoxRed";
            this.HistogramBoxRed.RangeHigh = 256;
            this.HistogramBoxRed.RangeLow = 0;
            this.HistogramBoxRed.Size = new System.Drawing.Size(316, 68);
            this.HistogramBoxRed.TabIndex = 1;
            this.HistogramBoxRed.UseOnlyLowRange = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel1, 2);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.HistogramBoxGreen, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.HistogramBoxBlue, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.HistogramBoxRed, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 43);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(342, 222);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // HistogramBoxGreen
            // 
            this.HistogramBoxGreen.BackColor = System.Drawing.SystemColors.Control;
            this.HistogramBoxGreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.HistogramBoxGreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HistogramBoxGreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HistogramBoxGreen.EnableRangeDrag = true;
            this.HistogramBoxGreen.Location = new System.Drawing.Point(23, 151);
            this.HistogramBoxGreen.Name = "HistogramBoxGreen";
            this.HistogramBoxGreen.RangeHigh = 256;
            this.HistogramBoxGreen.RangeLow = 0;
            this.HistogramBoxGreen.Size = new System.Drawing.Size(316, 68);
            this.HistogramBoxGreen.TabIndex = 6;
            this.HistogramBoxGreen.UseOnlyLowRange = false;
            // 
            // HistogramBoxBlue
            // 
            this.HistogramBoxBlue.BackColor = System.Drawing.SystemColors.Control;
            this.HistogramBoxBlue.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.HistogramBoxBlue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HistogramBoxBlue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HistogramBoxBlue.EnableRangeDrag = true;
            this.HistogramBoxBlue.Location = new System.Drawing.Point(23, 77);
            this.HistogramBoxBlue.Name = "HistogramBoxBlue";
            this.HistogramBoxBlue.RangeHigh = 256;
            this.HistogramBoxBlue.RangeLow = 0;
            this.HistogramBoxBlue.Size = new System.Drawing.Size(316, 68);
            this.HistogramBoxBlue.TabIndex = 5;
            this.HistogramBoxBlue.UseOnlyLowRange = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 74);
            this.label2.TabIndex = 3;
            this.label2.Text = "BLUE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 74);
            this.label1.TabIndex = 2;
            this.label1.Text = "RED";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 74);
            this.label3.TabIndex = 4;
            this.label3.Text = "GREEN";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btnNextWire, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPreviousWire, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(348, 268);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // btnNextWire
            // 
            this.btnNextWire.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNextWire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextWire.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNextWire.Location = new System.Drawing.Point(177, 3);
            this.btnNextWire.Name = "btnNextWire";
            this.btnNextWire.Size = new System.Drawing.Size(168, 34);
            this.btnNextWire.TabIndex = 4;
            this.btnNextWire.TabStop = false;
            this.btnNextWire.Text = "NEXT WIRE";
            this.btnNextWire.UseVisualStyleBackColor = true;
            this.btnNextWire.Click += new System.EventHandler(this.btnNextWire_Click);
            // 
            // btnPreviousWire
            // 
            this.btnPreviousWire.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPreviousWire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreviousWire.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreviousWire.Location = new System.Drawing.Point(3, 3);
            this.btnPreviousWire.Name = "btnPreviousWire";
            this.btnPreviousWire.Size = new System.Drawing.Size(168, 34);
            this.btnPreviousWire.TabIndex = 3;
            this.btnPreviousWire.TabStop = false;
            this.btnPreviousWire.Text = "PREVIOUS WIRE";
            this.btnPreviousWire.UseVisualStyleBackColor = true;
            this.btnPreviousWire.Click += new System.EventHandler(this.btnPreviousWire_Click);
            // 
            // ConnectorInspectionEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ConnectorInspectionEdit";
            this.splitPropertyControl.Panel1.ResumeLayout(false);
            this.splitPropertyControl.Panel1.PerformLayout();
            this.splitPropertyControl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPropertyControl)).EndInit();
            this.splitPropertyControl.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnControl.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageProcessing.Controls.HistogramBox HistogramBoxRed;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private ImageProcessing.Controls.HistogramBox HistogramBoxGreen;
        private ImageProcessing.Controls.HistogramBox HistogramBoxBlue;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Core.ButtonExtension btnNextWire;
        private Core.ButtonExtension btnPreviousWire;
    }
}
