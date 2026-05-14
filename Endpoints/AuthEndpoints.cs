using Microsoft.EntityFrameworkCore;
using LabApi.Data;
using LabApi.Models;

public static class AuthEndpoints {
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes) {
        // Giriş Endpoint'i
        routes.MapPost("/api/login", async (AppDbContext db, User loginData) => {
            
            // GELİŞTİRİCİ KOLAYLIĞI: Eğer veritabanında hiç kullanıcı yoksa otomatik admin oluştur
            if (!await db.Users.AnyAsync()) {
                db.Users.Add(new User { Username = "admin", Password = "123", Role = "Admin" });
                await db.SaveChangesAsync();
            }

            // Kullanıcı adı ve şifre eşleşmesi kontrol edilir
            var user = await db.Users.FirstOrDefaultAsync(u => 
                u.Username.ToLower() == loginData.Username.ToLower() && 
                u.Password == loginData.Password);

            if (user == null) return Results.Unauthorized(); // 401: Yetkisiz
            
            // Başarılıysa kullanıcı adı ve rolü JSON olarak döner
            return Results.Ok(new { username = user.Username, role = user.Role });
        });
    }
}
