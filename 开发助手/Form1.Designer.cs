namespace 开发助手
{
    partial class Form1
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
            this.mtxtv1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtv1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtv2 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mtxtv1
            // 
            this.mtxtv1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mtxtv1.Location = new System.Drawing.Point(0, 237);
            this.mtxtv1.Multiline = true;
            this.mtxtv1.Name = "mtxtv1";
            this.mtxtv1.Size = new System.Drawing.Size(1327, 207);
            this.mtxtv1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtv2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtv1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.mtxtv1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1327, 444);
            this.panel1.TabIndex = 1;
            // 
            // txtv1
            // 
            this.txtv1.Location = new System.Drawing.Point(79, 6);
            this.txtv1.Name = "txtv1";
            this.txtv1.Size = new System.Drawing.Size(100, 21);
            this.txtv1.TabIndex = 2;
            this.txtv1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtv1_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "注释1 内容:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(201, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "中转英翻译 内容:";
            // 
            // txtv2
            // 
            this.txtv2.Location = new System.Drawing.Point(299, 6);
            this.txtv2.Name = "txtv2";
            this.txtv2.Size = new System.Drawing.Size(100, 21);
            this.txtv2.TabIndex = 2;
            this.txtv2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtv2_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1327, 444);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "icxl";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox mtxtv1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtv1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtv2;
        private System.Windows.Forms.Label label2;
    }
}

