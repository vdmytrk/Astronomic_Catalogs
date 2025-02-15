using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models;

public class NameObject
{
    public int Id { get; set; }
    [Display(Name = "Інша назва")] 
    public string? Object { get; set; }
    [Display(Name = "Об'єкт NGC")] 
    public string? Name { get; set; }
    [Display(Name = "Коментар")] 
    public string? Comment { get; set; }
}
