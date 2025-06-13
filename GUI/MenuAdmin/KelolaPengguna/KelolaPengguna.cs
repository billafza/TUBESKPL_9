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

namespace GUI.MenuAdmin.KelolaPengguna
{
    public partial class KelolaPengguna : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");

        public KelolaPengguna()
        {
            InitializeComponent();
        }
        public void displaydata()
        {
            koneksi.Open();
            SqlCommand cmd = koneksi.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM UserData";
            cmd.ExecuteNonQuery();
            DataTable dta = new DataTable();
            SqlDataAdapter dataadp = new SqlDataAdapter(cmd);
            dataadp.Fill(dta);
            dataGridView1.DataSource = dta;
            koneksi.Close();
        }
        private void KelolaPengguna_Load(object sender, EventArgs e)
        {
            displaydata();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
