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
        // 1. CẤU HÌNH KẾT NỐI CSDL
        private const string ConnectionString = @"Data Source=.;Initial Catalog=HR_DATABASE_NET;Integrated Security=True";

        // Biến dùng cho chức năng Resize (Phóng to/Thu nhỏ)
        private float initialFormWidth;
        private float initialFormHeight;
        private Dictionary<Control, Rectangle> originalControlBounds = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalControlFontSizes = new Dictionary<Control, float>();

        public Form5()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            // Đăng ký các sự kiện Resize và Click bảng
            this.Load += Form5_Load;
            this.Resize += Form5_Resize;
            this.dgvPhongBan.CellClick += dgvPhongBan_CellClick;

            // Nếu bạn chưa kết nối sự kiện bên Design, đoạn này sẽ giúp kết nối an toàn
            // Lưu ý: Đảm bảo bạn đã đặt tên nút là btnThem, btnXoa...
            if (this.btnThem != null) this.btnThem.Click += btnThem_Click;
            if (this.btnXoa != null) this.btnXoa.Click += btnXoa_Click;
            if (this.btnSua != null) this.btnSua.Click += btnSua_Click;
            if (this.btnHuy != null) this.btnHuy.Click += btnHuy_Click;
            if (this.btnThoat != null) this.btnThoat.Click += btnThoat_Click;

            // Tìm kiếm (Giả sử bạn có nút btnTim)
            Control btnTimKiem = this.Controls["btnTim"];
            if (btnTimKiem != null) btnTimKiem.Click += (s, e) => LoadData(txtTimKiem.Text.Trim());
        }

        // --- SỰ KIỆN LOAD FORM ---
        private void Form5_Load(object sender, EventArgs e)
        {
            // Cấu hình giao diện
            if (txtMoTa != null) { txtMoTa.Multiline = true; txtMoTa.Height = 60; }

            // Lưu kích thước ban đầu để Resize
            initialFormWidth = this.Width;
            initialFormHeight = this.Height;
            SaveInitialControlInfo(this);

            // Tải dữ liệu
            LoadData();
            ResetControls();
        }

        // --- CÁC HÀM XỬ LÝ DỮ LIỆU ---

        private void LoadData(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    // Lấy thông tin phòng và đếm số nhân viên
                    string query = @"
                        SELECT d.dept_id, d.dept_name, d.description, COUNT(e.id) as SoLuongNV 
                        FROM departments d 
                        LEFT JOIN employees e ON d.dept_id = e.dept_id 
                        GROUP BY d.dept_id, d.dept_name, d.description";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query = @"
                        SELECT d.dept_id, d.dept_name, d.description, COUNT(e.id) as SoLuongNV 
                        FROM departments d 
                        LEFT JOIN employees e ON d.dept_id = e.dept_id 
                        WHERE d.dept_id LIKE @key OR d.dept_name LIKE @key
                        GROUP BY d.dept_id, d.dept_name, d.description";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@key", "%" + keyword + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvPhongBan.DataSource = dt;

                    // Đặt tên cột tiếng Việt
                    if (dgvPhongBan.Columns["dept_id"] != null) dgvPhongBan.Columns["dept_id"].HeaderText = "Mã Phòng";
                    if (dgvPhongBan.Columns["dept_name"] != null) dgvPhongBan.Columns["dept_name"].HeaderText = "Tên Phòng";
                    if (dgvPhongBan.Columns["description"] != null) dgvPhongBan.Columns["description"].HeaderText = "Mô Tả";
                    if (dgvPhongBan.Columns["SoLuongNV"] != null) dgvPhongBan.Columns["SoLuongNV"].HeaderText = "Số NV";

                    dgvPhongBan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void ResetControls()
        {
            txtMaPB.Clear(); txtTenPB.Clear(); txtMoTa.Clear();
            txtMaPB.Enabled = true;
            if (btnThem != null) btnThem.Enabled = true;
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
                if (btnThem != null) btnThem.Enabled = false;
            }
        }

        // --- CÁC NÚT CHỨC NĂNG (Theo tên bạn đặt: btnThem, btnXoa...) ---

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaPB.Text) || string.IsNullOrEmpty(txtTenPB.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã và Tên phòng ban!"); return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO departments (dept_id, dept_name, description) VALUES (@id, @name, @desc)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", txtMaPB.Text);
                    cmd.Parameters.AddWithValue("@name", txtTenPB.Text);
                    cmd.Parameters.AddWithValue("@desc", txtMoTa.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Thêm thành công!");
                    LoadData(); ResetControls();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) MessageBox.Show("Mã phòng đã tồn tại!");
                else MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaPB.Text)) return;

            // Kiểm tra nhân viên trước khi xóa
            int soNV = 0;
            foreach (DataGridViewRow row in dgvPhongBan.Rows)
            {
                if (row.Cells["dept_id"].Value.ToString() == txtMaPB.Text)
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
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        new SqlCommand($"DELETE FROM departments WHERE dept_id = '{txtMaPB.Text}'", conn).ExecuteNonQuery();
                    }
                    MessageBox.Show("Đã xóa!"); LoadData(); ResetControls();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtMaPB.Enabled) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string sql = "UPDATE departments SET dept_name = @name, description = @desc WHERE dept_id = @id";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", txtMaPB.Text);
                    cmd.Parameters.AddWithValue("@name", txtTenPB.Text);
                    cmd.Parameters.AddWithValue("@desc", txtMoTa.Text);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Cập nhật thành công!");
                    LoadData(); ResetControls();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            ResetControls();
            LoadData();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Thoát?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                this.Close();
        }

        // Hàm thừa từ code cũ của bạn, giữ lại để không báo lỗi
        private void textBox3_TextChanged(object sender, EventArgs e) { }

        // --- LOGIC RESIZE (CHUẨN) ---

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

        private void Form5_Resize(object sender, EventArgs e)
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
                // Xử lý Tiêu đề (Ghim giữa)
                if (c.Name == "label1" || c.Text.ToUpper().Contains("PHÒNG BAN"))
                {
                    if (originalControlFontSizes.ContainsKey(c))
                    {
                        float newSize = originalControlFontSizes[c] * Math.Min(scaleX, scaleY);
                        if (newSize < 14) newSize = 14;
                        c.Font = new Font("Arial", newSize, FontStyle.Bold);
                        c.AutoSize = true;
                        c.Location = new Point((this.ClientSize.Width - c.Width) / 2, 20);
                    }
                    continue;
                }

                if (originalControlBounds.ContainsKey(c))
                {
                    Rectangle rect = originalControlBounds[c];
                    c.Bounds = new Rectangle((int)(rect.X * scaleX), (int)(rect.Y * scaleY), (int)(rect.Width * scaleX), (int)(rect.Height * scaleY));
                    float newSize = originalControlFontSizes[c] * Math.Min(scaleX, scaleY);
                    if (newSize < 8) newSize = 8;
                    if (!(c is DataGridView)) c.Font = new Font(c.Font.FontFamily, newSize, c.Font.Style);
                }
                if (c.HasChildren) ResizeAllControls(c, scaleX, scaleY);
            }
        }
    }
}