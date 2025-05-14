using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BukuKita.Model
{
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
}
