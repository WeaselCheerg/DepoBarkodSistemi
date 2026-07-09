USE DepoYonetimDB;
GO

CREATE TABLE Urunler (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Barkod NVARCHAR(50) NOT NULL,
    UrunAdi NVARCHAR(100) NOT NULL,
    Miktar INT NOT NULL,
    Birim NVARCHAR(20) NOT NULL,
    Fiyat DECIMAL(18,2) NOT NULL,
    RafKonumu NVARCHAR(50) NULL,
    QRKodYolu NVARCHAR(250) NULL,
    OlusturmaTarihi DATETIME NOT NULL,
    SonGuncellemeTarihi DATETIME NOT NULL
);