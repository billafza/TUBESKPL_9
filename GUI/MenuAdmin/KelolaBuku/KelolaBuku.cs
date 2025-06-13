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
    public partial class KelolaBuku : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");
        public KelolaBuku()
        {
            InitializeComponent();
        }

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

        private void button1_Click(object sender, EventArgs e)
        {
            DisplayBuku();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TambahBuku tambah = new TambahBuku();
            tambah.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HapusBuku hapus = new HapusBuku();
            hapus.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            koneksi.Open();
            SqlCommand cmd = koneksi.CreateCommand();
            cmd.CommandType = CommandType.Text;
            string input = textBox1.Text;
            int number;

            if (int.TryParse(input, out number))
            {
                cmd.CommandText = "select * from Buku where tahun_terbit = '" + textBox1.Text + "'";
            }
            else
            {
                cmd.CommandText = "select * from Buku where id_buku = '" + textBox1.Text + "' " +
                    "OR judul = '" + textBox1.Text + "' " +
                    "OR penulis = '" + textBox1.Text + "' " +
                    "OR kategori = '" + textBox1.Text + "' ";
            }

            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(dt);
            dataGridView1.DataSource = dt;
            koneksi.Close();
            textBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ManajemenPeminjaman manage = new ManajemenPeminjaman();
            manage.Show();
        }
    }
}
