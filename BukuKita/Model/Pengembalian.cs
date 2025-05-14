using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;

namespace BukuKita.Model
{
    public class Pengembalian
    {
        public string NamaPeminjam { get; set; }
        public string IdBuku { get; set; }
        public DateTime TanggalPengembalian { get; set; }

        public Pengembalian(string namaPeminjam, string idBuku)
        {
            NamaPeminjam = namaPeminjam;
            IdBuku = idBuku;
            TanggalPengembalian = DateTime.Now;
        }

        public string Info()
        {
            return $"Buku dengan ID {IdBuku} telah dikembalikan oleh {NamaPeminjam} pada {TanggalPengembalian}.";
        }
    }
}
