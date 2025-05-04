using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tubeskpl
{
    class Library
    {
        public static void displayBuku(Buku b)
        {
            Console.WriteLine($"\nID Buku: {b.idBuku} \nJudul: {b.judul} oleh {b.penulis} \nKategori: {b.kategori} \nTahun Terbit: {b.tahunTerbit}");
        }

        

    }
}
