using AutoMapper;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.ViewModels;
using Astronomic_Catalogs.Utils;


namespace Astronomic_Catalogs.Profiles;

public class NGCICProfile : Profile
{
    public NGCICProfile()
    {
        CreateMap<NGCICOpendatasoft, NGCICViewModel>()
            .ForMember(dest => dest.Messier, opt => opt.MapFrom(src =>
                src.Messier.HasValue && src.Messier > 0 ? "M" + src.Messier.Value.ToString() : null
            ));

        CreateMap<NGCICViewModel, NGCICOpendatasoft>()
            .ForMember(dest => dest.Messier, opt => opt.ConvertUsing(new MessierStringToIntConverter(), src => src.Messier));
    }

}
