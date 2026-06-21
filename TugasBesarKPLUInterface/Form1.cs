using System;
using System.Drawing;
using System.Windows.Forms;
using TugasBesarKPL_Solution;

namespace TugasBesarKPL_UInterface
{
    public partial class Form1 : Form
    {
        private Panel panelContent;
        private OrderUI uiOrder;
        private MenuUI uiMenu;
        private MemberUI uiMember;
        private DiscountUI uiDiskon;
        private PaymentUI uiPayment;

        public Form1()
        {
            this.Text = "Tugas Besar KPL - Sistem Kasir";
            this.Size = new Size(450, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            uiOrder = new OrderUI();
            uiMenu = new MenuUI();
            uiMember = new MemberUI();
            uiDiskon = new DiscountUI();
            uiPayment = new PaymentUI();

            AppSession.PindahKeMenu = () => { TampilkanHalaman(uiMenu); uiMenu.RefreshMode(); };
            AppSession.PindahKeOrder = () => { TampilkanHalaman(uiOrder); uiOrder.RefreshData(); };
            AppSession.PindahKePayment = () => { TampilkanHalaman(uiPayment); uiPayment.RefreshMode(); };

            // TAMBAHAN BARU: Terima perintah bahwa meja sudah lunas dari PaymentUI
            uiPayment.OnPembayaranBerhasil += (meja) => {
                uiOrder.TandaiLunas(meja);
            };

            SetupLayout();
            TampilkanHalaman(uiOrder);
        }

        private void SetupLayout()
        {
            Panel header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.DarkSlateBlue };
            Label lblTitle = new Label { Text = "☕ KAFE KITA", ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(10, 15), AutoSize = true };
            header.Controls.Add(lblTitle);

            Panel bottomNav = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.White };
            string[] navItems = { "📝 Order", "🍔 Menu", "🎟️ Member", "💸 Diskon", "💳 Bayar" };
            UserControl[] pages = { uiOrder, uiMenu, uiMember, uiDiskon, uiPayment };

            for (int i = 0; i < navItems.Length; i++)
            {
                Button btn = new Button { Text = navItems[i], Width = 85, Height = 60, Location = new Point(i * 85 + 5, 5), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.LightGray };
                btn.FlatAppearance.BorderSize = 0;
                int index = i;
                btn.Click += (s, e) =>
                {
                    TampilkanHalaman(pages[index]);
                    if (pages[index] is OrderUI orderPage) orderPage.RefreshData();
                    if (pages[index] is MenuUI menuPage) menuPage.RefreshMode();
                    if (pages[index] is PaymentUI paymentPage) paymentPage.RefreshMode();
                };
                bottomNav.Controls.Add(btn);
            }

            panelContent = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };

            this.Controls.Add(panelContent);
            this.Controls.Add(header);
            this.Controls.Add(bottomNav);
        }

        private void TampilkanHalaman(UserControl halaman)
        {
            panelContent.Controls.Clear();
            halaman.Dock = DockStyle.Fill;
            panelContent.Controls.Add(halaman);
        }
    }
}