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
        // Khai báo các biến private (chỉ khai báo biến, KHÔNG viết lệnh xử lý ở đây)
        private float initialFormWidth = 0f;
        private float initialFontSize = 0f;
        private Label labelTieuDe;

        // Đây là hàm khởi tạo (Constructor) - Viết lệnh xử lý ở trong này
        public Form2()
        {
            InitializeComponent();

            // 1. Cấu hình Form ra giữa màn hình (Phải đặt SAU InitializeComponent)
            this.StartPosition = FormStartPosition.CenterScreen;

            // 2. Tìm và gán PictureBox
            PictureBox pictureBoxNen = this.Controls.Find("pictureBox1", true).FirstOrDefault() as PictureBox;

            // 3. Tìm và gán Label
            labelTieuDe = this.Controls.Find("label1", true).FirstOrDefault() as Label;

            // Logic xử lý hiển thị
            if (labelTieuDe != null && pictureBoxNen != null)
            {
                // Chỉ định PictureBox là Control cha của Label
                labelTieuDe.Parent = pictureBoxNen;

                // Điều chỉnh lại vị trí của Label cho đúng tọa độ trong PictureBox
                labelTieuDe.Location = new Point(labelTieuDe.Left - pictureBoxNen.Left, labelTieuDe.Top - pictureBoxNen.Top);

                // Lưu lại kích thước gốc
                initialFormWidth = this.Width;
                initialFontSize = labelTieuDe.Font.Size;
            }
            else if (labelTieuDe != null)
            {
                // Trường hợp không tìm thấy PictureBox
                initialFormWidth = this.Width;
                initialFontSize = labelTieuDe.Font.Size;
            }

            // Gán sự kiện Resize
            this.Resize += new EventHandler(Form2_Resize);
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            if (labelTieuDe == null || initialFormWidth == 0)
                return;

            // --- 1. Điều chỉnh Kích thước Font ---
            float scaleFactor = (float)this.Width / initialFormWidth;
            float newFontSize = initialFontSize * scaleFactor;

            if (newFontSize < 10) newFontSize = 10;
            if (newFontSize > 48) newFontSize = 48;

            labelTieuDe.Font = new Font(labelTieuDe.Font.FontFamily, newFontSize, labelTieuDe.Font.Style);

            // --- 2. Căn Giữa Label ---
            if (labelTieuDe.Parent != null)
            {
                // Căn giữa trong PictureBox cha
                labelTieuDe.Left = (labelTieuDe.Parent.Width - labelTieuDe.Width) / 2;
            }
            else
            {
                // Căn giữa trong Form
                labelTieuDe.Left = (this.ClientSize.Width - labelTieuDe.Width) / 2;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }
    }
}