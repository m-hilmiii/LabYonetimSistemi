using Microsoft.EntityFrameworkCore;
using LabApi.Data;
using LabApi.Models;

public static class LabEndpoints {
    public static void MapLabEndpoints(this IEndpointRouteBuilder routes) {
        var group = routes.MapGroup("/api/admin/labs");

        // Listeleme (PC'leri dahil ederek)
        group.MapGet("/", async (AppDbContext db) => 
            await db.Labs.Include(l => l.Computers).ToListAsync());

        // Ekleme
        group.MapPost("/", async (AppDbContext db, Lab lab) => {
            db.Labs.Add(lab);
            await db.SaveChangesAsync();
            return Results.Created($"/api/admin/labs/{lab.Id}", lab);
        });

        // Güncelleme
        group.MapPut("/{id}", async (AppDbContext db, int id, Lab lab) => {
            var item = await db.Labs.FindAsync(id);
            if (item == null) return Results.NotFound();
            item.Name = lab.Name;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Silme
        group.MapDelete("/{id}", async (AppDbContext db, int id) => {
            var item = await db.Labs.FindAsync(id);
            if (item == null) return Results.NotFound();
            db.Labs.Remove(item);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
