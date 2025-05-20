using static BookLibrary.BookLib;

namespace UnitTest
{
    [TestClass]
    public sealed class NashbillaNurfazza_Test
    {
        [TestMethod]
        public void Test_KetersediaanBuku()
        {
            var buku = new Buku
            {
                idBuku = "B01",
                judul = "Buku Aljabar",
                penulis = "Sri Suryati",
                kategori = "Matematika",
                tahunTerbit = 2021,
                Borrower = null
            };

            bool hasil = buku.IsAvailable;
            Assert.IsTrue(hasil);
        }

        [TestMethod]
        public void Test_FilterBuku()
        {
            var buku1 = new Buku { idBuku = "B01", judul = "C# Dasar", penulis = "Budi Hartono", kategori = "Pemrograman" };
            var buku2 = new Buku { idBuku = "B02", judul = "Fiksi Modern", penulis = "Mita", kategori = "Fiksi" };
            var daftar = new List<Buku> { buku1, buku2 };

            var hasil = Buku.FilterBuku(daftar, "Pemrograman");

            Assert.AreEqual(1, hasil.Count);
            Assert.AreEqual("B01", hasil[0].idBuku);
        }
    }
}
