using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tubeskpl
{
    public abstract class User
    {
        private String nama;
        private String email;
        private String password;
        private String role;

        public User() { }
        public User(string nama, string email, string password, string role) 
        {
            this.nama = nama;
            this.email = email;
            this.password = password;
            this.role = role;
        }

        public String getNama() {
            return nama;
        }
        public String getEmail()
        {
            return email;
        }
        public String getPassword()
        {
            return password;
        }
        public String getRole()
        {
            return role;
        }
        public void setNama(String nama)
        {
            this.nama = nama;
        }
        public void setEmail(String email)
        {
            this.email = email;
        }
        public void setPassword(String password)
        {
            this.password = password;
        }
        public void setRole(String role)
        {
            this.role = role;
        }

        public void display() {
            Console.WriteLine("\n=== Profil Pengguna ===");
            Console.WriteLine("Nama   : " + nama);
            Console.WriteLine("Email  : " + email);
            Console.WriteLine("Role   : " + role);
        }

        public bool validasiPass(String password)
        {
            return this.password.Equals(password);
        }
    }
    class Mahasiswa : User
    {
        private String NIM;
        private String jenisKelamin;

        public Mahasiswa(string nama, string email, string password, string role, String NIM, String jenisKelamin) : base(nama, email, password, role)
        {
            this.NIM = NIM;
            this.jenisKelamin = jenisKelamin;
        }

        public String getNIM()
        {
            return NIM;
        }
        public String getJenisKelamin()
        {
            return jenisKelamin;
        }
    }
    class Admin : User
    {
        private String ID;
        private String jenisKelamin;

        public Admin() { }
        public Admin(string nama, string email, string password, string role, String ID, String jenisKelamin) : base(nama, email, password, role)
        {
            this.ID = ID;
            this.jenisKelamin = jenisKelamin;
        }

        public String getID()
        {
            return ID;
        }
        public String getJenisKelamin()
        {
            return jenisKelamin;
        }

        public void KelolaBuku(List<Buku> book)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Buku b = new Buku();

                Console.WriteLine("\n=== MENU KELOLA BUKU ===");
                Console.WriteLine("1. Melihat Buku");
                Console.WriteLine("2. Menambah Buku");
                Console.WriteLine("3. Menghapus Buku");
                Console.WriteLine("0. Kembali");
                int pilih = int.Parse(Console.ReadLine());
                Console.WriteLine();
            
                switch (pilih)
                {
                    case 1:
                        b.DaftarBuku(book);
                        break;

                    case 2:
                        b.TambahBuku(book);
                        break;

                    case 3:
                        b.HapusBuku(book);
                        break;

                    case 0:
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nPilih menu yang sesuai.");
                        break;
                }
            }
        }
    }
}
