namespace LabApi.Models;

public class Software
{
    public int Id { get; set; }
    public string? Name { get; set; }       // Örn: "Visual Studio 2022"
    public bool IsRequired { get; set; }    // Zorunlu yazılım mı?
}
