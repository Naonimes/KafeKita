using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TugasBesarKPL_Solution;

namespace TugasBesarKPL_UInterface
{
    public class PaymentUI : UserControl
    {
        private PaymentModule _logic = new PaymentModule();
        private DiscountModule _discountLogic = new DiscountModule();
        private Panel scrollPanel;
        private ComboBox cmbPesanan;
        private ComboBox cmbMember;
        private CheckBox chkPromo;
        private Label lblTotalAwal;
        private Label lblTotalAkhir;
        private Label lblQrisBox;
        private Button btnTampilkanQR;
        private Button btnCekStatus;
        private TextBox txtStruk;

        private decimal totalHargaAsli = 0;
        private decimal totalHargaDiskon = 0;
        private (string namaPromo, decimal multiplier) promoHariIni;

        //Event yang dijalankan saat pembayaran sukses
        public event Action<string> OnPembayaranBerhasil;

        public PaymentUI()
        {
            promoHariIni = _discountLogic.GetActivePromo(DayOfWeek.Friday);

            scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            Label lblJudul = new Label { Text = "Selesaikan Pembayaran", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };

            Label lblPesanan = new Label { Text = "ID Pesanan / Meja:", Location = new Point(20, 50), AutoSize = true, Font = new Font("Segoe UI", 10) };
            cmbPesanan = new ComboBox { Location = new Point(20, 75), Width = 370, Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList };

            // menghapus Label Member dan memindahkan ComboBox ke samping Total
            lblTotalAwal = new Label { Text = "Tagihan: Rp 0", Location = new Point(20, 115), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            cmbMember = new ComboBox { Location = new Point(230, 115), Width = 160, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMember.Items.AddRange(new string[] { "Non-Member", "Bronze (0%)", "Silver (10%)", "Gold (20%)" });
            cmbMember.SelectedIndex = 0;

            chkPromo = new CheckBox
            {
                Text = $"Aktifkan Promo: {promoHariIni.namaPromo}",
                Location = new Point(20, 150),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.MediumSeaGreen
            };

            lblTotalAkhir = new Label { Text = "TOTAL BAYAR: Rp 0", Location = new Point(20, 190), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.DarkRed };

            btnTampilkanQR = new Button { Text = "Lakukan Pembayaran", Location = new Point(20, 230), Width = 370, Height = 40, BackColor = Color.DarkSlateBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat };

            lblQrisBox = new Label
            {
                Text = "📱\nSCAN QRIS DI SINI",
                Location = new Point(130, 280),
                Size = new Size(150, 150),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DimGray,
                Visible = false
            };

            btnCekStatus = new Button { Text = "Cek Status Pembayaran", Location = new Point(20, 445), Width = 370, Height = 40, BackColor = Color.DodgerBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Enabled = false };

            txtStruk = new TextBox { Location = new Point(20, 495), Width = 370, Height = 120, Multiline = true, ReadOnly = true, Font = new Font("Consolas", 9), BackColor = Color.LightYellow, ScrollBars = ScrollBars.Vertical };

            Label lblSpacer = new Label { Location = new Point(20, 620), Size = new Size(10, 30) };

            // --- EVENT HANDLERS ---
            cmbPesanan.SelectedIndexChanged += (s, e) => HitungUlangTotal();
            cmbMember.SelectedIndexChanged += (s, e) => HitungUlangTotal();
            chkPromo.CheckedChanged += (s, e) => HitungUlangTotal();

            btnTampilkanQR.Click += (s, e) =>
            {
                if (cmbPesanan.SelectedItem == null)
                {
                    MessageBox.Show("Pilih meja terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lblQrisBox.Visible = true;
                btnCekStatus.Enabled = true;

                btnTampilkanQR.Enabled = false;
                cmbPesanan.Enabled = false;
                cmbMember.Enabled = false;
                chkPromo.Enabled = false;

                // scroll sedikit ke bawah agar QR terlihat
                scrollPanel.VerticalScroll.Value += 100;
            };

            btnCekStatus.Click += async (s, e) =>
            {
                btnCekStatus.Enabled = false;
                btnCekStatus.Text = "⏳ Menghubungi Server Bank...";
                txtStruk.Text = "Menunggu konfirmasi pembayaran...\r\n";

                await Task.Delay(1500);

                string resi = _logic.GenerateReceiptID();
                txtStruk.Text += "\r\n✅ PEMBAYARAN BERHASIL\r\n";
                txtStruk.Text += "====================================\r\n";
                txtStruk.Text += $"Pesanan    : {cmbPesanan.SelectedItem.ToString().ToUpper()}\r\n";
                txtStruk.Text += $"Member     : {cmbMember.SelectedItem.ToString()}\r\n";
                txtStruk.Text += $"Total Akhir: Rp {totalHargaDiskon:N0}\r\n";
                txtStruk.Text += $"Receipt ID : {resi}\r\n";
                txtStruk.Text += $"Waktu      : {DateTime.Now.ToString("dd MMM yyyy HH:mm")}\r\n";
                txtStruk.Text += "====================================\r\n";

                btnCekStatus.Text = "Selesai";
                btnCekStatus.BackColor = Color.MediumSeaGreen;

                // Memberitahu Form1 bahwa meja ini sudah lunas
                OnPembayaranBerhasil?.Invoke(cmbPesanan.SelectedItem.ToString());
            };

            // memasukan semua  elemen ke dalam scrollPanel, BUKAN ke this.Controls
            scrollPanel.Controls.AddRange(new Control[] { lblJudul, lblPesanan, cmbPesanan, lblTotalAwal, cmbMember, chkPromo, lblTotalAkhir, btnTampilkanQR, lblQrisBox, btnCekStatus, txtStruk, lblSpacer });

            this.Controls.Add(scrollPanel);
        }

        public void RefreshMode()
        {
            cmbPesanan.Items.Clear();
            cmbPesanan.Items.AddRange(AppSession.KeranjangMeja.Keys.ToArray());

            if (!string.IsNullOrEmpty(AppSession.MejaTargetBayar) && cmbPesanan.Items.Contains(AppSession.MejaTargetBayar))
            {
                cmbPesanan.SelectedItem = AppSession.MejaTargetBayar;
            }

            lblQrisBox.Visible = false;
            btnTampilkanQR.Enabled = true;
            btnCekStatus.Enabled = false;
            btnCekStatus.Text = "Cek Status Pembayaran";
            btnCekStatus.BackColor = Color.DodgerBlue;
            cmbPesanan.Enabled = true;
            cmbMember.Enabled = true;
            chkPromo.Enabled = true;
            txtStruk.Clear();
            AppSession.MejaTargetBayar = "";

            // reset scroll ke atas
            scrollPanel.VerticalScroll.Value = 0;
        }

        private void HitungUlangTotal()
        {
            if (cmbPesanan.SelectedItem == null) return;
            string mejaPilihan = cmbPesanan.SelectedItem.ToString();

            totalHargaAsli = 0;
            if (AppSession.KeranjangMeja.ContainsKey(mejaPilihan))
            {
                totalHargaAsli = AppSession.KeranjangMeja[mejaPilihan].Sum(x => x.Harga);
            }

            totalHargaDiskon = totalHargaAsli;

            if (cmbMember.SelectedIndex == 2) totalHargaDiskon *= 0.9m;
            else if (cmbMember.SelectedIndex == 3) totalHargaDiskon *= 0.8m;

            if (chkPromo.Checked)
            {
                totalHargaDiskon *= promoHariIni.multiplier;
            }

            lblTotalAwal.Text = $"Tagihan: Rp {totalHargaAsli:N0}";
            lblTotalAkhir.Text = $"TOTAL BAYAR: Rp {Math.Round(totalHargaDiskon):N0}";
        }
    }
}
