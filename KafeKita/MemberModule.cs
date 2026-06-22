using System;
using System.Collections.Generic;
using System.Diagnostics; // Untuk Debug.Assert (Design by Contract)
using System.Text.RegularExpressions; // Untuk Regex (Defensive Programming)

namespace TugasBesarKPL_Solution
{
    // =========================================================================
    // CONFIGURATION: STATE & TRIGGER AUTOMATA
    // =========================================================================
    public enum MemberState { Active, Expired, Blocked }
    public enum MemberTrigger { LoseSubscription, Renew, Freeze, Unfreeze }

    public class MemberData
    {
        public string ID { get; set; }
        public string Nama { get; set; }
        public string NoTelp { get; set; }
        public string Tier { get; set; }
        public MemberState CurrentState { get; set; } = MemberState.Active;
    }

    public class MemberModule
    {
        // Kulkas data privat (Enkapsulasi)
        private Dictionary<string, MemberData> dbMember = new Dictionary<string, MemberData>();

        // Matriks Transisi Automata (Table-Driven Approach)
        private Dictionary<(MemberState, MemberTrigger), MemberState> HarveyTransitionTable =
            new Dictionary<(MemberState, MemberTrigger), MemberState>
        {
            { (MemberState.Active, MemberTrigger.LoseSubscription), MemberState.Expired },
            { (MemberState.Active, MemberTrigger.Freeze), MemberState.Blocked },
            { (MemberState.Expired, MemberTrigger.Renew), MemberState.Active },
            { (MemberState.Expired, MemberTrigger.Freeze), MemberState.Blocked },
            { (MemberState.Blocked, MemberTrigger.Unfreeze), MemberState.Active }
        };

        // Dummy Data awal untuk simulasi
        public MemberModule()
        {
            dbMember.Add("M01", new MemberData { ID = "M01", Nama = "Khaidiri Murteza", NoTelp = "08123456789", Tier = "🥉 BRONZE", CurrentState = MemberState.Active });
            dbMember.Add("M02", new MemberData { ID = "M02", Nama = "Hilmy Rafi", NoTelp = "08987654321", Tier = "🥇 GOLD", CurrentState = MemberState.Expired });
        }

        // =========================================================================
        // TEKNIK KPL 1: AUTOMATA METHOD
        // =========================================================================
        public string JalankanTriggerAutomata(string memberID, MemberTrigger trigger)
        {
            if (!dbMember.ContainsKey(memberID)) return "Member Tidak Ditemukan";

            var member = dbMember[memberID];
            var key = (member.CurrentState, trigger);

            if (HarveyTransitionTable.ContainsKey(key))
            {
                MemberState nextState = HarveyTransitionTable[key];
                member.CurrentState = nextState;
                return $"Sukses! Status sekarang: {nextState}";
            }
            return $"Gagal! Status {member.CurrentState} tidak bisa diberi aksi {trigger}";
        }

        // =========================================================================
        // TEKNIK KPL 2: LOCAL API METHOD (Aman untuk UI lama)
        // =========================================================================
        public (string nama, string tier) ValidasiMember(string memberID)
        {
            if (dbMember.ContainsKey(memberID))
            {
                var m = dbMember[memberID];
                return (m.Nama, m.Tier);
            }
            return ("", "Tidak Terdaftar / Expired");
        }

        public string RegisterMember(string nama, string telp, string tier)
        {
            // ---------------------------------------------------------------------
            // TEKNIK KPL 3: DESIGN BY CONTRACT (DbC) - PRE-CONDITION
            // ---------------------------------------------------------------------
            Debug.Assert(nama != null, "Pre-condition Gagal: Parameter nama tidak boleh null!");
            Debug.Assert(telp != null, "Pre-condition Gagal: Parameter nomor telepon tidak boleh null!");

            // ---------------------------------------------------------------------
            // TEKNIK KPL 4: DEFENSIVE PROGRAMMING (REGEX FILTER)
            // ---------------------------------------------------------------------
            if (!Regex.IsMatch(telp, @"^[0-9]+$"))
            {
                throw new ArgumentException("Defensive Programming Triggered: Nomor telepon harus angka semua!");
            }

            // Logika auto-increment ID Member (M01, M02, dst)
            int maxId = 0;
            foreach (var key in dbMember.Keys)
            {
                if (key.StartsWith("M") && int.TryParse(key.Substring(1), out int num))
                {
                    if (num > maxId) maxId = num;
                }
            }
            string newId = $"M{(maxId + 1):D2}";
            dbMember.Add(newId, new MemberData { ID = newId, Nama = nama, NoTelp = telp, Tier = tier, CurrentState = MemberState.Active });
            return newId;
        }
    }
}