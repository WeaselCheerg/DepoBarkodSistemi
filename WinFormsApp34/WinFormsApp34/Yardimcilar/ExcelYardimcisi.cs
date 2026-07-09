using System.Collections.Generic;
using ClosedXML.Excel;
using WinFormsApp34.Modeller;

namespace WinFormsApp34.Yardimcilar
{
    public static class ExcelYardimcisi
    {
        public static void UrunleriExceleAktar(List<Urun> urunler, string dosyaYolu)
        {
            using var workbook = new XLWorkbook();
            var sayfa = workbook.Worksheets.Add("Urunler");

            // Baslik satiri - depo/SAP raporlarinda yaygin kullanilan kolon adlarina yakin tutuldu
            sayfa.Cell(1, 1).Value = "Malzeme No";
            sayfa.Cell(1, 2).Value = "Urun Adi";
            sayfa.Cell(1, 3).Value = "Miktar";
            sayfa.Cell(1, 4).Value = "Birim";
            sayfa.Cell(1, 5).Value = "Depo Yeri";
            sayfa.Cell(1, 6).Value = "Fiyat";
            sayfa.Cell(1, 7).Value = "Son Guncelleme";
            sayfa.Range(1, 1, 1, 7).Style.Font.Bold = true;

            int satir = 2;
            foreach (var urun in urunler)
            {
                sayfa.Cell(satir, 1).Value = urun.Barkod;
                sayfa.Cell(satir, 2).Value = urun.UrunAdi;
                sayfa.Cell(satir, 3).Value = urun.Miktar;
                sayfa.Cell(satir, 4).Value = urun.Birim;
                sayfa.Cell(satir, 5).Value = urun.RafKonumu ?? "";
                sayfa.Cell(satir, 6).Value = urun.Fiyat;
                sayfa.Cell(satir, 7).Value = urun.SonGuncellemeTarihi;
                satir++;
            }

            sayfa.Columns().AdjustToContents();
            workbook.SaveAs(dosyaYolu);
        }
    }
}