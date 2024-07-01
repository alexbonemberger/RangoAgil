using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RangoAgilApi.Entities;

public class Ingrediente
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }
    public ICollection<Rango> Rangos { get; set; } = [];
    public Ingrediente()
    {

    }

    [SetsRequiredMembers]
    public Ingrediente(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

