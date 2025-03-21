using AutoMapper;
using OpenVision.Server.EntityFramework.Entities;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Mappers;

/// <summary>
/// AutoMapper profile for mapping between entity classes and response/DTO classes.
/// </summary>
internal class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappingProfile"/> class.
    /// Configures AutoMapper mappings between entity classes and response/DTO classes.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<ApiKey, ApiKeyResponse>();
        CreateMap<ApiKeyResponse, ApiKey>();

        CreateMap<Database, DatabaseResponse>()
            .ForCtorParam("targets", opt => opt.MapFrom(src => src.ImageTargets));

        CreateMap<DatabaseResponse, Database>()
            .ForMember(dest => dest.ImageTargets, opt => opt.MapFrom(src => src.Targets));

        CreateMap<ImageTarget, TargetResponse>()
            .ForCtorParam("xUnits", opt => opt.MapFrom(src => src.Width))
            .ForCtorParam("yUnits", opt => opt.MapFrom(src => src.Height));

        CreateMap<TargetResponse, ImageTarget>()
            .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.XUnits))
            .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.YUnits));

        CreateMap<ImageTarget, TargetRecordModel>()
            .ForCtorParam("targetId", opt => opt.MapFrom(src => src.Id.ToString()))
            .ForCtorParam("trackingRating", opt => opt.MapFrom(src => src.Recos));

        CreateMap<TargetRecordModel, ImageTarget>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.TargetId)))
            .ForMember(dest => dest.Recos, opt => opt.MapFrom(src => src.TrackingRating));
    }
}