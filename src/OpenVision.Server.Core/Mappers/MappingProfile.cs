using AutoMapper;
using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.GraphQL.Inputs;
using OpenVision.Server.Core.Helpers;
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
        // EF to DTO
        CreateMap<Database, DatabaseDto>()
            .ForMember(dest => dest.Targets, opt => opt.MapFrom(src => src.ImageTargets));

        CreateMap<ApiKey, ApiKeyDto>();

        CreateMap<ImageTarget, TargetDto>()
            .ForMember(dest => dest.XUnits, opt => opt.MapFrom(src => src.Width))
            .ForMember(dest => dest.YUnits, opt => opt.MapFrom(src => src.Height));

        // REST API request to DTO
        CreateMap<PostDatabaseRequest, CreateDatabaseDto>();
        CreateMap<UpdateDatabaseRequest, UpdateDatabaseDto>();
        CreateMap<PostTargetRequest, CreateTargetDto>();
        CreateMap<UpdateTargetRequest, UpdateTargetDto>();

        // DTO REST API response
        CreateMap<DatabaseDto, DatabaseResponse>();
        CreateMap<ApiKeyDto, ApiKeyResponse>();
        CreateMap<TargetDto, TargetResponse>();

        // SERVER API-KEY REST API request to DTO
        CreateMap<PostTrackableRequest, CreateTargetDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => Base64Helpers.GetAsByteArray(src.Image)));
        CreateMap<UpdateTrackableRequest, UpdateTargetDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => Base64Helpers.GetAsByteArray(src.Image)));

        // DTO to SERVER API-KEY REST API response
        CreateMap<TargetDto, TargetRecordModel>()
            .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.XUnits))
            .ForMember(dest => dest.TrackingRating, opt => opt.MapFrom(src => src.Rating));

        // DTO to GraphQL response
        CreateMap<DatabaseDto, GraphQL.Types.Database>();
        CreateMap<ApiKeyDto, GraphQL.Types.ApiKey>();
        CreateMap<TargetDto, GraphQL.Types.Target>();
        CreateMap<TargetDto, GraphQL.Types.TargetRecordModel>()
            .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.XUnits))
            .ForMember(dest => dest.TrackingRating, opt => opt.MapFrom(src => src.Rating));

        // GraphQL Input to DTO 
        CreateMap<PostTrackableInput, CreateTargetDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => Base64Helpers.GetAsByteArray(src.Image)));
        CreateMap<UpdateTrackableInput, UpdateTargetDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => Base64Helpers.GetAsByteArray(src.Image)));
    }
}