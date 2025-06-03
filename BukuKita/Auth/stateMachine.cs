using System;
using System.Collections.Generic;
using System.Linq;
using static BookLibrary.BookLib;
using BukuKita.Model;
using BukuKita.View;

namespace BukuKita.Auth
{
    public enum LoginState
    {
        Start,
        InputEmail,
        InputPassword,
        Validasi,
        Berhasil,
        Gagal
    }

    public class StateMachine
    {
        private LoginState currentState;

        public StateMachine()
        {
            currentState = LoginState.Start;
        }

        public void MulaiLogin(List<User> daftarUser, List<Buku> bukuList, List<Peminjaman> daftarPeminjaman, List<Pengembalian> daftarPengembalian, List<Approval> daftarApproval)
        {
            string email = "";
            string password = "";
            User userLogin = null;

            while (currentState != LoginState.Berhasil && currentState != LoginState.Gagal)
            {
                switch (currentState)
                {
                    case LoginState.Start:
                        Console.WriteLine("\n--- LOGIN ---");
                        currentState = LoginState.InputEmail;
                        break;
                    case LoginState.InputEmail:
                        Console.Write("Masukkan Email: ");
                        email = Console.ReadLine();
                        currentState = LoginState.InputPassword;
                        break;
                    case LoginState.InputPassword:
                        Console.Write("Masukkan Password: ");
                        password = Console.ReadLine();
                        currentState = LoginState.Validasi;
                        break;
                    case LoginState.Validasi:
                        userLogin = daftarUser.FirstOrDefault(u => u.email == email && u.ValidasiPass(password));
                        if (userLogin != null)
                        {
                            Console.WriteLine($"Login berhasil sebagai {userLogin.role}!\n");
                            userLogin.DisplayUser();
                            currentState = LoginState.Berhasil;
                            // Di method MulaiLogin
                            if (userLogin.role.ToLower() == "admin")
                            {
                                AdminView adminMenu = new AdminView();
                                adminMenu.displayMenu(bukuList, daftarPeminjaman, daftarApproval);
                            }
                            else if (userLogin.role.ToLower() == "mahasiswa")
                            {
                                MahasiswaView mhsMenu = new MahasiswaView();
                                mhsMenu.displayMenu(bukuList, daftarPeminjaman, daftarPengembalian, daftarApproval);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Login gagal! Email atau password salah.");
                            currentState = LoginState.Gagal;
                        }
                        break;
                }
            }
        }

        // HAPUS method overload yang bermasalah ini:
        // public void MulaiLogin(List<BukuKita.Tests.StateMachineTests.User> users, List<Buku> bukus, List<Peminjaman> peminjamen, List<Pengembalian> pengembalians, List<Approval> approvals)
        // {
        //     throw new NotImplementedException();
        // }
    }
}