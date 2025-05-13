using System;

namespace UserLibrary
{
    public static class UserLibrary
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

            public String getNama()
            {
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

            public void display()
            {
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
        public class Mahasiswa : User
        {
            private String NIM;
            private String jenisKelamin;

            public Mahasiswa() { }
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
        public class Admin : User
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
        }
    }
}
