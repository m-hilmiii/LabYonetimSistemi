using LabApi.Models; // Model klasörümüzdeki Computer sınıfına erişim sağlar.

var builder = WebApplication.CreateBuilder(args);
// Uygulama ayarlarını başlatan inşaatçı nesnesi.

// --- 1. SERVİS KAYITLARI (builder.Build'den önce olmalı) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// İnşaatçının hazırladığı ayarlar ile gerçek uygulamayı (app) oluşturur.

// --- 2. MIDDLEWARE (ARA YAZILIM) AYARLARI ---
// Swagger'ı etkinleştiriyoruz
app.UseSwagger();
app.UseSwaggerUI();

// Veritabanı öğrenene kadar kullanacağımız geçici listemiz (RAM üzerinde tutulur)
var pcListesi = new List<Computer>
{
    new Computer { Id = 1, Brand = "Monster", Ram = 16, HasIssue = false },
    new Computer { Id = 2, Brand = "Lenovo", Ram = 8, HasIssue = true }
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
    return $"Yeni bilgisayar ({yeniPc.Brand}) başarıyla eklendi!";
});

app.Run();
// Uygulamayı başlatır ve istekleri dinlemeye başlar.