using System;
using System.Drawing;
using System.Windows.Forms;
using TugasBesarKPL_Solution;

namespace TugasBesarKPL_UInterface
{
    public class MenuUI : UserControl
    {
        private MenuModule _logic = new MenuModule();

        private ListBox listMenu;
        private TextBox txtNama;
        private TextBox txtHarga;
        private ComboBox cmbKategori;

        public MenuUI()
        {
            Label lblJudul = new Label
            {
                Text = "Menu Kafe",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            Label lblKategori = new Label
            {
                Text = "Kategori",
                Location = new Point(20, 60),
                AutoSize = true
            };

            cmbKategori = new ComboBox
            {
                Location = new Point(20, 85),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbKategori.Items.Add("MKN");
            cmbKategori.Items.Add("MNM");
            cmbKategori.SelectedIndex = 0;
            cmbKategori.SelectedIndexChanged += (s, e) => TampilkanMenu();

            listMenu = new ListBox
            {
                Location = new Point(20, 125),
                Size = new Size(350, 180),
                Font = new Font("Segoe UI", 10)
            };

            Label lblTambah = new Label
            {
                Text = "Tambah Menu Baru",
                Location = new Point(20, 325),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            txtNama = new TextBox
            {
                Location = new Point(20, 355),
                Width = 220,
                PlaceholderText = "Nama menu"
            };

            txtHarga = new TextBox
            {
                Location = new Point(20, 390),
                Width = 220,
                PlaceholderText = "Harga menu"
            };

            Button btnTambah = new Button
            {
                Text = "Tambah Menu",
                Location = new Point(250, 355),
                Size = new Size(120, 60),
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnTambah.Click += BtnTambah_Click;

            this.Controls.Add(lblJudul);
            this.Controls.Add(lblKategori);
            this.Controls.Add(cmbKategori);
            this.Controls.Add(listMenu);
            this.Controls.Add(lblTambah);
            this.Controls.Add(txtNama);
            this.Controls.Add(txtHarga);
            this.Controls.Add(btnTambah);

            TampilkanMenu();
        }

        private void TampilkanMenu()
        {
            listMenu.Items.Clear();

            string kategori = cmbKategori.SelectedItem.ToString();

            foreach (var item in _logic.AmbilMenu(kategori))
            {
                listMenu.Items.Add($"{item.Kode} - {item.Nama} - Rp {item.Harga:N0}");
            }
        }

        private void BtnTambah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                MessageBox.Show("Nama menu harus diisi!");
                return;
            }

            if (!int.TryParse(txtHarga.Text, out int harga))
            {
                MessageBox.Show("Harga harus berupa angka!");
                return;
            }

            string kategori = cmbKategori.SelectedItem.ToString();

            _logic.TambahMenu(kategori, txtNama.Text, harga);

            txtNama.Clear();
            txtHarga.Clear();

            TampilkanMenu();

            MessageBox.Show("Menu berhasil ditambahkan!");
        }
    }
}