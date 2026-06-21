using System;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using TugasBesarKPL_Solution;

namespace TugasBesarKPL_UInterface
{
    public class OrderUI : UserControl
    {
        private OrderModule _logic = new OrderModule();
        private FlowLayoutPanel flowLayout;
        private Button btnTambahMeja;

        public OrderUI()
        {
            Label lblJudul = new Label { Text = "Daftar Pesanan Aktif", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            btnTambahMeja = new Button { Text = "➕ Meja Baru", Location = new Point(280, 15), Width = 140, Height = 35, BackColor = Color.DarkSlateBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            Label lblStatusFlow = new Label { Text = "Draft -> Paid -> Cooking -> Served", Location = new Point(20, 50), AutoSize = true, Font = new Font("Segoe UI", 10) };

            flowLayout = new FlowLayoutPanel
            {
                Location = new Point(20, 80),
                Size = new Size(400, 500),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            btnTambahMeja.Click += (s, e) =>
            {
                string namaMejaBaru = "Meja " + (_logic.GetAllMeja().Count + 1);
                _logic.TambahMejaBaru(namaMejaBaru);
                RefreshData();
            };

            this.Controls.AddRange(new Control[] { lblJudul, btnTambahMeja, lblStatusFlow, flowLayout });
            RefreshData();
        }

        public void RefreshData()
        {
            flowLayout.Controls.Clear();
            foreach (var meja in _logic.GetAllMeja())
            {
                flowLayout.Controls.Add(BuatKotakPesanan(meja));
            }
        }

        private Panel BuatKotakPesanan(string namaMeja)
        {
            Panel pnl = new Panel { Width = 370, BackColor = Color.White, Margin = new Padding(0, 0, 0, 15) };
            Label lblMeja = new Label { Text = namaMeja, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };

            Button btnHapus = new Button
            {
                Text = "✖",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(30, 30),
                Location = new Point(335, 8),
                Cursor = Cursors.Hand
            };
            btnHapus.FlatAppearance.BorderSize = 0;
            btnHapus.Click += (s, e) =>
            {
                if (MessageBox.Show($"Hapus {namaMeja} beserta pesanannya?", "Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _logic.HapusMeja(namaMeja);
                    RefreshData();
                }
            };

            int currentState = _logic.GetState(namaMeja);
            Label lblStatus = new Label { Text = $"Status: {_logic.GetStateText(currentState)}", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.DarkOrange, Location = new Point(180, 13), AutoSize = true };

            string detailPesanan = "";
            decimal totalBelanja = 0;

            if (AppSession.KeranjangMeja.ContainsKey(namaMeja) && AppSession.KeranjangMeja[namaMeja].Count > 0)
            {
                var grouped = AppSession.KeranjangMeja[namaMeja].GroupBy(m => new { m.Nama, m.Harga }).Select(g =>
                {
                    decimal subtotal = g.Count() * g.Key.Harga;
                    totalBelanja += subtotal;
                    return $"- {g.Count()}x {g.Key.Nama}  ->  Rp {subtotal:N0}";
                });
                detailPesanan = string.Join("\n", grouped) + $"\n--------------------------------------\nTotal Tagihan: Rp {totalBelanja:N0}";
            }
            else
            {
                detailPesanan = "Belum ada pesanan.";
            }

            Label lblDetail = new Label { Text = detailPesanan, Location = new Point(10, 45), AutoSize = true, Font = new Font("Segoe UI", 10) };
            int buttonY = 45 + lblDetail.PreferredHeight + 15;

            Button btnAksi = new Button { Location = new Point(10, buttonY), Width = 350, Height = 40, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            pnl.Height = buttonY + 55;

            // LOGIKA BARU: Alur tombol berdasarkan state terbaru
            if (currentState == 0 && AppSession.KeranjangMeja[namaMeja].Count == 0)
            {
                btnAksi.Text = "+ TAMBAH PESANAN";
                btnAksi.BackColor = Color.DodgerBlue;
                btnAksi.Click += (s, e) =>
                {
                    AppSession.MejaTargetPesanan = namaMeja;
                    AppSession.PindahKeMenu?.Invoke();
                };
            }
            else if (currentState == 0 && AppSession.KeranjangMeja[namaMeja].Count > 0)
            {
                // Jika sudah ada pesanan dan status masih DRAFT, arahkan ke PAY
                btnAksi.Text = "PAY";
                btnAksi.BackColor = Color.MediumSlateBlue;
                btnAksi.Enabled = true;

                btnAksi.Click += (s, e) =>
                {
                    AppSession.MejaTargetBayar = namaMeja;
                    AppSession.PindahKePayment?.Invoke();
                };
            }
            else if (currentState == 3)
            {
                btnAksi.Text = "DONE";
                btnAksi.BackColor = Color.Gray;
                btnAksi.Enabled = false;
            }
            else
            {
                // Untuk status PAID (1) ke COOKING (2) dan COOKING (2) ke SERVED (3)
                btnAksi.Text = "NEXT";
                btnAksi.BackColor = Color.MediumSeaGreen;
                btnAksi.Enabled = true;

                btnAksi.Click += (s, e) =>
                {
                    _logic.MajukanStatus(namaMeja);
                    RefreshData();
                };
            }

            pnl.Controls.AddRange(new Control[] { lblMeja, lblStatus, btnHapus, lblDetail, btnAksi });
            return pnl;
        }

        // Fungsi bantuan untuk dipanggil dari Form1 / AppSession
        public void TandaiLunas(string namaMeja)
        {
            _logic.SetStatePaid(namaMeja);
            RefreshData();
        }
    }
}