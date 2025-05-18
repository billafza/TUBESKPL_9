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
                        new MenuItem { Id = "3", Name = "Pengaturan" }, // Tambahkan menu pengaturan
                        new MenuItem { Id = "4", Name = "Keluar" }
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
                Console.WriteLine("Konfigurasi berhasil disimpan!");
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

        // Tambahkan menu konfigurasi
        private void HandleConfigurationMenu()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n=== MENU PENGATURAN APLIKASI ===");
                Console.WriteLine($"1. Nama Aplikasi (Saat ini: {GetConfigValue<string>("AppName")})");
                Console.WriteLine($"2. Versi (Saat ini: {GetConfigValue<string>("Version")})");
                Console.WriteLine($"3. Maks. Peminjaman per User (Saat ini: {GetConfigValue<int>("MaxPeminjamanPerUser")})");
                Console.WriteLine($"4. Maks. Durasi Peminjaman (Saat ini: {GetConfigValue<int>("MaxDurasiPeminjaman")} hari)");
                Console.WriteLine($"5. Denda per Hari (Saat ini: Rp {GetConfigValue<int>("DendaPerHari")})");
                Console.WriteLine($"6. Wajib Approval (Saat ini: {(GetConfigValue<bool>("RequireApproval") ? "Ya" : "Tidak")})");
                Console.WriteLine($"7. Kelola Item Menu");
                Console.WriteLine("0. Kembali ke Menu Utama");
                Console.Write("Pilih pengaturan: ");

                string pilihan = Console.ReadLine();

                switch (pilihan)
                {
                    case "1":
                        Console.Write("Masukkan nama aplikasi baru: ");
                        string appName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(appName))
                        {
                            SetConfigValue("AppName", appName);
                        }
                        break;
                    case "2":
                        Console.Write("Masukkan versi baru: ");
                        string version = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(version))
                        {
                            SetConfigValue("Version", version);
                        }
                        break;
                    case "3":
                        Console.Write("Masukkan maksimal peminjaman per user: ");
                        if (int.TryParse(Console.ReadLine(), out int maxPeminjaman) && maxPeminjaman > 0)
                        {
                            SetConfigValue("MaxPeminjamanPerUser", maxPeminjaman);
                        }
                        else
                        {
                            Console.WriteLine("Input tidak valid! Nilai harus berupa angka positif.");
                        }
                        break;
                    case "4":
                        Console.Write("Masukkan maksimal durasi peminjaman (hari): ");
                        if (int.TryParse(Console.ReadLine(), out int maxDurasi) && maxDurasi > 0)
                        {
                            SetConfigValue("MaxDurasiPeminjaman", maxDurasi);
                        }
                        else
                        {
                            Console.WriteLine("Input tidak valid! Nilai harus berupa angka positif.");
                        }
                        break;
                    case "5":
                        Console.Write("Masukkan denda per hari (Rp): ");
                        if (int.TryParse(Console.ReadLine(), out int denda) && denda >= 0)
                        {
                            SetConfigValue("DendaPerHari", denda);
                        }
                        else
                        {
                            Console.WriteLine("Input tidak valid! Nilai harus berupa angka non-negatif.");
                        }
                        break;
                    case "6":
                        Console.Write("Wajibkan approval untuk peminjaman? (y/n): ");
                        string requireApproval = Console.ReadLine().ToLower();
                        if (requireApproval == "y" || requireApproval == "n")
                        {
                            SetConfigValue("RequireApproval", requireApproval == "y");
                        }
                        else
                        {
                            Console.WriteLine("Input tidak valid! Masukkan 'y' atau 'n'.");
                        }
                        break;
                    case "7":
                        HandleMenuItemsConfiguration();
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }
        }

        // Menu untuk mengelola item menu
        private void HandleMenuItemsConfiguration()
        {
            bool isRunning = true;
            while (isRunning)
            {
                var menuItems = GetConfigValue<List<MenuItem>>("MenuItems", new List<MenuItem>());

                Console.WriteLine("\n=== KELOLA ITEM MENU ===");
                Console.WriteLine("Menu saat ini:");

                for (int i = 0; i < menuItems.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {menuItems[i].Id}. {menuItems[i].Name}");
                }

                Console.WriteLine("\nPilihan:");
                Console.WriteLine("1. Tambah Item Menu");
                Console.WriteLine("2. Ubah Item Menu");
                Console.WriteLine("3. Hapus Item Menu");
                Console.WriteLine("0. Kembali");
                Console.Write("Pilih: ");

                string pilihan = Console.ReadLine();

                switch (pilihan)
                {
                    case "1":
                        Console.Write("Masukkan ID menu baru: ");
                        string id = Console.ReadLine();
                        Console.Write("Masukkan nama menu baru: ");
                        string name = Console.ReadLine();

                        if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(name))
                        {
                            menuItems.Add(new MenuItem { Id = id, Name = name });
                            SetConfigValue("MenuItems", menuItems);
                        }
                        else
                        {
                            Console.WriteLine("ID dan Nama tidak boleh kosong!");
                        }
                        break;
                    case "2":
                        Console.Write("Masukkan nomor item yang ingin diubah (1-" + menuItems.Count + "): ");
                        if (int.TryParse(Console.ReadLine(), out int editIndex) && editIndex >= 1 && editIndex <= menuItems.Count)
                        {
                            Console.Write($"Masukkan ID baru (sebelumnya '{menuItems[editIndex - 1].Id}'): ");
                            string newId = Console.ReadLine();
                            Console.Write($"Masukkan nama baru (sebelumnya '{menuItems[editIndex - 1].Name}'): ");
                            string newName = Console.ReadLine();

                            if (!string.IsNullOrWhiteSpace(newId))
                            {
                                menuItems[editIndex - 1].Id = newId;
                            }

                            if (!string.IsNullOrWhiteSpace(newName))
                            {
                                menuItems[editIndex - 1].Name = newName;
                            }

                            SetConfigValue("MenuItems", menuItems);
                        }
                        else
                        {
                            Console.WriteLine("Nomor item tidak valid!");
                        }
                        break;
                    case "3":
                        Console.Write("Masukkan nomor item yang ingin dihapus (1-" + menuItems.Count + "): ");
                        if (int.TryParse(Console.ReadLine(), out int deleteIndex) && deleteIndex >= 1 && deleteIndex <= menuItems.Count)
                        {
                            Console.Write($"Yakin ingin menghapus '{menuItems[deleteIndex - 1].Name}'? (y/n): ");
                            if (Console.ReadLine().ToLower() == "y")
                            {
                                menuItems.RemoveAt(deleteIndex - 1);
                                SetConfigValue("MenuItems", menuItems);
                                Console.WriteLine("Item menu berhasil dihapus!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nomor item tidak valid!");
                        }
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }
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
                    Console.WriteLine("3. Pengaturan");
                    Console.WriteLine("4. Keluar");
                }

                Console.Write("Pilih opsi: ");
                string pilihan = Console.ReadLine();

                // Check if pilihan exists in menuItems
                var selectedItem = menuItems?.FirstOrDefault(i => i.Id == pilihan);

                if (selectedItem != null)
                {
                    switch (selectedItem.Name.ToLower())
                    {
                        case "login":
                            HandleLogin();
                            break;
                        case "register mahasiswa":
                            _auth.RegisterMahasiswa();
                            break;
                        case "pengaturan":
                            HandleConfigurationMenu();
                            break;
                        case "keluar":
                            isRunning = false;
                            Console.WriteLine($"Terima kasih telah menggunakan {appName}!");
                            break;
                        default:
                            Console.WriteLine($"Menu '{selectedItem.Name}' belum diimplementasikan.");
                            break;
                    }
                }
                else
                {
                    // Fallback to standard menu if configuration-based menu fails
                    switch (pilihan)
                    {
                        case "1":
                            HandleLogin();
                            break;
                        case "2":
                            _auth.RegisterMahasiswa();
                            break;
                        case "3":
                            HandleConfigurationMenu();
                            break;
                        case "4":
                            isRunning = false;
                            Console.WriteLine($"Terima kasih telah menggunakan {appName}!");
                            break;
                        default:
                            Console.WriteLine("Opsi tidak valid.");
                            break;
                    }
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

            // Tambahkan opsi untuk masuk ke menu konfigurasi
            bool showConfigMenu = true;
            while (showConfigMenu)
            {
                Console.WriteLine("\n=== MENU ADMIN TAMBAHAN ===");
                Console.WriteLine("1. Pengaturan Aplikasi");
                Console.WriteLine("0. Kembali ke Menu Utama");
                Console.Write("Pilih: ");

                string pilihan = Console.ReadLine();

                switch (pilihan)
                {
                    case "1":
                        HandleConfigurationMenu();
                        break;
                    case "0":
                        showConfigMenu = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }
        }

        // Handle mahasiswa menu
        private void HandleMahasiswaMenu(Mahasiswa mahasiswa)
        {
            MahasiswaView mhsMenu = new MahasiswaView();
            mhsMenu.displayMenu(_books, _peminjamans, _pengembalians, _approvals);

            // Tambahkan menu untuk melihat status approval peminjaman
            bool requireApproval = GetConfigValue<bool>("RequireApproval", true);
            if (requireApproval)
            {
                bool checkApproval = true;
                while (checkApproval)
                {
                    Console.WriteLine("\n=== MENU PEMINJAMAN TAMBAHAN ===");
                    Console.WriteLine("1. Lihat Status Approval Saya");
                    Console.WriteLine("2. Ajukan Peminjaman Baru");
                    Console.WriteLine("0. Kembali ke Menu Utama");
                    Console.Write("Pilih: ");

                    string pilihan = Console.ReadLine();

                    switch (pilihan)
                    {
                        case "1":
                            ShowUserApprovals(mahasiswa.nama);
                            break;
                        case "2":
                            RequestNewApproval(mahasiswa.nama);
                            break;
                        case "0":
                            checkApproval = false;
                            break;
                        default:
                            Console.WriteLine("Pilihan tidak valid!");
                            break;
                    }
                }
            }
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