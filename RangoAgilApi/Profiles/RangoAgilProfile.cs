using AutoMapper;
using RangoAgilApi.Domain;
using RangoAgilApi.Entities;

namespace RangoAgilApi.Profiles;
public class RangoAgilProfile : Profile
{
    public RangoAgilProfile()
    {
        CreateMap<Rango, RangoDTO>().ReverseMap();
        CreateMap<Rango, RangoForCreationDTO>().ReverseMap();
        CreateMap<Ingrediente, IngredienteDTO>()
            .ForMember(
                d => d.RangoId,
                o => o.MapFrom(s => s.Rangos.First().Id)
            );
    }
}