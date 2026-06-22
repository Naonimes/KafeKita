using System;
using System.Drawing;
using System.Windows.Forms;
using TugasBesarKPL_Solution; // Ini penting agar UI bisa membaca DiscountModule

namespace TugasBesarKPL_UInterface
{
    public class DiscountUI : UserControl
    {
        private DiscountModule _logic = new DiscountModule();
        private Panel scrollPanel;
        private FlowLayoutPanel flpInfoPromo;
        private Panel pnlInputPromo;
        private DateTimePicker dtpTanggal;
        private TextBox txtNamaPromo, txtDiskon, txtDeskripsi, txtInput;
        private Label lblHari, lblPromo, lblHasil;

        public DiscountUI()
        {
            scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            lblHari = new Label { Font = new Font("Segoe UI", 12), Location = new Point(20, 15), AutoSize = true };
            lblPromo = new Label { Location = new Point(20, 45), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };

            Label lblSimulasi = new Label { Text = "Masukkan Total Belanja (Rp):", Location = new Point(20, 90), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtInput = new TextBox { Location = new Point(20, 115), Width = 200, Font = new Font("Segoe UI", 12), PlaceholderText = "Misal: 100000" };
            Button btnHitung = new Button { Text = "Hitung", Location = new Point(230, 113), Width = 100, Height = 32, BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            lblHasil = new Label { Text = "Total Bayar: Rp 0", Location = new Point(20, 170), AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold) };

            btnHitung.Click += (s, e) => HitungSimulasi();

            Label lblInfoJudul = new Label { Text = "Daftar Promo Kafe Kita", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, 230), AutoSize = true };
            flpInfoPromo = new FlowLayoutPanel { Location = new Point(20, 260), Size = new Size(380, 150), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, AutoScroll = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };

            Button btnBukaForm = new Button { Text = "➕ Tambah Promo Baru", Location = new Point(20, 425), Width = 380, Height = 40, BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            pnlInputPromo = new Panel { Location = new Point(20, 475), Size = new Size(380, 205), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle, Visible = false };

            Label lblTgl = new Label { Text = "Pilih Tanggal Berlaku:", Location = new Point(10, 10), AutoSize = true };
            dtpTanggal = new DateTimePicker { Location = new Point(10, 30), Width = 200, Format = DateTimePickerFormat.Short };
            txtNamaPromo = new TextBox { Location = new Point(10, 65), Width = 350, PlaceholderText = "Nama Promo" };
            txtDeskripsi = new TextBox { Location = new Point(10, 100), Width = 350, PlaceholderText = "Deskripsi / Benefit" };
            txtDiskon = new TextBox { Location = new Point(10, 135), Width = 150, PlaceholderText = "Diskon %" };

            Button btnSimpanPromo = new Button { Text = "SIMPAN PROMO", Location = new Point(170, 134), Width = 190, Height = 30, BackColor = Color.OrangeRed, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            pnlInputPromo.Controls.AddRange(new Control[] { lblTgl, dtpTanggal, txtNamaPromo, txtDeskripsi, txtDiskon, btnSimpanPromo });

            Label lblSpacer = new Label { Location = new Point(20, 690), Size = new Size(10, 30) };

            btnBukaForm.Click += (s, e) => {
                pnlInputPromo.Visible = !pnlInputPromo.Visible;
                if (pnlInputPromo.Visible) scrollPanel.VerticalScroll.Value += 200;
            };
            btnSimpanPromo.Click += BtnSimpanPromo_Click;

            scrollPanel.Controls.AddRange(new Control[] { lblHari, lblPromo, lblSimulasi, txtInput, btnHitung, lblHasil, lblInfoJudul, flpInfoPromo, btnBukaForm, pnlInputPromo, lblSpacer });
            this.Controls.Add(scrollPanel);
            RefreshTampilan();
        }

        private void RefreshTampilan()
        {
            var promo = _logic.GetActivePromo(DayOfWeek.Friday);
            lblHari.Text = "📅 Hari ini: Jumat";
            lblPromo.Text = $"PROMO AKTIF: {promo.namaPromo}";
            lblPromo.ForeColor = Color.Green;
            flpInfoPromo.Controls.Clear();
            TambahLabelPromo("🕌 Jumat Berkah\nDiskon otomatis 20% untuk semua transaksi di hari Jumat.");
            TambahLabelPromo("🎉 Promo Akhir Pekan (Sabtu & Minggu)\nDiskon otomatis 10% untuk menemani waktu nongkrong.");

            foreach (var cp in _logic.GetCustomPromos())
            {
                string diskonText = cp.Multiplier < 1 ? $"Diskon {(1 - cp.Multiplier) * 100}%" : "Benefit";
                TambahLabelPromo($"🌟 {cp.Tanggal:dd MMM yyyy} - {cp.NamaPromo}\n{cp.Deskripsi} ({diskonText})");
            }
        }

        private void TambahLabelPromo(string teks)
        {
            flpInfoPromo.Controls.Add(new Label { Text = teks, AutoSize = true, Font = new Font("Segoe UI", 10), Margin = new Padding(10, 10, 10, 5), MaximumSize = new Size(340, 0) });
        }

        private void HitungSimulasi()
        {
            if (decimal.TryParse(txtInput.Text, out decimal nominal))
            {
                var promo = _logic.GetActivePromo(DayOfWeek.Friday);
                lblHasil.Text = $"Total Bayar: Rp {_logic.HitungTotal(nominal, promo.multiplier):N0}";
            }
        }

        private void BtnSimpanPromo_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtDiskon.Text, out decimal diskonPersen))
            {
                _logic.TambahPromoCustom(dtpTanggal.Value, txtNamaPromo.Text, (100 - diskonPersen) / 100m, txtDeskripsi.Text);
                txtNamaPromo.Clear(); txtDeskripsi.Clear(); txtDiskon.Clear(); pnlInputPromo.Visible = false;
                RefreshTampilan();
            }
        }
    }
}