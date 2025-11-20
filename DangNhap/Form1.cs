using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace DangNhap
{

    public partial class Form1 : Form
    {


        public Form1()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;

            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 500);

            // Đăng ký sự kiện
            btnOK.Click += BtnOK_Click;
            txtPassword.KeyDown += TxtPassword_KeyDown;

            // Hiện dấu * cho password (nếu chưa đặt trong Designer)
            txtPassword.UseSystemPasswordChar = true;

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text; // không Trim() để giữ khoảng trắng nếu thực sự cần

           
        }
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            // Nhấn Enter trong ô password sẽ kích nút OK
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // tránh tiếng ding
                BtnOK_Click(btnOK, EventArgs.Empty);
            }
        }

        // Hàm kiểm tra - hiện tại dùng hardcoded để test
        private bool IsValidCredentials(string user, string pass)
        {
            // Ví dụ: username = admin, password = 123
            return user == "admin" && pass == "123";
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            CreateRoundedPanel();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Tạo màu chuyển sắc xanh đậm → xanh nhạt
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(0, 102, 255),   // xanh đậm
                Color.FromArgb(0, 212, 255),   // xanh nhạt
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

       
        private void CreateRoundedPanel()
        {
            panelLogin.BackColor = Color.White;
            panelLogin.Size = new Size(350, 300);
            panelLogin.Location = new Point(
                (this.ClientSize.Width - panelLogin.Width) / 2,
                (this.ClientSize.Height - panelLogin.Height) / 2);

            panelLogin.Region = new Region(GetRoundedPath(
                new Rectangle(0, 0, panelLogin.Width, panelLogin.Height), 25));

            // Evenly arrange controls inside panelLogin
            int margin = 36;
            label1.Top = 16;
            label1.Left = (panelLogin.Width - label1.Width) / 2;

            int rowY = 80;
            int rowSpacing = 60;
            // Username row
            label2.Top = rowY;
            label2.Left = margin;
            txtUsername.Top = rowY;
            txtUsername.Left = panelLogin.Width - margin - txtUsername.Width;

            // Password row
            label3.Top = rowY + rowSpacing;
            label3.Left = margin;
            txtPassword.Top = rowY + rowSpacing;
            txtPassword.Left = panelLogin.Width - margin - txtPassword.Width;

            // Buttons: help on left, OK centered
            int buttonsY = rowY + rowSpacing * 2 + 10;
            btnOK.Top = buttonsY;
            btnOK.Left = (panelLogin.Width - btnOK.Width) / 2;

            panelLogin.BringToFront();
            panelLogin.Refresh();
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90); // góc trái trên
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90); // góc phải trên
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);   // góc phải dưới
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);  // góc trái dưới
            path.CloseFigure();

            return path;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            panelLogin.Size = new Size(400, 300);
            panelLogin.Location = new Point(
                (this.ClientSize.Width - panelLogin.Width) / 2,
                (this.ClientSize.Height - panelLogin.Height) / 2
            );
            panelLogin.BackColor = Color.White;

            panelLogin.Region = new Region(GetRoundedPath(panelLogin.ClientRectangle, 30));
        }





        private void Login_Paint(object sender, PaintEventArgs e)
        {
            int radius = 30;
            int shadowSize = 10;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw a soft light-blue shadow behind the white panel
            Color lightBlueShadow = Color.FromArgb(80, 0, 204, 255); // semi-transparent light blue
           

            // Draw the white rounded panel on top
            using (SolidBrush panelBrush = new SolidBrush(Color.White))
            using (GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, panelLogin.Width, panelLogin.Height), radius))
            {
                e.Graphics.FillPath(panelBrush, path);
            }
        }


        private void ApplyRoundedCorners(Panel panel, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(panel.Location, size);
            path.StartFigure();
            path.AddArc(arc, 180, 90);
            arc.X = panel.Width - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = panel.Height - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = 0;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            panel.Region = new Region(path);
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Kiểm tra tài khoản (ví dụ cứng để test)
           

        }
    }
}
