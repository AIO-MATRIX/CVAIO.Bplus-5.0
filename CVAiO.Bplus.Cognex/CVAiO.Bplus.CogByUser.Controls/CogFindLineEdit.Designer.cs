
namespace CVAiO.Bplus.CogByUser.Controls
{
    partial class CogFindLineEdit
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
            this.btnCogToolView = new CVAiO.Bplus.Core.ButtonExtension();
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
            this.ToolProperty.Size = new System.Drawing.Size(345, 692);
            // 
            // splitPropertyControl
            // 
            this.splitPropertyControl.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitPropertyControl.SplitterDistance = 725;
            // 
            // splitContainer
            // 
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.btnCogToolView);
            this.pnControl.Size = new System.Drawing.Size(348, 41);
            // 
            // btnCogToolView
            // 
            this.btnCogToolView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCogToolView.FlatAppearance.BorderSize = 0;
            this.btnCogToolView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCogToolView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCogToolView.Location = new System.Drawing.Point(0, 0);
            this.btnCogToolView.Name = "btnCogToolView";
            this.btnCogToolView.Size = new System.Drawing.Size(348, 41);
            this.btnCogToolView.TabIndex = 1;
            this.btnCogToolView.TabStop = false;
            this.btnCogToolView.Text = "COGNEX TOOL VIEW";
            this.btnCogToolView.UseVisualStyleBackColor = true;
            this.btnCogToolView.Click += new System.EventHandler(this.btnCogToolView_Click);
            // 
            // CogFindLineEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CogFindLineEdit";
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

        private Core.ButtonExtension btnCogToolView;
    }
}
