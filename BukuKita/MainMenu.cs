using BukuKita.Auth;
using BukuKita.Model;
using BukuKita.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static BookLibrary.BookLib;

namespace BukuKita
{
    /// <summary>
    /// Kelas MainMenu yang menangani fungsionalitas utama aplikasi konsol dan konfigurasi
    /// </summary>
    public class MainMenu
    {
        private readonly string _configFilePath = "config.json";
        private Dictionary<string, object> _configuration;
        private AuthSystem _auth;
        private List<Buku> _books;
        private List<Peminjaman> _peminjamans;
        private List<Pengembalian> _pengembalians;
        private List<Approval> _approvals;

        /// <summary>
        /// Invariant kelas:
        /// - Path file konfigurasi harus valid
        /// - Sistem autentikasi harus diinisialisasi dengan benar
        /// - Daftar buku, peminjaman, pengembalian, dan approval harus selalu diinisialisasi
        /// </summary>

        /// <summary>
        /// Menginisialisasi instance baru dari kelas MainMenu
        /// </summary>
        public MainMenu()
        {
            // Prekondisi: Tidak ada untuk konstruktor

            // Inisialisasi objek dan data
            _auth = new AuthSystem();
            _books = InitializeBooks();
            _peminjamans = new List<Peminjaman>();
            _pengembalians = new List<Pengembalian>();
            _approvals = new List<Approval>();

            // Load konfigurasi
            _configuration = LoadConfiguration();

            // Postkondisi
            Debug.Assert(_auth != null, "Sistem autentikasi harus diinisialisasi");
            Debug.Assert(_books != null && _books.Count > 0, "Daftar buku harus diinisialisasi dan tidak kosong");
            Debug.Assert(_peminjamans != null, "Daftar peminjaman harus diinisialisasi");
            Debug.Assert(_pengembalians != null, "Daftar pengembalian harus diinisialisasi");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");
            Debug.Assert(_configuration != null, "Konfigurasi harus dimuat");
        }

        #region Konfigurasi

