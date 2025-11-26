using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DangNhap
{
    public partial class Form2 : Form
    {
        private AutoScaler autoScaler;
        private PictureBox pictureBoxNen;

        public Form2()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            // 1. Tìm PictureBox nền
            pictureBoxNen = this.Controls.Find("pictureBox1", true).FirstOrDefault() as PictureBox;

            // 2. Làm trong suốt các nút
            // QUAN TRỌNG: Hàm này giờ sẽ reset Anchor luôn để tránh lỗi nút Thoát nhảy lung tung
            MakeButtonTransparent(button1);
            MakeButtonTransparent(button2);
            MakeButtonTransparent(button3);
            MakeButtonTransparent(button4);
            MakeButtonTransparent(button5);
            MakeButtonTransparent(button6); // Nút Thoát

            // 3. Xử lý Label tiêu đề
            Label labelTieuDe = this.Controls.Find("label1", true).FirstOrDefault() as Label;
            if (labelTieuDe != null && pictureBoxNen != null)
            {
                var originalPos = labelTieuDe.Location;
                labelTieuDe.Parent = pictureBoxNen;
                labelTieuDe.BackColor = Color.Transparent;
                labelTieuDe.Location = new Point(originalPos.X - pictureBoxNen.Left, originalPos.Y - pictureBoxNen.Top);
            }

            // 4. Kích hoạt AutoScaler
            // AutoScaler sẽ ghi nhớ vị trí MỚI (sau khi đã đưa vào PictureBox) để tính toán chuẩn xác
            autoScaler = new AutoScaler(this);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        // Hàm làm trong suốt và sửa lỗi vị trí
        private void MakeButtonTransparent(Button btn)
        {
            if (btn == null || pictureBoxNen == null) return;

            // --- BƯỚC SỬA LỖI QUAN TRỌNG ---
            // Tắt Anchor của WinForms để nó không tự ý kéo nút đi lung tung
            // Hãy để AutoScaler lo việc đó.
            btn.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            // -------------------------------

            // Cấu hình giao diện nút
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.White;
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;

            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 255, 255, 255);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 255, 255, 255);

            // Tính toán vị trí mới TRƯỚC khi đổi Parent
            Point currentPos = btn.Location;

            // Đổi cha sang PictureBox
            btn.Parent = pictureBoxNen;

            // Đặt lại vị trí tương đối theo PictureBox
            btn.Location = new Point(currentPos.X - pictureBoxNen.Left, currentPos.Y - pictureBoxNen.Top);

            btn.BringToFront();
        }

        // ================== CHUYỂN FORM ==================

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 formNV = new Form3();
            formNV.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form6 formCQ = new Form6();
            formCQ.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 formTL = new Form4();
            formTL.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form5 formPB = new Form5();
            formPB.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form7 formCC = new Form7();
            formCC.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1 frmDangNhap = new Form1();
            frmDangNhap.Show();
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}