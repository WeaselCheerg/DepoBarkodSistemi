using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinFormsApp34.Modeller;
using WinFormsApp34.Yardimcilar;

namespace WinFormsApp34
{
    public partial class Form1 : Form
    {
        private readonly VeritabaniYardimcisi _db = new VeritabaniYardimcisi();

        private TextBox txtBarkod;
        private Label lblUrunBilgisi;
        private DataGridView dgvUrunler;
        private Button btnYeniUrun;
        private Button btnStokGirisi;
        private Button btnStokCikisi;
        private Button btnExcelAktar;
        private Button btnYenile;
        private PictureBox pbQRKod;

        private Urun _seciliUrun;

        public Form1()
        {
            InitializeComponent();
            ArayuzuOlustur();
            ListeyiYenile();
        }

        private void ArayuzuOlustur()
        {
            this.Text = "Depo ve Stok Takip Sistemi - Barkod/QR";
            this.ClientSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);

            var lblBarkodBaslik = new Label
            {
                Text = "Barkod / QR Okut:",
                Location = new Point(20, 20),
                Size = new Size(140, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            txtBarkod = new TextBox
            {
                Location = new Point(160, 18),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 12)
            };
            txtBarkod.KeyDown += TxtBarkod_KeyDown;

            lblUrunBilgisi = new Label
            {
                Text = "Bir barkod okutun...",
                Location = new Point(470, 20),
                Size = new Size(490, 30),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DarkBlue
            };

            dgvUrunler = new DataGridView
            {
                Location = new Point(20, 65),
                Size = new Size(700, 520),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dgvUrunler.SelectionChanged += DgvUrunler_SelectionChanged;

            btnYeniUrun = new Button
            {
                Text = "Yeni Ürün Ekle (QR Oluştur)",
                Location = new Point(740, 65),
                Size = new Size(230, 40),
                Font = new Font("Segoe UI", 9.5f)
            };
            btnYeniUrun.Click += BtnYeniUrun_Click;

            btnStokGirisi = new Button
            {
                Text = "Stok Girişi (+)",
                Location = new Point(740, 115),
                Size = new Size(110, 35)
            };
            btnStokGirisi.Click += (s, e) => StokGuncelleDialog(true);

            btnStokCikisi = new Button
            {
                Text = "Stok Çıkışı (-)",
                Location = new Point(860, 115),
                Size = new Size(110, 35)
            };
            btnStokCikisi.Click += (s, e) => StokGuncelleDialog(false);

            btnExcelAktar = new Button
            {
                Text = "Excel'e Aktar",
                Location = new Point(740, 160),
                Size = new Size(110, 35)
            };
            btnExcelAktar.Click += BtnExcelAktar_Click;

            btnYenile = new Button
            {
                Text = "Listeyi Yenile",
                Location = new Point(860, 160),
                Size = new Size(110, 35)
            };
            btnYenile.Click += (s, e) => ListeyiYenile();

            pbQRKod = new PictureBox
            {
                Location = new Point(740, 210),
                Size = new Size(230, 230),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            this.Controls.Add(lblBarkodBaslik);
            this.Controls.Add(txtBarkod);
            this.Controls.Add(lblUrunBilgisi);
            this.Controls.Add(dgvUrunler);
            this.Controls.Add(btnYeniUrun);
            this.Controls.Add(btnStokGirisi);
            this.Controls.Add(btnStokCikisi);
            this.Controls.Add(btnExcelAktar);
            this.Controls.Add(btnYenile);
            this.Controls.Add(pbQRKod);

            this.Shown += (s, e) => txtBarkod.Focus();
        }

        private void TxtBarkod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string barkod = txtBarkod.Text.Trim();
                txtBarkod.Clear();

                if (string.IsNullOrEmpty(barkod)) return;

                var urun = _db.BarkodIleUrunGetir(barkod);
                if (urun == null)
                {
                    lblUrunBilgisi.ForeColor = Color.Red;
                    lblUrunBilgisi.Text = $"'{barkod}' kayıtlı değil. Yeni ürün olarak ekleyebilirsiniz.";
                    _seciliUrun = null;
                    pbQRKod.Image = null;
                }
                else
                {
                    lblUrunBilgisi.ForeColor = Color.DarkGreen;
                    lblUrunBilgisi.Text = $"{urun.UrunAdi} | Mevcut Stok: {urun.Miktar} {urun.Birim} | Raf: {urun.RafKonumu}";
                    _seciliUrun = urun;

                    if (!string.IsNullOrEmpty(urun.QRKodYolu) && File.Exists(urun.QRKodYolu))
                    {
                        pbQRKod.Image = Image.FromFile(urun.QRKodYolu);
                    }
                }
            }
        }

