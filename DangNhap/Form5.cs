using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DangNhap
{
    public partial class Form5 : Form
    {
        // 1. KHAI BÁO BIẾN
        private AutoScaler autoScaler;
        private Database db = new Database();

        public Form5()
        {
            InitializeComponent();

            // 2. KHỞI TẠO AUTOSCALER
            autoScaler = new AutoScaler(this);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Đăng ký các sự kiện (Event)
            this.Load += Form5_Load;
            this.dgvPhongBan.CellClick += dgvPhongBan_CellClick;

            // Gán sự kiện cho các nút
            this.btnThem.Click += btnThem_Click;
            this.btnXoa.Click += btnXoa_Click;
            this.btnSua.Click += btnSua_Click;
            this.btnHuy.Click += btnHuy_Click;
            this.btnThoat.Click += btnThoat_Click;
            this.btnTim.Click += btnTim_Click;

            // Tìm kiếm khi gõ phím
            this.txtTimKiem.TextChanged += (s, e) => LoadData(txtTimKiem.Text.Trim());
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            // Xử lý giao diện
            label1.Parent = pictureBox1;
            label1.BackColor = Color.Transparent;
            groupBox1.Parent = pictureBox1;
            groupBox1.BackColor = Color.White;

            LoadData();
            ResetControls();
        }

        // --- CÁC HÀM XỬ LÝ DỮ LIỆU ---

        private void LoadData(string keyword = "")
        {
            try
            {
                string query = @"
                        SELECT d.dept_id, d.dept_name, d.description, COUNT(e.id) as SoLuongNV 
                        FROM departments d 
                        LEFT JOIN employees e ON d.dept_id = e.dept_id";

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += $" WHERE d.dept_id LIKE N'%{keyword}%' OR d.dept_name LIKE N'%{keyword}%'";
                }

                query += " GROUP BY d.dept_id, d.dept_name, d.description";

                DataTable dt = db.GetDataTable(query);
                dgvPhongBan.DataSource = dt;

                if (dgvPhongBan.Columns["dept_id"] != null) dgvPhongBan.Columns["dept_id"].HeaderText = "Mã Phòng";
                if (dgvPhongBan.Columns["dept_name"] != null) dgvPhongBan.Columns["dept_name"].HeaderText = "Tên Phòng";
                if (dgvPhongBan.Columns["description"] != null) dgvPhongBan.Columns["description"].HeaderText = "Mô Tả";
                if (dgvPhongBan.Columns["SoLuongNV"] != null) dgvPhongBan.Columns["SoLuongNV"].HeaderText = "Số NV";

                dgvPhongBan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message); }
        }

        private void ResetControls()
        {
            txtMaPB.Clear();
            txtTenPB.Clear();
            txtMoTa.Clear();

            txtMaPB.Enabled = true;
            btnThem.Enabled = true;
        }

        private void dgvPhongBan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPhongBan.Rows[e.RowIndex];
                txtMaPB.Text = row.Cells["dept_id"].Value.ToString();
                txtTenPB.Text = row.Cells["dept_name"].Value.ToString();
                txtMoTa.Text = row.Cells["description"].Value.ToString();

                txtMaPB.Enabled = false;
                btnThem.Enabled = false;
            }
        }

        // --- CÁC NÚT CHỨC NĂNG (ĐÃ SỬA LỖI TẠI ĐÂY) ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaPB.Text) || string.IsNullOrEmpty(txtTenPB.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã và Tên phòng ban!"); return;
            }
            try
            {
                string sql = "INSERT INTO departments (dept_id, dept_name, description) VALUES (@id, @name, @desc)";

                // SỬA LỖI 1: Bọc các tham số vào mảng new SqlParameter[] { ... }
                if (db.ExecuteQuery(sql, new SqlParameter[] {
                    new SqlParameter("@id", txtMaPB.Text),
                    new SqlParameter("@name", txtTenPB.Text),
                    new SqlParameter("@desc", txtMoTa.Text)
                }))
                {
                    MessageBox.Show("Thêm thành công!");
                    LoadData();
                    ResetControls();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaPB.Text)) return;

            int soNV = 0;
            foreach (DataGridViewRow row in dgvPhongBan.Rows)
            {
                if (row.Cells["dept_id"].Value != null && row.Cells["dept_id"].Value.ToString() == txtMaPB.Text)
                {
                    soNV = Convert.ToInt32(row.Cells["SoLuongNV"].Value);
                    break;
                }
            }

            if (soNV > 0)
            {
                MessageBox.Show($"Phòng này đang có {soNV} nhân viên. Không thể xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xóa phòng {txtTenPB.Text}?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    string sql = "DELETE FROM departments WHERE dept_id = @id";

                    // SỬA LỖI 2: Bọc tham số vào mảng, dù chỉ có 1 tham số
                    if (db.ExecuteQuery(sql, new SqlParameter[] {
                        new SqlParameter("@id", txtMaPB.Text)
                    }))
                    {
                        MessageBox.Show("Đã xóa!");
                        LoadData();
                        ResetControls();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtMaPB.Enabled) return;

            try
            {
                string sql = "UPDATE departments SET dept_name = @name, description = @desc WHERE dept_id = @id";

                // SỬA LỖI 3: Bọc các tham số vào mảng new SqlParameter[] { ... }
                if (db.ExecuteQuery(sql, new SqlParameter[] {
                    new SqlParameter("@name", txtTenPB.Text),
                    new SqlParameter("@desc", txtMoTa.Text),
                    new SqlParameter("@id", txtMaPB.Text)
                }))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadData();
                    ResetControls();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            ResetControls();
            LoadData();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            LoadData(txtTimKiem.Text.Trim());
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Đảm bảo Form2 tồn tại hoặc sửa thành tên form quản lý chính của bạn
            Form2 ql = new Form2();
            ql.Show();
        }

        private void label4_Click(object sender, EventArgs e) { }
        private void txtMoTa_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}