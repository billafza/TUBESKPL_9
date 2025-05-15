using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BukuKita.Model;
using static BukuKita.Model.User;

namespace BukuKita.Auth
{
    public class AuthSystem
    {
        public List<User> daftarUser;

        public AuthSystem()
        {
            daftarUser = new List<User>
        {
           new Admin("Admin1", "ADM01", EnumJenisKelamin.PRIA, "admin@mail.com", "admin123", "admin"),
           new Mahasiswa("Mhs1", "103022300126", EnumJenisKelamin.WANITA, "mhs1@mail.com", "mhs123", "mahasiswa")
        };
        }

        public User Login(string email, string password)
        {
            return daftarUser.FirstOrDefault(u => u.email == email && u.password == password);
        }

        public void RegisterMahasiswa()
        {
            string currentState = "NAMA";
            Mahasiswa newUser = new Mahasiswa();
            bool abort = false;

            var handlers = new Dictionary<string, Func<string>>()
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
                newUser.password = Console.ReadLine();
                return "SELESAI";
            }
        }
    };

            while (currentState != "SELESAI")
            {
                currentState = handlers[currentState]();
            }

            if (abort)
                return;

            newUser.role = "mahasiswa"; // pastikan role terisi
            daftarUser.Add(newUser);
            Console.WriteLine("\nRegistrasi berhasil! Berikut data Anda:");
            newUser.DisplayUser();
        }


        public List<User> GetUsers() => daftarUser;
    }

}


