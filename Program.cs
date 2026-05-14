using LabApi.Models; // Model klasörümüzdeki Computer sınıfına erişim sağlar.

var builder = WebApplication.CreateBuilder(args);
// Uygulama ayarlarını başlatan inşaatçı nesnesi.

// --- 1. SERVİS KAYITLARI (builder.Build'den önce olmalı) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// İnşaatçının hazırladığı ayarlar ile gerçek uygulamayı (app) oluşturur.

// --- 2. MIDDLEWARE (ARA YAZILIM) AYARLARI ---
app.UseSwagger();
app.UseSwaggerUI(); // Swagger arayüzünü /swagger adresinde açar

// Veritabanı öğrenene kadar kullanacağımız geçici listemiz (RAM üzerinde tutulur)
var pcListesi = new List<Computer>
{
    new Computer { Id = 1, Marka = "Monster", Processor = "Intel Core i7", Ram = 16, BozukMu = false },
    new Computer { Id = 2, Marka = "Lenovo", Processor = "Intel Core i5", Ram = 8, BozukMu = true },
    new Computer { Id = 3, Marka = "HP", Processor = "AMD Ryzen 5", Ram = 8, BozukMu = false },
    new Computer { Id = 4, Marka = "Acer", Processor = "Intel Core i3", Ram = 4, BozukMu = false },
    new Computer { Id = 5, Marka = "MSI", Processor = "AMD Ryzen 7", Ram = 32, BozukMu = false }
};

// --- GET: TÜM LİSTEYİ GETİR ---
app.MapGet("/api/bilgisayarlar", () => pcListesi);
// MapGet: "/" sonrasına bu adresi yazanlara pcListesi'ni JSON olarak döner.
// () => pcListesi: "Hiçbir giriş parametresi alma, direkt listeyi gönder" demektir.

// --- GET: TEK BİR BİLGİSAYAR GETİR (ID ile) ---
app.MapGet("/api/bilgisayar/{id}", (int id) => {
    var bulunan = pcListesi.Find(x => x.Id == id);
    // .Find(x => x.Id == id): Liste içinde 'id'si uyan ilk kaydı bulur. 
    // Buradaki 'x' listedeki her bir Computer nesnesini temsil eder.
    return bulunan;
});

// --- POST: YENİ BİLGİSAYAR EKLE ---
app.MapPost("/api/bilgisayar-ekle", (Computer yeniPc) => {
    pcListesi.Add(yeniPc);
    // MapPost: Tarayıcıdan değil, bir araç (Postman/Swagger) üzerinden gönderilen veriyi yakalar.
    // (Computer yeniPc): Gelen JSON verisini otomatik olarak Computer nesnesine dönüştürür.
    return $"Yeni bilgisayar ({yeniPc.Marka}) başarıyla eklendi!";
});

// --- GET: ARIZALI BİLGİSAYARLARI GETİR (Görev 1) ---
app.MapGet("/api/arizali-pcler", () =>
{
    var arizalilar = pcListesi.Where(pc => pc.BozukMu == true).ToList();
    // .Where(pc => pc.BozukMu == true): Listedeki her bilgisayarı kontrol eder,
    // BozukMu alanı true olanları yeni bir listeye alır ve döndürür.
    return arizalilar;
});

// ================================================================
// HAFTA 4: LINQ ile Veri Sorgulama
// ================================================================

// --- GET: LAB İSTATİSTİK RAPORU ---
app.MapGet("/api/lab-istatistik", () =>
{
    // 1. WHERE: RAM'i 8'den büyük olan bilgisayarları filtrele
    var gucluPcler = pcListesi.Where(p => p.Ram > 8).ToList();

    // 2. ORDERBY: Markaya göre A'dan Z'ye alfabetik sırala
    var siraliPcler = pcListesi.OrderBy(p => p.Marka).ToList();

    // 3. FIRSTORDEFAULT: ID'si 1 olan bilgisayarı getir (bulamazsa null döner, çökmez)
    var tekPc = pcListesi.FirstOrDefault(p => p.Id == 1);

    // 4. SELECT: Listeden sadece markaları çek
    var markalar = pcListesi.Select(p => p.Marka).ToList();

    // 5. COUNT: Kaç tane arızalı bilgisayar var?
    var arizaSayisi = pcListesi.Count(p => p.BozukMu == true);

    // 6. ANY: 32GB RAM'li bilgisayar sistemde var mı?
    bool canavarVarMi = pcListesi.Any(p => p.Ram == 32);

    // --- MİNİ GÖREV 1: && ile çoklu koşul ---
    // Hem RAM'i 16 olan HEM DE markası "HP" olan bilgisayar var mı?
    bool hp16GbVarMi = pcListesi.Any(p => p.Ram == 16 && p.Marka == "HP");

    // --- MİNİ GÖREV 2: RAM'e göre büyükten küçüğe sıralama ---
    var rameSirali = pcListesi.OrderByDescending(p => p.Ram).ToList();

    // Tüm sonuçları tek pakette döndür (Anonim Nesne)
    return new
    {
        YuksekRamliCihazlar = gucluPcler,
        AlfabetikListe = siraliPcler,
        ArananBilgisayar = tekPc,
        SistemdekiMarkalar = markalar,
        ToplamAriza = arizaSayisi,
        LuksPcVarMi = canavarVarMi,
        HP_16GB_VarMi = hp16GbVarMi,           // Mini Görev 1
        RameGoreSiralanmis = rameSirali         // Mini Görev 2
    };
});

// --- POST: ARIZA BİLDİR ---
app.MapPost("/api/ariza-bildir", (int pcId) =>
{
    // 1. ADIM: Bilgisayarı listede bul
    var bulunanPc = pcListesi.FirstOrDefault(x => x.Id == pcId);

    // 2. ADIM: Güvenlik kontrolü — Bilgisayar sistemde kayıtlı mı?
    if (bulunanPc == null)
    {
        return Results.NotFound($"Hata: {pcId} numaralı bilgisayar sistemde kayıtlı değil!");
    }

    // 3. ADIM: Bilgisayarın durumunu arızalı olarak güncelle
    bulunanPc.BozukMu = true;

    // 4. ADIM: Öncelik hesaplama (RAM ne kadar yüksekse o kadar kritik)
    string oncelikDurumu;
    if (bulunanPc.Ram >= 16)
    {
        oncelikDurumu = "KRİTİK: Laboratuvarın en güçlü cihazlarından biri arızalandı!";
    }
    else
    {
        oncelikDurumu = "NORMAL: Standart cihaz arızası.";
    }

    // --- MİNİ GÖREV: Bilgisayarın listedeki index'ini hesapla ---
    var index = pcListesi.IndexOf(bulunanPc);

    // 5. ADIM: Sonucu paketleyip döndür
    return Results.Ok(new
    {
        Mesaj = "Arıza kaydı başarıyla oluşturuldu.",
        Cihaz = bulunanPc.Marka,
        Oncelik = oncelikDurumu,
        ListedeKacinciSirada = index,           // Mini Görev
        KayitTarihi = DateTime.Now.ToShortDateString()
    });
});

app.Run();
// Uygulamayı başlatır ve istekleri dinlemeye başlar.