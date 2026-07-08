using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using WinFormsApp34.Modeller;

namespace WinFormsApp34.Yardimcilar
{
    public class VeritabaniYardimcisi
    {
        // Bilgisayarına göre sunucu adını burada güncelle.
        // LocalDB: (localdb)\MSSQLLocalDB   |   SQL Express: .\SQLEXPRESS
        private readonly string _baglantiMetni =
            @"Server=(localdb)\MSSQLLocalDB;Database=DepoYonetimDB;Trusted_Connection=True;TrustServerCertificate=True;";

        private SqlConnection BaglantiAc()
        {
            var baglanti = new SqlConnection(_baglantiMetni);
            baglanti.Open();
            return baglanti;
        }

        public bool BarkodVarMi(string barkod)
        {
            using var baglanti = BaglantiAc();
            using var komut = new SqlCommand("SELECT COUNT(1) FROM Urunler WHERE Barkod = @Barkod", baglanti);
            komut.Parameters.AddWithValue("@Barkod", barkod);
            return (int)komut.ExecuteScalar() > 0;
        }

        public int UrunEkle(Urun urun)
        {
            using var baglanti = BaglantiAc();
            using var komut = new SqlCommand(@"
                INSERT INTO Urunler (UrunAdi, Barkod, Miktar, Birim, RafKonumu, Fiyat, QRKodYolu, OlusturmaTarihi, SonGuncellemeTarihi)
                OUTPUT INSERTED.Id
                VALUES (@UrunAdi, @Barkod, @Miktar, @Birim, @RafKonumu, @Fiyat, @QRKodYolu, GETDATE(), GETDATE())", baglanti);

            komut.Parameters.AddWithValue("@UrunAdi", urun.UrunAdi);
            komut.Parameters.AddWithValue("@Barkod", urun.Barkod);
            komut.Parameters.AddWithValue("@Miktar", urun.Miktar);
            komut.Parameters.AddWithValue("@Birim", urun.Birim ?? "Adet");
            komut.Parameters.AddWithValue("@RafKonumu", (object)urun.RafKonumu ?? DBNull.Value);
            komut.Parameters.AddWithValue("@Fiyat", (object)urun.Fiyat ?? DBNull.Value);
            komut.Parameters.AddWithValue("@QRKodYolu", (object)urun.QRKodYolu ?? DBNull.Value);

            return (int)komut.ExecuteScalar();
        }

        public Urun BarkodIleUrunGetir(string barkod)
        {
            using var baglanti = BaglantiAc();
            using var komut = new SqlCommand("SELECT * FROM Urunler WHERE Barkod = @Barkod", baglanti);
            komut.Parameters.AddWithValue("@Barkod", barkod);
            using var okuyucu = komut.ExecuteReader();
            if (okuyucu.Read())
            {
                return SatiriUrunaCevir(okuyucu);
            }
            return null;
        }

        public List<Urun> TumUrunleriGetir()
        {
            var liste = new List<Urun>();
            using var baglanti = BaglantiAc();
            using var komut = new SqlCommand("SELECT * FROM Urunler ORDER BY UrunAdi", baglanti);
            using var okuyucu = komut.ExecuteReader();
            while (okuyucu.Read())
            {
                liste.Add(SatiriUrunaCevir(okuyucu));
            }
            return liste;
        }

        // miktarDegisikligi pozitifse stok girisi, negatifse stok cikisi olarak islenir.
        public void StokGuncelle(int urunId, int miktarDegisikligi, string hareketTipi, string aciklama = null)
        {
            using var baglanti = BaglantiAc();
            using var islem = baglanti.BeginTransaction();
            try
            {
                using (var komut = new SqlCommand(@"
                    UPDATE Urunler 
                    SET Miktar = Miktar + @MiktarDegisikligi, SonGuncellemeTarihi = GETDATE() 
                    WHERE Id = @Id", baglanti, islem))
                {
                    komut.Parameters.AddWithValue("@MiktarDegisikligi", miktarDegisikligi);
                    komut.Parameters.AddWithValue("@Id", urunId);
                    komut.ExecuteNonQuery();
                }

                using (var komut = new SqlCommand(@"
                    INSERT INTO StokHareketleri (UrunId, HareketTipi, Miktar, HareketTarihi, Aciklama)
                    VALUES (@UrunId, @HareketTipi, @Miktar, GETDATE(), @Aciklama)", baglanti, islem))
                {
                    komut.Parameters.AddWithValue("@UrunId", urunId);
                    komut.Parameters.AddWithValue("@HareketTipi", hareketTipi);
                    komut.Parameters.AddWithValue("@Miktar", Math.Abs(miktarDegisikligi));
                    komut.Parameters.AddWithValue("@Aciklama", (object)aciklama ?? DBNull.Value);
                    komut.ExecuteNonQuery();
                }

                islem.Commit();
            }
            catch
            {
                islem.Rollback();
                throw;
            }
        }

        private Urun SatiriUrunaCevir(SqlDataReader okuyucu)
        {
            return new Urun
            {
                Id = okuyucu.GetInt32(okuyucu.GetOrdinal("Id")),
                UrunAdi = okuyucu.GetString(okuyucu.GetOrdinal("UrunAdi")),
                Barkod = okuyucu.GetString(okuyucu.GetOrdinal("Barkod")),
                Miktar = okuyucu.GetInt32(okuyucu.GetOrdinal("Miktar")),
                Birim = okuyucu.GetString(okuyucu.GetOrdinal("Birim")),
                RafKonumu = okuyucu.IsDBNull(okuyucu.GetOrdinal("RafKonumu")) ? null : okuyucu.GetString(okuyucu.GetOrdinal("RafKonumu")),
                Fiyat = okuyucu.IsDBNull(okuyucu.GetOrdinal("Fiyat")) ? (decimal?)null : okuyucu.GetDecimal(okuyucu.GetOrdinal("Fiyat")),
                QRKodYolu = okuyucu.IsDBNull(okuyucu.GetOrdinal("QRKodYolu")) ? null : okuyucu.GetString(okuyucu.GetOrdinal("QRKodYolu")),
                OlusturmaTarihi = okuyucu.GetDateTime(okuyucu.GetOrdinal("OlusturmaTarihi")),
                SonGuncellemeTarihi = okuyucu.GetDateTime(okuyucu.GetOrdinal("SonGuncellemeTarihi"))
            };
        }
    }
}