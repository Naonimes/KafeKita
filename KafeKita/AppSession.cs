using System;
using System.Collections.Generic;

namespace TugasBesarKPL_Solution
{
    public static class AppSession
    {
        public static string MejaTargetPesanan = "";

        // TAMBAHAN BARU: Untuk menyimpan target meja yang mau dibayar
        public static string MejaTargetBayar = "";

        public static Dictionary<string, List<MenuItem>> KeranjangMeja = new Dictionary<string, List<MenuItem>>();

        public static Action PindahKeMenu;
        public static Action PindahKeOrder;

        // TAMBAHAN BARU: Trigger pindah ke layar Payment
        public static Action PindahKePayment;
    }
}