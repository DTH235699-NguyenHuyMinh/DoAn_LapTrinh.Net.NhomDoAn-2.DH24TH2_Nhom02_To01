using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DangNhap
{
    public partial class Form2 : Form
    {
        private DataTable dtNhanVien;
        private bool isAdding = false;  // Kiểm tra đang thêm hay sửa
        private int editingRowIndex = -1; // Ghi nhớ dòng đang sửa
        public Form2()
        {
            InitializeComponent();
            this.FormClosed += Form2_FormClosed;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Khi Form2 đóng, thoát chương trình hoàn toàn
            Application.Exit();
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cboChucVu_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboChucVu.Items.Add("Trưởng phòng");
            cboChucVu.Items.Add("Phó trưởng phòng");
            cboChucVu.Items.Add("Nhân viên");
            cboChucVu.Items.Add("Thực tập");
            cboChucVu.Items.Add("Kế toán");

            cboChucVu.SelectedIndex = 0;
        }
    }
}
