using LabApi.Data;                  // AppDbContext sınıfına erişim
using LabApi.Models;                // Computer sınıfına erişim
using Microsoft.EntityFrameworkCore; // EF Core özellikleri için

var builder = WebApplication.CreateBuilder(args);
// Uygulama ayarlarını başlatan inşaatçı nesnesi.

// --- 1. SERVİS KAYITLARI (builder.Build'den önce olmalı) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Veritabanı servisini sisteme tanıtıyoruz.
// appsettings.json'daki "DefaultConnection" satırını okuyup SQLite'a bağlanır.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
// İnşaatçının hazırladığı ayarlar ile gerçek uygulamayı (app) oluşturur.

// --- OTOMATİK VERİTABANI KURULUMU ---
// Uygulama her başladığında veritabanını kontrol eder, yoksa oluşturur.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Migration'ları uygula (tablo yoksa oluşturur)
}

// --- 2. MIDDLEWARE (ARA YAZILIM) AYARLARI ---
app.UseSwagger();
app.UseSwaggerUI(); // Swagger arayüzünü /swagger adresinde açar

// ================================================================
// TEMEL ENDPOINT'LER — Artık RAM'deki liste değil, VERİTABANI kullanılıyor
// ================================================================

// --- GET: TÜM LİSTEYİ GETİR ---
app.MapGet("/api/bilgisayarlar", async (AppDbContext context) =>
{
    // context.Computers: Veritabanındaki tabloya git.
    // ToListAsync(): Tüm satırları asenkron (hızlı) şekilde liste olarak getir.
    return await context.Computers.ToListAsync();
});

// --- GET: TEK BİR BİLGİSAYAR GETİR (ID ile) ---
app.MapGet("/api/bilgisayar/{id}", async (AppDbContext context, int id) =>
{
    var bulunan = await context.Computers.FindAsync(id);
    // FindAsync: Birincil anahtara (Id) göre arama yapar. Bulamazsa null döner.
    if (bulunan == null)
        return Results.NotFound($"{id} numaralı bilgisayar bulunamadı.");
    return Results.Ok(bulunan);
});

// --- POST: YENİ BİLGİSAYAR EKLE ---
app.MapPost("/api/bilgisayar-ekle", async (AppDbContext context, Computer yeniPc) =>
{
    context.Computers.Add(yeniPc);       // Yeni nesneyi tabloya ekle (henüz kaydedilmedi).
    await context.SaveChangesAsync();    // Değişiklikleri veritabanı dosyasına fiziksel olarak işle.
    return Results.Ok(yeniPc);
});

// --- GET: ARIZALI BİLGİSAYARLARI GETİR ---
app.MapGet("/api/arizali-pcler", async (AppDbContext context) =>
{
    var arizalilar = await context.Computers
        .Where(pc => pc.BozukMu == true)
        .ToListAsync();
    return arizalilar;
});

// ================================================================
// HAFTA 4: LINQ ile Veri Sorgulama
// ================================================================

// --- GET: LAB İSTATİSTİK RAPORU ---
app.MapGet("/api/lab-istatistik", async (AppDbContext context) =>
{
    var tumListe = await context.Computers.ToListAsync();

    // 1. WHERE: RAM'i 8'den büyük olan bilgisayarları filtrele
    var gucluPcler = tumListe.Where(p => p.Ram > 8).ToList();

    // 2. ORDERBY: Markaya göre A'dan Z'ye alfabetik sırala
    var siraliPcler = tumListe.OrderBy(p => p.Marka).ToList();

    // 3. FIRSTORDEFAULT: ID'si 1 olan bilgisayarı getir (bulamazsa null döner, çökmez)
    var tekPc = tumListe.FirstOrDefault(p => p.Id == 1);

    // 4. SELECT: Listeden sadece markaları çek
    var markalar = tumListe.Select(p => p.Marka).ToList();

    // 5. COUNT: Kaç tane arızalı bilgisayar var?
    var arizaSayisi = tumListe.Count(p => p.BozukMu == true);

    // 6. ANY: 32GB RAM'li bilgisayar sistemde var mı?
    bool canavarVarMi = tumListe.Any(p => p.Ram == 32);

    // Mini Görev 1: Hem RAM'i 16 olan HEM DE markası "HP" olan var mı?
    bool hp16GbVarMi = tumListe.Any(p => p.Ram == 16 && p.Marka == "HP");

    // Mini Görev 2: RAM'e göre büyükten küçüğe sıralama
    var rameSirali = tumListe.OrderByDescending(p => p.Ram).ToList();

    return new
    {
        YuksekRamliCihazlar = gucluPcler,
        AlfabetikListe = siraliPcler,
        ArananBilgisayar = tekPc,
        SistemdekiMarkalar = markalar,
        ToplamAriza = arizaSayisi,
        LuksPcVarMi = canavarVarMi,
        HP_16GB_VarMi = hp16GbVarMi,
        RameGoreSiralanmis = rameSirali
    };
});

// --- POST: ARIZA BİLDİR ---
app.MapPost("/api/ariza-bildir", async (AppDbContext context, int pcId) =>
{
    // 1. ADIM: Bilgisayarı veritabanında bul
    var bulunanPc = await context.Computers.FindAsync(pcId);

    // 2. ADIM: Güvenlik kontrolü
    if (bulunanPc == null)
        return Results.NotFound($"Hata: {pcId} numaralı bilgisayar sistemde kayıtlı değil!");

    // 3. ADIM: Durumu güncelle
    bulunanPc.BozukMu = true;

    // 4. ADIM: Öncelik hesaplama
    string oncelikDurumu;
    if (bulunanPc.Ram >= 16)
        oncelikDurumu = "KRİTİK: Laboratuvarın en güçlü cihazlarından biri arızalandı!";
    else
        oncelikDurumu = "NORMAL: Standart cihaz arızası.";

    // 5. ADIM: Değişikliği veritabanına kaydet
    await context.SaveChangesAsync();

    return Results.Ok(new
    {
        Mesaj = "Arıza kaydı başarıyla oluşturuldu.",
        Cihaz = bulunanPc.Marka,
        Oncelik = oncelikDurumu,
        KayitTarihi = DateTime.Now.ToShortDateString()
    });
});

app.Run();
// Uygulamayı başlatır ve istekleri dinlemeye başlar.