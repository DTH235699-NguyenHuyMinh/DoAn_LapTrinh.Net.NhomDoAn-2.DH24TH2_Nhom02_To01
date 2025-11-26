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
        // --- 1. BIẾN CHO RESIZE ---
        private float initialFormWidth;
        private float initialFormHeight;
        private Dictionary<Control, Rectangle> originalControlBounds = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalControlFontSizes = new Dictionary<Control, float>();

        // --- 2. BIẾN KẾT NỐI CSDL ---
        private const string ConnectionString = "Data Source=.;Initial Catalog=HR_DATABASE_NET;Integrated Security=True";

        public Form3()
        {
            InitializeComponent();

            // *** CẬP NHẬT 1: ĐỂ FORM LUÔN HIỆN GIỮA MÀN HÌNH KHI MỞ ***
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Resize += new EventHandler(Form3_Resize);
            this.dgvNhanVien.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNhanVien_CellClick);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // A. LƯU TRẠNG THÁI GIAO DIỆN BAN ĐẦU
            initialFormWidth = this.Width;
            initialFormHeight = this.Height;
            SaveInitialControlInfo(this);

            // B. TẢI DỮ LIỆU
            LoadPhongBan();
            LoadDataToGrid();
            ClearInputs();
        }

        // --- 3. XỬ LÝ RESIZE (PHÓNG TO VÀ CĂN GIỮA) ---

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

        private void Form3_Resize(object sender, EventArgs e)
        {
            if (initialFormWidth == 0 || initialFormHeight == 0) return;

            float scaleX = (float)this.Width / initialFormWidth;
            float scaleY = (float)this.Height / initialFormHeight;

            ResizeAllControls(this, scaleX, scaleY);

            // *** CẬP NHẬT 2: ÉP TIÊU ĐỀ (LABEL1) LUÔN NẰM GIỮA FORM ***
            // Giả sử tiêu đề của bạn tên là label1. Nếu tên khác (ví dụ labelTieuDe), hãy sửa 'label1' thành tên đó.
            if (label1 != null)
            {
                // Tính toán vị trí giữa: (Chiều rộng Form - Chiều rộng Label) / 2
                label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            }
        }

        private void ResizeAllControls(Control parent, float scaleX, float scaleY)
        {
            foreach (Control c in parent.Controls)
            {
                // Bỏ qua label1 trong vòng lặp này vì ta đã căn giữa nó thủ công ở trên
                if (c.Name == "label1") continue;

                if (originalControlBounds.ContainsKey(c))
                {
                    Rectangle originalRect = originalControlBounds[c];
                    float originalFontSize = originalControlFontSizes[c];

                    // Tính toán vị trí mới
                    int newX = (int)(originalRect.X * scaleX);
                    int newY = (int)(originalRect.Y * scaleY);
                    int newWidth = (int)(originalRect.Width * scaleX);
                    int newHeight = (int)(originalRect.Height * scaleY);

                    c.Bounds = new Rectangle(newX, newY, newWidth, newHeight);

                    // Tính toán cỡ chữ
                    float minScale = Math.Min(scaleX, scaleY);
                    float newFontSize = originalFontSize * minScale;
                    if (newFontSize < 8) newFontSize = 8;

                    if (!(c is DataGridView))
                    {
                        c.Font = new Font(c.Font.FontFamily, newFontSize, c.Font.Style);
                    }
                }
                if (c.HasChildren) ResizeAllControls(c, scaleX, scaleY);
            }
        }

        // --- 4. LOGIC NGHIỆP VỤ (GIỮ NGUYÊN) ---

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
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT dept_id, dept_name FROM departments ORDER BY dept_name";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cboPhongBan.DataSource = dt;
                    cboPhongBan.DisplayMember = "dept_name";
                    cboPhongBan.ValueMember = "dept_id";
                    cboPhongBan.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải phòng ban: " + ex.Message); }
        }

        private void LoadDataToGrid(string filter = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT e.*, d.dept_name FROM employees e LEFT JOIN departments d ON e.dept_id = d.dept_id";
                    if (!string.IsNullOrEmpty(filter)) query += $" WHERE e.id LIKE N'%{filter}%' OR e.fullname LIKE N'%{filter}%'";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvNhanVien.DataSource = dt;

                    // Đặt tên cột
                    if (dgvNhanVien.Columns["id"] != null) dgvNhanVien.Columns["id"].HeaderText = "Mã NV";
                    if (dgvNhanVien.Columns["fullname"] != null) dgvNhanVien.Columns["fullname"].HeaderText = "Họ và Tên";
                    if (dgvNhanVien.Columns["dept_name"] != null) dgvNhanVien.Columns["dept_name"].HeaderText = "Phòng Ban";
                    if (dgvNhanVien.Columns["position"] != null) dgvNhanVien.Columns["position"].HeaderText = "Chức Vụ";
                }
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
                if (row.Cells["dept_id"].Value != DBNull.Value) cboPhongBan.SelectedValue = row.Cells["dept_id"].Value.ToString();
                txtChucVu.Text = row.Cells["position"].Value.ToString();
                txtDiaChi.Text = row.Cells["address"].Value.ToString();
                txtDienThoai.Text = row.Cells["phone"].Value.ToString();
                if (row.Cells["birthday"].Value != DBNull.Value) dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["birthday"].Value);
                if (row.Cells["gender"].Value.ToString() == "Nam") radNam.Checked = true; else radNu.Checked = true;
                txtMaNV.ReadOnly = true;
            }
        }

        // --- 5. LOGIC BUTTON (ĐÃ SẮP XẾP 6->1) ---

        // Button 6: THÊM
        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNV.Text) || string.IsNullOrEmpty(txtHoTen.Text)) { MessageBox.Show("Thiếu thông tin!"); return; }
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO employees (id, fullname, dept_id, position, address, phone, birthday, gender, salarycoefficient) VALUES (@id, @name, @dept, @pos, @addr, @phone, @dob, @gender, @salary)", conn);
                    cmd.Parameters.AddWithValue("@id", txtMaNV.Text);
                    cmd.Parameters.AddWithValue("@name", txtHoTen.Text);
                    cmd.Parameters.AddWithValue("@dept", cboPhongBan.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pos", txtChucVu.Text);
                    cmd.Parameters.AddWithValue("@addr", txtDiaChi.Text);
                    cmd.Parameters.AddWithValue("@phone", txtDienThoai.Text);
                    cmd.Parameters.AddWithValue("@dob", dtpNgaySinh.Value);
                    cmd.Parameters.AddWithValue("@gender", radNam.Checked ? "Nam" : "Nữ");
                    cmd.Parameters.AddWithValue("@salary", 0);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Thêm thành công!"); LoadDataToGrid(); ClearInputs();
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
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        conn.Open();
                        new SqlCommand($"DELETE FROM employees WHERE id = '{txtMaNV.Text}'", conn).ExecuteNonQuery();
                    }
                    MessageBox.Show("Đã xóa!"); LoadDataToGrid(); ClearInputs();
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE employees SET fullname=@name, dept_id=@dept, position=@pos, address=@addr, phone=@phone, birthday=@dob, gender=@gender WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", txtMaNV.Text);
                    cmd.Parameters.AddWithValue("@name", txtHoTen.Text);
                    cmd.Parameters.AddWithValue("@dept", cboPhongBan.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pos", txtChucVu.Text);
                    cmd.Parameters.AddWithValue("@addr", txtDiaChi.Text);
                    cmd.Parameters.AddWithValue("@phone", txtDienThoai.Text);
                    cmd.Parameters.AddWithValue("@dob", dtpNgaySinh.Value);
                    cmd.Parameters.AddWithValue("@gender", radNam.Checked ? "Nam" : "Nữ");
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Cập nhật xong!"); LoadDataToGrid(); ClearInputs();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // Button 3: HỦY
        private void button3_Click(object sender, EventArgs e) { ClearInputs(); LoadDataToGrid(); }

        // Button 2: THOÁT
        private void button2_Click(object sender, EventArgs e)
        {
            // Ẩn form hiện tại
            this.Hide();

            // Quay về Form2
            Form2 ql = new Form2();
            ql.Show();
        }

        // Button 1: TÌM KIẾM
        private void button1_Click(object sender, EventArgs e)
        {
            string key = txtMaNV.Text.Trim();
            if (string.IsNullOrEmpty(key)) key = txtHoTen.Text.Trim();
            LoadDataToGrid(key);
        }

        private void textBox4_TextChanged(object sender, EventArgs e) { }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {

        }
    }
}