namespace LabApi.Models;

public class Computer
{
    public int Id { get; set; }
    public string? Marka { get; set; }
    public string? Processor { get; set; } // Görev 1: İşlemci alanı eklendi
    public int Ram { get; set; }
    public bool BozukMu { get; set; }
}