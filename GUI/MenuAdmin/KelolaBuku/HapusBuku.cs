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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GUI.MenuAdmin.KelolaBuku
{
    public partial class HapusBuku : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");

        public HapusBuku()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            koneksi.Open();
            SqlCommand cmd = koneksi.CreateCommand();

            if (textBox1.Text == "")
            {
                MessageBox.Show("Masukkan ID BUKU yang ingin dihapus!");
            }
            else
            {
                DialogResult DR = MessageBox.Show("Apakah anda yakin ingin menghapus buku dengan ID: " + textBox1.Text + "?","Perhatian", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (DR == DialogResult.OK)
                {
                    cmd.CommandText = "delete from Buku where id_buku = '"+textBox1.Text+"'";
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Buku berhasil dihapus");
                }
            }
            koneksi.Close();
            textBox1.Text = "";
        }
    }
}
