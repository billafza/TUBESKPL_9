using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BukuKita.Auth;
using BukuKita.Model;
using BukuKita.View;
using static BookLibrary.BookLib;

namespace BukuKita
{
    public class MainMenu
    {
        private readonly string _configFilePath = "config.json";
        private Dictionary<string, object> _configuration;
        private AuthSystem _auth;
        private List<Buku> _books;
        private List<Peminjaman> _peminjamans;
        private List<Pengembalian> _pengembalians;
        private List<Approval> _approvals;

        // Constructor
        public MainMenu()
        {
            // Inisialisasi objek dan data
            _auth = new AuthSystem();
            _books = InitializeBooks();
            _peminjamans = new List<Peminjaman>();
            _pengembalians = new List<Pengembalian>();
            _approvals = new List<Approval>();

            // Load configuration
            _configuration = LoadConfiguration();
        }

        #region Runtime Configuration

        // Load configuration dari file
        private Dictionary<string, object> LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    string jsonString = File.ReadAllText(_configFilePath);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
            }

            // Default configuration
            return new Dictionary<string, object>
            {
                { "AppName", "BukuKita" },
                { "Version", "1.0" },
                { "MaxPeminjamanPerUser", 3 },
                { "MaxDurasiPeminjaman", 14 },
                { "DendaPerHari", 2000 },
                { "RequireApproval", true },
                { "MenuItems", new List<MenuItem>
                    {
                        new MenuItem { Id = "1", Name = "Login" },
                        new MenuItem { Id = "2", Name = "Register Mahasiswa" },
                        new MenuItem { Id = "3", Name = "Keluar" }
                    }
                }
            };
        }

        // Simpan configuration ke file
        private void SaveConfiguration()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(_configuration, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }

        // Get configuration value
        public T GetConfigValue<T>(string key, T defaultValue = default)
        {
            if (_configuration.TryGetValue(key, out object value))
            {
                if (value is JsonElement element)
                {
                    // Handle JsonElement conversion
                    switch (element.ValueKind)
                    {
                        case JsonValueKind.String:
                            return (T)(object)element.GetString();
                        case JsonValueKind.Number:
                            return (T)(object)element.GetInt32();
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            return (T)(object)element.GetBoolean();
                        default:
                            return defaultValue;
                    }
                }
                return (T)value;
            }
            return defaultValue;
        }

        // Set configuration value
        public void SetConfigValue<T>(string key, T value)
        {
            _configuration[key] = value;
            SaveConfiguration();
        }

        #endregion

        #region API Methods

        // API - Get all books
        public List<Buku> GetAllBooks()
        {
            return _books;
        }

        // API - Search books
        public List<Buku> SearchBooks(string keyword)
        {
            return _books.Where(b =>
                b.judul.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.penulis.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.kategori.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // API - Get book by ID
        public Buku GetBookById(string id)
        {
            return _books.FirstOrDefault(b => b.idBuku == id);
        }

        // API - Get all peminjaman
        public List<Peminjaman> GetAllPeminjaman()
        {
            return _peminjamans;
        }

        // API - Get pengembalian
        public List<Pengembalian> GetAllPengembalian()
        {
            return _pengembalians;
        }

        // API - Create peminjaman
        public Peminjaman CreatePeminjaman(Buku buku, string namaPeminjam)
        {
            var peminjaman = new Peminjaman(buku, namaPeminjam);
            _peminjamans.Add(peminjaman);
            return peminjaman;
        }

        // API - Create approval
        public Approval CreateApproval(string idBuku, string judulBuku, string namaPeminjam)
        {
            string idApproval = $"APV{_approvals.Count + 1:D3}";
            var approval = new Approval(idApproval, idBuku, judulBuku, namaPeminjam);
            _approvals.Add(approval);
            return approval;
        }

        // API - Get all approvals
        public List<Approval> GetAllApprovals()
        {
            return _approvals;
        }

        // API - Get pending approvals
        public List<Approval> GetPendingApprovals()
        {
            return _approvals.Where(a => a.status == "Pending").ToList();
        }

        // API - Process approval
        public void ProcessApproval(Approval approval, string newStatus, string keterangan = "")
        {
            approval.status = newStatus;

            if (!string.IsNullOrEmpty(keterangan))
            {
                approval.keterangan = keterangan;
            }

            // If approved, update book status
            if (newStatus == "Approved")
            {
                var buku = GetBookById(approval.idBuku);
                if (buku != null)
                {
                    buku.Borrower = approval.namaPeminjam;
                    buku.BorrowedAt = DateTime.Now;
                }
            }
        }

        #endregion

        #region Helper Methods

        // Initialize books
        private List<Buku> InitializeBooks()
        {
            return new List<Buku>
            {
                new Buku {idBuku = "B01", judul = "Kancil Cerdik", kategori = "Dongeng", penulis = "Cecylia", tahunTerbit = 2010},
                new Buku {idBuku = "B02", judul = "Artificial Intelegence", kategori = "Teknik Komputer", penulis = "Budiono", tahunTerbit = 2015},
                new Buku {idBuku = "B03", judul = "Bunga Sayu", kategori = "Novel", penulis = "Suci Ratna", tahunTerbit = 2020},
                new Buku {idBuku = "B04", judul = "Sejarah Indonesia", kategori = "Sejarah", penulis = "Sri Handayani", tahunTerbit = 2021},
                new Buku {idBuku = "B05", judul = "Sejarah Umum Indonesia", kategori = "Sejarah", penulis = "Sri Suryani", tahunTerbit = 2011}
            };
        }

        #endregion

        #region Main Menu

        // Display main menu
        public void DisplayMainMenu()
        {
            bool isRunning = true;
            while (isRunning)
            {
                string appName = GetConfigValue<string>("AppName", "BukuKita");
                string version = GetConfigValue<string>("Version", "1.0");

                Console.WriteLine($"\n=== {appName} v{version} ===");
                Console.WriteLine("=== MENU UTAMA ===");

                var menuItems = GetConfigValue<List<MenuItem>>("MenuItems", null);
                if (menuItems != null)
                {
                    foreach (var item in menuItems)
                    {
                        Console.WriteLine($"{item.Id}. {item.Name}");
                    }
                }
                else
                {
                    // Default menu items if configuration is not available
                    Console.WriteLine("1. Login");
                    Console.WriteLine("2. Register Mahasiswa");
                    Console.WriteLine("3. Keluar");
                }

                Console.Write("Pilih opsi: ");
                string pilihan = Console.ReadLine();

                switch (pilihan)
                {
                    case "1":
                        HandleLogin();
                        break;
                    case "2":
                        _auth.RegisterMahasiswa();
                        break;
                    case "3":
                        isRunning = false;
                        Console.WriteLine($"Terima kasih telah menggunakan {appName}!");
                        break;
                    default:
                        Console.WriteLine("Opsi tidak valid.");
                        break;
                }

                if (isRunning)
                {
                    Console.Write("\nKembali ke menu utama? (y/n): ");
                    string lanjut = Console.ReadLine();
                    if (lanjut.ToLower() != "y") isRunning = false;
                }
            }
        }

        // Handle login
        private void HandleLogin()
        {
            Console.WriteLine("\n=== LOGIN ===");
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            User userLogin = _auth.Login(email, password);
            if (userLogin != null)
            {
                Console.WriteLine($"\nLogin berhasil sebagai {userLogin.role} - {userLogin.nama}");

                if (userLogin.role.ToLower() == "admin")
                {
                    HandleAdminMenu(userLogin as Admin);
                }
                else if (userLogin.role.ToLower() == "mahasiswa")
                {
                    HandleMahasiswaMenu(userLogin as Mahasiswa);
                }
            }
            else
            {
                Console.WriteLine("\nEmail atau password salah.");
            }
        }

        // Handle admin menu
        private void HandleAdminMenu(Admin admin)
        {
            AdminView adminView = new AdminView();
            adminView.displayMenu(_books, _peminjamans, _approvals);
        }

        // Handle mahasiswa menu
        private void HandleMahasiswaMenu(Mahasiswa mahasiswa)
        {
            MahasiswaView mhsMenu = new MahasiswaView();
            mhsMenu.displayMenu(_books, _peminjamans, _pengembalians, _approvals);
        }

        // Show user approvals
        private void ShowUserApprovals(string namaPeminjam)
        {
            var userApprovals = _approvals.Where(a => a.namaPeminjam == namaPeminjam).ToList();

            if (userApprovals.Count == 0)
            {
                Console.WriteLine("Anda belum memiliki approval peminjaman.");
                return;
            }

            Console.WriteLine("\n=== STATUS APPROVAL SAYA ===");
            foreach (var approval in userApprovals)
            {
                var buku = GetBookById(approval.idBuku);
                Console.WriteLine($"ID: {approval.idApproval}, Buku: {buku?.judul ?? approval.idBuku}");
                Console.WriteLine($"Tanggal Pengajuan: {approval.tanggalPengajuan.ToString("dd/MM/yyyy HH:mm")}");
                Console.WriteLine($"Status: {approval.status}");

                if (approval.status != "Pending")
                {
                    if (!string.IsNullOrEmpty(approval.keterangan))
                    {
                        Console.WriteLine($"Keterangan: {approval.keterangan}");
                    }
                }

                Console.WriteLine("-----------------------------");
            }
        }

        // Request new approval
        private void RequestNewApproval(string namaPeminjam)
        {
            Console.WriteLine("\n=== AJUKAN PEMINJAMAN BARU ===");
            Console.WriteLine("Daftar Buku Tersedia:");

            var availableBooks = _books.Where(b => string.IsNullOrEmpty(b.Borrower)).ToList();
            if (availableBooks.Count == 0)
            {
                Console.WriteLine("Tidak ada buku yang tersedia untuk dipinjam.");
                return;
            }

            for (int i = 0; i < availableBooks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. [{availableBooks[i].idBuku}] {availableBooks[i].judul} - {availableBooks[i].penulis} ({availableBooks[i].tahunTerbit})");
            }

            Console.Write("\nPilih nomor buku yang ingin dipinjam: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= availableBooks.Count)
            {
                var selectedBook = availableBooks[index - 1];

                // Check if already has pending approval for this book
                bool alreadyRequested = _approvals.Any(a =>
                    a.idBuku == selectedBook.idBuku &&
                    a.namaPeminjam == namaPeminjam &&
                    a.status == "Pending");

                if (alreadyRequested)
                {
                    Console.WriteLine("Anda sudah mengajukan peminjaman untuk buku ini. Harap tunggu proses approval.");
                    return;
                }

                // Create new approval
                var newApproval = CreateApproval(selectedBook.idBuku, selectedBook.judul, namaPeminjam);

                // Create peminjaman with pending status
                var peminjaman = CreatePeminjaman(selectedBook, namaPeminjam);

                Console.WriteLine($"Pengajuan peminjaman buku '{selectedBook.judul}' berhasil dibuat!");
                Console.WriteLine($"ID Approval: {newApproval.idApproval}");
                Console.WriteLine("Silakan tunggu approval dari admin.");
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }
        }

        #endregion
    }

    // Class untuk menu item di configuration
    public class MenuItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}