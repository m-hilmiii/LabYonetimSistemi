using LabApi.Data;
using LabApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVİS KAYITLARI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- OTOMATİK VERİTABANI KURULUMU ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// --- 2. MIDDLEWARE ---
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();

// ================================================================
// BİLGİSAYAR ENDPOINT'LERİ
// ================================================================

// GET: Tüm bilgisayarları getir
app.MapGet("/api/bilgisayarlar", async (AppDbContext context) =>
{
    return await context.Computers.ToListAsync();
});

// GET: ID'ye göre tek bilgisayar
app.MapGet("/api/bilgisayar/{id}", async (AppDbContext context, int id) =>
{
    var bulunan = await context.Computers.FindAsync(id);
    if (bulunan == null)
        return Results.NotFound($"{id} numaralı bilgisayar bulunamadı.");
    return Results.Ok(bulunan);
});

// POST: Yeni bilgisayar ekle
app.MapPost("/api/bilgisayar-ekle", async (AppDbContext context, Computer yeniPc) =>
{
    context.Computers.Add(yeniPc);
    await context.SaveChangesAsync();
    return Results.Ok(yeniPc);
});

// GET: Arızalı bilgisayarlar (Issues tablosunda çözülmemiş kaydı olanlar)
app.MapGet("/api/arizali-pcler", async (AppDbContext context) =>
{
    // Issues tablosunda IsResolved=false olan PC ID'lerini bul
    var arizaliPcIdleri = await context.Issues
        .Where(i => i.IsResolved == false)
        .Select(i => i.ComputerId)
        .Distinct()
        .ToListAsync();

    // O bilgisayarları getir
    var arizalilar = await context.Computers
        .Where(pc => arizaliPcIdleri.Contains(pc.Id))
        .ToListAsync();

    return arizalilar;
});

// ================================================================
// LAB ENDPOINT'LERİ
// ================================================================

// GET: Tüm labları bilgisayarlarıyla birlikte getir
app.MapGet("/api/labs", async (AppDbContext context) =>
{
    return await context.Labs.Include(l => l.Computers).ToListAsync();
    // Include: "Labları getirirken ona ait bilgisayarları da birlikte getir" demektir.
});

// GET: ID'ye göre tek lab
app.MapGet("/api/labs/{id}", async (AppDbContext context, int id) =>
{
    var lab = await context.Labs.Include(l => l.Computers).FirstOrDefaultAsync(l => l.Id == id);
    if (lab == null)
        return Results.NotFound($"{id} numaralı lab bulunamadı.");
    return Results.Ok(lab);
});

// POST: Yeni lab ekle (Admin)
app.MapPost("/api/admin/labs", async (AppDbContext context, Lab yeniLab) =>
{
    context.Labs.Add(yeniLab);
    await context.SaveChangesAsync();
    return Results.Ok(yeniLab);
});

// ================================================================
// ARIZA KAYDI ENDPOINT'LERİ
// ================================================================

// POST: Arıza bildir (Issue tablosuna kayıt oluştur)
app.MapPost("/api/ariza-bildir", async (AppDbContext context, Issue yeniAriza) =>
{
    var pc = await context.Computers.FindAsync(yeniAriza.ComputerId);
    if (pc == null)
        return Results.NotFound($"Hata: {yeniAriza.ComputerId} numaralı bilgisayar bulunamadı!");

    // Öncelik hesaplama
    string oncelik = pc.Ram >= 16
        ? "KRİTİK: Güçlü cihaz arızası!"
        : "NORMAL: Standart cihaz arızası.";

    context.Issues.Add(yeniAriza);
    await context.SaveChangesAsync();

    return Results.Ok(new
    {
        Mesaj = "Arıza kaydı oluşturuldu.",
        Cihaz = pc.Brand,
        Oncelik = oncelik,
        ArizaId = yeniAriza.Id
    });
});

// GET: Tüm arıza kayıtları
app.MapGet("/api/arizalar", async (AppDbContext context) =>
{
    return await context.Issues.ToListAsync();
});

// ================================================================
// HAFTA 4: LINQ ile Veri Sorgulama (Yeni alan adlarıyla güncellendi)
// ================================================================

app.MapGet("/api/lab-istatistik", async (AppDbContext context) =>
{
    var tumListe = await context.Computers.ToListAsync();

    // 1. WHERE: RAM'i 8'den büyük olanlar
    var gucluPcler = tumListe.Where(p => p.Ram > 8).ToList();

    // 2. ORDERBY: Markaya göre A'dan Z'ye
    var siraliPcler = tumListe.OrderBy(p => p.Brand).ToList();

    // 3. FIRSTORDEFAULT: ID'si 1 olan
    var tekPc = tumListe.FirstOrDefault(p => p.Id == 1);

    // 4. SELECT: Sadece markaları çek
    var markalar = tumListe.Select(p => p.Brand).ToList();

    // 5. COUNT: HDMI'ı olan kaç bilgisayar var?
    var hdmiSayisi = tumListe.Count(p => p.HasHdmi == true);

    // 6. ANY: 32GB RAM'li bilgisayar var mı?
    bool canavarVarMi = tumListe.Any(p => p.Ram == 32);

    // 7. RAM'e göre büyükten küçüğe sırala
    var rameSirali = tumListe.OrderByDescending(p => p.Ram).ToList();

    return new
    {
        YuksekRamliCihazlar = gucluPcler,
        AlfabetikListe = siraliPcler,
        ArananBilgisayar = tekPc,
        SistemdekiMarkalar = markalar,
        HdmiluCihazSayisi = hdmiSayisi,
        LuksPcVarMi = canavarVarMi,
        RameGoreSiralanmis = rameSirali
    };
});

app.Run();