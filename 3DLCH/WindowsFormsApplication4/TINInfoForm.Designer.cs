namespace WindowsFormsApplication4
{
    partial class TINInfoForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_X = new System.Windows.Forms.TextBox();
            this.textBox_Y = new System.Windows.Forms.TextBox();
            this.textBox_Elevation = new System.Windows.Forms.TextBox();
            this.textBox_Slope = new System.Windows.Forms.TextBox();
            this.textBox_Aspect = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 211);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Elevation:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(49, 283);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 18);
            this.label4.TabIndex = 3;
            this.label4.Text = "Slope:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(40, 355);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 18);
            this.label5.TabIndex = 4;
            this.label5.Text = "Aspect:";
            // 
            // textBox_X
            // 
            this.textBox_X.Location = new System.Drawing.Point(166, 55);
            this.textBox_X.Name = "textBox_X";
            this.textBox_X.Size = new System.Drawing.Size(129, 28);
            this.textBox_X.TabIndex = 5;
            // 
            // textBox_Y
            // 
            this.textBox_Y.Location = new System.Drawing.Point(166, 133);
            this.textBox_Y.Name = "textBox_Y";
            this.textBox_Y.Size = new System.Drawing.Size(129, 28);
            this.textBox_Y.TabIndex = 6;
            // 
            // textBox_Elevation
            // 
            this.textBox_Elevation.Location = new System.Drawing.Point(166, 208);
            this.textBox_Elevation.Name = "textBox_Elevation";
            this.textBox_Elevation.Size = new System.Drawing.Size(129, 28);
            this.textBox_Elevation.TabIndex = 7;
            // 
            // textBox_Slope
            // 
            this.textBox_Slope.Location = new System.Drawing.Point(166, 280);
            this.textBox_Slope.Name = "textBox_Slope";
            this.textBox_Slope.Size = new System.Drawing.Size(129, 28);
            this.textBox_Slope.TabIndex = 8;
            // 
            // textBox_Aspect
            // 
            this.textBox_Aspect.Location = new System.Drawing.Point(166, 352);
            this.textBox_Aspect.Name = "textBox_Aspect";
            this.textBox_Aspect.Size = new System.Drawing.Size(129, 28);
            this.textBox_Aspect.TabIndex = 9;
            // 
            // TINInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 469);
            this.Controls.Add(this.textBox_Aspect);
            this.Controls.Add(this.textBox_Slope);
            this.Controls.Add(this.textBox_Elevation);
            this.Controls.Add(this.textBox_Y);
            this.Controls.Add(this.textBox_X);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TINInfoForm";
            this.Text = "TINInfoForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_X;
        private System.Windows.Forms.TextBox textBox_Y;
        private System.Windows.Forms.TextBox textBox_Elevation;
        private System.Windows.Forms.TextBox textBox_Slope;
        private System.Windows.Forms.TextBox textBox_Aspect;
    }
}