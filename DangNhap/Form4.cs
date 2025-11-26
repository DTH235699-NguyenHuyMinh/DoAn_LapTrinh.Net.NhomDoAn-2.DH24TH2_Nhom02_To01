using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DangNhap
{
    public partial class Form4 : Form
    {
        // 1. KHAI BÁO BIẾN
        private AutoScaler autoScaler;
        private Database db = new Database();
        private const decimal LUONG_CO_BAN = 1500000;
        private string currentSlrId = "";

        public Form4()
        {
            InitializeComponent();

            // 2. KHỞI TẠO AUTOSCALER
            autoScaler = new AutoScaler(this);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Load += Form4_Load;
            this.dgvLuong.CellClick += dgvLuong_CellClick;
            this.txtNgayCong.TextChanged += TinhLuongTuDong;
            this.cboNhanVien.SelectedIndexChanged += TinhLuongTuDong;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadComboBoxNhanVien();
            LoadDataToGrid();

            txtThang.Text = DateTime.Now.Month.ToString();
            txtNam.Text = DateTime.Now.Year.ToString();
            ResetControls();
        }

        // --- XỬ LÝ DỮ LIỆU (Dùng Class Database) ---

        private void LoadComboBoxNhanVien()
        {
            try
            {
                string query = "SELECT id, fullname, salarycoefficient FROM employees";
                DataTable dt = db.GetDataTable(query);

                cboNhanVien.DataSource = dt;
                cboNhanVien.DisplayMember = "fullname";
                cboNhanVien.ValueMember = "id";
                cboNhanVien.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải nhân viên: " + ex.Message); }
        }

        private void LoadDataToGrid(string keyword = "")
        {
            try
            {
                // Join bảng lương và nhân viên để hiển thị tên
                string query = @"SELECT s.slr_id, s.emp_id, e.fullname, s.month, s.year, s.workingdays, s.totalsalary 
                                 FROM salaries s JOIN employees e ON s.emp_id = e.id";

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += $" WHERE e.fullname LIKE N'%{keyword}%' OR s.emp_id LIKE '%{keyword}%'";
                }

                query += " ORDER BY s.year DESC, s.month DESC";

                DataTable dt = db.GetDataTable(query);
                dgvLuong.DataSource = dt;

                // Định dạng cột
                if (dgvLuong.Columns["slr_id"] != null) dgvLuong.Columns["slr_id"].Visible = false;
                if (dgvLuong.Columns["emp_id"] != null) dgvLuong.Columns["emp_id"].HeaderText = "Mã NV";
                if (dgvLuong.Columns["fullname"] != null) dgvLuong.Columns["fullname"].HeaderText = "Họ Tên";
                if (dgvLuong.Columns["month"] != null) dgvLuong.Columns["month"].HeaderText = "Tháng";
                if (dgvLuong.Columns["year"] != null) dgvLuong.Columns["year"].HeaderText = "Năm";
                if (dgvLuong.Columns["workingdays"] != null) dgvLuong.Columns["workingdays"].HeaderText = "Ngày Công";
                if (dgvLuong.Columns["totalsalary"] != null)
                {
                    dgvLuong.Columns["totalsalary"].HeaderText = "Tổng Lương";
                    dgvLuong.Columns["totalsalary"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message); }
        }

        private void TinhLuongTuDong(object sender, EventArgs e)
        {
            if (cboNhanVien.SelectedIndex == -1 || string.IsNullOrEmpty(txtNgayCong.Text))
            {
                txtTongLuong.Text = "0"; return;
            }
            try
            {
                DataRowView row = (DataRowView)cboNhanVien.SelectedItem;
                if (row["salarycoefficient"] != DBNull.Value)
                {
                    decimal heSo = Convert.ToDecimal(row["salarycoefficient"]);
                    decimal ngayCong = decimal.Parse(txtNgayCong.Text);
                    decimal tongLuong = LUONG_CO_BAN * heSo * (ngayCong / 26);
                    txtTongLuong.Text = Math.Round(tongLuong, 0).ToString("N0");
                }
            }
            catch { txtTongLuong.Text = "0"; }
        }

        private void ResetControls()
        {
            cboNhanVien.SelectedIndex = -1;
            txtNgayCong.Clear();
            txtTongLuong.Text = "0";
            currentSlrId = "";
            btnThem.Enabled = true;
            cboNhanVien.Enabled = true;
        }

        private void dgvLuong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLuong.Rows[e.RowIndex];
                currentSlrId = row.Cells["slr_id"].Value.ToString();
                cboNhanVien.SelectedValue = row.Cells["emp_id"].Value.ToString();
                txtThang.Text = row.Cells["month"].Value.ToString();
                txtNam.Text = row.Cells["year"].Value.ToString();
                txtNgayCong.Text = row.Cells["workingdays"].Value.ToString();

                if (row.Cells["totalsalary"].Value != DBNull.Value)
                    txtTongLuong.Text = Convert.ToDecimal(row.Cells["totalsalary"].Value).ToString("N0");

                btnThem.Enabled = false;
                cboNhanVien.Enabled = false;
            }
        }

        // --- NÚT BẤM (BUTTON) ---

        // Nút THÊM
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cboNhanVien.SelectedValue == null || string.IsNullOrEmpty(txtNgayCong.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!"); return;
            }
            try
            {
                // Kiểm tra xem đã có lương tháng này chưa
                string checkQuery = "SELECT COUNT(*) FROM salaries WHERE emp_id=@id AND month=@m AND year=@y";
                SqlParameter[] checkParams = {
                    new SqlParameter("@id", cboNhanVien.SelectedValue),
                    new SqlParameter("@m", txtThang.Text),
                    new SqlParameter("@y", txtNam.Text)
                };

                int count = (int)db.ExecuteScalar(checkQuery, checkParams);
                if (count > 0)
                {
                    MessageBox.Show("Nhân viên này đã có lương tháng này!"); return;
                }

                // Thêm mới
                decimal luong = decimal.Parse(txtTongLuong.Text.Replace(",", "").Replace(".", ""));
                string sql = "INSERT INTO salaries (emp_id, month, year, workingdays, totalsalary) VALUES (@id, @m, @y, @wd, @total)";

                SqlParameter[] insertParams = {
                    new SqlParameter("@id", cboNhanVien.SelectedValue),
                    new SqlParameter("@m", txtThang.Text),
                    new SqlParameter("@y", txtNam.Text),
                    new SqlParameter("@wd", txtNgayCong.Text),
                    new SqlParameter("@total", luong)
                };

                if (db.ExecuteQuery(sql, insertParams))
                {
                    MessageBox.Show("Thêm thành công!");
                    LoadDataToGrid();
                    ResetControls();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // Nút XÓA
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentSlrId)) return;
            if (MessageBox.Show("Xóa bản ghi lương này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    string sql = "DELETE FROM salaries WHERE slr_id = @id";
                    SqlParameter[] parameters = { new SqlParameter("@id", currentSlrId) };

                    if (db.ExecuteQuery(sql, parameters))
                    {
                        MessageBox.Show("Đã xóa!");
                        LoadDataToGrid();
                        ResetControls();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        // Nút SỬA
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentSlrId)) return;
            try
            {
                decimal luong = decimal.Parse(txtTongLuong.Text.Replace(",", "").Replace(".", ""));
                string sql = "UPDATE salaries SET workingdays=@wd, totalsalary=@total, month=@m, year=@y WHERE slr_id=@id";

                SqlParameter[] parameters = {
                    new SqlParameter("@wd", txtNgayCong.Text),
                    new SqlParameter("@total", luong),
                    new SqlParameter("@m", txtThang.Text),
                    new SqlParameter("@y", txtNam.Text),
                    new SqlParameter("@id", currentSlrId)
                };

                if (db.ExecuteQuery(sql, parameters))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadDataToGrid();
                    ResetControls();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // Nút HỦY
        private void btnHuy_Click(object sender, EventArgs e)
        {
            ResetControls();
            LoadDataToGrid();
        }

        // Nút TÌM KIẾM
        private void btnTim_Click(object sender, EventArgs e)
        {
            // Nếu bạn chưa có TextBox txtTimKiem trên Form4, hãy thêm vào
            // Hoặc thay bằng cboNhanVien.Text nếu muốn tìm theo tên đang chọn
            string keyword = "";
            if (cboNhanVien.Text != "") keyword = cboNhanVien.Text;

            // Nếu có txtTimKiem thì ưu tiên dùng nó:
            // string keyword = txtTimKiem.Text.Trim(); 

            LoadDataToGrid(keyword);
        }

        // Nút THOÁT
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 ql = new Form2();
            ql.Show();
        }

        // Các sự kiện không dùng đến (Giữ lại để tránh lỗi Designer)
        private void Form4_Resize(object sender, EventArgs e) { } // Đã có AutoScaler lo
        private void txtNam_TextChanged(object sender, EventArgs e) { }
    }
}