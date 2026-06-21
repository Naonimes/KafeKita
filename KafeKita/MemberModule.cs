using System;
using System.Collections.Generic;

namespace TugasBesarKPL_Solution
{
    // === 1. AUTOMATA STATE & TRIGGER (Dosen bakal nyari ini) ===
    public enum MemberState { Active, Expired, Blocked }
    public enum MemberTrigger { LoseSubscription, Renew, Freeze, Unfreeze }

    public class MemberData
    {
        public string ID { get; set; }
        public string Nama { get; set; }
        public string NoTelp { get; set; }
        public string Tier { get; set; }
        public MemberState CurrentState { get; set; } = MemberState.Active; // State awal otomatis Active
    }

    public class MemberModule
    {
        private Dictionary<string, MemberData> dbMember = new Dictionary<string, MemberData>();

        // Matriks Transisi Automata (Table-Driven) standar KPL
        private Dictionary<(MemberState, MemberTrigger), MemberState> HarveyTransitionTable =
            new Dictionary<(MemberState, MemberTrigger), MemberState>
        {
            { (MemberState.Active, MemberTrigger.LoseSubscription), MemberState.Expired },
            { (MemberState.Active, MemberTrigger.Freeze), MemberState.Blocked },
            { (MemberState.Expired, MemberTrigger.Renew), MemberState.Active },
            { (MemberState.Expired, MemberTrigger.Freeze), MemberState.Blocked },
            { (MemberState.Blocked, MemberTrigger.Unfreeze), MemberState.Active }
        };

        public MemberModule()
        {
            dbMember.Add("M01", new MemberData { ID = "M01", Nama = "Khaidiri Murteza", NoTelp = "08123456789", Tier = "🥉 BRONZE", CurrentState = MemberState.Active });
            dbMember.Add("M02", new MemberData { ID = "M02", Nama = "Hilmy Rafi", NoTelp = "08987654321", Tier = "🥇 GOLD", CurrentState = MemberState.Expired });
        }

        // === 2. API UTAMA UNTUK AUTOMATA ===
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

        // === 3. FUNGSI ASLI LU (Gua balikin ke format awal biar UI lu GAK RUSAK) ===
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