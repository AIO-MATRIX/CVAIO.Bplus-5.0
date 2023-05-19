
namespace CVAiO.Bplus.ToolByUser.Controls
{
    partial class BlobFillEdit
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
            this.lvBlobResult = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitPropertyControl)).BeginInit();
            this.splitPropertyControl.Panel1.SuspendLayout();
            this.splitPropertyControl.Panel2.SuspendLayout();
            this.splitPropertyControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.lvBlobResult);
            // 
            // splitPropertyControl
            // 
            // 
            // splitContainer
            // 

            // 
            // lvBlobResult
            // 
            this.lvBlobResult.AutoArrange = false;
            this.lvBlobResult.BackColor = System.Drawing.SystemColors.Control;
            this.lvBlobResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader7,
            this.columnHeader8});
            this.lvBlobResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvBlobResult.FullRowSelect = true;
            this.lvBlobResult.GridLines = true;
            this.lvBlobResult.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvBlobResult.HideSelection = false;
            this.lvBlobResult.Location = new System.Drawing.Point(3, 3);
            this.lvBlobResult.MultiSelect = false;
            this.lvBlobResult.Name = "lvBlobResult";
            this.lvBlobResult.ShowItemToolTips = true;
            this.lvBlobResult.Size = new System.Drawing.Size(334, 332);
            this.lvBlobResult.TabIndex = 40;
            this.lvBlobResult.UseCompatibleStateImageBehavior = false;
            this.lvBlobResult.View = System.Windows.Forms.View.Details;
            this.lvBlobResult.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvBlobResult_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 35;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Area";
            this.columnHeader2.Width = 55;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "CenterMassX";
            this.columnHeader3.Width = 91;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "CenterMassY";
            this.columnHeader4.Width = 91;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Width";
            this.columnHeader7.Width = 70;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Height";
            this.columnHeader8.Width = 70;
            // 
            // BlobFillEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "BlobFillEdit";
            this.splitPropertyControl.Panel1.ResumeLayout(false);
            this.splitPropertyControl.Panel1.PerformLayout();
            this.splitPropertyControl.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPropertyControl)).EndInit();
            this.splitPropertyControl.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvBlobResult;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
    }
}
