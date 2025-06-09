using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BukuKita.Model;
using static BookLibrary.BookLib;

namespace UnitTest
{
    internal class NadyaSekarRahmawati_Test
    {
        [TestMethod]
        public void Peminjaman()
        {
            var buku = new Buku
            {
                idBuku = "B001",
                judul = "Clean Code"
            };
            string namaPeminjam = "Andi";

            var peminjaman = new Peminjaman(buku, namaPeminjam);
            Assert.AreEqual(buku, peminjaman.BukuDipinjam);
            Assert.AreEqual(namaPeminjam, peminjaman.NamaPeminjam);

            var now = DateTime.Now;
            Assert.IsTrue((now - peminjaman.TanggalPinjam).TotalSeconds < 5, "TanggalPeminjaman tidak sesuai");
        }

        [TestMethod]
        public void Peminjaman_TanggalPeminjaman_IsRecent()
        {
            var buku = new Buku { idBuku = "B002", judul = "Refactoring" };
            var peminjaman = new Peminjaman(buku, "Budi");

            var timeDiff = DateTime.Now - peminjaman.TanggalPinjam;
            Assert.IsTrue(timeDiff.TotalSeconds < 5, "TanggalPeminjaman harus waktu saat ini");
        }
    }
}

