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

namespace GUI.MenuAdmin.KelolaBuku
{
    public partial class ManajemenPeminjaman : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");

        public ManajemenPeminjaman()
        {
            InitializeComponent();
        }
        public void displaydata()
        {
            koneksi.Open();
            SqlCommand cmd = koneksi.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT Peminjaman.nama, Peminjaman.id_buku,Buku.judul from Peminjaman INNER JOIN Buku ON Peminjaman.id_buku = Buku.id_buku WHERE Peminjaman.status = 'Menunggu Approve'";
            cmd.ExecuteNonQuery();
            DataTable dta = new DataTable();
            SqlDataAdapter dataadp = new SqlDataAdapter(cmd);
            dataadp.Fill(dta);
            dataGridView1.DataSource = dta;
            koneksi.Close();

        }

        private void ManajemenPeminjaman_Load(object sender, EventArgs e)
        {
            displaydata();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Lengkapi informasi Approve Peminjaman!");
            }
            else
            {
                koneksi.Open();
                SqlCommand cmd = koneksi.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select count(*) from Peminjaman where nama = '" + textBox1.Text + "' and id_buku = '" + textBox2.Text + "'";
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    // Update status peminjaman menjadi "Disetujui"
                    cmd.CommandText = "UPDATE Peminjaman SET status = 'Disetujui' WHERE nama = '" + textBox1.Text + "' AND id_buku = '" + textBox2.Text + "'";
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Menyetujui Peminjaman Buku\nNama Peminjam: " + textBox1.Text + "\nID Buku:" + textBox2.Text + "\nPada Tanggal " + DateTime.Now.ToShortDateString());
                }
                else
                {
                    // Jika data tidak ada
                    MessageBox.Show("Data tidak ditemukan.");
                }
            }
            koneksi.Close();
            displaydata();
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Lengkapi informasi Approve Peminjaman!");
            }
            else
            {
                koneksi.Open();
                SqlCommand cmd = koneksi.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select count(*) from Peminjaman where nama = '" + textBox1.Text + "' and id_buku = '" + textBox2.Text + "'";
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    // Update status peminjaman menjadi "Disetujui"
                    cmd.CommandText = "UPDATE Peminjaman SET status = 'Ditolak' WHERE nama = '" + textBox1.Text + "' AND id_buku = '" + textBox2.Text + "'";
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Menolak Peminjaman Buku\nNama Peminjam: " + textBox1.Text + "\nID Buku:" + textBox2.Text + "\nPada Tanggal " + DateTime.Now.ToShortDateString());
                }
                else
                {
                    // Jika data tidak ada
                    MessageBox.Show("Data tidak ditemukan.");
                }
            }
            koneksi.Close();
            displaydata();
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
