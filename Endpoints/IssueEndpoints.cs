using Microsoft.EntityFrameworkCore;
using LabApi.Data;
using LabApi.Models;

public static class IssueEndpoints {
    public static void MapIssueEndpoints(this IEndpointRouteBuilder routes) {
        
        // POST: Arıza bildir (Issue tablosuna kayıt oluştur)
        routes.MapPost("/api/ariza-bildir", async (AppDbContext db, Issue yeniAriza) => {
            var pc = await db.Computers.FindAsync(yeniAriza.ComputerId);
            if (pc == null)
                return Results.NotFound($"Hata: {yeniAriza.ComputerId} numaralı bilgisayar bulunamadı!");

            // Öğrenciden gelen açıklamayı ve kategoriyi alıp kaydediyoruz
            yeniAriza.IsResolved = false;
            
            db.Issues.Add(yeniAriza);
            await db.SaveChangesAsync();

            return Results.Ok(new {
                Message = "Arıza kaydı başarıyla oluşturuldu.",
                ArizaId = yeniAriza.Id
            });
        });

        // GET: Tüm arıza kayıtları (Açık ve Çözülmüş) - Bilgisayar kodlarıyla birlikte
        routes.MapGet("/api/arizalar", async (AppDbContext db) => {
            var data = await (from i in db.Issues
                              join c in db.Computers on i.ComputerId equals c.Id
                              orderby i.IsResolved ascending, i.Id descending
                              select new {
                                  i.Id,
                                  i.Category,
                                  i.Description,
                                  i.IsResolved,
                                  AssetCode = c.AssetCode,
                                  LabId = c.LabId
                              }).ToListAsync();
            return Results.Ok(data);
        });

        // PUT: Arızayı "Çözüldü" olarak işaretle
        routes.MapPut("/api/arizalar/{id}/resolve", async (AppDbContext db, int id) => {
            var issue = await db.Issues.FindAsync(id);
            if (issue == null) return Results.NotFound("Arıza bulunamadı.");

            issue.IsResolved = true;
            await db.SaveChangesAsync();
            
            return Results.Ok(new { Message = "Arıza çözüldü olarak işaretlendi." });
        });

    }
}
