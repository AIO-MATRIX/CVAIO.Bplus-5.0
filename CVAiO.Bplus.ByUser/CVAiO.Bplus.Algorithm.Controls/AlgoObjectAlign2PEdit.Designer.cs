
namespace CVAiO.Bplus.Algorithm.Controls
{
    partial class AlgoObjectAlign2PEdit
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
            this.btnSetMaster = new CVAiO.Bplus.Core.ButtonExtension();
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
            // ToolProperty
            // 
            this.ToolProperty.Size = new System.Drawing.Size(345, 659);
            // 
            // splitPropertyControl
            // 
            this.splitPropertyControl.SplitterDistance = 692;
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.btnSetMaster);
            this.pnControl.Size = new System.Drawing.Size(348, 74);
            // 
            // splitContainer
            // 
            // 
            // btnSetMaster
            // 
            this.btnSetMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSetMaster.FlatAppearance.BorderSize = 0;
            this.btnSetMaster.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetMaster.Location = new System.Drawing.Point(3, 3);
            this.btnSetMaster.Name = "btnSetMaster";
            this.btnSetMaster.Size = new System.Drawing.Size(334, 42);
            this.btnSetMaster.TabIndex = 1;
            this.btnSetMaster.TabStop = false;
            this.btnSetMaster.Text = "SET MASTER POSITION";
            this.btnSetMaster.UseVisualStyleBackColor = true;
            this.btnSetMaster.Click += new System.EventHandler(this.btnSetMaster_Click);
            // 
            // AlgoObjectAlign1PEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AlgoObjectAlign1PEdit";
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

        private Core.ButtonExtension btnSetMaster;
    }
}
