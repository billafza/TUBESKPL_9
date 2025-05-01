public class Buku
{
    public string idBuku { get; set; }
    public string judul { get; set; }
    public string penulis { get; set; }
    public string kategori { get; set; }
    public int tahunTerbit { get; set; }

    public override string ToString()
    {
        return $"ID Buku: {idBuku} \nJudul: {judul} oleh {penulis} \nKategori: {kategori} \nTahun Terbit: {tahunTerbit}";
    }
}

public class FilterBuku
{
    public string? kategori { get; set; }
    public string? penulis { get; set; }
    public int? tahunTerbit { get; set; }

    public bool Sesuai(Buku b)
    {
        return (kategori == null || b.kategori == kategori)
            && (penulis == null || b.penulis == penulis)
            && (tahunTerbit == null || b.tahunTerbit == tahunTerbit);
    }
}

class program {
    static void Main() {
        var katalog = new List<Buku> {
        new Buku {idBuku = "B01", judul = "Kancil Cerdik", kategori = "Dongeng", penulis = "Cecylia", tahunTerbit = 2010},
        new Buku {idBuku = "B02", judul = "Artificial Intelegence", kategori = "Teknik Komputer", penulis = "Budiono", tahunTerbit = 2015},
        new Buku {idBuku = "B03", judul = "Bunga Sayu", kategori = "Novel", penulis = "Suci Ratna", tahunTerbit = 2020},
        new Buku {idBuku = "B04", judul = "Sejarah Indonesia", kategori = "Sejarah", penulis = "Sri Handayani", tahunTerbit = 2021},
        new Buku {idBuku = "B05", judul = "Sejarah Umum Indonesia", kategori = "Sejarah", penulis = "Sri Suryani", tahunTerbit = 2011}
        };

        Console.WriteLine("--- Katalog Buku ---");
        katalog.ForEach(book => Console.WriteLine(book + "\n"));

        Console.WriteLine("Masukkan kategori buku yang anda cari: ");
        String input = Console.ReadLine();
        var filter = new FilterBuku { kategori = input };
        var hasil = katalog.Where(b => filter.Sesuai(b)).ToList();

        Console.WriteLine("\n--- Kategori Buku " + input + " ---\n");
        hasil.ForEach(k => Console.WriteLine(k + "\n"));
    }
}