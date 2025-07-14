namespace Astronomic_Catalogs.Entities;

public class Exoplanet
{
    public int Id { get; set; } 
    public string Hostname { get; set; } = string.Empty;
    public string PlLetter { get; set; } = string.Empty;
    public decimal? PlRade { get; set; } 
    public decimal? PlRadJ { get; set; } 
    public decimal? PlMasse { get; set; } 
    public decimal? PlMassJ { get; set; } 
    public decimal? PlOrbsmax { get; set; } 

}
