using System;
using System.Collections.Generic;
using System.IO;
using BukuKita.Auth;
using BukuKita.Model;
using BukuKita.View;
using static BookLibrary.BookLib;

namespace BukuKita.Tests
{
    public class StateMachineTests
    {
        // Turunan User agar bisa instantiate
        public class DummyUser : User
        {
            public DummyUser(string nama, EnumJenisKelamin jnsKelamin, string email, string password, string role)
                : base(nama, jnsKelamin, email, password, role)
            {
            }
        }

        private List<User> buatDaftarUser()
        {
            return new List<User>
            {
                new DummyUser("Admin", User.EnumJenisKelamin.PRIA, "admin@buku.com", "admin123", "admin"),
                new DummyUser("Mahasiswa", User.EnumJenisKelamin.WANITA, "mhs@buku.com", "mhs123", "mahasiswa")
            };
        }

        // Dummy View classes supaya tidak error saat test
        public class AdminView
        {
            public void displayMenu(List<Buku> bukuList, List<Peminjaman> daftarPeminjaman, List<Approval> daftarApproval) { }
        }
        public class MahasiswaView
        {
            public void displayMenu(List<Buku> bukuList, List<Peminjaman> daftarPeminjaman, List<Pengembalian> daftarPengembalian, List<Approval> daftarApproval) { }
        }

        public void LoginBerhasilAdmin()
        {
            var stateMachine = new StateMachine();
            var users = buatDaftarUser();

            var input = new StringReader("admin@buku.com\nadmin123\n");
            Console.SetIn(input);

            stateMachine.MulaiLogin(users, new List<Buku>(), new List<Peminjaman>(), new List<Pengembalian>(), new List<Approval>());

            // Test sukses jika tidak throw error
        }

        public void LoginBerhasilMahasiswa()
        {
            var stateMachine = new StateMachine();
            var users = buatDaftarUser();

            var input = new StringReader("mhs@buku.com\nmhs123\n");
            Console.SetIn(input);

            stateMachine.MulaiLogin(users, new List<Buku>(), new List<Peminjaman>(), new List<Pengembalian>(), new List<Approval>());
        }

        public void LoginGagalKarenaPasswordSalah()
        {
            var stateMachine = new StateMachine();
            var users = buatDaftarUser();

            var input = new StringReader("mhs@buku.com\nsalahpass\n");
            Console.SetIn(input);

            stateMachine.MulaiLogin(users, new List<Buku>(), new List<Peminjaman>(), new List<Pengembalian>(), new List<Approval>());
        }
    }
}
