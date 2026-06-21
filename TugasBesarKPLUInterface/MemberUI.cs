using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TugasBesarKPL_Solution;

namespace TugasBesarKPL_UInterface
{
    public class MemberUI : UserControl
    {
        private MemberModule _logic = new MemberModule();

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MemberUI
            // 
            Name = "MemberUI";
            ResumeLayout(false);

        }

        public MemberUI()
        {
            // --- BAGIAN 1: PENCARIAN MEMBER ---
            Label lblJudul = new Label { Text = "Cek ID Member", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            TextBox txtInput = new TextBox { Location = new Point(20, 50), Width = 260, Font = new Font("Segoe UI", 12), PlaceholderText = "Contoh: M01" };
            Button btnCek = new Button { Text = "Cek", Location = new Point(290, 48), Width = 100, Height = 32, BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            Label lblHasil = new Label { Location = new Point(20, 90), AutoSize = true, Font = new Font("Segoe UI", 11) };

            // --- BAGIAN 2: REGISTRASI MEMBER BARU ---
            Label lblRegJudul = new Label { Text = "Daftar Langganan Baru", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 160), AutoSize = true };

            Panel pnlReg = new Panel { Location = new Point(20, 195), Size = new Size(370, 185), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };

            TextBox txtRegNama = new TextBox { Location = new Point(15, 15), Width = 335, Font = new Font("Segoe UI", 10), PlaceholderText = "Nama Lengkap" };
            TextBox txtRegTelp = new TextBox { Location = new Point(15, 55), Width = 335, Font = new Font("Segoe UI", 10), PlaceholderText = "Nomor Telepon (08xxx)" };
            ComboBox cmbRegTier = new ComboBox { Location = new Point(15, 95), Width = 335, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRegTier.Items.AddRange(new string[] { "🥉 BRONZE", "🥈 SILVER", "🥇 GOLD" });
            cmbRegTier.SelectedIndex = 0;

            Button btnReg = new Button { Text = "DAFTAR", Location = new Point(15, 135), Width = 335, Height = 35, BackColor = Color.MediumSeaGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat };

            pnlReg.Controls.AddRange(new Control[] { txtRegNama, txtRegTelp, cmbRegTier, btnReg });

            // --- BAGIAN 3: INFORMASI TIER (HARGA BERLANGGANAN) ---
            Label lblInfoJudul = new Label { Text = "Biaya Langganan & Benefit", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, 400), AutoSize = true };

            Panel pnlInfo = new Panel { Location = new Point(20, 430), Size = new Size(370, 180), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            Label lblBronze = new Label { Text = "🥉 BRONZE - Rp 50.000 / Bulan\nBenefit: Promo khusus di hari tertentu.", Location = new Point(15, 15), AutoSize = true, Font = new Font("Segoe UI", 10) };
            Label lblSilver = new Label { Text = "🥈 SILVER - Rp 100.000 / Bulan\nBenefit: Promo hari tertentu + Diskon 10%.", Location = new Point(15, 65), AutoSize = true, Font = new Font("Segoe UI", 10) };
            Label lblGold = new Label { Text = "🥇 GOLD - Rp 150.000 / Bulan\nBenefit: Diskon 20% + Layanan Prioritas.", Location = new Point(15, 115), AutoSize = true, Font = new Font("Segoe UI", 10) };

            pnlInfo.Controls.AddRange(new Control[] { lblBronze, lblSilver, lblGold });

            // --- EVENT HANDLER TOMBOL CEK ---
            btnCek.Click += async (s, e) =>
            {
                btnCek.Enabled = false;
                lblHasil.Text = "⏳ Menghubungi server...";
                lblHasil.ForeColor = Color.Black;

                await Task.Delay(200);

                var hasil = _logic.ValidasiMember(txtInput.Text);
                if (hasil.nama != "")
                {
                    lblHasil.Text = $"Halo, {hasil.nama}!\nStatus Langganan: {hasil.tier}";
                    if (hasil.tier.Contains("GOLD")) lblHasil.ForeColor = Color.Goldenrod;
                    else if (hasil.tier.Contains("SILVER")) lblHasil.ForeColor = Color.DimGray;
                    else lblHasil.ForeColor = Color.SaddleBrown;
                }
                else
                {
                    lblHasil.Text = "Member tidak ditemukan / Langganan habis.";
                    lblHasil.ForeColor = Color.Red;
                }

                btnCek.Enabled = true;
            };

            // --- EVENT HANDLER TOMBOL DAFTAR ---
            btnReg.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtRegNama.Text) || string.IsNullOrWhiteSpace(txtRegTelp.Text))
                {
                    MessageBox.Show("Nama dan Nomor Telepon harus diisi lengkap!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kirim data ke Logic dan dapatkan ID baru
                string newId = _logic.RegisterMember(txtRegNama.Text, txtRegTelp.Text, cmbRegTier.SelectedItem.ToString());

                // Tampilkan pesan sukses di Windows Forms MessageBox
                MessageBox.Show($"Pendaftaran Berhasil!\n\nNama: {txtRegNama.Text}\nTier: {cmbRegTier.SelectedItem.ToString()}\n\nUID MEMBER KAMU: {newId}", "Registrasi Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Bersihkan form setelah sukses
                txtRegNama.Clear();
                txtRegTelp.Clear();
                cmbRegTier.SelectedIndex = 0;
            };

            this.Controls.AddRange(new Control[] { lblJudul, txtInput, btnCek, lblHasil, lblRegJudul, pnlReg, lblInfoJudul, pnlInfo });
        }
    }
}