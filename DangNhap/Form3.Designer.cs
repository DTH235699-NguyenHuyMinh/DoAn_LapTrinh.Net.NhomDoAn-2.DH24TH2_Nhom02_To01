namespace DangNhap
{
    partial class Form3
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
            this.btnNhanSu = new System.Windows.Forms.Button();
            this.btnPhongBan = new System.Windows.Forms.Button();
            this.btnLuong = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(344, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "Quản lý";
            // 
            // btnNhanSu
            // 
            this.btnNhanSu.Location = new System.Drawing.Point(55, 187);
            this.btnNhanSu.Name = "btnNhanSu";
            this.btnNhanSu.Size = new System.Drawing.Size(162, 185);
            this.btnNhanSu.TabIndex = 1;
            this.btnNhanSu.Text = "QUẢN LÝ NHÂN SỰ";
            this.btnNhanSu.UseVisualStyleBackColor = true;
            this.btnNhanSu.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnPhongBan
            // 
            this.btnPhongBan.Location = new System.Drawing.Point(316, 187);
            this.btnPhongBan.Name = "btnPhongBan";
            this.btnPhongBan.Size = new System.Drawing.Size(162, 185);
            this.btnPhongBan.TabIndex = 2;
            this.btnPhongBan.Text = "QUẢN LÝ PHÒNG BAN";
            this.btnPhongBan.UseVisualStyleBackColor = true;
            // 
            // btnLuong
            // 
            this.btnLuong.Location = new System.Drawing.Point(593, 187);
            this.btnLuong.Name = "btnLuong";
            this.btnLuong.Size = new System.Drawing.Size(153, 185);
            this.btnLuong.TabIndex = 3;
            this.btnLuong.Text = "QUẢN LÝ LƯƠNG";
            this.btnLuong.UseVisualStyleBackColor = true;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnLuong);
            this.Controls.Add(this.btnPhongBan);
            this.Controls.Add(this.btnNhanSu);
            this.Controls.Add(this.label1);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNhanSu;
        private System.Windows.Forms.Button btnPhongBan;
        private System.Windows.Forms.Button btnLuong;
    }
}