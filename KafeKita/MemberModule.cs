using System.Collections.Generic;

namespace TugasBesarKPL_Solution
{
    // 1. Cetakan Data Member
    public class MemberData
    {
        public string ID { get; set; }
        public string Nama { get; set; }
        public string NoTelp { get; set; }
        public string Tier { get; set; }
    }

    public class MemberModule
    {
        // 2. Database Sementara (Dictionary)
        private Dictionary<string, MemberData> dbMember = new Dictionary<string, MemberData>();

        public MemberModule()
        {
            // Data bawaan saat aplikasi dijalankan
            dbMember.Add("M01", new MemberData { ID = "M01", Nama = "Khaidiri Murteza", NoTelp = "08123456789", Tier = "🥉 BRONZE" });
            dbMember.Add("M02", new MemberData { ID = "M02", Nama = "Hilmy Rafi", NoTelp = "08987654321", Tier = "🥇 GOLD" });
        }

        // 3. Fungsi Validasi (Poin dihapus, mengembalikan Nama dan Tier)
        public (string nama, string tier) ValidasiMember(string memberID)
        {
            if (dbMember.ContainsKey(memberID))
            {
                var m = dbMember[memberID];
                return (m.Nama, m.Tier);
            }
            return ("", "Tidak Terdaftar / Expired");
        }

        // 4. Fungsi Registrasi & Auto-Generate ID
        public string RegisterMember(string nama, string telp, string tier)
        {
            int maxId = 0;

            // Mencari ID terbesar di database
            foreach (var key in dbMember.Keys)
            {
                // Memotong awalan "M" (misal: "M02" jadi "02") lalu jadikan angka
                if (key.StartsWith("M") && int.TryParse(key.Substring(1), out int num))
                {
                    if (num > maxId) maxId = num;
                }
            }

            // Generate ID baru (Misal max 2, maka jadi 3 -> "M03")
            string newId = $"M{(maxId + 1):D2}";

            // Simpan ke database
            dbMember.Add(newId, new MemberData { ID = newId, Nama = nama, NoTelp = telp, Tier = tier });

            return newId;
        }
    }
}