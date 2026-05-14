namespace LabApi.Models;

public class Issue
{
    public int Id { get; set; }
    public string? Category { get; set; }    // Örn: "Donanım", "Yazılım"
    public string? Description { get; set; } // Arıza açıklaması
    public bool IsResolved { get; set; }     // Çözüldü mü?
    public int ComputerId { get; set; }      // Hangi bilgisayara ait?
}
