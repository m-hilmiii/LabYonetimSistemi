using Microsoft.EntityFrameworkCore; // EF Core özelliklerini kullanmak için
using LabApi.Models;                  // Computer sınıfına erişmek için

namespace LabApi.Data;

// DbContext: Veritabanı ile uygulama arasındaki ana köprüdür.
public class AppDbContext : DbContext
{
    // Constructor: Ayarların dışarıdan (Program.cs'den) gelmesini sağlar.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet: Veritabanındaki "Computers" tablosunu temsil eder.
    // Bu satır sayesinde "C# listesiyle oynar gibi" veritabanıyla işlem yapacağız.
    public DbSet<Computer> Computers { get; set; }
}
