using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TugasBesarKPL_Solution;

namespace TugasBesarKPL_Tests
{
    [TestClass]
    public class DiscountModuleTests
    {
        private DiscountModule _discountModule;

        [TestInitialize]
        public void Setup()
        {
            // Diinisialisasi setiap kali sebelum test case berjalan
            _discountModule = new DiscountModule();
        }

        [TestMethod]
        public void Test_GetActivePromo_HariJumat_HarusDiskon20Persen()
        {
            // Arrange
            DayOfWeek hariUji = DayOfWeek.Friday;

            // Act
            var hasil = _discountModule.GetActivePromo(hariUji);

            // Assert
            Assert.AreEqual("Jumat Berkah - Diskon 20%", hasil.namaPromo);
            Assert.AreEqual(0.8m, hasil.multiplier);
        }

        [TestMethod]
        public void Test_GetActivePromo_HariBiasa_HarusTidakAdaPromo()
        {
            // Arrange
            DayOfWeek hariUji = DayOfWeek.Monday; // Senin tidak terdaftar di tabel promo

            // Act
            var hasil = _discountModule.GetActivePromo(hariUji);

            // Assert
            Assert.AreEqual("Tidak ada promo aktif", hasil.namaPromo);
            Assert.AreEqual(1.0m, hasil.multiplier);
        }

        [TestMethod]
        public void Test_TambahPromoCustom_Kalender_HarusTerbacaSesuaiTanggal()
        {
            // Arrange
            DateTime tanggalSpesial = new DateTime(2026, 8, 17); // Hari Kemerdekaan 2026
            string namaPromo = "Promo Merdeka";
            decimal multiplier = 0.5m; // Diskon 50%
            string deskripsi = "Diskon setengah harga kemerdekaan RI";

            // Act
            _discountModule.TambahPromoCustom(tanggalSpesial, namaPromo, multiplier, deskripsi);
            var hasil = _discountModule.GetActivePromo(tanggalSpesial);

            // Assert
            Assert.AreEqual(namaPromo, hasil.namaPromo);
            Assert.AreEqual(multiplier, hasil.multiplier);
        }

        [TestMethod]
        public void Test_HitungTotal_PerhitunganDiskon_HarusDibulatkan()
        {
            // Arrange
            decimal totalBelanja = 15500m;
            decimal multiplierDiskon = 0.8m; // Diskon 20%
            // Perhitungan manual: 15500 * 0.8 = 12400

            // Act
            decimal hasilAkhir = _discountModule.HitungTotal(totalBelanja, multiplierDiskon);

            // Assert
            Assert.AreEqual(12400m, hasilAkhir);
        }
    }
}