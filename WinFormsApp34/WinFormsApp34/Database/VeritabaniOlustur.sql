-- ============================================
-- Depo ve Stok Takip Sistemi - Veritabani Scripti
-- Barkod / QR Kod Tabanli Urun ve Stok Yonetimi
-- ============================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DepoYonetimDB')
BEGIN
    CREATE DATABASE DepoYonetimDB;
END
GO

USE DepoYonetimDB;
GO

-- Urunler tablosu: her urunun barkod/QR degeri, stok miktari ve konum bilgisi
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Urunler')
BEGIN
    CREATE TABLE Urunler
    (
        Id                     INT IDENTITY(1,1) PRIMARY KEY,
        UrunAdi                NVARCHAR(200)   NOT NULL,
        Barkod                 NVARCHAR(100)   NOT NULL UNIQUE,
        Miktar                 INT             NOT NULL DEFAULT 0,
        Birim                  NVARCHAR(20)    NOT NULL DEFAULT N'Adet',
        RafKonumu              NVARCHAR(100)   NULL,
        Fiyat                  DECIMAL(18,2)   NULL,
        QRKodYolu              NVARCHAR(300)   NULL,
        OlusturmaTarihi        DATETIME        NOT NULL DEFAULT GETDATE(),
        SonGuncellemeTarihi    DATETIME        NOT NULL DEFAULT GETDATE()
    );
END
GO

-- StokHareketleri tablosu: her giris/cikis hareketinin denetim kaydi
-- (SAP'teki malzeme hareketi mantigina benzer sekilde tutulur; ileride raporlama icin kullanislidir)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StokHareketleri')
BEGIN
    CREATE TABLE StokHareketleri
    (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        UrunId          INT             NOT NULL FOREIGN KEY REFERENCES Urunler(Id),
        HareketTipi     NVARCHAR(10)    NOT NULL,  -- 'GIRIS' veya 'CIKIS'
        Miktar          INT             NOT NULL,
        HareketTarihi   DATETIME        NOT NULL DEFAULT GETDATE(),
        Aciklama        NVARCHAR(300)   NULL
    );
END
GO

PRINT N'Veritabani ve tablolar hazir.';