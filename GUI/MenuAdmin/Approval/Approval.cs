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

namespace GUI.MenuAdmin.Approval
{
    public partial class Approval : Form
    {
        SqlConnection koneksi = new SqlConnection(@"Data Source=NASHBILLA;Initial Catalog=BukuKita;Integrated Security=True;TrustServerCertificate=True");

        public Approval()
        {
            InitializeComponent();
        }

        private void Approval_Load(object sender, EventArgs e)
        {
            DisplayPendingApprovals();
        }

        public void DisplayPendingApprovals()
        {
            try
            {
                koneksi.Open();
                SqlCommand cmd = koneksi.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT p.id_peminjaman, p.nama, p.id_buku, b.judul, 
                                   p.tanggal_peminjaman, p.status 
                                   FROM Peminjaman p 
                                   INNER JOIN Buku b ON p.id_buku = b.id_buku 
                                   WHERE p.status = 'Menunggu Approve'";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
                sqlDA.Fill(dt);
                dataGridView1.DataSource = dt;
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                if (koneksi.State == ConnectionState.Open)
                    koneksi.Close();
            }
        }

        public void DisplayAllApprovals()
        {
            try
            {
                koneksi.Open();
                SqlCommand cmd = koneksi.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT p.id_peminjaman, p.nama, p.id_buku, b.judul, 
                                   p.tanggal_peminjaman, p.status 
                                   FROM Peminjaman p 
                                   INNER JOIN Buku b ON p.id_buku = b.id_buku 
                                   ORDER BY p.tanggal_peminjaman DESC";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
                sqlDA.Fill(dt);
                dataGridView1.DataSource = dt;
                koneksi.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                if (koneksi.State == ConnectionState.Open)
                    koneksi.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e) // Tampilkan Pending
        {
            DisplayPendingApprovals();
        }

        private void button2_Click(object sender, EventArgs e) // Tampilkan Semua
        {
            DisplayAllApprovals();
        }

        private void button3_Click(object sender, EventArgs e) // Approve
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string idPeminjaman = selectedRow.Cells["id_peminjaman"].Value.ToString();
                string nama = selectedRow.Cells["nama"].Value.ToString();
                string idBuku = selectedRow.Cells["id_buku"].Value.ToString();
                string judul = selectedRow.Cells["judul"].Value.ToString();
                string status = selectedRow.Cells["status"].Value.ToString();

                if (status != "Menunggu Approve")
                {
                    MessageBox.Show("Hanya peminjaman dengan status 'Menunggu Approve' yang dapat disetujui!");
                    return;
                }

                DialogResult result = MessageBox.Show($"Setujui peminjaman buku '{judul}' oleh {nama}?",
                                                    "Konfirmasi Approval",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        koneksi.Open();
                        SqlCommand cmd = koneksi.CreateCommand();
                        cmd.CommandType = CommandType.Text;

                        // Update status peminjaman
                        cmd.CommandText = "UPDATE Peminjaman SET status = 'Disetujui' WHERE id_peminjaman = " + idPeminjaman;
                        cmd.ExecuteNonQuery();

                        // Update status buku menjadi Dipinjam
                        cmd.CommandText = "UPDATE Buku SET status = 'Dipinjam' WHERE id_buku = '" + idBuku + "'";
                        cmd.ExecuteNonQuery();

                        koneksi.Close();
                        MessageBox.Show("Peminjaman berhasil disetujui!");
                        DisplayPendingApprovals();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        if (koneksi.State == ConnectionState.Open)
                            koneksi.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih peminjaman yang akan disetujui!");
            }
        }

        private void button4_Click(object sender, EventArgs e) // Reject
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string idPeminjaman = selectedRow.Cells["id_peminjaman"].Value.ToString();
                string nama = selectedRow.Cells["nama"].Value.ToString();
                string judul = selectedRow.Cells["judul"].Value.ToString();
                string status = selectedRow.Cells["status"].Value.ToString();

                if (status != "Menunggu Approve")
                {
                    MessageBox.Show("Hanya peminjaman dengan status 'Menunggu Approve' yang dapat ditolak!");
                    return;
                }

                DialogResult result = MessageBox.Show($"Tolak peminjaman buku '{judul}' oleh {nama}?",
                                                    "Konfirmasi Penolakan",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        koneksi.Open();
                        SqlCommand cmd = koneksi.CreateCommand();
                        cmd.CommandType = CommandType.Text;

                        // Update status peminjaman
                        cmd.CommandText = "UPDATE Peminjaman SET status = 'Ditolak' WHERE id_peminjaman = " + idPeminjaman;
                        cmd.ExecuteNonQuery();

                        koneksi.Close();
                        MessageBox.Show("Peminjaman berhasil ditolak!");
                        DisplayPendingApprovals();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        if (koneksi.State == ConnectionState.Open)
                            koneksi.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih peminjaman yang akan ditolak!");
            }
        }

        private void button5_Click(object sender, EventArgs e) // Cari
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Masukkan kata kunci pencarian!");
                return;
            }

            try
            {
                koneksi.Open();
                SqlCommand cmd = koneksi.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT p.id_peminjaman, p.nama, p.id_buku, b.judul, 
                                   p.tanggal_peminjaman, p.status 
                                   FROM Peminjaman p 
                                   INNER JOIN Buku b ON p.id_buku = b.id_buku 
                                   WHERE p.nama LIKE '%" + textBox1.Text + @"%' 
                                   OR b.judul LIKE '%" + textBox1.Text + @"%' 
                                   OR p.id_buku LIKE '%" + textBox1.Text + @"%' 
                                   OR p.status LIKE '%" + textBox1.Text + @"%'";
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter sqlDA = new SqlDataAdapter(cmd);
                sqlDA.Fill(dt);
                dataGridView1.DataSource = dt;
                koneksi.Close();
                textBox1.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                if (koneksi.State == ConnectionState.Open)
                    koneksi.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e) // Kembali
        {
            this.Hide();
        }
    }
}