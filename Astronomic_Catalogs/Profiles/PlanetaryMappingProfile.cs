using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.ViewModels;
using AutoMapper;

namespace Astronomic_Catalogs.Profiles;

public class PlanetaryMappingProfile : Profile
{
    public PlanetaryMappingProfile()
    {
        // FlatRow -> Exoplanet
        CreateMap<PlanetarySystemFlatRow, Exoplanet>();

        // FlatRow -> PlanetarySystem
        CreateMap<PlanetarySystemFlatRow, PlanetarySystem>()
            .ForMember(dest => dest.Exoplanets, opt => opt.MapFrom(src =>
                new List<Exoplanet>
                {
                    new Exoplanet
                    {
                        Hostname = src.Hostname,
                        PlLetter = src.PlLetter,
                        PlRade = src.PlRade,
                        PlRadJ = src.PlRadJ,
                        PlMasse = src.PlMasse,
                        PlMassJ = src.PlMassJ,
                        PlOrbsmax = src.PlOrbsmax
                    }
                }));

        CreateMap<PlanetarySystem, PlanetarySystemFlatRow>()
            .ForMember(dest => dest.PlLetter, opt => opt.MapFrom(src =>
                src.Exoplanets != null && src.Exoplanets.Count > 0 ? src.Exoplanets[0].PlLetter : null))
            .ForMember(dest => dest.PlRade, opt => opt.MapFrom(src =>
                src.Exoplanets != null && src.Exoplanets.Count > 0 ? src.Exoplanets[0].PlRade : null))
            .ForMember(dest => dest.PlRadJ, opt => opt.MapFrom(src =>
                src.Exoplanets != null && src.Exoplanets.Count > 0 ? src.Exoplanets[0].PlRadJ : null))
            .ForMember(dest => dest.PlMasse, opt => opt.MapFrom(src =>
                src.Exoplanets != null && src.Exoplanets.Count > 0 ? src.Exoplanets[0].PlMasse : null))
            .ForMember(dest => dest.PlMassJ, opt => opt.MapFrom(src =>
                src.Exoplanets != null && src.Exoplanets.Count > 0 ? src.Exoplanets[0].PlMassJ : null))
            .ForMember(dest => dest.PlOrbsmax, opt => opt.MapFrom(src =>
                src.Exoplanets != null && src.Exoplanets.Count > 0 ? src.Exoplanets[0].PlOrbsmax : null));

    }
}
