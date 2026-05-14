namespace LabApi.Models;

public class User
{
    public int Id { get; set; }
    public string? Username { get; set; } // Giriş adı (öğrenci no veya "admin")
    public string? Password { get; set; } // Şifre
    public string? Role { get; set; }     // "Admin" veya "Student"
}
