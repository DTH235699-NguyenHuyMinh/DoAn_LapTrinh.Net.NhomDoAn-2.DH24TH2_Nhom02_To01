using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Data.SqlClient; 
using System.Windows.Forms;

namespace DangNhap
{
    public partial class Form3 : Form
    {
        // 1. Khai báo AutoScaler và Database
        private AutoScaler autoScaler;
        private Database db = new Database();

        public Form3()
        {
            InitializeComponent();

            // 2. Khởi tạo AutoScaler
            autoScaler = new AutoScaler(this);

            this.StartPosition = FormStartPosition.CenterScreen;
            this.dgvNhanVien.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNhanVien_CellClick);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            LoadPhongBan();
            LoadDataToGrid();
            ClearInputs();
        }

        // --- LOGIC NGHIỆP VỤ (Đã chuyển sang dùng class Database) ---

        private void ClearInputs()
        {
            txtMaNV.Clear();
            txtHoTen.Clear();
            cboPhongBan.SelectedIndex = -1;
            txtChucVu.Clear();
            txtDiaChi.Clear();
            txtDienThoai.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            radNam.Checked = true;
            txtMaNV.ReadOnly = false;
        }

        private void LoadPhongBan()
        {
            try
            {
                string query = "SELECT dept_id, dept_name FROM departments ORDER BY dept_name";

                // Sử dụng hàm GetDataTable từ class Database của bạn
                DataTable dt = db.GetDataTable(query);

                cboPhongBan.DataSource = dt;
                cboPhongBan.DisplayMember = "dept_name";
                cboPhongBan.ValueMember = "dept_id";
                cboPhongBan.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải phòng ban: " + ex.Message); }
        }

        private void LoadDataToGrid(string filter = "")
        {
            try
            {
                string query = "SELECT e.*, d.dept_name FROM employees e LEFT JOIN departments d ON e.dept_id = d.dept_id";

                // Xử lý tìm kiếm
                if (!string.IsNullOrEmpty(filter))
                {
                    query += $" WHERE e.id LIKE N'%{filter}%' OR e.fullname LIKE N'%{filter}%'";
                }

                // Sử dụng hàm GetDataTable từ class Database
                DataTable dt = db.GetDataTable(query);
                dgvNhanVien.DataSource = dt;

                // Đặt tên cột hiển thị
                if (dgvNhanVien.Columns["id"] != null) dgvNhanVien.Columns["id"].HeaderText = "Mã NV";
                if (dgvNhanVien.Columns["fullname"] != null) dgvNhanVien.Columns["fullname"].HeaderText = "Họ và Tên";
                if (dgvNhanVien.Columns["dept_name"] != null) dgvNhanVien.Columns["dept_name"].HeaderText = "Phòng Ban";
                if (dgvNhanVien.Columns["position"] != null) dgvNhanVien.Columns["position"].HeaderText = "Chức Vụ";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message); }
        }

        private void dgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvNhanVien.Rows[e.RowIndex];
                txtMaNV.Text = row.Cells["id"].Value.ToString();
                txtHoTen.Text = row.Cells["fullname"].Value.ToString();

                if (row.Cells["dept_id"].Value != DBNull.Value)
                    cboPhongBan.SelectedValue = row.Cells["dept_id"].Value.ToString();

                txtChucVu.Text = row.Cells["position"].Value.ToString();
                txtDiaChi.Text = row.Cells["address"].Value.ToString();
                txtDienThoai.Text = row.Cells["phone"].Value.ToString();

                if (row.Cells["birthday"].Value != DBNull.Value)
                    dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["birthday"].Value);

                if (row.Cells["gender"].Value.ToString() == "Nam")
                    radNam.Checked = true;
                else
                    radNu.Checked = true;

                txtMaNV.ReadOnly = true;
            }
        }

        // --- CÁC NÚT CHỨC NĂNG (Sử dụng db.ExecuteQuery) ---

        // Button 6: THÊM
        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNV.Text) || string.IsNullOrEmpty(txtHoTen.Text))
            {
                MessageBox.Show("Thiếu thông tin!");
                return;
            }

            try
            {
                string query = "INSERT INTO employees (id, fullname, dept_id, position, address, phone, birthday, gender, salarycoefficient) VALUES (@id, @name, @dept, @pos, @addr, @phone, @dob, @gender, @salary)";

                // Tạo mảng tham số
                SqlParameter[] parameters = {
                    new SqlParameter("@id", txtMaNV.Text),
                    new SqlParameter("@name", txtHoTen.Text),
                    new SqlParameter("@dept", cboPhongBan.SelectedValue ?? DBNull.Value),
                    new SqlParameter("@pos", txtChucVu.Text),
                    new SqlParameter("@addr", txtDiaChi.Text),
                    new SqlParameter("@phone", txtDienThoai.Text),
                    new SqlParameter("@dob", dtpNgaySinh.Value),
                    new SqlParameter("@gender", radNam.Checked ? "Nam" : "Nữ"),
                    new SqlParameter("@salary", 0) // Mặc định hệ số lương là 0
                };

                // Gọi hàm ExecuteQuery từ class Database
                if (db.ExecuteQuery(query, parameters))
                {
                    MessageBox.Show("Thêm thành công!");
                    LoadDataToGrid();
                    ClearInputs();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // Button 5: XÓA
        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNV.Text)) return;

            if (MessageBox.Show("Xóa nhân viên " + txtMaNV.Text + "?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM employees WHERE id = @id";
                    SqlParameter[] parameters = { new SqlParameter("@id", txtMaNV.Text) };

                    if (db.ExecuteQuery(query, parameters))
                    {
                        MessageBox.Show("Đã xóa!");
                        LoadDataToGrid();
                        ClearInputs();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        // Button 4: SỬA
        private void button4_Click(object sender, EventArgs e)
        {
            if (txtMaNV.ReadOnly == false) return;

            try
            {
                string query = "UPDATE employees SET fullname=@name, dept_id=@dept, position=@pos, address=@addr, phone=@phone, birthday=@dob, gender=@gender WHERE id=@id";

                SqlParameter[] parameters = {
                    new SqlParameter("@id", txtMaNV.Text),
                    new SqlParameter("@name", txtHoTen.Text),
                    new SqlParameter("@dept", cboPhongBan.SelectedValue ?? DBNull.Value),
                    new SqlParameter("@pos", txtChucVu.Text),
                    new SqlParameter("@addr", txtDiaChi.Text),
                    new SqlParameter("@phone", txtDienThoai.Text),
                    new SqlParameter("@dob", dtpNgaySinh.Value),
                    new SqlParameter("@gender", radNam.Checked ? "Nam" : "Nữ")
                };

                if (db.ExecuteQuery(query, parameters))
                {
                    MessageBox.Show("Cập nhật xong!");
                    LoadDataToGrid();
                    ClearInputs();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // Button 3: HỦY (Làm mới)
        private void button3_Click(object sender, EventArgs e)
        {
            ClearInputs();
            LoadDataToGrid();
        }

        // Button 2: THOÁT
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 ql = new Form2();
            ql.Show();
        }

        // Button 1: TÌM KIẾM
        private void button1_Click(object sender, EventArgs e)
        {
            string key = txtTimKiem.Text.Trim();
            LoadDataToGrid(key);
        }

        // Sự kiện tìm kiếm tức thì khi gõ
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string key = txtTimKiem.Text.Trim();
            LoadDataToGrid(key);
        }

        // Các sự kiện thừa (Giữ lại để tránh lỗi Designer nếu đã lỡ click tạo)
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
    }
}