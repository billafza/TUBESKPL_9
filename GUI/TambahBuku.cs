using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class TambahBuku : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");

        public TambahBuku()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            koneksi.Open();
            SqlCommand cmd = koneksi.CreateCommand();

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "") 
            {
                MessageBox.Show("Lengkapi informasi buku!");
            }
            else
            {
                cmd.CommandText = "insert into Buku (id_buku, judul, penulis, kategori, tahun_terbit) values ('" + textBox1.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "', '" + textBox4.Text + "', " + textBox5.Text + ", 'Tersedia')";
                cmd.ExecuteNonQuery();
                MessageBox.Show("Buku berhasil ditambahkan");
            }
            koneksi.Close();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }
    }
}
