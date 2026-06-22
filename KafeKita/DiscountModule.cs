using System;
using System.Collections.Generic;
using System.Linq;

namespace TugasBesarKPL_Solution
{
    public class CustomPromo
    {
        public DateTime Tanggal { get; set; }
        public string NamaPromo { get; set; } = "";
        public decimal Multiplier { get; set; }
        public string Deskripsi { get; set; } = "";
    }

    public class DiscountModule
    {
        private readonly Dictionary<DayOfWeek, (string namaPromo, decimal multiplier)> _promoTable =
            new Dictionary<DayOfWeek, (string, decimal)>
            {
                { DayOfWeek.Friday,   ("Jumat Berkah - Diskon 20%", 0.8m) },
                { DayOfWeek.Saturday, ("Akhir Pekan - Diskon 10%", 0.9m) },
                { DayOfWeek.Sunday,   ("Akhir Pekan - Diskon 10%", 0.9m) }
            };

        private List<CustomPromo> _customPromos = new List<CustomPromo>();

        public (string namaPromo, decimal multiplier) GetActivePromo(DayOfWeek hariIni)
        {
            if (_promoTable.TryGetValue(hariIni, out var promo)) return promo;
            return ("Tidak ada promo aktif", 1.0m);
        }

        public (string namaPromo, decimal multiplier) GetActivePromo(DateTime tanggalIni)
        {
            var custom = _customPromos.FirstOrDefault(p => p.Tanggal.Date == tanggalIni.Date);
            if (custom != null) return (custom.NamaPromo, custom.Multiplier);
            return GetActivePromo(tanggalIni.DayOfWeek);
        }

        public decimal HitungTotal(decimal nominal, decimal multiplier)
        {
            return Math.Round(nominal * multiplier);
        }

        public void TambahPromoCustom(DateTime tgl, string nama, decimal multiplier, string deskripsi)
        {
            _customPromos.Add(new CustomPromo { Tanggal = tgl.Date, NamaPromo = nama, Multiplier = multiplier, Deskripsi = deskripsi });
        }

        public List<CustomPromo> GetCustomPromos() => _customPromos;
    }
}