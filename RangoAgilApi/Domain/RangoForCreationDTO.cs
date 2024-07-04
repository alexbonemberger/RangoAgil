using System.ComponentModel.DataAnnotations;

namespace RangoAgilApi.Domain;

public class RangoForCreationDTO
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Name { get; set; }
}
