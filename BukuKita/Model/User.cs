using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BukuKita.Model
{
    public abstract class User
    {
        public enum EnumJenisKelamin { PRIA, WANITA };

        public string nama { get; set; }
        private EnumJenisKelamin jnsKelamin { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }

        //Default konstruktor
        public User() { }

        //Konstruktor
        public User(string nama, EnumJenisKelamin jnsKelamin, string email, string password, string role)
        {
            this.nama = nama;
            this.jnsKelamin = jnsKelamin;
            this.email = email;
            this.password = password;
            this.role = role;
        }


        public void DisplayUser()
        {
            Console.WriteLine("\n=== Profil Pengguna ===");
            Console.WriteLine("Nama   : " + nama);
            Console.WriteLine("Email  : " + email);
            Console.WriteLine("Role   : " + role);
        }

        public bool ValidasiPass(String password)
        {
            return this.password.Equals(password);
        }
    }
}
