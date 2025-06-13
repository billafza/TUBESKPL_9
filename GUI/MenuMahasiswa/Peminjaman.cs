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

namespace GUI.MenuMahasiswa
{
    public partial class Peminjaman : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");
        public void DisplayBuku()
        {
            koneksi.Open();
            SqlCommand cmd = koneksi.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from Buku";
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(dt);
            dataGridView1.DataSource = dt;
            koneksi.Close();
        }

        public Peminjaman()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Lengkapi informasi Data Peminjaman!");
            }
            else
            {
                koneksi.Open();
                SqlCommand cmd = koneksi.CreateCommand();
                cmd.CommandType = CommandType.Text;

                // 1. Cek status buku
                cmd.CommandText = "SELECT status FROM Buku WHERE id_buku = '" + textBox2.Text + "'";
                string statusBuku = cmd.ExecuteScalar() as string;

                if (statusBuku != "Tersedia")
                {
                    MessageBox.Show("Buku tidak tersedia untuk dipinjam!");
                }
                else
                {
                    // 2. Jika tersedia, masukkan data peminjaman dengan status 'Menunggu Approve'
                    cmd.CommandText = "INSERT INTO Peminjaman (nama, id_buku, tanggal_peminjaman, status) VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "', '" + dateTimePicker1.Text + "', 'Menunggu Approve')";
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Peminjaman berhasil diajukan, menunggu approval admin!");
                }
            }
            koneksi.Close();
            textBox1.Text = "";
            textBox2.Text = "";
        }


        private void Peminjaman_Load(object sender, EventArgs e)
        {
            DisplayBuku();
        }
    }
}
