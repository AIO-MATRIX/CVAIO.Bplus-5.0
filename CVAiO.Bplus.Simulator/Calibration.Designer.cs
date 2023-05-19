
namespace CVAiO.Bplus.Simulator
{
    partial class Calibration
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
            this.components = new System.ComponentModel.Container();
            this.PropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOutReady = new System.Windows.Forms.Button();
            this.btnOutStartAck = new System.Windows.Forms.Button();
            this.btnOutMoveAck = new System.Windows.Forms.Button();
            this.btnOutDone = new System.Windows.Forms.Button();
            this.btnOutEndAck = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btnInEnd = new System.Windows.Forms.Button();
            this.btnInDoneAck = new System.Windows.Forms.Button();
            this.btnInMove = new System.Windows.Forms.Button();
            this.btnInStart = new System.Windows.Forms.Button();
            this.btnInReady = new System.Windows.Forms.Button();
            this.tbX = new System.Windows.Forms.TextBox();
            this.tbY = new System.Windows.Forms.TextBox();
            this.tbT = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PropertyGrid
            // 
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Left;
            this.PropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.PropertyGrid.Name = "PropertyGrid";
            this.PropertyGrid.Size = new System.Drawing.Size(210, 450);
            this.PropertyGrid.TabIndex = 0;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(264, 28);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(83, 45);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "OPEN";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(353, 28);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(83, 45);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOutReady
            // 
            this.btnOutReady.Location = new System.Drawing.Point(353, 117);
            this.btnOutReady.Name = "btnOutReady";
            this.btnOutReady.Size = new System.Drawing.Size(83, 34);
            this.btnOutReady.TabIndex = 3;
            this.btnOutReady.Text = "READY";
            this.btnOutReady.UseVisualStyleBackColor = true;
            this.btnOutReady.Click += new System.EventHandler(this.btnReady_Click);
            // 
            // btnOutStartAck
            // 
            this.btnOutStartAck.Location = new System.Drawing.Point(353, 157);
            this.btnOutStartAck.Name = "btnOutStartAck";
            this.btnOutStartAck.Size = new System.Drawing.Size(83, 34);
            this.btnOutStartAck.TabIndex = 4;
            this.btnOutStartAck.Text = "START ACK";
            this.btnOutStartAck.UseVisualStyleBackColor = true;
            this.btnOutStartAck.Click += new System.EventHandler(this.btnStartAck_Click);
            // 
            // btnOutMoveAck
            // 
            this.btnOutMoveAck.Location = new System.Drawing.Point(353, 197);
            this.btnOutMoveAck.Name = "btnOutMoveAck";
            this.btnOutMoveAck.Size = new System.Drawing.Size(83, 34);
            this.btnOutMoveAck.TabIndex = 4;
            this.btnOutMoveAck.Text = "MOVE ACK";
            this.btnOutMoveAck.UseVisualStyleBackColor = true;
            this.btnOutMoveAck.Click += new System.EventHandler(this.btnMoveAck_Click);
            // 
            // btnOutDone
            // 
            this.btnOutDone.Location = new System.Drawing.Point(353, 237);
            this.btnOutDone.Name = "btnOutDone";
            this.btnOutDone.Size = new System.Drawing.Size(83, 34);
            this.btnOutDone.TabIndex = 4;
            this.btnOutDone.Text = "DONE";
            this.btnOutDone.UseVisualStyleBackColor = true;
            this.btnOutDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // btnOutEndAck
            // 
            this.btnOutEndAck.Location = new System.Drawing.Point(353, 277);
            this.btnOutEndAck.Name = "btnOutEndAck";
            this.btnOutEndAck.Size = new System.Drawing.Size(83, 34);
            this.btnOutEndAck.TabIndex = 4;
            this.btnOutEndAck.Text = "END ACK";
            this.btnOutEndAck.UseVisualStyleBackColor = true;
            this.btnOutEndAck.Click += new System.EventHandler(this.btnEndAck_Click);
            // 
            // timer
            // 
            this.timer.Interval = 200;
            // 
            // btnInEnd
            // 
            this.btnInEnd.Location = new System.Drawing.Point(264, 277);
            this.btnInEnd.Name = "btnInEnd";
            this.btnInEnd.Size = new System.Drawing.Size(83, 34);
            this.btnInEnd.TabIndex = 6;
            this.btnInEnd.Text = "END";
            this.btnInEnd.UseVisualStyleBackColor = true;
            // 
            // btnInDoneAck
            // 
            this.btnInDoneAck.Location = new System.Drawing.Point(264, 237);
            this.btnInDoneAck.Name = "btnInDoneAck";
            this.btnInDoneAck.Size = new System.Drawing.Size(83, 34);
            this.btnInDoneAck.TabIndex = 7;
            this.btnInDoneAck.Text = "DONE ACK";
            this.btnInDoneAck.UseVisualStyleBackColor = true;
            // 
            // btnInMove
            // 
            this.btnInMove.Location = new System.Drawing.Point(264, 197);
            this.btnInMove.Name = "btnInMove";
            this.btnInMove.Size = new System.Drawing.Size(83, 34);
            this.btnInMove.TabIndex = 8;
            this.btnInMove.Text = "MOVE";
            this.btnInMove.UseVisualStyleBackColor = true;
            // 
            // btnInStart
            // 
            this.btnInStart.Location = new System.Drawing.Point(264, 157);
            this.btnInStart.Name = "btnInStart";
            this.btnInStart.Size = new System.Drawing.Size(83, 34);
            this.btnInStart.TabIndex = 9;
            this.btnInStart.Text = "START";
            this.btnInStart.UseVisualStyleBackColor = true;
            // 
            // btnInReady
            // 
            this.btnInReady.Location = new System.Drawing.Point(264, 117);
            this.btnInReady.Name = "btnInReady";
            this.btnInReady.Size = new System.Drawing.Size(83, 34);
            this.btnInReady.TabIndex = 5;
            this.btnInReady.Text = "READY";
            this.btnInReady.UseVisualStyleBackColor = true;
            this.btnInReady.Click += new System.EventHandler(this.btnInReady_Click);
            // 
            // tbX
            // 
            this.tbX.Location = new System.Drawing.Point(273, 336);
            this.tbX.Name = "tbX";
            this.tbX.Size = new System.Drawing.Size(100, 20);
            this.tbX.TabIndex = 10;
            // 
            // tbY
            // 
            this.tbY.Location = new System.Drawing.Point(273, 362);
            this.tbY.Name = "tbY";
            this.tbY.Size = new System.Drawing.Size(100, 20);
            this.tbY.TabIndex = 11;
            // 
            // tbT
            // 
            this.tbT.Location = new System.Drawing.Point(273, 388);
            this.tbT.Name = "tbT";
            this.tbT.Size = new System.Drawing.Size(100, 20);
            this.tbT.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(253, 339);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(253, 366);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(253, 391);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "T:";
            // 
            // Calibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbT);
            this.Controls.Add(this.tbY);
            this.Controls.Add(this.tbX);
            this.Controls.Add(this.btnInEnd);
            this.Controls.Add(this.btnInDoneAck);
            this.Controls.Add(this.btnInMove);
            this.Controls.Add(this.btnInStart);
            this.Controls.Add(this.btnInReady);
            this.Controls.Add(this.btnOutEndAck);
            this.Controls.Add(this.btnOutDone);
            this.Controls.Add(this.btnOutMoveAck);
            this.Controls.Add(this.btnOutStartAck);
            this.Controls.Add(this.btnOutReady);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.PropertyGrid);
            this.Name = "Calibration";
            this.Size = new System.Drawing.Size(800, 450);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PropertyGrid PropertyGrid;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOutReady;
        private System.Windows.Forms.Button btnOutStartAck;
        private System.Windows.Forms.Button btnOutMoveAck;
        private System.Windows.Forms.Button btnOutDone;
        private System.Windows.Forms.Button btnOutEndAck;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button btnInEnd;
        private System.Windows.Forms.Button btnInDoneAck;
        private System.Windows.Forms.Button btnInMove;
        private System.Windows.Forms.Button btnInStart;
        private System.Windows.Forms.Button btnInReady;
        private System.Windows.Forms.TextBox tbX;
        private System.Windows.Forms.TextBox tbY;
        private System.Windows.Forms.TextBox tbT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

