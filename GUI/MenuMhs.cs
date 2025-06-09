using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ComponentModel.Design.Serialization;

namespace GUI
{
    public partial class MenuMhs : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");

        public MenuMhs()
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

        private void button6_Click(object sender, EventArgs e)
        {
            DisplayBuku();
        }

        private void button4_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult DR = MessageBox.Show("Apakah anda yakin untuk keluar?", "Peringatan", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (DR == DialogResult.OK)
            {
                this.Hide();
                Form1 login = new Form1();
                login.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TambahBuku tambah = new TambahBuku();
            tambah.Show();
        }
    }
}
