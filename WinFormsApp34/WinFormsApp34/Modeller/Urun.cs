using System;

namespace WinFormsApp34.Modeller
{
    public class Urun
    {
        public int Id { get; set; }
        public string Barkod { get; set; }
        public string UrunAdi { get; set; }
        public int Miktar { get; set; }
        public string Birim { get; set; }
        public decimal Fiyat { get; set; } // EKLENDİ: Fiyat hatası için
        public string RafKonumu { get; set; }
        public string QRKodYolu { get; set; } // DÜZELTİLDİ: QRCodeYolu -> QRKodYolu
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime SonGuncellemeTarihi { get; set; }

        public Urun()
        {
            OlusturmaTarihi = DateTime.Now;
            SonGuncellemeTarihi = DateTime.Now;
        }

        public Urun(int id, string barkod, string urunAdi, int miktar, string birim, decimal fiyat, string rafKonumu, string qrKodYolu, DateTime olusturmaTarihi, DateTime sonGuncellemeTarihi)
        {
            Id = id;
            Barkod = barkod;
            UrunAdi = urunAdi;
            Miktar = miktar;
            Birim = birim;
            Fiyat = fiyat;
            RafKonumu = rafKonumu;
            QRKodYolu = qrKodYolu;
            OlusturmaTarihi = olusturmaTarihi;
            SonGuncellemeTarihi = sonGuncellemeTarihi;
        }
    }
}