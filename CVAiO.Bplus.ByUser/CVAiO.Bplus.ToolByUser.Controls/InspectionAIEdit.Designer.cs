
namespace CVAiO.Bplus.ToolByUser.Controls
{
    partial class InspectionAIEdit
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOnnxFile = new CVAiO.Bplus.Core.ButtonExtension();
            this.btnClassFile = new CVAiO.Bplus.Core.ButtonExtension();
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
            this.ToolProperty.Size = new System.Drawing.Size(345, 686);
            // 
            // splitPropertyControl
            // 
            this.splitPropertyControl.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitPropertyControl.IsSplitterFixed = true;
            this.splitPropertyControl.SplitterDistance = 718;
            this.splitPropertyControl.SplitterWidth = 1;
            // 
            // splitContainer
            // 
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.tableLayoutPanel1);
            this.pnControl.Size = new System.Drawing.Size(348, 51);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnOnnxFile, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClassFile, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(348, 51);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnOnnxFile
            // 
            this.btnOnnxFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOnnxFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOnnxFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOnnxFile.Location = new System.Drawing.Point(3, 3);
            this.btnOnnxFile.Name = "btnOnnxFile";
            this.btnOnnxFile.Size = new System.Drawing.Size(168, 45);
            this.btnOnnxFile.TabIndex = 0;
            this.btnOnnxFile.TabStop = false;
            this.btnOnnxFile.Text = "Onnx File";
            this.btnOnnxFile.UseVisualStyleBackColor = true;
            this.btnOnnxFile.Click += new System.EventHandler(this.btnOnnxFile_Click);
            // 
            // btnClassFile
            // 
            this.btnClassFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClassFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClassFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClassFile.Location = new System.Drawing.Point(177, 3);
            this.btnClassFile.Name = "btnClassFile";
            this.btnClassFile.Size = new System.Drawing.Size(168, 45);
            this.btnClassFile.TabIndex = 1;
            this.btnClassFile.TabStop = false;
            this.btnClassFile.Text = "Class File";
            this.btnClassFile.UseVisualStyleBackColor = true;
            this.btnClassFile.Click += new System.EventHandler(this.btnClassFile_Click);
            // 
            // InspectionAIEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "InspectionAIEdit";
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

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private CVAiO.Bplus.Core.ButtonExtension btnOnnxFile;
        private CVAiO.Bplus.Core.ButtonExtension btnClassFile;
    }
}
