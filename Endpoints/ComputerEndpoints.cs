using Microsoft.EntityFrameworkCore;
using LabApi.Data;
using LabApi.Models;

public static class ComputerEndpoints {
    public static void MapComputerEndpoints(this IEndpointRouteBuilder routes) {
        
        // ================================================================
        // BİLGİSAYAR ENDPOINT'LERİ
        // ================================================================

        // GET: Tüm bilgisayarları getir
        routes.MapGet("/api/bilgisayarlar", async (AppDbContext db) => {
            return await db.Computers.ToListAsync();
        });

        // GET: ID'ye göre tek bilgisayar
        routes.MapGet("/api/bilgisayar/{id}", async (AppDbContext db, int id) => {
            var bulunan = await db.Computers.FindAsync(id);
            if (bulunan == null)
                return Results.NotFound($"{id} numaralı bilgisayar bulunamadı.");
            return Results.Ok(bulunan);
        });

        // POST: Yeni bilgisayar ekle (Otomatik Demirbaş Kodu Üretimi)
        routes.MapPost("/api/bilgisayar-ekle", async (AppDbContext db, Computer yeniPc) => {
            
            // Aynı laboratuvardaki mevcut bilgisayar sayısını bul
            int count = await db.Computers.CountAsync(c => c.LabId == yeniPc.LabId);
            
            // Otomatik Demirbaş Kodu Üretimi (Örn: LAB1-PC-01)
            // (count + 1) formatlanırken :D2 kullanılarak 1 yerine 01, 2 yerine 02 yazdırılır.
            yeniPc.AssetCode = $"LAB{yeniPc.LabId}-PC-{(count + 1):D2}";

            db.Computers.Add(yeniPc);
            await db.SaveChangesAsync();
            return Results.Ok(yeniPc);
        });

        // GET: Arızalı bilgisayarlar (Issues tablosunda çözülmemiş kaydı olanlar)
        routes.MapGet("/api/arizali-pcler", async (AppDbContext db) => {
            var arizaliPcIdleri = await db.Issues
                .Where(i => i.IsResolved == false)
                .Select(i => i.ComputerId)
                .Distinct()
                .ToListAsync();

            var arizalilar = await db.Computers
                .Where(pc => arizaliPcIdleri.Contains(pc.Id))
                .ToListAsync();

            return Results.Ok(arizalilar);
        });

        // DELETE: Bilgisayar Sil
        routes.MapDelete("/api/bilgisayar/{id}", async (AppDbContext db, int id) => {
            var pc = await db.Computers.FindAsync(id);
            if (pc == null) return Results.NotFound();

            db.Computers.Remove(pc);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
