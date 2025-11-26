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
        // 1. CẤU HÌNH KẾT NỐI
        private const string ConnectionString = @"Data Source=.;Initial Catalog=HR_DATABASE_NET;Integrated Security=True";
        private const decimal LUONG_CO_BAN = 1500000;
        private string currentSlrId = "";

        // Biến Resize
        private float initialFormWidth;
        private float initialFormHeight;
        private Dictionary<Control, Rectangle> originalControlBounds = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalControlFontSizes = new Dictionary<Control, float>();

        public Form4()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Load += Form4_Load;
            this.Resize += Form4_Resize;
            this.dgvLuong.CellClick += dgvLuong_CellClick;

            this.txtNgayCong.TextChanged += TinhLuongTuDong;
            this.cboNhanVien.SelectedIndexChanged += TinhLuongTuDong;

           
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            initialFormWidth = this.Width;
            initialFormHeight = this.Height;
            SaveInitialControlInfo(this);

            LoadComboBoxNhanVien();
            LoadDataToGrid();

            txtThang.Text = DateTime.Now.Month.ToString();
            txtNam.Text = DateTime.Now.Year.ToString();
            ResetControls();

        }

        // --- XỬ LÝ DỮ LIỆU ---
        private void LoadComboBoxNhanVien()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    // Lấy dữ liệu từ bảng employees
                    SqlDataAdapter da = new SqlDataAdapter("SELECT id, fullname, salarycoefficient FROM employees", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    cboNhanVien.DataSource = dt;
                    cboNhanVien.DisplayMember = "fullname";
                    cboNhanVien.ValueMember = "id";
                    cboNhanVien.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải nhân viên: " + ex.Message); }
        }

        private void LoadDataToGrid(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    // Join bảng lương và nhân viên để hiển thị tên
                    string query = @"SELECT s.slr_id, s.emp_id, e.fullname, s.month, s.year, s.workingdays, s.totalsalary 
                                     FROM salaries s JOIN employees e ON s.emp_id = e.id";
                    if (!string.IsNullOrEmpty(keyword))
                        query += $" WHERE e.fullname LIKE N'%{keyword}%' OR s.emp_id LIKE '%{keyword}%'";
                    query += " ORDER BY s.year DESC, s.month DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvLuong.DataSource = dt;

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
            }
            catch { }
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
            btnThem.Enabled = true; // Button1 là nút Thêm
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
                btnThem.Enabled = false; cboNhanVien.Enabled = false;
            }
        }

        // --- NÚT BẤM (BUTTON) ---

        // Nút THÊM (button1)
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cboNhanVien.SelectedValue == null || string.IsNullOrEmpty(txtNgayCong.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!"); return;
            }
            try
            {
                decimal luong = decimal.Parse(txtTongLuong.Text.Replace(",", "").Replace(".", ""));
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string check = "SELECT COUNT(*) FROM salaries WHERE emp_id=@id AND month=@m AND year=@y";
                    SqlCommand cmdCheck = new SqlCommand(check, conn);
                    cmdCheck.Parameters.AddWithValue("@id", cboNhanVien.SelectedValue);
                    cmdCheck.Parameters.AddWithValue("@m", txtThang.Text);
                    cmdCheck.Parameters.AddWithValue("@y", txtNam.Text);
                    if ((int)cmdCheck.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Nhân viên này đã có lương tháng này!"); return;
                    }
                    string sql = "INSERT INTO salaries (emp_id, month, year, workingdays, totalsalary) VALUES (@id, @m, @y, @wd, @total)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", cboNhanVien.SelectedValue);
                    cmd.Parameters.AddWithValue("@m", txtThang.Text);
                    cmd.Parameters.AddWithValue("@y", txtNam.Text);
                    cmd.Parameters.AddWithValue("@wd", txtNgayCong.Text);
                    cmd.Parameters.AddWithValue("@total", luong);
                    cmd.ExecuteNonQuery();
                    LoadDataToGrid(); ResetControls(); MessageBox.Show("Thêm thành công!");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // Nút XÓA (button2)
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentSlrId)) return;
            if (MessageBox.Show("Xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        new SqlCommand("DELETE FROM salaries WHERE slr_id = " + currentSlrId, conn).ExecuteNonQuery();
                    }
                    LoadDataToGrid(); ResetControls(); MessageBox.Show("Đã xóa!");
                }
                catch { }
            }
        }

        // Nút SỬA (button3)
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentSlrId)) return;
            try
            {
                decimal luong = decimal.Parse(txtTongLuong.Text.Replace(",", "").Replace(".", ""));
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string sql = "UPDATE salaries SET workingdays=@wd, totalsalary=@total, month=@m, year=@y WHERE slr_id=@id";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@wd", txtNgayCong.Text);
                    cmd.Parameters.AddWithValue("@total", luong);
                    cmd.Parameters.AddWithValue("@m", txtThang.Text);
                    cmd.Parameters.AddWithValue("@y", txtNam.Text);
                    cmd.Parameters.AddWithValue("@id", currentSlrId);
                    cmd.ExecuteNonQuery();
                    LoadDataToGrid(); ResetControls(); MessageBox.Show("Cập nhật thành công!");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnHuy_Click(object sender, EventArgs e) { ResetControls(); LoadDataToGrid(); } // HỦY

        // Nút TÌM KIẾM (button6)
        private void btnTim_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim(); // Cần có TextBox txtTimKiem trên form
            if (string.IsNullOrEmpty(keyword) && cboNhanVien.Text != "") keyword = cboNhanVien.Text;
            LoadDataToGrid(keyword);
        }

        // --- RESIZE ---
        private void SaveInitialControlInfo(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (!originalControlBounds.ContainsKey(c))
                {
                    originalControlBounds.Add(c, c.Bounds);
                    originalControlFontSizes.Add(c, c.Font.Size);
                }
                if (c.HasChildren) SaveInitialControlInfo(c);
            }
        }
        private void Form4_Resize(object sender, EventArgs e)
        {
            if (initialFormWidth == 0 || initialFormHeight == 0) return;

            float scaleX = (float)this.Width / initialFormWidth;
            float scaleY = (float)this.Height / initialFormHeight;

            ResizeAllControls(this, scaleX, scaleY);
        }
        private void ResizeAllControls(Control parent, float scaleX, float scaleY)
        {
            foreach (Control c in parent.Controls)
            {
                // --- TRƯỜNG HỢP 1: XỬ LÝ RIÊNG CHO TIÊU ĐỀ (LABEL1) ---
                if (c.Name == "label1" || c.Text.Contains("TIỀN LƯƠNG"))
                {
                    if (originalControlFontSizes.ContainsKey(c))
                    {
                        // 1. Chỉ chỉnh cỡ chữ to lên (Không chỉnh vị trí theo tỷ lệ nữa)
                        float newSize = originalControlFontSizes[c] * Math.Min(scaleX, scaleY);
                        if (newSize < 16) newSize = 16; // Giữ chữ luôn to, không bao giờ bé

                        c.Font = new Font("Arial", newSize, FontStyle.Bold);
                        c.AutoSize = true; // Để khung chữ tự nở ra theo nội dung

                        // 2. GHIM VỊ TRÍ (QUAN TRỌNG NHẤT)
                        // Luôn bắt nó nằm chính giữa chiều ngang
                        int centerX = (this.ClientSize.Width - c.Width) / 2;

                        // Luôn bắt nó nằm sát mép trên (cách đỉnh 20 đơn vị), không cho chạy lung tung
                        c.Location = new Point(centerX, 20);
                    }
                    continue; // Xử lý xong tiêu đề thì bỏ qua, đi tiếp
                }

                // --- TRƯỜNG HỢP 2: CÁC NÚT VÀ Ô KHÁC (GIỮ NGUYÊN) ---
                if (originalControlBounds.ContainsKey(c))
                {
                    Rectangle rect = originalControlBounds[c];

                    // Tính toán vị trí
                    int newX = (int)(rect.X * scaleX);
                    int newY = (int)(rect.Y * scaleY);
                    int newWidth = (int)(rect.Width * scaleX);
                    int newHeight = (int)(rect.Height * scaleY);

                    c.Bounds = new Rectangle(newX, newY, newWidth, newHeight);

                    // Tính toán cỡ chữ
                    float newFontSize = originalControlFontSizes[c] * Math.Min(scaleX, scaleY);
                    if (newFontSize < 8) newFontSize = 8;

                    if (!(c is DataGridView))
                    {
                        c.Font = new Font(c.Font.FontFamily, newFontSize, c.Font.Style);
                    }
                }

                if (c.HasChildren) ResizeAllControls(c, scaleX, scaleY);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại
            this.Hide();

            // Quay về Form2 
            Form2 ql = new Form2();
            ql.Show();
        }

        private void txtNam_TextChanged(object sender, EventArgs e)
        {

        }
    }
}