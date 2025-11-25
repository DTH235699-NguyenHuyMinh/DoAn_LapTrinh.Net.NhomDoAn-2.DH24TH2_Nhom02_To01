using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DangNhap
{
    public partial class Form8 : Form
    {
        private string currentEmpId;
        Database db = new Database();
        private AutoScaler autoScaler;
        private Dictionary<Control, Rectangle> originalControlBounds = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalControlFontSizes = new Dictionary<Control, float>();


        public Form8(String empId)
        {
            InitializeComponent();
            autoScaler = new AutoScaler(this);
            this.currentEmpId = empId;
            this.StartPosition = FormStartPosition.CenterScreen; // Vị trí này đã có sẵn
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            LoadProfile();
            CheckAttendanceStatus();
            LoadAttendanceHistory();

            // Timer chạy đồng hồ
            Timer t = new Timer();
            t.Interval = 1000;
            t.Tick += (s, args) => { lblTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); };
            t.Start();
        }

        // ================= TAB 1: CHẤM CÔNG =================
        private void CheckAttendanceStatus()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string query = "SELECT checkin, checkout FROM attendance WHERE emp_id = @id AND date = @date";
            SqlParameter[] p = {
                new SqlParameter("@id", currentEmpId),
                new SqlParameter("@date", today)
            };

            DataTable dt = db.GetDataTable(query, p);

            if (dt.Rows.Count == 0)
            {
                lblStatus.Text = "Chào ngày mới! Vui lòng Check-in.";
                lblStatus.ForeColor = Color.Red;
                btnCheckIn.Enabled = true;
                btnCheckOut.Enabled = false;
            }
            else
            {
                var row = dt.Rows[0];
                if (row["checkout"] == DBNull.Value)
                {
                    lblStatus.Text = "Đang làm việc (Đã Check-in lúc " + row["checkin"] + ")";
                    lblStatus.ForeColor = Color.Green;
                    btnCheckIn.Enabled = false;
                    btnCheckOut.Enabled = true;
                }
                else
                {
                    lblStatus.Text = "Bạn đã hoàn thành công việc hôm nay!";
                    lblStatus.ForeColor = Color.Blue;
                    btnCheckIn.Enabled = false;
                    btnCheckOut.Enabled = false;
                }
            }
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            // Logic Insert giữ nguyên, SQL tự hiểu string
            string query = "INSERT INTO attendance (emp_id, date, checkin, note) VALUES (@id, @date, @time, N'Đang làm việc')";
            SqlParameter[] p = {
                new SqlParameter("@id", currentEmpId),
                new SqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                new SqlParameter("@time", DateTime.Now.ToString("HH:mm:ss"))
            };

            if (db.ExecuteQuery(query, p))
            {
                MessageBox.Show("Check-in thành công!");
                CheckAttendanceStatus();
                LoadAttendanceHistory();
            }
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            string query = "UPDATE attendance SET checkout = @time, note = N'Đã về' WHERE emp_id = @id AND date = @date";
            SqlParameter[] p = {
                new SqlParameter("@time", DateTime.Now.ToString("HH:mm:ss")),
                new SqlParameter("@id", currentEmpId),
                new SqlParameter("@date", DateTime.Now.ToString("yyyy-MM-dd"))
            };

            if (db.ExecuteQuery(query, p))
            {
                MessageBox.Show("Check-out thành công! Hẹn gặp lại.");
                CheckAttendanceStatus();
                LoadAttendanceHistory();
            }
        }

        // ================= TAB 2: HỒ SƠ =================
        private void LoadProfile()
        {
            // Join bảng để lấy tên phòng ban
            string query = @"SELECT e.*, d.dept_name 
                             FROM employees e 
                             JOIN departments d ON e.dept_id = d.dept_id 
                             WHERE e.id = @id";
            SqlParameter[] p = { new SqlParameter("@id", currentEmpId) };

            DataTable dt = db.GetDataTable(query, p);
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                txtFullName.Text = r["fullname"].ToString();
                txtPosition.Text = r["position"].ToString();
                txtDept.Text = r["dept_name"].ToString();
                txtPhone.Text = r["phone"].ToString();
                txtAddress.Text = r["address"].ToString();
            }
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            // Sửa câu lệnh Update theo ID string
            string query = "UPDATE employees SET phone = @phone, address = @addr WHERE id = @id";
            SqlParameter[] p = {
                new SqlParameter("@phone", txtPhone.Text),
                new SqlParameter("@addr", txtAddress.Text),
                new SqlParameter("@id", currentEmpId)
            };

            if (db.ExecuteQuery(query, p)) MessageBox.Show("Cập nhật thông tin thành công!");
        }

        // ================= TAB 3: LỊCH SỬ & LƯƠNG =================
        private void LoadAttendanceHistory()
        {
            string query = "SELECT date, checkin, checkout, note FROM attendance WHERE emp_id = @id ORDER BY date DESC";
            SqlParameter[] p = { new SqlParameter("@id", currentEmpId) };
            dgvHistory.DataSource = db.GetDataTable(query, p);
        }

        private void btnViewSalary_Click(object sender, EventArgs e)
        {
            string query = "SELECT totalsalary, workingdays FROM salaries WHERE emp_id = @id AND month = @m AND year = @y";
            SqlParameter[] p = {
                new SqlParameter("@id", currentEmpId),
                new SqlParameter("@m", cboMonth.Text),
                new SqlParameter("@y", txtYear.Text)
            };

            DataTable dt = db.GetDataTable(query, p);
            if (dt.Rows.Count > 0)
            {
                decimal luong = Convert.ToDecimal(dt.Rows[0]["totalsalary"]);
                int ngayCong = Convert.ToInt32(dt.Rows[0]["workingdays"]);
                lblSalaryResult.Text = string.Format("Lương tháng {0}/{1}: {2:N0} VNĐ (Ngày công: {3})",
                    cboMonth.Text, txtYear.Text, luong, ngayCong);
            }
            else
            {
                lblSalaryResult.Text = "Chưa có dữ liệu lương.";
            }
        }

        // ================= TAB 4: ĐỔI MẬT KHẨU =================
        private void btnChangePass_Click(object sender, EventArgs e)
        {
            if (txtNewPass.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            // Database mới: Bảng users dùng emp_id làm khoá chính luôn
            string queryCheck = "SELECT count(*) FROM users WHERE emp_id = @id AND password = @old";
            SqlParameter[] pCheck = {
                new SqlParameter("@id", currentEmpId),
                new SqlParameter("@old", txtOldPass.Text)
            };

            int count = (int)db.ExecuteScalar(queryCheck, pCheck);
            if (count > 0)
            {
                string queryUpd = "UPDATE users SET password = @new WHERE emp_id = @id";
                SqlParameter[] pUpd = {
                    new SqlParameter("@new", txtNewPass.Text),
                    new SqlParameter("@id", currentEmpId)
                };
                db.ExecuteQuery(queryUpd, pUpd);
                MessageBox.Show("Đổi mật khẩu thành công!");
            }
            else
            {
                MessageBox.Show("Mật khẩu cũ không đúng!");
            }
        }


        private void tabAttendance_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại
            this.Hide();

            // Quay về Form1 (màn hình đăng nhập)
            Form1 login = new Form1();
            login.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại
            this.Hide();

            // Quay về Form1 (màn hình đăng nhập)
            Form1 login = new Form1();
            login.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại
            this.Hide();

            // Quay về Form1 (màn hình đăng nhập)
            Form1 login = new Form1();
            login.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại
            this.Hide();

            // Quay về Form1 (màn hình đăng nhập)
            Form1 login = new Form1();
            login.Show();
        }
    }
}
