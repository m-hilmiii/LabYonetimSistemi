using Microsoft.EntityFrameworkCore;
using LabApi.Data;

public static class StudentEndpoints {
    public static void MapStudentEndpoints(this IEndpointRouteBuilder routes) {
        
        // GET: Öğrencinin kendine zimmetli olan bilgisayarını getir
        routes.MapGet("/api/student/my-pc", async (AppDbContext db, string username) => {
            
            // Öğrenciyi username ile bul
            var student = await db.Students.FirstOrDefaultAsync(s => s.Username == username);
            if (student == null) return Results.NotFound("Öğrenci kaydı bulunamadı.");

            // Öğrenciye atalı bilgisayarı bul
            var pc = await db.Computers.FirstOrDefaultAsync(c => c.Id == student.ComputerId);
            if (pc == null) return Results.NotFound("Size atanmış bir bilgisayar bulunamadı.");

            return Results.Ok(pc);
        });

    }
}
