# LRP - Laboratuvar Kaynak Planlama Sistemi (Aşama-1)

Bu proje, okul bünyesindeki laboratuvarların, bilgisayarların ve bu bilgisayarlardan sorumlu öğrencilerin dijital ortamda takip edilmesini sağlayan bir **Single Page Application (SPA)** yönetim sistemidir.

## 🚀 Teknolojiler
- **Backend:** .NET 8 Minimal APIs, Entity Framework Core (Code-First)
- **Veritabanı:** SQLite
- **Frontend:** HTML5, CSS3, Bootstrap 5, FontAwesome, Vanilla JavaScript (Fetch API)

## 📌 Kurulum ve Çalıştırma
Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin:

1. Projeyi klonlayın:
   ```bash
   git clone https://github.com/m-hilmiii/LabYonetimSistemi.git
   cd LabYonetimSistemi
   ```

2. Gerekli paketleri geri yükleyin:
   ```bash
   dotnet restore
   ```

3. Veritabanını oluşturun (Code-First Migration):
   ```bash
   dotnet ef database update
   ```
   *(Eğer `dotnet ef` kurulu değilse: `dotnet tool install --global dotnet-ef` komutunu çalıştırın)*

4. Projeyi çalıştırın:
   ```bash
   dotnet run
   ```

5. Tarayıcıda açın:
   Proje varsayılan olarak `http://localhost:5000` adresinde çalışacaktır. Giriş sayfasına (`login.html`) giderek sistemi kullanmaya başlayabilirsiniz.

## 🔐 Varsayılan Kullanıcılar
Sistem çalıştığında eğer veritabanı boşsa otomatik olarak şu admin hesabı oluşur:
- **Kullanıcı Adı:** `admin`
- **Şifre:** `123`

## 📸 Ekran Görüntüleri
*(Buraya projeye ait ekran görüntüleri eklenecektir)*

- Admin Paneli (Dashboard)
- Laboratuvar ve Bilgisayar Yönetimi
- Öğrenci Portalı
