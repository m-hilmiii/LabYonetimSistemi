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
    return $"Yeni bilgisayar ({yeniPc.Brand}) başarıyla eklendi!";
});

app.Run();
// Uygulamayı başlatır ve istekleri dinlemeye başlar.