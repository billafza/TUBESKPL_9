using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography; // 🔐 Untuk hashing password

using BukuKita.Model;
using static BukuKita.Model.User;

namespace BukuKita.Auth
{
    public class AuthSystem
    {
        public List<User> daftarUser; // 🧼 Penamaan variabel deskriptif

        public AuthSystem()
        {
            daftarUser = new List<User>
            {
                // 🔐 Password disimpan dalam bentuk hash, bukan plaintext
                new Admin("Admin1", "ADM01", EnumJenisKelamin.PRIA, "admin@mail.com", HashPassword("admin123"), "admin"),
                new Mahasiswa("Mhs1", "103022300126", EnumJenisKelamin.WANITA, "mhs1@mail.com", HashPassword("mhs123"), "mahasiswa")
            };
        }

        /// <summary>
        /// Melakukan login berdasarkan email dan password
        /// </summary>
        public User Login(string email, string password)
        {
            // 🔐 Bandingkan password dengan versi hash-nya, bukan langsung teks
            return daftarUser.FirstOrDefault(u => u.email == email && VerifyPassword(password, u.password));
        }

        /// <summary>
        /// Registrasi Mahasiswa baru menggunakan state machine
        /// </summary>
        public void RegisterMahasiswa()
        {
            string currentState = "NAMA"; // 🧼 Menggunakan state machine sederhana
            Mahasiswa newUser = new Mahasiswa();
            bool abort = false;

            // 🧼 Clean Code: Gunakan dictionary handler agar alur input modular dan rapi
            var handlers = new Dictionary<string, Func<string>>
            {
                { "NAMA", () =>
                    {
                        Console.Write("Masukkan nama: ");
                        newUser.nama = Console.ReadLine();
                        return "NIM";
                    }
                },
                { "NIM", () =>
                    {
                        Console.Write("Masukkan NIM: ");
                        string nimInput = Console.ReadLine();

                        // 🔐 Validasi: Cegah duplikasi NIM
                        if (daftarUser.Any(u => u is Mahasiswa m && m.NIM == nimInput))
                        {
                            Console.WriteLine("NIM sudah terdaftar!");
                            abort = true;
                            return "SELESAI";
                        }
                        newUser.NIM = nimInput;
                        return "JENISKELAMIN";
                    }
                },
                { "JENISKELAMIN", () =>
                    {
                        Console.Write("Masukkan jenis kelamin (1 = PRIA, 2 = WANITA): ");
                        string input = Console.ReadLine();

                        if (input == "1")
                            newUser.jnsKelamin = EnumJenisKelamin.PRIA;
                        else if (input == "2")
                            newUser.jnsKelamin = EnumJenisKelamin.WANITA;
                        else
                        {
                            Console.WriteLine("Input jenis kelamin tidak valid!");
                            abort = true;
                            return "SELESAI";
                        }
                        return "EMAIL";
                    }
                },
                { "EMAIL", () =>
                    {
                        Console.Write("Masukkan email: ");
                        string emailInput = Console.ReadLine();

                        // 🔐 Validasi: Email unik
                        if (daftarUser.Any(u => u.email == emailInput))
                        {
                            Console.WriteLine("Email sudah digunakan!");
                            abort = true;
                            return "SELESAI";
                        }
                        newUser.email = emailInput;
                        return "PASSWORD";
                    }
                },
                { "PASSWORD", () =>
                    {
                        Console.Write("Masukkan password: ");
                        string passwordInput = Console.ReadLine();

                        // 🔐 Simpan password dalam bentuk hash, bukan teks asli
                        newUser.password = HashPassword(passwordInput);
                        return "SELESAI";
                    }
                }
            };

            // 🧼 Clean Code: Loop untuk handler modular
            while (currentState != "SELESAI")
            {
                currentState = handlers[currentState]();
            }

            if (abort)
                return;

            newUser.role = "mahasiswa";
            daftarUser.Add(newUser);

            Console.WriteLine("\nRegistrasi berhasil! Berikut data Anda:");
            newUser.DisplayUser();
        }

        // 🔐 Secure Coding: Fungsi hashing password menggunakan SHA256
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // 🔐 Secure Coding: Verifikasi password dengan membandingkan hash
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            return HashPassword(inputPassword) == storedHash;
        }

        /// <summary>
        /// Mengembalikan daftar seluruh user.
        /// </summary>
        public List<User> GetUsers() => daftarUser; // 🧼 Fungsi ringkas dan jelas
    }
}
