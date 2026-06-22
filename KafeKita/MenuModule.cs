using System.Collections.Generic;

namespace TugasBesarKPL_Solution
{
    public class MenuItem
    {
        public string Kode { get; set; }
        public string Nama { get; set; }
        public int Harga { get; set; }
    }

    public class MenuModule
    {
        private Dictionary<string, List<MenuItem>> databaseMenu = new Dictionary<string, List<MenuItem>>();

        public MenuModule()
        {
            databaseMenu["MKN"] = new List<MenuItem>
            {
                new MenuItem { Kode = "MKN-01", Nama = "Nasi Goreng", Harga = 18000 },
                new MenuItem { Kode = "MKN-02", Nama = "Mie Tek-Tek", Harga = 15000 },
                new MenuItem { Kode = "MKN-03", Nama = "Mie Dog-Dog", Harga = 15000 }
            };

            databaseMenu["MNM"] = new List<MenuItem>
            {
                new MenuItem { Kode = "MNM-01", Nama = "Es Teh Manis", Harga = 5000 },
                new MenuItem { Kode = "MNM-02", Nama = "Kopi Susu", Harga = 12000 },
                new MenuItem { Kode = "MNM-03", Nama = "Matcha Latte", Harga = 15000 }
            };
        }

        public List<MenuItem> AmbilMenu(string kategori)
        {
            if (databaseMenu.ContainsKey(kategori))
            {
                return databaseMenu[kategori];
            }

            return new List<MenuItem>();
        }

        public void TambahMenu(string kategori, string namaBaru, int hargaBaru)
        {
            if (!databaseMenu.ContainsKey(kategori))
            {
                databaseMenu[kategori] = new List<MenuItem>();
            }

            var daftarMenu = databaseMenu[kategori];

            int maxId = 0;

            foreach (var item in daftarMenu)
            {
                string[] parts = item.Kode.Split('-');

                if (parts.Length == 2 && int.TryParse(parts[1], out int id))
                {
                    if (id > maxId)
                    {
                        maxId = id;
                    }
                }
            }

            string kodeBaru = $"{kategori}-{(maxId + 1):D2}";

            daftarMenu.Add(new MenuItem
            {
                Kode = kodeBaru,
                Nama = namaBaru,
                Harga = hargaBaru
            });
        }
    }
}