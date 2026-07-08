using System;

namespace WinFormsApp34.Modeller
{
    public class Urun
    {
        public int Id { get; set; }
        public string UrunAdi { get; set; }
        public string Barkod { get; set; }
        public int Miktar { get; set; }
        public string Birim { get; set; }
        public string RafKonumu { get; set; }
        public decimal? Fiyat { get; set; }
        public string QRKodYolu { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime SonGuncellemeTarihi { get; set; }
    }
}