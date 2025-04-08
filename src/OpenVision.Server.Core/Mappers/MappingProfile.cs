using AutoMapper;
using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Dtos;
using OpenVision.Shared.Requests;
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

        CreateMap<ApiKey, ApiKeyDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.Updated, opt => opt.MapFrom(src => src.Updated));

        CreateMap<Database, DatabaseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ApiKeys, opt => opt.MapFrom(src => src.ApiKeys))
            .ForMember(dest => dest.Targets, opt => opt.MapFrom(src => src.ImageTargets))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.Updated, opt => opt.MapFrom(src => src.Updated));

        CreateMap<ImageTarget, TargetDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.DatabaseId, opt => opt.MapFrom(src => src.DatabaseId))
           .ForMember(dest => dest.PreprocessImage, opt => opt.MapFrom(src => src.PreprocessImage))
           .ForMember(dest => dest.AfterProcessImage, opt => opt.MapFrom(src => src.AfterProcessImage))
           .ForMember(dest => dest.AfterProcessImageWithKeypoints, opt => opt.MapFrom(src => src.AfterProcessImageWithKeypoints))
           .ForMember(dest => dest.XUnits, opt => opt.MapFrom(src => src.Width))
           .ForMember(dest => dest.YUnits, opt => opt.MapFrom(src => src.Height))
           .ForMember(dest => dest.Recos, opt => opt.MapFrom(src => src.Recos))
           .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
           .ForMember(dest => dest.ActiveFlag, opt => opt.MapFrom(src => src.ActiveFlag))
           .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => src.Metadata))
           .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
           .ForMember(dest => dest.Updated, opt => opt.MapFrom(src => src.Updated))
           .ForMember(dest => dest.Database, opt => opt.MapFrom(src => src.Database));

        CreateMap<DatabaseDto, DatabaseResponse>()
                 .ReverseMap();

        CreateMap<ApiKeyDto, ApiKeyResponse>()
                 .ReverseMap();

        CreateMap<TargetDto, TargetResponse>()
                 .ReverseMap();

        CreateMap<TargetRecordModel, ImageTarget>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.TargetId)))
            .ForMember(dest => dest.Recos, opt => opt.MapFrom(src => src.TrackingRating))
            .ReverseMap()
            .ForCtorParam("targetId", opt => opt.MapFrom(src => src.Id.ToString()))
            .ForCtorParam("trackingRating", opt => opt.MapFrom(src => src.Recos));

        CreateMap<CreateDatabaseDto, PostDatabaseRequest>()
                 .ReverseMap();

        CreateMap<UpdateDatabaseDto, UpdateDatabaseRequest>()
                 .ReverseMap();

        CreateMap<CreateTargetDto, PostTargetRequest>()
                 .ReverseMap();

        CreateMap<UpdateTargetRequest, UpdateTargetDto>()
            .ReverseMap();

        CreateMap<DownloadFileResult, DatabaseFileDto>()
            .ReverseMap();
    }
}