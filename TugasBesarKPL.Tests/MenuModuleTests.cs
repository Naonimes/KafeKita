using Microsoft.VisualStudio.TestTools.UnitTesting;
using TugasBesarKPL_Solution;

namespace TugasBesarKPL.Tests
{
    [TestClass]
    public class MenuModuleTests
    {
        [TestMethod]
        public void AmbilMenu_KategoriMakanan_MengembalikanDaftarMenu()
        {
            MenuModule menuModule = new MenuModule();

            var hasil = menuModule.AmbilMenu("MKN");

            Assert.IsTrue(hasil.Count > 0);
        }

        [TestMethod]
        public void AmbilMenu_KategoriMinuman_MengembalikanDaftarMenu()
        {
            MenuModule menuModule = new MenuModule();

            var hasil = menuModule.AmbilMenu("MNM");

            Assert.IsTrue(hasil.Count > 0);
        }

        [TestMethod]
        public void AmbilMenu_KategoriTidakAda_MengembalikanListKosong()
        {
            MenuModule menuModule = new MenuModule();

            var hasil = menuModule.AmbilMenu("XYZ");

            Assert.AreEqual(0, hasil.Count);
        }

        [TestMethod]
        public void TambahMenu_KategoriMakanan_BerhasilMenambahMenu()
        {
            MenuModule menuModule = new MenuModule();
            int jumlahAwal = menuModule.AmbilMenu("MKN").Count;

            menuModule.TambahMenu("MKN", "Ayam Geprek", 17000);

            int jumlahAkhir = menuModule.AmbilMenu("MKN").Count;

            Assert.AreEqual(jumlahAwal + 1, jumlahAkhir);
        }

        [TestMethod]
        public void TambahMenu_KategoriBaru_BerhasilMembuatKategori()
        {
            MenuModule menuModule = new MenuModule();

            menuModule.TambahMenu("DSR", "Puding Coklat", 12000);

            var hasil = menuModule.AmbilMenu("DSR");

            Assert.AreEqual(1, hasil.Count);
            Assert.AreEqual("Puding Coklat", hasil[0].Nama);
        }

        [TestMethod]
        public void TambahMenu_MenuBaru_MembuatKodeOtomatis()
        {
            MenuModule menuModule = new MenuModule();

            menuModule.TambahMenu("MKN", "Ayam Geprek", 17000);

            var daftarMenu = menuModule.AmbilMenu("MKN");
            var menuTerakhir = daftarMenu[daftarMenu.Count - 1];

            Assert.AreEqual("MKN-04", menuTerakhir.Kode);
        }
    }
}