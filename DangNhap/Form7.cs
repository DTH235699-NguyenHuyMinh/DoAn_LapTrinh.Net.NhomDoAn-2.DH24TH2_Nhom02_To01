using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // Thư viện SQL


namespace DangNhap
{
    public partial class Form7 : Form
    {
        // Gọi class kết nối Database (Giả sử bạn đã có class này từ các form trước)
        Database db = new Database();

        // Biến lưu mã chấm công đang chọn (để sửa/xóa)
        private string selectedAtdId = "";

        public Form7()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            LoadEmployeesToComboBox(); // Tải ds nhân viên
            LoadDataToGrid();          // Tải dữ liệu chấm công

            // Cài đặt mặc định cho ComboBox đánh giá
            cboRating.Items.Clear();
            cboRating.Items.AddRange(new string[] { "A", "B", "C", "D" });
        }

        // --- 1. Tải danh sách nhân viên vào ComboBox ---
        private void LoadEmployeesToComboBox()
        {
            try
            {
                // Lấy ID và Tên để hiển thị
                string query = "SELECT id, fullname FROM employees";
                DataTable dt = db.GetDataTable(query);

                // Tạo cột hiển thị gộp (Ví dụ: NV001 - Nguyễn Văn A)
                dt.Columns.Add("DisplayInfo", typeof(string), "id + ' - ' + fullname");

                cboEmpId.DataSource = dt;
                cboEmpId.DisplayMember = "DisplayInfo";
                cboEmpId.ValueMember = "id";
                cboEmpId.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải nhân viên: " + ex.Message);
            }
        }

        // --- 2. Tải dữ liệu chấm công lên lưới ---
        // Sửa hàm này để nhận tham số keyword
        private void LoadDataToGrid(string keyword = "")
        {
            try
            {
                string query = @"SELECT a.atd_id, a.emp_id, e.fullname, a.date, a.checkin, a.checkout, a.note, a.rating 
                         FROM attendance a 
                         JOIN employees e ON a.emp_id = e.id";

                // Thêm điều kiện lọc nếu có từ khóa
                if (!string.IsNullOrEmpty(keyword))
                {
                    // Tìm theo Mã NV hoặc Tên NV
                    query += $" WHERE a.emp_id LIKE N'%{keyword}%' OR e.fullname LIKE N'%{keyword}%'";
                }

                query += " ORDER BY a.date DESC"; // Sắp xếp sau khi lọc

                DataTable dt = db.GetDataTable(query);
                dgvAttendance.DataSource = dt;

                // ... (phần đặt tên cột giữ nguyên) ...
                dgvAttendance.Columns["atd_id"].HeaderText = "ID";
                dgvAttendance.Columns["emp_id"].HeaderText = "Mã NV";
                dgvAttendance.Columns["fullname"].HeaderText = "Họ Tên";
                dgvAttendance.Columns["date"].HeaderText = "Ngày";
                dgvAttendance.Columns["checkin"].HeaderText = "Giờ Vào";
                dgvAttendance.Columns["checkout"].HeaderText = "Giờ Ra";
                dgvAttendance.Columns["note"].HeaderText = "Ghi Chú";
                dgvAttendance.Columns["rating"].HeaderText = "Đánh Giá";

                dgvAttendance.Columns["atd_id"].Visible = false;
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // --- 3. Làm mới ô nhập liệu ---
        private void ClearInputFields()
        {
            cboEmpId.SelectedIndex = -1;
            cboEmpId.Enabled = true; // Mở khóa cho phép chọn lại nhân viên
            dtpDate.Value = DateTime.Now;
            txtCheckIn.Clear();
            txtCheckOut.Clear();
            txtNote.Clear();
            cboRating.SelectedIndex = -1;
            selectedAtdId = ""; // Reset ID đang chọn
        }

        // --- 4. Sự kiện Click vào bảng ---
        private void dgvAttendance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAttendance.Rows[e.RowIndex];

                // Lấy ID chấm công để dùng cho việc Sửa/Xóa
                selectedAtdId = row.Cells["atd_id"].Value.ToString();

                // Đưa dữ liệu lên các ô nhập
                cboEmpId.SelectedValue = row.Cells["emp_id"].Value.ToString();
                cboEmpId.Enabled = false; // Không cho đổi người khi đang sửa dòng này

                dtpDate.Value = Convert.ToDateTime(row.Cells["date"].Value);

                // Xử lý hiển thị giờ (TimeSpan trong SQL -> String)
                txtCheckIn.Text = row.Cells["checkin"].Value?.ToString();
                txtCheckOut.Text = row.Cells["checkout"].Value?.ToString();

                txtNote.Text = row.Cells["note"].Value?.ToString();
                cboRating.Text = row.Cells["rating"].Value?.ToString();
            }
        }

