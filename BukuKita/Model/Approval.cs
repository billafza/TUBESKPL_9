using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;

namespace BukuKita.Model
{
    public class Approval
    {
        public string idApproval { get; set; }
        public string idBuku { get; set; }
        public string judulBuku { get; set; }
        public string namaPeminjam { get; set; }
        public DateTime tanggalPengajuan { get; set; }
        public string status { get; set; } // "Pending", "Approved", "Rejected"
        public string keterangan { get; set; }

        public Approval()
        {
            tanggalPengajuan = DateTime.Now;
            status = "Pending";
            keterangan = "";
        }

        public Approval(string idApproval, string idBuku, string judulBuku, string namaPeminjam)
        {
            this.idApproval = idApproval;
            this.idBuku = idBuku;
            this.judulBuku = judulBuku;
            this.namaPeminjam = namaPeminjam;
            this.tanggalPengajuan = DateTime.Now;
            this.status = "Pending";
            this.keterangan = "";
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"ID Approval: {idApproval}");
            Console.WriteLine($"ID Buku: {idBuku}");
            Console.WriteLine($"Judul Buku: {judulBuku}");
            Console.WriteLine($"Nama Peminjam: {namaPeminjam}");
            Console.WriteLine($"Tanggal Pengajuan: {tanggalPengajuan.ToString("dd/MM/yyyy HH:mm")}");
            Console.WriteLine($"Status: {status}");
            if (!string.IsNullOrEmpty(keterangan))
            {
                Console.WriteLine($"Keterangan: {keterangan}");
            }
        }
    }
}