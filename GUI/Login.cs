using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace GUI
{
    public partial class Form1 : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Masukkan Email dan Password dengan sesuai.", "Perhatian");
            }
            else
            {
                SqlDataAdapter sqlDA = new SqlDataAdapter("select email,password,role from UserData where email = '" + textBox1.Text + "' and password = '" + textBox2.Text + "'", koneksi);
                DataTable dt = new DataTable();
                sqlDA.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["role"].ToString() == "admin")
                        {
                            this.Hide();
                            MenuAdm adm = new MenuAdm();
                            adm.Show();
                        }
                        else if (dr["role"].ToString() == "mahasiswa")
                        {
                            this.Hide();
                            MenuMhs mhs = new MenuMhs();
                            mhs.Show();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Email atau Password anda salah!", "Perhatian");
                }
                koneksi.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            koneksi.Open();
            SqlCommand cmd = koneksi.CreateCommand();

            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Lengkapi Email dan Password anda!");
            }
            else
            {
                if (!Regex.IsMatch(textBox1.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Format email tidak valid. Harus mengandung simbol '@'.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (textBox2.Text.Length < 8 || textBox2.Text.Length > 20)
                {
                    MessageBox.Show("Password minimal 8-20 karakter", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    cmd.CommandText = "insert into UserData (email, password, role) values ('" + textBox1.Text + "', '" + textBox2.Text + "', 'mahasiswa')";
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Pengguna berhasil ditambahkan");
                }
            }
            koneksi.Close();
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
