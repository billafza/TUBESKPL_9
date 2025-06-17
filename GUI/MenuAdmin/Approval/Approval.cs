using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data; // PENTING: Untuk DataTable
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using API.Services;
using BukuKita;
using BukuKita.Model;

namespace GUI.MenuAdmin.Approval
{
    public partial class Approval : Form
    {
        private readonly ApprovalService _approvalService;
        private readonly MainMenu _mainMenu;
        private List<BukuKita.Model.Approval> _currentApprovals = new List<BukuKita.Model.Approval>();

        public Approval()
        {
            InitializeComponent();

            try
            {
                // Initialize MainMenu dan ApprovalService
                _mainMenu = new MainMenu();
                _approvalService = new ApprovalService(_mainMenu);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing services: {ex.Message}", "Initialization Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Approval_Load(object sender, EventArgs e)
        {
            // Test service connection
            if (_approvalService == null)
            {
                MessageBox.Show("ApprovalService tidak berhasil diinisialisasi!", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load pending approvals by default
            DisplayPendingApprovals();
        }

        public void DisplayPendingApprovals()
        {
            try
            {
                var approvals = _approvalService.GetPendingApprovals();
                _currentApprovals = approvals;

                var dataTable = ConvertApprovalsToDataTable(approvals);
                dataGridView1.DataSource = dataTable;

                FormatDataGridView();
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading pending approvals: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DisplayAllApprovals()
        {
            try
            {
                var approvals = _approvalService.GetAllApprovals();
                _currentApprovals = approvals;

                var dataTable = ConvertApprovalsToDataTable(approvals);
                dataGridView1.DataSource = dataTable;

                FormatDataGridView();
                UpdateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading all approvals: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Explicitly specify System.Data.DataTable to avoid ambiguity
        private System.Data.DataTable ConvertApprovalsToDataTable(List<BukuKita.Model.Approval> approvals)
        {
            var dataTable = new System.Data.DataTable();

            // Add columns
            dataTable.Columns.Add("id_approval", typeof(string));
            dataTable.Columns.Add("nama", typeof(string));
            dataTable.Columns.Add("id_buku", typeof(string));
            dataTable.Columns.Add("judul", typeof(string));
            dataTable.Columns.Add("tanggal_peminjaman", typeof(DateTime));
            dataTable.Columns.Add("status", typeof(string));
            dataTable.Columns.Add("keterangan", typeof(string));

            // Add rows
            foreach (var approval in approvals)
            {
                dataTable.Rows.Add(
                    approval.idApproval,
                    approval.namaPeminjam,
                    approval.idBuku,
                    approval.judulBuku,
                    approval.tanggalPengajuan,
                    approval.status,
                    approval.keterangan ?? ""
                );
            }

            return dataTable;
        }

        private void FormatDataGridView()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                // Set column headers
                dataGridView1.Columns["id_approval"].HeaderText = "ID Approval";
                dataGridView1.Columns["nama"].HeaderText = "Nama Peminjam";
                dataGridView1.Columns["id_buku"].HeaderText = "ID Buku";
                dataGridView1.Columns["judul"].HeaderText = "Judul Buku";
                dataGridView1.Columns["tanggal_peminjaman"].HeaderText = "Tanggal Pengajuan";
                dataGridView1.Columns["status"].HeaderText = "Status";
                dataGridView1.Columns["keterangan"].HeaderText = "Keterangan";

                // Set column widths
                dataGridView1.Columns["id_approval"].Width = 100;
                dataGridView1.Columns["nama"].Width = 150;
                dataGridView1.Columns["id_buku"].Width = 80;
                dataGridView1.Columns["judul"].Width = 200;
                dataGridView1.Columns["tanggal_peminjaman"].Width = 140;
                dataGridView1.Columns["status"].Width = 120;
                dataGridView1.Columns["keterangan"].Width = 150;

                // Format tanggal
                dataGridView1.Columns["tanggal_peminjaman"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }
        }

        private void UpdateStatusLabel()
        {
            try
            {
                var statusCounts = _currentApprovals.GroupBy(a => a.status)
                                                  .ToDictionary(g => g.Key, g => g.Count());
                var statusText = string.Join(" | ", statusCounts.Select(kv => $"{kv.Key}: {kv.Value}"));
                this.Text = $"Kelola Approval Peminjaman - {statusText}";
            }
            catch
            {
                this.Text = "Kelola Approval Peminjaman";
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
                string idApproval = selectedRow.Cells["id_approval"].Value?.ToString() ?? "";
                string nama = selectedRow.Cells["nama"].Value?.ToString() ?? "";
                string judul = selectedRow.Cells["judul"].Value?.ToString() ?? "";
                string status = selectedRow.Cells["status"].Value?.ToString() ?? "";

                if (!status.Equals("Pending", StringComparison.OrdinalIgnoreCase) &&
                    !status.Equals("Menunggu Approve", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Hanya approval dengan status 'Pending' yang dapat disetujui!");
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
                        var approvalResult = _approvalService.ProcessApproval(idApproval, "Approved", "Disetujui melalui GUI Admin");

                        if (approvalResult.IsSuccess)
                        {
                            MessageBox.Show("Peminjaman berhasil disetujui!", "Sukses",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DisplayPendingApprovals(); // Refresh
                        }
                        else
                        {
                            MessageBox.Show($"Gagal menyetujui: {approvalResult.Message}", "Error",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih approval yang akan disetujui!");
            }
        }

        private void button4_Click(object sender, EventArgs e) // Reject
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string idApproval = selectedRow.Cells["id_approval"].Value?.ToString() ?? "";
                string nama = selectedRow.Cells["nama"].Value?.ToString() ?? "";
                string judul = selectedRow.Cells["judul"].Value?.ToString() ?? "";
                string status = selectedRow.Cells["status"].Value?.ToString() ?? "";

                if (!status.Equals("Pending", StringComparison.OrdinalIgnoreCase) &&
                    !status.Equals("Menunggu Approve", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Hanya approval dengan status 'Pending' yang dapat ditolak!");
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
                        var approvalResult = _approvalService.ProcessApproval(idApproval, "Rejected", "Ditolak melalui GUI Admin");

                        if (approvalResult.IsSuccess)
                        {
                            MessageBox.Show("Peminjaman berhasil ditolak!", "Sukses",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DisplayPendingApprovals(); // Refresh
                        }
                        else
                        {
                            MessageBox.Show($"Gagal menolak: {approvalResult.Message}", "Error",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih approval yang akan ditolak!");
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
                string keyword = textBox1.Text.Trim();

                var filteredApprovals = _currentApprovals.Where(a =>
                    a.namaPeminjam.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    a.judulBuku.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    a.idBuku.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    a.status.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    a.idApproval.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                var dataTable = ConvertApprovalsToDataTable(filteredApprovals);
                dataGridView1.DataSource = dataTable;
                FormatDataGridView();

                textBox1.Text = "";
                MessageBox.Show($"Ditemukan {filteredApprovals.Count} hasil pencarian.", "Hasil Pencarian",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during search: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e) // Kembali
        {
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Optional: Handle cell click events
        }
    }
}