namespace LabApi.Models;

public class Computer
{
    public int Id { get; set; }
    public string? AssetCode { get; set; }   // Demirbaş kodu: Örn: "LAB1-PC-01"
    public string? Brand { get; set; }       // Marka: Örn: "Dell"
    public string? Processor { get; set; }   // İşlemci: Örn: "Intel Core i7"
    public int Ram { get; set; }             // RAM (GB)
    public bool HasHdmi { get; set; }        // HDMI çıkışı var mı?
    public bool HasInternet { get; set; }    // İnternet bağlantısı var mı?
    public bool HasVeyon { get; set; }       // Veyon yazılımı kurulu mu?

    public int LabId { get; set; }           // Hangi laba ait? (Foreign Key)
}