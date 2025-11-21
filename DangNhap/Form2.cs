using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DangNhap
{
    public partial class Form2 : Form
    {
        // 1. THÊM BIẾN LƯU CHIỀU CAO BAN ĐẦU
        private float initialFormWidth = 0f;
        private float initialFormHeight = 0f; // <-- Thêm cái này
        private float initialLabelFontSize = 0f;

        private Label labelTieuDe;
        private PictureBox pictureBoxNen;

        private Dictionary<Button, Rectangle> originalButtonBounds = new Dictionary<Button, Rectangle>();
        private Dictionary<Button, float> originalButtonFontSizes = new Dictionary<Button, float>();

        public Form2()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            pictureBoxNen = this.Controls.Find("pictureBox1", true).FirstOrDefault() as PictureBox;
            labelTieuDe = this.Controls.Find("label1", true).FirstOrDefault() as Label;

            // Lưu kích thước ban đầu của Form
            initialFormWidth = this.Width;
            initialFormHeight = this.Height; // <-- Lưu chiều cao

            if (labelTieuDe != null && pictureBoxNen != null)
            {
                labelTieuDe.Parent = pictureBoxNen;
                labelTieuDe.BackColor = Color.Transparent;
                labelTieuDe.Location = new Point(labelTieuDe.Left - pictureBoxNen.Left, labelTieuDe.Top - pictureBoxNen.Top);
                initialLabelFontSize = labelTieuDe.Font.Size;
            }
            else if (labelTieuDe != null)
            {
                initialLabelFontSize = labelTieuDe.Font.Size;
            }

            this.Resize += new EventHandler(Form2_Resize);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Áp dụng cho tất cả các nút
            MakeButtonTransparent(button1);
            MakeButtonTransparent(button2);
            MakeButtonTransparent(button3);
            MakeButtonTransparent(button4);
            MakeButtonTransparent(button5);
            MakeButtonTransparent(button6);
        }

        private void MakeButtonTransparent(Button btn)
        {
            if (btn == null || pictureBoxNen == null) return;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.White;
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 255, 255, 255);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 255, 255, 255);

            Point currentPos = btn.Location;
            btn.Parent = pictureBoxNen;
            btn.Location = new Point(currentPos.X - pictureBoxNen.Left, currentPos.Y - pictureBoxNen.Top);
            btn.BringToFront();

            if (!originalButtonBounds.ContainsKey(btn))
            {
                originalButtonBounds.Add(btn, btn.Bounds);
                originalButtonFontSizes.Add(btn, btn.Font.Size);
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            if (initialFormWidth == 0 || initialFormHeight == 0) return;

            // 2. TÍNH TOÁN TỈ LỆ RIÊNG CHO CHIỀU RỘNG VÀ CHIỀU CAO
            float scaleX = (float)this.Width / initialFormWidth;
            float scaleY = (float)this.Height / initialFormHeight;

            // A. Resize Label Tiêu đề
            if (labelTieuDe != null)
            {
                // Dùng scaleX cho Font chữ tiêu đề để nó to theo bề ngang
                float newLabelSize = initialLabelFontSize * scaleX;
                if (newLabelSize < 10) newLabelSize = 10;
                labelTieuDe.Font = new Font(labelTieuDe.Font.FontFamily, newLabelSize, labelTieuDe.Font.Style);

                if (labelTieuDe.Parent != null)
                    labelTieuDe.Left = (labelTieuDe.Parent.Width - labelTieuDe.Width) / 2;
                else
                    labelTieuDe.Left = (this.ClientSize.Width - labelTieuDe.Width) / 2;
            }

            // B. Resize Các nút (Quan trọng)
            foreach (var entry in originalButtonBounds)
            {
                Button btn = entry.Key;
                Rectangle originalRect = entry.Value;
                float originalFontSize = originalButtonFontSizes[btn];

                // --- CÔNG THỨC MỚI ---
                // Vị trí ngang (X) và Chiều rộng nút -> Nhân theo tỷ lệ Width (scaleX)
                int newX = (int)(originalRect.X * scaleX);
                int newWidth = (int)(originalRect.Width * scaleX);

                // Vị trí dọc (Y) và Chiều cao nút -> Nhân theo tỷ lệ Height (scaleY)
                // Điều này đảm bảo nếu nút ở đáy (như nút Thoát), nó sẽ luôn ở đáy (90% chiều cao)
                int newY = (int)(originalRect.Y * scaleY);
                int newHeight = (int)(originalRect.Height * scaleY);

                btn.Bounds = new Rectangle(newX, newY, newWidth, newHeight);

                // Cỡ chữ: Lấy tỷ lệ nhỏ nhất giữa X và Y để chữ không bị vỡ hoặc tràn
                float minScale = Math.Min(scaleX, scaleY);
                float newBtnFontSize = originalFontSize * minScale;

                if (newBtnFontSize < 8) newBtnFontSize = 8;

                btn.Font = new Font(btn.Font.FontFamily, newBtnFontSize, btn.Font.Style);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void button6_Click(object sender, EventArgs e) 
        {
            Form1 frmDangNhap = new Form1();

            frmDangNhap.Show();

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Form3 formNV = new Form3();
            formNV.Show();
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

        private void button2_Click(object sender, EventArgs e)
        {
            Form6 formCQ = new Form6();
            formCQ.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form7 formCC = new Form7();
            formCC.Show();
            this.Hide();
        }
    }
    
}