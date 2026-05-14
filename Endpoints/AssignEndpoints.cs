using Microsoft.EntityFrameworkCore;
using LabApi.Data;
using LabApi.Models;

public static class AssignEndpoints {
    public static void MapAssignEndpoints(this IEndpointRouteBuilder routes) {
        
        // POST: Öğrenci Ata
        routes.MapPost("/api/admin/assign", async (AppDbContext db, Student student) => {
            
            var pc = await db.Computers.FindAsync(student.ComputerId);
            if (pc == null) return Results.NotFound("Bilgisayar bulunamadı.");

            // Öğrenci no'nun gönderilip gönderilmediğini kontrol edelim
            if (string.IsNullOrEmpty(student.StudentNumber)) 
                return Results.BadRequest("Öğrenci numarası zorunludur.");

            // 1. KONTROL: Bu öğrenci (Öğrenci No ile) sisteme zaten kayıtlı mı?
            bool isStudentExists = await db.Students.AnyAsync(s => s.StudentNumber == student.StudentNumber);
            if (isStudentExists)
                return Results.BadRequest("Bu öğrenci numarasına sahip öğrenciye zaten bir bilgisayar atanmış!");

            // Türkçe karakterleri temizleme fonksiyonu (Kullanıcı adı oluştururken sorun yaşamamak için)
            string rawName = student.FullName?.ToLower().Replace(" ", ".") ?? "ogr";
            rawName = rawName.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                             .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");

            // 2. KONTROL: Bu kullanıcı adı (Örn: ali.yilmaz) zaten Users tablosunda var mı?
            // Varsa sonuna rastgele bir sayı ekleyelim (ali.yilmaz145 gibi)
            string username = rawName;
            bool isUserExists = await db.Users.AnyAsync(u => u.Username == username);
            if (isUserExists) {
                username = username + new Random().Next(100, 999);
            }

            student.Username = username;

            db.Students.Add(student);

            db.Users.Add(new User {
                Username = username,
                Password = "123", // Varsayılan şifre
                Role = "Student"
            });

            await db.SaveChangesAsync();

            return Results.Ok(new {
                Message = "Öğrenci atandı ve hesabı oluşturuldu.",
                Username = username,
                Password = "123"
            });
        });

        // GET: Atanmış Öğrencileri ve Cihaz Bilgilerini Getir
        routes.MapGet("/api/admin/students", async (AppDbContext db) => {
            var data = await (from s in db.Students
                              join c in db.Computers on s.ComputerId equals c.Id
                              join u in db.Users on s.Username equals u.Username
                              select new {
                                  s.Id, 
                                  s.FullName, 
                                  s.StudentNumber,
                                  AssetCode = c.AssetCode, 
                                  LabId = c.LabId,
                                  Username = u.Username, 
                                  Password = u.Password
                              }).ToListAsync();
            return Results.Ok(data);
        });

        // PUT: Öğrenci Şifresini Sıfırla/Değiştir
        routes.MapPut("/api/admin/students/{username}/password", async (AppDbContext db, string username, User pwdData) => {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Results.NotFound("Kullanıcı bulunamadı.");

            user.Password = pwdData.Password;
            await db.SaveChangesAsync();
            return Results.Ok(new { Message = "Şifre başarıyla güncellendi." });
        });

        // DELETE: Öğrenci Kaydını ve Kullanıcı Hesabını Sil
        routes.MapDelete("/api/admin/students/{username}", async (AppDbContext db, string username) => {
            var student = await db.Students.FirstOrDefaultAsync(s => s.Username == username);
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (student != null) db.Students.Remove(student);
            if (user != null) db.Users.Remove(user);

            await db.SaveChangesAsync();
            return Results.Ok(new { Message = "Öğrenci başarıyla silindi." });
        });

    }
}
