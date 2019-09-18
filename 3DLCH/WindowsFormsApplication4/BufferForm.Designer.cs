namespace WindowsFormsApplication4
{
    partial class BufferForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblUnits = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBuffer = new System.Windows.Forms.Button();
            this.btnOutputLayer = new System.Windows.Forms.Button();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBufferDistance = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboLayers = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblUnits
            // 
            this.lblUnits.AutoSize = true;
            this.lblUnits.Location = new System.Drawing.Point(156, 41);
            this.lblUnits.Name = "lblUnits";
            this.lblUnits.Size = new System.Drawing.Size(53, 12);
            this.lblUnits.TabIndex = 21;
            this.lblUnits.Text = "地图单位";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(193, 107);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBuffer
            // 
            this.btnBuffer.Location = new System.Drawing.Point(68, 107);
            this.btnBuffer.Name = "btnBuffer";
            this.btnBuffer.Size = new System.Drawing.Size(75, 23);
            this.btnBuffer.TabIndex = 19;
            this.btnBuffer.Text = "分析";
            this.btnBuffer.UseVisualStyleBackColor = true;
            this.btnBuffer.Click += new System.EventHandler(this.btnBuffer_Click);
            // 
            // btnOutputLayer
            // 
            this.btnOutputLayer.Location = new System.Drawing.Point(285, 70);
            this.btnOutputLayer.Name = "btnOutputLayer";
            this.btnOutputLayer.Size = new System.Drawing.Size(40, 23);
            this.btnOutputLayer.TabIndex = 18;
            this.btnOutputLayer.Text = "...";
            this.btnOutputLayer.UseVisualStyleBackColor = true;
            this.btnOutputLayer.Click += new System.EventHandler(this.btnOutputLayer_Click);
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Location = new System.Drawing.Point(78, 72);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.ReadOnly = true;
            this.txtOutputPath.Size = new System.Drawing.Size(201, 21);
            this.txtOutputPath.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "输出图层:";
            // 
            // txtBufferDistance
            // 
            this.txtBufferDistance.Location = new System.Drawing.Point(77, 38);
            this.txtBufferDistance.Name = "txtBufferDistance";
            this.txtBufferDistance.Size = new System.Drawing.Size(73, 21);
            this.txtBufferDistance.TabIndex = 15;
            this.txtBufferDistance.Text = "1.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "缓冲半径:";
            // 
            // cboLayers
            // 
            this.cboLayers.FormattingEnabled = true;
            this.cboLayers.Location = new System.Drawing.Point(77, 10);
            this.cboLayers.Name = "cboLayers";
            this.cboLayers.Size = new System.Drawing.Size(248, 20);
            this.cboLayers.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "选择图层:";
            // 
            // BufferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 138);
            this.Controls.Add(this.lblUnits);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBuffer);
            this.Controls.Add(this.btnOutputLayer);
            this.Controls.Add(this.txtOutputPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBufferDistance);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboLayers);
            this.Controls.Add(this.label1);
            this.Name = "BufferForm";
            this.Text = "缓冲区分析";
            this.Load += new System.EventHandler(this.BufferForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUnits;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBuffer;
        private System.Windows.Forms.Button btnOutputLayer;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBufferDistance;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboLayers;
        private System.Windows.Forms.Label label1;

    }
}