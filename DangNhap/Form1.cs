using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
        Database db = new Database();

        public Form1()
        {
            this.AutoScaleMode = AutoScaleMode.None;

            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(818, 497);

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

        

        private void Form1_Load(object sender, EventArgs e)
        {
            panelLogin.Size = new Size(400, 300);
            panelLogin.Location = new Point(
                (this.ClientSize.Width - panelLogin.Width) / 2,
                (this.ClientSize.Height - panelLogin.Height) / 2
            );
            panelLogin.BackColor = Color.White;

            panelLogin.Region = new Region(GetRoundedPath(panelLogin.ClientRectangle, 30));

            button1.Text = "x";                        // Chỉ chữ X
            button1.Font = new Font("Arial", 14, FontStyle.Bold);
            button1.ForeColor = Color.White;          // Chữ X màu đen
            button1.BackColor = Color.Transparent;     // Nền nút trong suốt
            button1.FlatStyle = FlatStyle.Flat;        // Bỏ viền
            button1.FlatAppearance.BorderSize = 0;     // Không viền

            // Kích thước và vị trí
            button1.Width = 30;
            button1.Height = 30;
            button1.Top = 0;
            button1.Left = this.ClientSize.Width - button1.Width; // góc trên bên phải
            button1.TextAlign = ContentAlignment.MiddleCenter;   // chữ X căn giữa

            // Giữ vị trí khi resize form
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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


        private void btnOK_Click_1(object sender, EventArgs e)
        {
            Database db = new Database(); // Khởi tạo helper
            string user = txtUsername.Text;
            string pass = txtPassword.Text;

            string query = "SELECT role, emp_id FROM users WHERE username = @u AND password = @p";
            SqlParameter[] parameters = {
        new SqlParameter("@u", user),
        new SqlParameter("@p", pass)
    };

            DataTable dt = db.GetDataTable(query, parameters);

            if (dt.Rows.Count > 0)
            {
                string role = dt.Rows[0]["role"].ToString();

                // --- SỬA LỖI TẠI ĐÂY: Khai báo biến empId rõ ràng ---
                string empId = "";
                if (dt.Rows[0]["emp_id"] != DBNull.Value)
                {
                    empId = dt.Rows[0]["emp_id"].ToString();
                }

                MessageBox.Show("Đăng nhập thành công! Vai trò: " + role);
                this.Hide();

                if (role == "admin")
                {
                    Form2 adminForm = new Form2(); // Form admin
                    adminForm.Show();
                }
                else if (role == "user")
                {
                    Form8 userForm = new Form8(empId);
                    userForm.Show();
                }
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Dừng chương trình, đóng tất cả form
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