        // ================= CHỨC NĂNG THÊM / SỬA / XÓA =================

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cboEmpId.SelectedValue == null) { MessageBox.Show("Chưa chọn nhân viên!"); return; }

            string empId = cboEmpId.SelectedValue.ToString();
            string date = dtpDate.Value.ToString("yyyy-MM-dd");
            string checkin = txtCheckIn.Text.Trim();
            string checkout = txtCheckOut.Text.Trim();
            string note = txtNote.Text;
            string rating = cboRating.Text;

            // Kiểm tra định dạng giờ đơn giản
            if (string.IsNullOrEmpty(checkin)) { MessageBox.Show("Vui lòng nhập giờ vào!"); return; }

            string query = "INSERT INTO attendance (emp_id, date, checkin, checkout, note, rating) VALUES (@eid, @date, @in, @out, @note, @rate)";

            SqlParameter[] parameters = {
                new SqlParameter("@eid", empId),
                new SqlParameter("@date", date),
                new SqlParameter("@in", checkin),
                new SqlParameter("@out", string.IsNullOrEmpty(checkout) ? (object)DBNull.Value : checkout), // Cho phép checkout rỗng
                new SqlParameter("@note", note),
                new SqlParameter("@rate", rating)
            };

            if (db.ExecuteQuery(query, parameters))
            {
                MessageBox.Show("Chấm công thành công!");
                LoadDataToGrid();
            }
            else MessageBox.Show("Lỗi khi thêm!");
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedAtdId)) { MessageBox.Show("Vui lòng chọn dòng cần sửa!"); return; }

            string checkin = txtCheckIn.Text.Trim();
            string checkout = txtCheckOut.Text.Trim();
            string note = txtNote.Text;
            string rating = cboRating.Text;
            string date = dtpDate.Value.ToString("yyyy-MM-dd");

            string query = "UPDATE attendance SET date = @date, checkin = @in, checkout = @out, note = @note, rating = @rate WHERE atd_id = @id";

            SqlParameter[] parameters = {
                new SqlParameter("@date", date),
                new SqlParameter("@in", checkin),
                new SqlParameter("@out", string.IsNullOrEmpty(checkout) ? (object)DBNull.Value : checkout),
                new SqlParameter("@note", note),
                new SqlParameter("@rate", rating),
                new SqlParameter("@id", selectedAtdId)
            };

            if (db.ExecuteQuery(query, parameters))
            {
                MessageBox.Show("Cập nhật thành công!");
                LoadDataToGrid();
            }
            else MessageBox.Show("Lỗi cập nhật!");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedAtdId)) { MessageBox.Show("Vui lòng chọn dòng cần xóa!"); return; }

            if (MessageBox.Show("Bạn chắc chắn muốn xóa bản ghi chấm công này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string query = "DELETE FROM attendance WHERE atd_id = @id";
                SqlParameter[] p = { new SqlParameter("@id", selectedAtdId) };

                if (db.ExecuteQuery(query, p))
                {
                    MessageBox.Show("Xóa thành công!");
                    LoadDataToGrid();
                }
                else MessageBox.Show("Lỗi xóa!");
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }

        // Nút Thoát (button1 cũ của bạn)
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 ql = new Form2();
            ql.Show();
        }

        // Giữ lại sự kiện cũ nếu Designer đã lỡ tạo, để tránh lỗi
        private void label1_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { btnThoat_Click(sender, e); }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            string key = txtTimKiem.Text.Trim();
            LoadDataToGrid(key);
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string key = txtTimKiem.Text.Trim();
            LoadDataToGrid(key);
        }
    }
}