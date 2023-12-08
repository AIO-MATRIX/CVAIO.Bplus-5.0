
namespace CVAiO.Bplus.ToolByUser.Controls
{
    partial class GlueBeadInspectionEdit
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
            this.btnBeadAnalysis = new CVAiO.Bplus.Core.ButtonExtension();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitPropertyControl)).BeginInit();
            this.splitPropertyControl.Panel1.SuspendLayout();
            this.splitPropertyControl.Panel2.SuspendLayout();
            this.splitPropertyControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnControl.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolProperty
            // 
            this.ToolProperty.Size = new System.Drawing.Size(345, 687);
            // 
            // splitPropertyControl
            // 
            this.splitPropertyControl.SplitterDistance = 720;
            // 
            // splitContainer
            // 
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.tableLayoutPanel1);
            this.pnControl.Size = new System.Drawing.Size(348, 46);
            // 
            // btnBeadAnalysis
            // 
            this.btnBeadAnalysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBeadAnalysis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBeadAnalysis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBeadAnalysis.Location = new System.Drawing.Point(3, 3);
            this.btnBeadAnalysis.Name = "btnBeadAnalysis";
            this.btnBeadAnalysis.Size = new System.Drawing.Size(342, 40);
            this.btnBeadAnalysis.TabIndex = 2;
            this.btnBeadAnalysis.TabStop = false;
            this.btnBeadAnalysis.Text = "BEAD ANALYSIS";
            this.btnBeadAnalysis.UseVisualStyleBackColor = true;
            this.btnBeadAnalysis.Click += new System.EventHandler(this.btnBeadAnalysis_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnBeadAnalysis, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(348, 46);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // GlueBeadInspectionEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "GlueBeadInspectionEdit";
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
            this.ResumeLayout(false);

        }

        #endregion

        private Core.ButtonExtension btnBeadAnalysis;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
