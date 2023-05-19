
namespace CVAiO.Bplus
{
    partial class ProcessCreatorUI
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
            this.processCreatorUI1 = new CVAiO.Bplus.ProcessCreator.ProcessCreatorUI();
            this.SuspendLayout();
            // 
            // processCreatorUI1
            // 
            this.processCreatorUI1.BackColor = System.Drawing.SystemColors.Control;
            this.processCreatorUI1.CognexVisionPro = false;
            this.processCreatorUI1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processCreatorUI1.HalconMVTec = false;
            this.processCreatorUI1.Location = new System.Drawing.Point(0, 0);
            this.processCreatorUI1.Name = "processCreatorUI1";
            this.processCreatorUI1.Padding = new System.Windows.Forms.Padding(2);
            this.processCreatorUI1.Size = new System.Drawing.Size(939, 661);
            this.processCreatorUI1.TabIndex = 0;
            // 
            // ProcessCreatorUI
            // 
            this.ClientSize = new System.Drawing.Size(939, 661);
            this.Controls.Add(this.processCreatorUI1);
            this.Name = "ProcessCreatorUI";
            this.ResumeLayout(false);

        }

        #endregion

        private Bplus.ProcessCreator.ProcessCreatorUI processCreatorUI1;
    }
}