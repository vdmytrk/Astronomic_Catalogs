using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models;

public class NameObject
{
    public int Id { get; set; }
    public string? Object { get; set; }
    public string? Name { get; set; }
    public string? Comment { get; set; }
}
