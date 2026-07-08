using System;
using System.IO;
using QRCoder;

namespace WinFormsApp34.Yardimcilar
{
    public static class QRKodYardimcisi
    {
        // Verilen icerigi QR koda cevirir, PNG olarak "QRKodlari" klasorune kaydeder ve dosya yolunu dondurur.
        public static string QRKodOlusturVeKaydet(string icerik, string dosyaAdi)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(icerik, QRCodeGenerator.ECCLevel.Q);
            using var qrKod = new PngByteQRCode(qrCodeData);
            byte[] pngBaytlari = qrKod.GetGraphic(20);

            string klasorYolu = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QRKodlari");
            Directory.CreateDirectory(klasorYolu);

            string dosyaYolu = Path.Combine(klasorYolu, $"{dosyaAdi}.png");
            File.WriteAllBytes(dosyaYolu, pngBaytlari);

            return dosyaYolu;
        }
    }
}