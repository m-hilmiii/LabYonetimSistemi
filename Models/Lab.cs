namespace LabApi.Models;

public class Lab
{
    public int Id { get; set; }
    public string? Name { get; set; }                          // Örn: "Lab-A"

    // Navigation Property: Bir labın birden fazla bilgisayarı olabilir
    public List<Computer> Computers { get; set; } = new();
}
