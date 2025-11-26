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
    public partial class Form6 : Form
    {
        Database db = new Database();
        private AutoScaler autoScaler;

        public Form6()
        {
            InitializeComponent();
            autoScaler = new AutoScaler(this);
            cboRole.SelectedIndex = 1; // Mặc định chọn 'user'
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            LoadDataToGrid();
        }

        /// <summary>
        /// Tải dữ liệu từ bảng users và thông tin nhân viên (fullname) lên DataGridView
        /// </summary>
        private void LoadDataToGrid()
        {
            try
            {
                string query = "SELECT u.emp_id, e.fullname, u.username, u.password, u.role " +
                               "FROM users u JOIN employees e ON u.emp_id = e.id";

                DataTable dt = db.GetDataTable(query);
                dgvUsers.DataSource = dt;

                // Đổi tên cột cho dễ đọc
                dgvUsers.Columns["emp_id"].HeaderText = "Mã NV";
                dgvUsers.Columns["fullname"].HeaderText = "Họ Tên";
                dgvUsers.Columns["username"].HeaderText = "Username";
                dgvUsers.Columns["password"].HeaderText = "Mật Khẩu";
                dgvUsers.Columns["role"].HeaderText = "Quyền";

                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xóa nội dung trong các trường nhập liệu
        /// </summary>
        private void ClearInputFields()
        {
            txtEmpId.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            cboRole.SelectedIndex = 1; // Đặt lại về 'user'
            txtEmpId.Focus();
        }

        // Xử lý khi click vào một dòng trên DataGridView
        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

                // Gán dữ liệu vào các controls
                txtEmpId.Text = row.Cells["emp_id"].Value.ToString();
                txtUsername.Text = row.Cells["username"].Value.ToString();
                // Lưu ý: Mật khẩu được ẩn, nên tạm thời không lấy nếu cột password.Visible = false;
                // Nếu muốn lấy mật khẩu (chỉ phục vụ demo):
                txtPassword.Text = row.Cells["password"].Value.ToString(); 
                cboRole.Text = row.Cells["role"].Value.ToString();

                // Khóa trường Mã NV khi đang sửa/xóa
                txtEmpId.Enabled = false;
            }
        }

        //---------------------------------------------------------
        //                 CHỨC NĂNG NÚT BẤM
        //---------------------------------------------------------

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            txtEmpId.Enabled = true; // Cho phép nhập lại mã NV mới
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string empId = txtEmpId.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text; // Lưu ý: Cần mã hóa trong thực tế
            string role = cboRole.Text.Trim();

            if (string.IsNullOrEmpty(empId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Kiểm tra Mã NV có tồn tại trong bảng employees không
            object checkEmp = db.ExecuteScalar("SELECT COUNT(*) FROM employees WHERE id = @EmpId", new SqlParameter[]
            {
                new SqlParameter("@EmpId", empId)
            });

            if (checkEmp == null || (int)checkEmp == 0)
            {
                MessageBox.Show("Mã nhân viên không tồn tại trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Kiểm tra tài khoản đã tồn tại chưa
            object checkUser = db.ExecuteScalar("SELECT COUNT(*) FROM users WHERE emp_id = @EmpId OR username = @Username", new SqlParameter[]
            {
                new SqlParameter("@EmpId", empId),
                new SqlParameter("@Username", username)
            });

            if (checkUser != null && (int)checkUser > 0)
            {
                MessageBox.Show("Mã nhân viên hoặc Username này đã có tài khoản.", "Lỗi Trùng Lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Thực hiện thêm mới
            string query = "INSERT INTO users (emp_id, username, password, role) VALUES (@EmpId, @Username, @Password, @Role)";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmpId", empId),
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password),
                new SqlParameter("@Role", role)
            };

            if (db.ExecuteQuery(query, parameters))
            {
                MessageBox.Show("Thêm user thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid();
            }
            else
            {
                MessageBox.Show("Thêm user thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string empId = txtEmpId.Text.Trim(); // Không cho sửa, nhưng dùng để định danh
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string role = cboRole.Text.Trim();

            if (string.IsNullOrEmpty(empId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng chọn user và điền đầy đủ thông tin để cập nhật.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cập nhật tất cả các trường, ngoại trừ emp_id
            string query = "UPDATE users SET username = @Username, password = @Password, role = @Role WHERE emp_id = @EmpId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password),
                new SqlParameter("@Role", role),
                new SqlParameter("@EmpId", empId)
            };

            if (db.ExecuteQuery(query, parameters))
            {
                MessageBox.Show("Cập nhật quyền thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid();
            }
            else
            {
                MessageBox.Show("Cập nhật quyền thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string empId = txtEmpId.Text.Trim();

            if (string.IsNullOrEmpty(empId))
            {
                MessageBox.Show("Vui lòng chọn user cần xóa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa user của Mã NV: {empId}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM users WHERE emp_id = @EmpId";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EmpId", empId)
                };

                if (db.ExecuteQuery(query, parameters))
                {
                    MessageBox.Show("Xóa user thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToGrid();
                }
                else
                {
                    MessageBox.Show("Xóa user thất bại. User có thể không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Không cần làm gì ở đây, giữ nguyên nếu Form Designer tạo ra
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại
            this.Hide();

            // Quay về Form2 (màn hình đăng nhập)
            Form2 ql = new Form2();
            ql.Show();
        }
    }
}

