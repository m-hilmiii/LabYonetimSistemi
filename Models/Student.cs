namespace LabApi.Models;

public class Student
{
    public int Id { get; set; }
    public string? FullName { get; set; }   // Ad Soyad
    public string? StudentNumber { get; set; }  // Öğrenci No
    public int ComputerId { get; set; }     // Sorumlu olduğu bilgisayarın ID'si
    public string? Username { get; set; }   // User tablosuyla eşleşecek kullanıcı adı
}
