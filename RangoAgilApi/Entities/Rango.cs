using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RangoAgilApi.Entities;

public class Rango
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }
    public ICollection<Ingrediente> Ingredientes { get; set; } = [];
    public Rango()
    {

    }

    [SetsRequiredMembers]
    public Rango(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
