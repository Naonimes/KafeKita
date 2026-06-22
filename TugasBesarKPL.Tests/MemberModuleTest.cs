using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TugasBesarKPL_Solution
{
    [TestClass]
    public class MemberModuleTests
    {
        private MemberModule memberBll;

        [TestInitialize]
        public void Setup()
        {
            memberBll = new MemberModule();
        }

        // -----------------------------------------------------------------
        // VERIFIKASI API & REGISTRASI DATA
        // -----------------------------------------------------------------
        [TestMethod]
        public void Test_RegistrasiDanValidasiAPI()
        {
            string newId = memberBll.RegisterMember("Fadhil Ganteng", "0811223344", "🥈 SILVER");
            var (nama, tier) = memberBll.ValidasiMember(newId);

            Assert.AreEqual("Fadhil Ganteng", nama);
            Assert.AreEqual("🥈 SILVER", tier);
        }

        // -----------------------------------------------------------------
        // VERIFIKASI AUTOMATA (TRANSISI VALID)
        // -----------------------------------------------------------------
        [TestMethod]
        public void Test_Automata_TransisiValid_ActiveKeBlocked()
        {
            string hasil = memberBll.JalankanTriggerAutomata("M01", MemberTrigger.Freeze);
            StringAssert.Contains(hasil, "Sukses");
        }

        // -----------------------------------------------------------------
        // VERIFIKASI AUTOMATA (TRANSISI ILEGAL / DICEKAL)
        // -----------------------------------------------------------------
        [TestMethod]
        public void Test_Automata_TransisiIlegal_ExpiredKeUnfreeze()
        {
            string hasil = memberBll.JalankanTriggerAutomata("M02", MemberTrigger.Unfreeze);
            StringAssert.Contains(hasil, "Gagal");
        }

        // -----------------------------------------------------------------
        // VERIFIKASI DEFENSIVE PROGRAMMING (REGEX TRAP) - VERSI .NET LAMA compatible
        // -----------------------------------------------------------------
        [TestMethod]
        public void Test_DefensiveProgramming_NomorHPAdaHuruf()
        {
            try
            {
                // Sengaja memasukkan input kotor yang mengandung huruf 'abc'
                memberBll.RegisterMember("User Iseng", "0812abc34", "🥉 BRONZE");

                // Jika baris di atas lolos tanpa error, berarti sistem pertahanan kita gagal
                Assert.Fail("Harusnya melempar ArgumentException karena nomor HP kotor!");
            }
            catch (ArgumentException)
            {
                // Jika berhasil menangkap ArgumentException, berarti sukses menahan input kotor
                // Test otomatis dianggap PASSED (Centang Hijau)
            }
        }
    }
}