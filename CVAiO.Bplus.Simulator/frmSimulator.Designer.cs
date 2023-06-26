
namespace CVAiO.Bplus.Simulator
{
    partial class frmSimulator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSimulator));
            this.calibration1 = new CVAiO.Bplus.Simulator.Calibration();
            this.SuspendLayout();
            // 
            // calibration1
            // 
            this.calibration1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.calibration1.Location = new System.Drawing.Point(0, 0);
            this.calibration1.Name = "calibration1";
            this.calibration1.Size = new System.Drawing.Size(1099, 532);
            this.calibration1.TabIndex = 0;
            // 
            // frmSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 532);
            this.Controls.Add(this.calibration1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSimulator";
            this.Text = "CVAIO B+ Simulator";
            this.ResumeLayout(false);

        }

        #endregion

        private Calibration calibration1;
    }
}