        /// <summary>
        /// Memuat konfigurasi dari file atau membuat konfigurasi default jika file tidak ada
        /// </summary>
        /// <returns>Dictionary berisi nilai konfigurasi</returns>
        private Dictionary<string, object> LoadConfiguration()
        {
            // Prekondisi: ConfigFilePath harus diatur
            Debug.Assert(!string.IsNullOrEmpty(_configFilePath), "Path file konfigurasi harus diatur");

            try
            {
                if (File.Exists(_configFilePath))
                {
                    string jsonString = File.ReadAllText(_configFilePath);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var config = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, options);

                    // Postkondisi: Konfigurasi yang dimuat tidak boleh null
                    Debug.Assert(config != null, "Konfigurasi yang dimuat tidak boleh null");
                    return config;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
            }

            // Konfigurasi default
            var defaultConfig = new Dictionary<string, object>
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
                        new MenuItem { Id = "3", Name = "Pengaturan" },
                        new MenuItem { Id = "4", Name = "Keluar" }
                    }
                }
            };

            // Postkondisi: Konfigurasi default harus berisi kunci yang diperlukan
            Debug.Assert(defaultConfig.ContainsKey("AppName"), "Konfigurasi default harus berisi AppName");
            Debug.Assert(defaultConfig.ContainsKey("Version"), "Konfigurasi default harus berisi Version");
            Debug.Assert(defaultConfig.ContainsKey("MenuItems"), "Konfigurasi default harus berisi MenuItems");

            return defaultConfig;
        }

        /// <summary>
        /// Menyimpan konfigurasi saat ini ke file
        /// </summary>
        private void SaveConfiguration()
        {
            // Prekondisi: Konfigurasi harus diinisialisasi dan path file harus valid
            Debug.Assert(_configuration != null, "Konfigurasi harus diinisialisasi sebelum disimpan");
            Debug.Assert(!string.IsNullOrEmpty(_configFilePath), "Path file konfigurasi harus diatur");

            try
            {
                string jsonString = JsonSerializer.Serialize(_configuration, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configFilePath, jsonString);
                Console.WriteLine("Konfigurasi berhasil disimpan!");

                // Postkondisi: File konfigurasi harus ada setelah disimpan
                Debug.Assert(File.Exists(_configFilePath), "File konfigurasi harus ada setelah disimpan");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// Mendapatkan nilai konfigurasi dengan kunci tertentu
        /// </summary>
        /// <typeparam name="T">Tipe nilai konfigurasi</typeparam>
        /// <param name="key">Kunci konfigurasi yang dicari</param>
        /// <param name="defaultValue">Nilai default jika kunci tidak ditemukan</param>
        /// <returns>Nilai konfigurasi atau default jika tidak ditemukan</returns>
        public T GetConfigValue<T>(string key, T defaultValue = default)
        {
            // Prekondisi: Kunci tidak boleh null atau kosong dan konfigurasi harus diinisialisasi
            Debug.Assert(!string.IsNullOrEmpty(key), "Kunci konfigurasi tidak boleh null atau kosong");
            Debug.Assert(_configuration != null, "Konfigurasi harus diinisialisasi");

            if (_configuration.TryGetValue(key, out object value))
            {
                if (value is JsonElement element)
                {
                    // Menangani konversi JsonElement
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

        /// <summary>
        /// Menetapkan nilai konfigurasi dengan kunci tertentu dan menyimpan konfigurasi ke file
        /// </summary>
        /// <typeparam name="T">Tipe nilai konfigurasi</typeparam>
        /// <param name="key">Kunci konfigurasi yang akan diatur</param>
        /// <param name="value">Nilai yang akan ditetapkan untuk kunci konfigurasi</param>
        public void SetConfigValue<T>(string key, T value)
        {
            // Prekondisi: Kunci tidak boleh null atau kosong dan konfigurasi harus diinisialisasi
            Debug.Assert(!string.IsNullOrEmpty(key), "Kunci konfigurasi tidak boleh null atau kosong");
            Debug.Assert(_configuration != null, "Konfigurasi harus diinisialisasi");

            _configuration[key] = value;
            SaveConfiguration();

            // Postkondisi: Konfigurasi harus berisi nilai baru
            Debug.Assert(_configuration.ContainsKey(key), "Konfigurasi harus berisi kunci baru");
            Debug.Assert(_configuration[key].Equals(value), "Nilai konfigurasi harus cocok dengan nilai yang ditetapkan");
        }

        /// <summary>
        /// Menampilkan dan menangani menu konfigurasi
        /// </summary>
        private void HandleConfigurationMenu()
        {
            // Prekondisi: Konfigurasi harus diinisialisasi
            Debug.Assert(_configuration != null, "Konfigurasi harus diinisialisasi");

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

        /// <summary>
        /// Menangani submenu konfigurasi item menu
        /// </summary>
        private void HandleMenuItemsConfiguration()
        {
            // Prekondisi: Konfigurasi harus diinisialisasi
            Debug.Assert(_configuration != null, "Konfigurasi harus diinisialisasi");

            bool isRunning = true;
            while (isRunning)
            {
                var menuItems = GetConfigValue<List<MenuItem>>("MenuItems", new List<MenuItem>());

                // Invariant: Daftar item menu harus diinisialisasi
                Debug.Assert(menuItems != null, "Daftar item menu tidak boleh null");

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

                            // Postkondisi untuk kasus ini: Menu items harus berisi item baru
                            Debug.Assert(menuItems.Any(m => m.Id == id && m.Name == name),
                                "Daftar item menu harus berisi item yang baru ditambahkan");
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
                                var deletedItem = menuItems[deleteIndex - 1];
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

        #region Console Application Methods

        /// <summary>
        /// Mendapatkan semua buku dalam sistem (untuk console application)
        /// </summary>
        /// <returns>Daftar semua buku</returns>
        public List<Buku> GetAllBooks()
        {
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");
            return _books.ToList();
        }

        /// <summary>
        /// Mencari buku berdasarkan kata kunci
        /// </summary>
        /// <param name="keyword">Kata kunci pencarian</param>
        /// <returns>Daftar buku yang cocok</returns>
        public List<Buku> SearchBooks(string keyword)
        {
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");
            Debug.Assert(keyword != null, "Kata kunci pencarian tidak boleh null");

            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Buku>();

            var result = _books.Where(b =>
                b.judul.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.penulis.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.kategori.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            return result;
        }

        /// <summary>
        /// Mendapatkan buku berdasarkan ID
        /// </summary>
        /// <param name="id">ID buku</param>
        /// <returns>Buku atau null jika tidak ditemukan</returns>
        public Buku GetBookById(string id)
        {
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");
            Debug.Assert(!string.IsNullOrEmpty(id), "ID buku tidak boleh null atau kosong");

            return _books.FirstOrDefault(b => b.idBuku == id);
        }

        /// <summary>
        /// Membuat catatan peminjaman baru
        /// </summary>
        /// <param name="buku">Buku yang akan dipinjam</param>
        /// <param name="namaPeminjam">Nama peminjam</param>
        /// <returns>Catatan peminjaman</returns>
        public Peminjaman CreatePeminjaman(Buku buku, string namaPeminjam)
        {
            Debug.Assert(buku != null, "Buku tidak boleh null");
            Debug.Assert(!string.IsNullOrEmpty(namaPeminjam), "Nama peminjam tidak boleh null atau kosong");
            Debug.Assert(_peminjamans != null, "Daftar peminjaman harus diinisialisasi");

            var peminjaman = new Peminjaman(buku, namaPeminjam);
            _peminjamans.Add(peminjaman);

            return peminjaman;
        }

        /// <summary>
        /// Membuat catatan approval baru
        /// </summary>
        /// <param name="idBuku">ID buku</param>
        /// <param name="judulBuku">Judul buku</param>
        /// <param name="namaPeminjam">Nama peminjam</param>
        /// <returns>Approval yang dibuat</returns>
        public Approval CreateApproval(string idBuku, string judulBuku, string namaPeminjam)
        {
            Debug.Assert(!string.IsNullOrEmpty(idBuku), "ID buku tidak boleh null atau kosong");
            Debug.Assert(!string.IsNullOrEmpty(judulBuku), "Judul buku tidak boleh null atau kosong");
            Debug.Assert(!string.IsNullOrEmpty(namaPeminjam), "Nama peminjam tidak boleh null atau kosong");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

            string idApproval = $"APV{_approvals.Count + 1:D3}";
            var approval = new Approval(idApproval, idBuku, judulBuku, namaPeminjam);
            _approvals.Add(approval);

            return approval;
        }

        /// <summary>
        /// Memproses approval (approve/reject)
        /// </summary>
        /// <param name="approval">Approval yang akan diproses</param>
        /// <param name="newStatus">Status baru</param>
        /// <param name="keterangan">Keterangan</param>
        public void ProcessApproval(Approval approval, string newStatus, string keterangan = "")
        {
            Debug.Assert(approval != null, "Approval tidak boleh null");
            Debug.Assert(newStatus == "Approved" || newStatus == "Rejected", "Status baru harus 'Approved' atau 'Rejected'");
            Debug.Assert(_approvals.Contains(approval), "Approval harus ada dalam daftar approval");
            Debug.Assert(approval.status == "Pending", "Hanya dapat memproses approval dengan status 'Pending'");

            approval.status = newStatus;

            if (!string.IsNullOrEmpty(keterangan))
            {
                approval.keterangan = keterangan;
            }

            // Jika disetujui, perbarui status buku
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

        /// <summary>
        /// Mendapatkan semua peminjaman
        /// </summary>
        /// <returns>Daftar peminjaman</returns>
        public List<Peminjaman> GetAllPeminjaman()
        {
            Debug.Assert(_peminjamans != null, "Daftar peminjaman harus diinisialisasi");
            return _peminjamans.ToList();
        }

        /// <summary>
        /// Mendapatkan semua pengembalian
        /// </summary>
        /// <returns>Daftar pengembalian</returns>
        public List<Pengembalian> GetAllPengembalian()
        {
            Debug.Assert(_pengembalians != null, "Daftar pengembalian harus diinisialisasi");
            return _pengembalians.ToList();
        }

        /// <summary>
        /// Mendapatkan semua approval
        /// </summary>
        /// <returns>Daftar approval</returns>
        public List<Approval> GetAllApprovals()
        {
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");
            return _approvals.ToList();
        }

        /// <summary>
        /// Mendapatkan approval pending
        /// </summary>
        /// <returns>Daftar approval pending</returns>
        public List<Approval> GetPendingApprovals()
        {
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");
            return _approvals.Where(a => a.status == "Pending").ToList();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Menginisialisasi daftar buku dengan data sampel
        /// </summary>
        /// <returns>Daftar buku yang diinisialisasi</returns>
        private List<Buku> InitializeBooks()
        {
            var books = new List<Buku>
            {
                new Buku {idBuku = "B01", judul = "Kancil Cerdik", kategori = "Dongeng", penulis = "Cecylia", tahunTerbit = 2010},
                new Buku {idBuku = "B02", judul = "Artificial Intelligence", kategori = "Teknik Komputer", penulis = "Budiono", tahunTerbit = 2015},
                new Buku {idBuku = "B03", judul = "Bunga Sayu", kategori = "Novel", penulis = "Suci Ratna", tahunTerbit = 2020},
                new Buku {idBuku = "B04", judul = "Sejarah Indonesia", kategori = "Sejarah", penulis = "Sri Handayani", tahunTerbit = 2021},
                new Buku {idBuku = "B05", judul = "Sejarah Umum Indonesia", kategori = "Sejarah", penulis = "Sri Suryani", tahunTerbit = 2011}
            };

            Debug.Assert(books != null && books.Count > 0, "Harus mengembalikan daftar buku yang tidak kosong");
            return books;
        }

        #endregion

        #region Main Menu

        /// <summary>
        /// Menampilkan menu utama aplikasi console
        /// </summary>
        public void DisplayMainMenu()
        {
            Debug.Assert(_configuration != null, "Konfigurasi harus diinisialisasi");
            Debug.Assert(_auth != null, "Sistem autentikasi harus diinisialisasi");
            Debug.Assert(_books != null && _books.Count > 0, "Daftar buku harus diinisialisasi dan tidak kosong");

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
                    // Item menu default jika konfigurasi tidak tersedia
                    Console.WriteLine("1. Login");
                    Console.WriteLine("2. Register Mahasiswa");
                    Console.WriteLine("3. Pengaturan");
                    Console.WriteLine("4. Keluar");
                }

                Console.Write("Pilih opsi: ");
                string pilihan = Console.ReadLine();

                // Periksa apakah pilihan ada di menuItems
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
                    // Fallback ke menu standar jika menu berbasis konfigurasi gagal
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

        /// <summary>
        /// Menangani proses login
        /// </summary>
        private void HandleLogin()
        {
            Debug.Assert(_auth != null, "Sistem autentikasi harus diinisialisasi");

            Console.WriteLine("\n=== LOGIN ===");
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            Debug.Assert(email != null, "Email tidak boleh null");
            Debug.Assert(password != null, "Password tidak boleh null");

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

        /// <summary>
        /// Menangani menu admin untuk aplikasi console
        /// </summary>
        /// <param name="admin">Pengguna admin</param>
        private void HandleAdminMenu(Admin admin)
        {
            Debug.Assert(admin != null, "Pengguna admin tidak boleh null");
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");
            Debug.Assert(_peminjamans != null, "Daftar peminjaman harus diinisialisasi");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

            AdminView adminView = new AdminView();
            adminView.DisplayMenu(_books, _peminjamans, _approvals);

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

        /// <summary>
        /// Menangani menu mahasiswa untuk aplikasi console
        /// </summary>
        /// <param name="mahasiswa">Pengguna mahasiswa</param>
        private void HandleMahasiswaMenu(Mahasiswa mahasiswa)
        {
            Debug.Assert(mahasiswa != null, "Pengguna mahasiswa tidak boleh null");
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");
            Debug.Assert(_peminjamans != null, "Daftar peminjaman harus diinisialisasi");
            Debug.Assert(_pengembalians != null, "Daftar pengembalian harus diinisialisasi");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

            MahasiswaView mhsMenu = new MahasiswaView();
            mhsMenu.DisplayMenu(_books, _peminjamans, _pengembalians, _approvals);

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

        /// <summary>
        /// Menampilkan status approval untuk pengguna tertentu
        /// </summary>
        /// <param name="namaPeminjam">Nama peminjam</param>
        private void ShowUserApprovals(string namaPeminjam)
        {
            Debug.Assert(!string.IsNullOrEmpty(namaPeminjam), "Nama peminjam tidak boleh null atau kosong");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

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

        /// <summary>
        /// Menangani permintaan approval peminjaman buku baru
        /// </summary>
        /// <param name="namaPeminjam">Nama peminjam</param>
        private void RequestNewApproval(string namaPeminjam)
        {
            Debug.Assert(!string.IsNullOrEmpty(namaPeminjam), "Nama peminjam tidak boleh null atau kosong");
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

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

                // Periksa apakah sudah memiliki approval pending untuk buku ini
                bool alreadyRequested = _approvals.Any(a =>
                    a.idBuku == selectedBook.idBuku &&
                    a.namaPeminjam == namaPeminjam &&
                    a.status == "Pending");

                if (alreadyRequested)
                {
                    Console.WriteLine("Anda sudah mengajukan peminjaman untuk buku ini. Harap tunggu proses approval.");
                    return;
                }

                // Buat approval baru
                var newApproval = CreateApproval(selectedBook.idBuku, selectedBook.judul, namaPeminjam);

                // Buat peminjaman dengan status pending
                var peminjaman = CreatePeminjaman(selectedBook, namaPeminjam);

                Console.WriteLine($"Pengajuan peminjaman buku '{selectedBook.judul}' berhasil dibuat!");
                Console.WriteLine($"ID Approval: {newApproval.idApproval}");
                Console.WriteLine("Silakan tunggu approval dari admin.");

                Debug.Assert(_approvals.Contains(newApproval), "Approval baru harus ditambahkan ke daftar approval");
                Debug.Assert(newApproval.status == "Pending", "Approval baru harus memiliki status 'Pending'");
                Debug.Assert(newApproval.idBuku == selectedBook.idBuku, "Approval harus mereferensikan buku yang benar");
                Debug.Assert(newApproval.namaPeminjam == namaPeminjam, "Approval harus memiliki nama peminjam yang benar");
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }
        }

        #endregion
    }

    /// <summary>
    /// Kelas untuk merepresentasikan item menu dalam konfigurasi
    /// </summary>
    public class MenuItem
    {
        private string _id = string.Empty;
        private string _name = string.Empty;

        /// <summary>
        /// Mendapatkan atau menetapkan ID item menu
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                Debug.Assert(!string.IsNullOrEmpty(value), "ID item menu tidak boleh null atau kosong");
                _id = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Mendapatkan atau menetapkan nama item menu
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                Debug.Assert(!string.IsNullOrEmpty(value), "Nama item menu tidak boleh null atau kosong");
                _name = value ?? string.Empty;
            }
        }
    }
}