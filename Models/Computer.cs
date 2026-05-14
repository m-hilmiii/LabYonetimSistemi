namespace LabApi.Models;

public class Computer
{
    public int Id { get; set; }
    public string Marka { get; set; }
    public int Ram { get; set; }
    public bool BozukMu { get; set; }
}

public class Processor
{
    public string ACER { get; set; }
    public string HP { get; set; }
    public string MSI { get; set; }
}