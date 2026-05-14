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

// --- MODÜLER ENDPOINT KAYITLARI ---
app.MapAuthEndpoints();
app.MapStatEndpoints();
app.MapLabEndpoints();
app.MapComputerEndpoints();
app.MapAssignEndpoints();
app.MapStudentEndpoints();
app.MapIssueEndpoints();

app.Run();