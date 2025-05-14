using System;

namespace UserLibrary
{
    public static class UserLib
    {
        public abstract class User
        {
            public enum EnumJenisKelamin { PRIA, WANITA};

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

        public class Mahasiswa : User
        {
            private string NIM { get; set; }

            //Default konstruktor
            public Mahasiswa() { }

            //Konstruktor
            public Mahasiswa(string nama, string NIM, EnumJenisKelamin jnsKelamin, string email, string password, string role) : base(nama, jnsKelamin, email, password, role)
            {
                this.NIM = NIM;
            }

        }

        public class Admin : User
        {
            private string ID { get; set; }

            //Default konstruktor
            public Admin() { }

            //Konstruktor
            public Admin(string nama, String ID, EnumJenisKelamin jnsKelamin, string email, string password, string role) : base(nama, jnsKelamin, email, password, role)
            {
                this.ID = ID;
            }
        }
    }
}
