using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DangNhap
{
    public partial class Form2 : Form
    {
        private float initialFormWidth = 0f;
        private float initialFontSize = 0f;
        private Label labelTieuDe;
        private PictureBox pictureBoxNen;

        public Form2()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            // Lấy PictureBox và Label
            pictureBoxNen = this.Controls.Find("pictureBox1", true).FirstOrDefault() as PictureBox;
            labelTieuDe = this.Controls.Find("label1", true).FirstOrDefault() as Label;

            // Xử lý Label (như cũ bạn đã làm tốt)
            if (labelTieuDe != null && pictureBoxNen != null)
            {
                labelTieuDe.Parent = pictureBoxNen; // Label là con của Ảnh -> Trong suốt
                labelTieuDe.BackColor = Color.Transparent;
                labelTieuDe.Location = new Point(labelTieuDe.Left - pictureBoxNen.Left, labelTieuDe.Top - pictureBoxNen.Top);
                initialFormWidth = this.Width;
                initialFontSize = labelTieuDe.Font.Size;
            }
            else if (labelTieuDe != null) // Phòng hờ không có ảnh
            {
                initialFormWidth = this.Width;
                initialFontSize = labelTieuDe.Font.Size;
            }

            this.Resize += new EventHandler(Form2_Resize);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // --- ĐÂY LÀ CHỖ SỬA QUAN TRỌNG ---
            // Gọi hàm làm đẹp cho TẤT CẢ các nút
            MakeButtonTransparent(button1);
            MakeButtonTransparent(button2);
            MakeButtonTransparent(button3);
            MakeButtonTransparent(button4);
            MakeButtonTransparent(button5);
            if (button6 != null) MakeButtonTransparent(button6);
        }

        // Hàm biến nút thành trong suốt giống Label
        private void MakeButtonTransparent(Button btn)
        {
            if (btn == null || pictureBoxNen == null) return;

            // 1. Cấu hình giao diện nút cho phẳng và đẹp
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1; // Viền mỏng màu trắng
            btn.FlatAppearance.BorderColor = Color.White;
            btn.BackColor = Color.Transparent; // Nền trong suốt
            btn.ForeColor = Color.White;       // Chữ trắng
            btn.Font = new Font("Segoe UI", 12, FontStyle.Bold); // Font đẹp hơn
            btn.Cursor = Cursors.Hand;         // Chuột thành bàn tay

            // Hiệu ứng khi rê chuột (Sáng lên tí)
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 255, 255, 255);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 255, 255, 255);

            // 2. "NHẬP KHẨU" NÚT VÀO BỨC TRANH (Phép thuật ở đây)
            // Lưu lại vị trí hiện tại trên màn hình
            Point currentPos = btn.Location;

            // Gán cha là PictureBox
            btn.Parent = pictureBoxNen;

            // Tính toán lại vị trí mới (Vì toạ độ giờ tính theo góc bức tranh)
            // Công thức: Vị trí mới = Vị trí cũ - Vị trí bức tranh
            btn.Location = new Point(currentPos.X - pictureBoxNen.Left, currentPos.Y - pictureBoxNen.Top);

            // Đẩy nút lên trên cùng để không bị che
            btn.BringToFront();
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            if (labelTieuDe == null || initialFormWidth == 0) return;
            float scaleFactor = (float)this.Width / initialFormWidth;
            float newFontSize = initialFontSize * scaleFactor;
            if (newFontSize < 10) newFontSize = 10;
            if (newFontSize > 48) newFontSize = 48;
            labelTieuDe.Font = new Font(labelTieuDe.Font.FontFamily, newFontSize, labelTieuDe.Font.Style);

            if (labelTieuDe.Parent != null)
                labelTieuDe.Left = (labelTieuDe.Parent.Width - labelTieuDe.Width) / 2;
            else
                labelTieuDe.Left = (this.ClientSize.Width - labelTieuDe.Width) / 2;
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }

    }
}