        private void DgvUrunler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUrunler.CurrentRow?.DataBoundItem is Urun urun)
            {
                _seciliUrun = urun;
                lblUrunBilgisi.ForeColor = Color.DarkGreen;
                lblUrunBilgisi.Text = $"{urun.UrunAdi} | Mevcut Stok: {urun.Miktar} {urun.Birim} | Raf: {urun.RafKonumu}";

                if (!string.IsNullOrEmpty(urun.QRKodYolu) && File.Exists(urun.QRKodYolu))
                {
                    pbQRKod.Image = Image.FromFile(urun.QRKodYolu);
                }
                else
                {
                    pbQRKod.Image = null;
                }
            }
        }

        private void BtnYeniUrun_Click(object sender, EventArgs e)
        {
            using var dlg = new UrunEkleForm();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ListeyiYenile();
                if (dlg.OlusturulanQRYolu != null && File.Exists(dlg.OlusturulanQRYolu))
                {
                    pbQRKod.Image = Image.FromFile(dlg.OlusturulanQRYolu);
                }
                MessageBox.Show("Ürün eklendi, QR kod oluşturuldu ve veritabanına kaydedildi.",
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void StokGuncelleDialog(bool girisMi)
        {
            if (_seciliUrun == null)
            {
                MessageBox.Show("Önce bir ürün seçin veya barkod okutun.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string baslik = girisMi ? "Stok Girişi" : "Stok Çıkışı";
            string girdi = BasitGirdiKutusu(
                $"{_seciliUrun.UrunAdi} için {(girisMi ? "eklenecek" : "çıkarılacak")} miktarı girin:",
                baslik, "1");

            if (girdi != null && int.TryParse(girdi, out int miktar) && miktar > 0)
            {
                int degisiklik = girisMi ? miktar : -miktar;
                _db.StokGuncelle(_seciliUrun.Id, degisiklik, girisMi ? "GIRIS" : "CIKIS");
                ListeyiYenile();
            }
        }

        private void BtnExcelAktar_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "Excel Dosyası (*.xlsx)|*.xlsx",
                FileName = $"Urun_Listesi_{DateTime.Now:yyyyMMdd}.xlsx"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var urunler = _db.TumUrunleriGetir();
                ExcelYardimcisi.UrunleriExceleAktar(urunler, sfd.FileName);
                MessageBox.Show("Excel dosyası oluşturuldu.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ListeyiYenile()
        {
            var urunler = _db.TumUrunleriGetir();
            dgvUrunler.DataSource = null;
            dgvUrunler.DataSource = urunler;

            if (dgvUrunler.Columns["QRKodYolu"] != null) dgvUrunler.Columns["QRKodYolu"].Visible = false;
            if (dgvUrunler.Columns["Id"] != null) dgvUrunler.Columns["Id"].Visible = false;
            if (dgvUrunler.Columns["OlusturmaTarihi"] != null) dgvUrunler.Columns["OlusturmaTarihi"].Visible = false;

            SutunBasligiAyarla("UrunAdi", "Ürün Adı");
            SutunBasligiAyarla("Barkod", "Barkod / QR Değeri");
            SutunBasligiAyarla("Miktar", "Stok Miktarı");
            SutunBasligiAyarla("Birim", "Birim");
            SutunBasligiAyarla("RafKonumu", "Raf Konumu");
            SutunBasligiAyarla("Fiyat", "Fiyat (₺)");
            SutunBasligiAyarla("SonGuncellemeTarihi", "Son Güncelleme");
        }

        private void SutunBasligiAyarla(string kolonAdi, string baslik)
        {
            if (dgvUrunler.Columns[kolonAdi] != null)
                dgvUrunler.Columns[kolonAdi].HeaderText = baslik;
        }

        // Basit bir miktar girme penceresi (ekstra referans gerektirmez)
        private static string BasitGirdiKutusu(string mesaj, string baslik, string varsayilan = "")
        {
            using var form = new Form
            {
                Width = 350,
                Height = 150,
                Text = baslik,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Left = 15, Top = 15, Width = 300, Text = mesaj };
            var txt = new TextBox { Left = 15, Top = 45, Width = 300, Text = varsayilan };
            var btnTamam = new Button { Text = "Tamam", Left = 150, Top = 75, Width = 80, DialogResult = DialogResult.OK };
            var btnIptal = new Button { Text = "İptal", Left = 240, Top = 75, Width = 80, DialogResult = DialogResult.Cancel };

            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            form.Controls.Add(btnTamam);
            form.Controls.Add(btnIptal);
            form.AcceptButton = btnTamam;
            form.CancelButton = btnIptal;

            return form.ShowDialog() == DialogResult.OK ? txt.Text : null;
        }
    }
}