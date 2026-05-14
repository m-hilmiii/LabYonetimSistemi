namespace LabApi.Models;

public class Student
{
    public int Id { get; set; }
    public string? FullName { get; set; }   // Ad Soyad
    public int Grade { get; set; }          // 1 veya 2. sınıf
    public int ComputerId { get; set; }     // Sorumlu olduğu bilgisayarın ID'si
    public string? Username { get; set; }   // User tablosuyla eşleşecek kullanıcı adı
}
