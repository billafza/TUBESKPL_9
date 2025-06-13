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
    /// Kelas MainMenu yang menangani fungsionalitas utama aplikasi dan konfigurasi
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

            // Postkondisi: Nilai pengembalian harus dari tipe T atau default
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

            // Postkondisi: Tidak ada nilai return karena ini adalah metode UI
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

                            // Postkondisi untuk kasus ini: Item menu harus diperbarui
                            Debug.Assert(menuItems[editIndex - 1].Id == (!string.IsNullOrWhiteSpace(newId) ? newId : menuItems[editIndex - 1].Id),
                                "ID item menu harus diperbarui jika nilai baru disediakan");
                            Debug.Assert(menuItems[editIndex - 1].Name == (!string.IsNullOrWhiteSpace(newName) ? newName : menuItems[editIndex - 1].Name),
                                "Nama item menu harus diperbarui jika nilai baru disediakan");
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

                                // Postkondisi untuk kasus ini: Item menu harus dihapus
                                Debug.Assert(!menuItems.Any(m => m.Id == deletedItem.Id && m.Name == deletedItem.Name),
                                    "Item menu yang dihapus tidak boleh ada lagi dalam daftar menu items");
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

            // Postkondisi: Tidak ada nilai return karena ini adalah metode UI
        }

        #endregion

        #region API Methods - Enhanced for Approval API

        /// <summary>
        /// Mendapatkan semua buku dalam sistem
        /// </summary>
        /// <returns>Daftar semua buku</returns>
        public List<Buku> GetAllBooks()
        {
            // Prekondisi: Daftar buku harus diinisialisasi
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");

            // Postkondisi: Harus mengembalikan daftar yang tidak null
            Debug.Assert(_books != null, "GetAllBooks harus mengembalikan daftar yang tidak null");
            return _books.ToList(); // Return copy untuk safety
        }

        /// <summary>
        /// Mencari buku yang cocok dengan kata kunci yang ditentukan
        /// </summary>
        /// <param name="keyword">Kata kunci pencarian yang tidak boleh null</param>
        /// <returns>Daftar buku yang cocok dengan kriteria pencarian</returns>
        public List<Buku> SearchBooks(string keyword)
        {
            // Prekondisi: Daftar buku harus diinisialisasi dan kata kunci tidak boleh null
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");
            Debug.Assert(keyword != null, "Kata kunci pencarian tidak boleh null");

            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Buku>();

            var result = _books.Where(b =>
                b.judul.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.penulis.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.kategori.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            // Postkondisi: Harus mengembalikan daftar yang tidak null
            Debug.Assert(result != null, "SearchBooks harus mengembalikan daftar yang tidak null");
            return result;
        }

        /// <summary>
        /// Mendapatkan buku berdasarkan ID-nya
        /// </summary>
        /// <param name="id">ID buku yang tidak boleh null atau kosong</param>
        /// <returns>Buku dengan ID yang ditentukan atau null jika tidak ditemukan</returns>
        public Buku GetBookById(string id)
        {
            // Prekondisi: Daftar buku harus diinisialisasi dan id tidak boleh null atau kosong
            Debug.Assert(_books != null, "Daftar buku harus diinisialisasi");
            Debug.Assert(!string.IsNullOrEmpty(id), "ID buku tidak boleh null atau kosong");

            var book = _books.FirstOrDefault(b => b.idBuku == id);

            // Postkondisi: Tidak ada jaminan pada nilai return karena bergantung pada keberadaan buku
            return book;
        }

        /// <summary>
        /// Mendapatkan semua catatan peminjaman
        /// </summary>
        /// <returns>Daftar semua catatan peminjaman</returns>
        public List<Peminjaman> GetAllPeminjaman()
        {
            // Prekondisi: Daftar peminjaman harus diinisialisasi
            Debug.Assert(_peminjamans != null, "Daftar peminjaman harus diinisialisasi");

            // Postkondisi: Harus mengembalikan daftar yang tidak null
            Debug.Assert(_peminjamans != null, "GetAllPeminjaman harus mengembalikan daftar yang tidak null");
            return _peminjamans.ToList();
        }

        /// <summary>
        /// Mendapatkan semua catatan pengembalian
        /// </summary>
        /// <returns>Daftar semua catatan pengembalian</returns>
        public List<Pengembalian> GetAllPengembalian()
        {
            // Prekondisi: Daftar pengembalian harus diinisialisasi
            Debug.Assert(_pengembalians != null, "Daftar pengembalian harus diinisialisasi");

            // Postkondisi: Harus mengembalikan daftar yang tidak null
            Debug.Assert(_pengembalians != null, "GetAllPengembalian harus mengembalikan daftar yang tidak null");
            return _pengembalians.ToList();
        }

        /// <summary>
        /// Membuat catatan peminjaman baru
        /// </summary>
        /// <param name="buku">Buku yang akan dipinjam, tidak boleh null</param>
        /// <param name="namaPeminjam">Nama peminjam, tidak boleh null atau kosong</param>
        /// <returns>Catatan peminjaman yang dibuat</returns>
        public Peminjaman CreatePeminjaman(Buku buku, string namaPeminjam)
        {
            // Prekondisi
            Debug.Assert(buku != null, "Buku tidak boleh null");
            Debug.Assert(!string.IsNullOrEmpty(namaPeminjam), "Nama peminjam tidak boleh null atau kosong");
            Debug.Assert(_peminjamans != null, "Daftar peminjaman harus diinisialisasi");

            var peminjaman = new Peminjaman(buku, namaPeminjam);
            _peminjamans.Add(peminjaman);

            // Postkondisi
            Debug.Assert(_peminjamans.Contains(peminjaman), "Peminjaman yang dibuat harus ditambahkan ke daftar");
            Debug.Assert(peminjaman.BukuDipinjam.idBuku == buku.idBuku, "Peminjaman harus mereferensikan buku yang benar");
            Debug.Assert(peminjaman.NamaPeminjam == namaPeminjam, "Peminjaman harus memiliki nama peminjam yang benar");

            return peminjaman;
        }

        /// <summary>
        /// Membuat catatan approval baru (Method asli untuk kompatibilitas)
        /// </summary>
        /// <param name="idBuku">ID buku, tidak boleh null atau kosong</param>
        /// <param name="judulBuku">Judul buku, tidak boleh null atau kosong</param>
        /// <param name="namaPeminjam">Nama peminjam, tidak boleh null atau kosong</param>
        /// <returns>Catatan approval yang dibuat</returns>
        public Approval CreateApproval(string idBuku, string judulBuku, string namaPeminjam)
        {
            // Prekondisi
            Debug.Assert(!string.IsNullOrEmpty(idBuku), "ID buku tidak boleh null atau kosong");
            Debug.Assert(!string.IsNullOrEmpty(judulBuku), "Judul buku tidak boleh null atau kosong");
            Debug.Assert(!string.IsNullOrEmpty(namaPeminjam), "Nama peminjam tidak boleh null atau kosong");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

            string idApproval = $"APV{_approvals.Count + 1:D3}";
            var approval = new Approval(idApproval, idBuku, judulBuku, namaPeminjam);
            _approvals.Add(approval);

            // Postkondisi
            Debug.Assert(_approvals.Contains(approval), "Approval yang dibuat harus ditambahkan ke daftar");
            Debug.Assert(approval.idBuku == idBuku, "Approval harus mereferensikan ID buku yang benar");
            Debug.Assert(approval.judulBuku == judulBuku, "Approval harus memiliki judul buku yang benar");
            Debug.Assert(approval.namaPeminjam == namaPeminjam, "Approval harus memiliki nama peminjam yang benar");
            Debug.Assert(approval.status == "Pending", "Approval baru harus memiliki status 'Pending'");

            return approval;
        }

        /// <summary>
        /// Membuat catatan approval baru dengan validasi enhanced untuk API
        /// </summary>
        /// <param name="idBuku">ID buku, tidak boleh null atau kosong</param>
        /// <param name="judulBuku">Judul buku, tidak boleh null atau kosong</param>
        /// <param name="namaPeminjam">Nama peminjam, tidak boleh null atau kosong</param>
        /// <returns>Result dengan approval yang dibuat atau error message</returns>
        public ApiResult<Approval> CreateApprovalWithValidation(string idBuku, string judulBuku, string namaPeminjam)
        {
            Debug.Assert(!string.IsNullOrEmpty(idBuku), "ID buku tidak boleh null atau kosong");
            Debug.Assert(!string.IsNullOrEmpty(judulBuku), "Judul buku tidak boleh null atau kosong");
            Debug.Assert(!string.IsNullOrEmpty(namaPeminjam), "Nama peminjam tidak boleh null atau kosong");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

            // Check if user already has pending approval for this book
            bool hasPendingApproval = _approvals.Any(a =>
                a.idBuku == idBuku &&
                a.namaPeminjam.Equals(namaPeminjam, StringComparison.OrdinalIgnoreCase) &&
                a.status == "Pending");

            if (hasPendingApproval)
            {
                return ApiResult<Approval>.Error("Anda sudah memiliki approval pending untuk buku ini");
            }

            // Check if book exists and is available
            var buku = GetBookById(idBuku);
            if (buku == null)
            {
                return ApiResult<Approval>.Error("Buku tidak ditemukan");
            }

            if (!string.IsNullOrEmpty(buku.Borrower))
            {
                return ApiResult<Approval>.Error("Buku sedang dipinjam oleh orang lain");
            }

            // Check maximum approvals per user
            int maxPeminjaman = GetConfigValue<int>("MaxPeminjamanPerUser", 3);
            var userPendingApprovals = _approvals.Count(a =>
                a.namaPeminjam.Equals(namaPeminjam, StringComparison.OrdinalIgnoreCase) &&
                a.status == "Pending");

            if (userPendingApprovals >= maxPeminjaman)
            {
                return ApiResult<Approval>.Error($"Maksimum {maxPeminjaman} approval pending per user sudah tercapai");
            }

            string idApproval = $"APV{_approvals.Count + 1:D3}";
            var approval = new Approval(idApproval, idBuku, judulBuku, namaPeminjam);
            _approvals.Add(approval);

            Debug.Assert(_approvals.Contains(approval), "Approval yang dibuat harus ditambahkan ke daftar");

            return ApiResult<Approval>.Success(approval, "Approval berhasil dibuat");
        }

        /// <summary>
        /// Mendapatkan semua catatan approval
        /// </summary>
        /// <returns>Daftar semua catatan approval</returns>
        public List<Approval> GetAllApprovals()
        {
            // Prekondisi: Daftar approval harus diinisialisasi
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

            // Postkondisi: Harus mengembalikan daftar yang tidak null
            Debug.Assert(_approvals != null, "GetAllApprovals harus mengembalikan daftar yang tidak null");
            return _approvals.ToList();
        }

        /// <summary>
        /// Mendapatkan approval berdasarkan ID
        /// </summary>
        /// <param name="idApproval">ID approval</param>
        /// <returns>Approval atau null jika tidak ditemukan</returns>
        public Approval GetApprovalById(string idApproval)
        {
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");
            Debug.Assert(!string.IsNullOrEmpty(idApproval), "ID approval tidak boleh null atau kosong");

            return _approvals.FirstOrDefault(a => a.idApproval == idApproval);
        }

        /// <summary>
        /// Mendapatkan approval berdasarkan nama peminjam
        /// </summary>
        /// <param name="namaPeminjam">Nama peminjam</param>
        /// <returns>Daftar approval dari peminjam tersebut</returns>
        public List<Approval> GetApprovalsByUser(string namaPeminjam)
        {
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");
            Debug.Assert(!string.IsNullOrEmpty(namaPeminjam), "Nama peminjam tidak boleh null atau kosong");

            return _approvals.Where(a => a.namaPeminjam.Equals(namaPeminjam, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Mendapatkan semua catatan approval yang statusnya pending
        /// </summary>
        /// <returns>Daftar catatan approval pending</returns>
        public List<Approval> GetPendingApprovals()
        {
            // Prekondisi: Daftar approval harus diinisialisasi
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

            var pendingApprovals = _approvals.Where(a => a.status == "Pending").ToList();

            // Postkondisi: Harus mengembalikan daftar yang tidak null
            Debug.Assert(pendingApprovals != null, "GetPendingApprovals harus mengembalikan daftar yang tidak null");
            Debug.Assert(pendingApprovals.All(a => a.status == "Pending"), "Semua approval yang dikembalikan harus memiliki status 'Pending'");

            return pendingApprovals;
        }

        /// <summary>
        /// Memproses permintaan approval dengan memperbarui statusnya (Method asli untuk kompatibilitas)
        /// </summary>
        /// <param name="approval">Approval yang akan diproses, tidak boleh null</param>
        /// <param name="newStatus">Status baru yang akan ditetapkan, harus 'Approved' atau 'Rejected'</param>
        /// <param name="keterangan">Komentar/catatan opsional</param>
        public void ProcessApproval(Approval approval, string newStatus, string keterangan = "")
        {
            // Prekondisi
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
                    // Invariant: Buku harus tersedia untuk dipinjam
                    Debug.Assert(string.IsNullOrEmpty(buku.Borrower), "Buku harus tersedia untuk dipinjam");

                    buku.Borrower = approval.namaPeminjam;
                    buku.BorrowedAt = DateTime.Now;
                }
            }

            // Postkondisi
            Debug.Assert(approval.status == newStatus, "Status approval harus diperbarui ke status baru");
            if (!string.IsNullOrEmpty(keterangan))
            {
                Debug.Assert(approval.keterangan == keterangan, "Komentar approval harus diperbarui jika disediakan");
            }

            if (newStatus == "Approved")
            {
                var buku = GetBookById(approval.idBuku);
                if (buku != null)
                {
                    Debug.Assert(buku.Borrower == approval.namaPeminjam, "Peminjam buku harus disetel ke pemohon approval");
                }
            }
        }

        /// <summary>
        /// Memproses permintaan approval dengan memperbarui statusnya (Enhanced untuk API)
        /// </summary>
        /// <param name="idApproval">ID approval yang akan diproses</param>
        /// <param name="newStatus">Status baru yang akan ditetapkan, harus 'Approved' atau 'Rejected'</param>
        /// <param name="keterangan">Komentar/catatan opsional</param>
        /// <returns>Result dari pemrosesan approval</returns>
        public ApiResult<Approval> ProcessApprovalWithValidation(string idApproval, string newStatus, string keterangan = "")
        {
            Debug.Assert(!string.IsNullOrEmpty(idApproval), "ID approval tidak boleh null atau kosong");
            Debug.Assert(newStatus == "Approved" || newStatus == "Rejected", "Status baru harus 'Approved' atau 'Rejected'");

            var approval = GetApprovalById(idApproval);
            if (approval == null)
            {
                return ApiResult<Approval>.Error("Approval tidak ditemukan");
            }

            if (approval.status != "Pending")
            {
                return ApiResult<Approval>.Error("Hanya dapat memproses approval dengan status 'Pending'");
            }

            approval.status = newStatus;
            if (!string.IsNullOrEmpty(keterangan))
            {
                approval.keterangan = keterangan;
            }

            // Jika disetujui, perbarui status buku dan buat peminjaman
            if (newStatus == "Approved")
            {
                var buku = GetBookById(approval.idBuku);
                if (buku != null)
                {
                    if (!string.IsNullOrEmpty(buku.Borrower))
                    {
                        // Rollback approval status
                        approval.status = "Pending";
                        approval.keterangan = "";
                        return ApiResult<Approval>.Error("Buku sudah dipinjam oleh orang lain");
                    }

                    // Update book status
                    buku.Borrower = approval.namaPeminjam;
                    buku.BorrowedAt = DateTime.Now;

                    // Create peminjaman record
                    var peminjaman = CreatePeminjaman(buku, approval.namaPeminjam);
                }
            }

            Debug.Assert(approval.status == newStatus, "Status approval harus diperbarui ke status baru");

            string message = newStatus == "Approved" ? "Approval berhasil disetujui" : "Approval berhasil ditolak";
            return ApiResult<Approval>.Success(approval, message);
        }

        /// <summary>
        /// Menghapus approval (hanya yang masih pending)
        /// </summary>
        /// <param name="idApproval">ID approval yang akan dihapus</param>
        /// <returns>Result dari penghapusan approval</returns>
        public ApiResult<bool> DeleteApproval(string idApproval)
        {
            Debug.Assert(!string.IsNullOrEmpty(idApproval), "ID approval tidak boleh null atau kosong");
            Debug.Assert(_approvals != null, "Daftar approval harus diinisialisasi");

            var approval = GetApprovalById(idApproval);
            if (approval == null)
            {
                return ApiResult<bool>.Error("Approval tidak ditemukan");
            }

            if (approval.status != "Pending")
            {
                return ApiResult<bool>.Error("Hanya dapat menghapus approval dengan status 'Pending'");
            }

            bool removed = _approvals.Remove(approval);
            if (removed)
            {
                return ApiResult<bool>.Success(true, "Approval berhasil dihapus");
            }
            else
            {
                return ApiResult<bool>.Error("Gagal menghapus approval");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Menginisialisasi daftar buku dengan data sampel
        /// </summary>
        /// <returns>Daftar buku yang diinisialisasi</returns>
        private List<Buku> InitializeBooks()
        {
            // Prekondisi: Tidak ada untuk metode ini
                
            var books = new List<Buku>
            {
                new Buku {idBuku = "B01", judul = "Kancil Cerdik", kategori = "Dongeng", penulis = "Cecylia", tahunTerbit = 2010},
                new Buku {idBuku = "B02", judul = "Artificial Intelligence", kategori = "Teknik Komputer", penulis = "Budiono", tahunTerbit = 2015},
                new Buku {idBuku = "B03", judul = "Bunga Sayu", kategori = "Novel", penulis = "Suci Ratna", tahunTerbit = 2020},
                new Buku {idBuku = "B04", judul = "Sejarah Indonesia", kategori = "Sejarah", penulis = "Sri Handayani", tahunTerbit = 2021},
                new Buku {idBuku = "B05", judul = "Sejarah Umum Indonesia", kategori = "Sejarah", penulis = "Sri Suryani", tahunTerbit = 2011}
            };

            // Postkondisi
            Debug.Assert(books != null && books.Count > 0, "Harus mengembalikan daftar buku yang tidak kosong");
            Debug.Assert(books.All(b => !string.IsNullOrEmpty(b.idBuku)), "Semua buku harus memiliki ID yang tidak kosong");
            Debug.Assert(books.All(b => !string.IsNullOrEmpty(b.judul)), "Semua buku harus memiliki judul yang tidak kosong");
            Debug.Assert(books.All(b => !string.IsNullOrEmpty(b.kategori)), "Semua buku harus memiliki kategori yang tidak kosong");
            Debug.Assert(books.All(b => !string.IsNullOrEmpty(b.penulis)), "Semua buku harus memiliki nama penulis yang tidak kosong");
            Debug.Assert(books.All(b => b.tahunTerbit > 0), "Semua buku harus memiliki tahun terbit yang valid");

            return books;
        }

        #endregion

        #region Main Menu

        /// <summary>
        /// Menampilkan menu utama dan menangani interaksi pengguna
        /// </summary>
        public void DisplayMainMenu()
        {
            // Prekondisi: Konfigurasi, auth, dan daftar buku harus diinisialisasi
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

            // Postkondisi: Tidak ada nilai return karena ini adalah metode UI
        }

        /// <summary>
        /// Menangani proses login
        /// </summary>
        private void HandleLogin()
        {
            // Prekondisi: Sistem autentikasi harus diinisialisasi
            Debug.Assert(_auth != null, "Sistem autentikasi harus diinisialisasi");

            Console.WriteLine("\n=== LOGIN ===");
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            // Invariant: Email dan password tidak boleh null
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

            // Postkondisi: Tidak ada nilai return karena ini adalah metode UI
        }

        /// <summary>
        /// Menangani menu admin dan fungsionalitas khusus admin
        /// </summary>
        /// <param name="admin">Pengguna admin, tidak boleh null</param>
        private void HandleAdminMenu(Admin admin)
        {
            // Prekondisi: Admin tidak boleh null
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

            // Postkondisi: Tidak ada nilai return karena ini adalah metode UI
        }

        /// <summary>
        /// Menangani menu mahasiswa dan fungsionalitas khusus mahasiswa
        /// </summary>
        /// <param name="mahasiswa">Pengguna mahasiswa, tidak boleh null</param>
        private void HandleMahasiswaMenu(Mahasiswa mahasiswa)
        {
            // Prekondisi: Mahasiswa tidak boleh null
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

            // Postkondisi: Tidak ada nilai return karena ini adalah metode UI
        }

        /// <summary>
        /// Menampilkan status approval untuk pengguna tertentu
        /// </summary>
        /// <param name="namaPeminjam">Nama peminjam, tidak boleh null atau kosong</param>
        private void ShowUserApprovals(string namaPeminjam)
        {
            // Prekondisi: namaPeminjam tidak boleh null atau kosong dan daftar approval harus diinisialisasi
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

            // Postkondisi: Tidak ada nilai return karena ini adalah metode UI
        }

        /// <summary>
        /// Menangani permintaan approval peminjaman buku baru
        /// </summary>
        /// <param name="namaPeminjam">Nama peminjam, tidak boleh null atau kosong</param>
        private void RequestNewApproval(string namaPeminjam)
        {
            // Prekondisi: namaPeminjam tidak boleh null atau kosong dan daftar buku harus diinisialisasi
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

                // Postkondisi
                Debug.Assert(_approvals.Contains(newApproval), "Approval baru harus ditambahkan ke daftar approval");
                Debug.Assert(newApproval.status == "Pending", "Approval baru harus memiliki status 'Pending'");
                Debug.Assert(newApproval.idBuku == selectedBook.idBuku, "Approval harus mereferensikan buku yang benar");
                Debug.Assert(newApproval.namaPeminjam == namaPeminjam, "Approval harus memiliki nama peminjam yang benar");
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }

            // Tidak ada nilai return karena ini adalah metode UI
        }

        #endregion
    }

    /// <summary>
    /// Generic result class untuk API responses
    /// </summary>
    /// <typeparam name="T">Tipe data yang dikembalikan</typeparam>
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; } = default(T);
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public static ApiResult<T> Success(T data, string message = "")
        {
            return new ApiResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResult<T> Error(string errorMessage)
        {
            return new ApiResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

    /// <summary>
    /// Kelas untuk merepresentasikan item menu dalam konfigurasi
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// Invariant kelas:
        /// - Id tidak boleh null atau kosong
        /// - Name tidak boleh null atau kosong
        /// </summary>

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
                // Prekondisi: Id tidak boleh null atau kosong
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
                // Prekondisi: Name tidak boleh null atau kosong
                Debug.Assert(!string.IsNullOrEmpty(value), "Nama item menu tidak boleh null atau kosong");
                _name = value ?? string.Empty;
            }
        }
    }
}
