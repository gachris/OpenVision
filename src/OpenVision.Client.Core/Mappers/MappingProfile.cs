using AutoMapper;
using OpenVision.Client.Core.Helpers;
using OpenVision.Client.Core.ViewModels;
using OpenVision.Shared.Requests;

namespace OpenVision.Client.Core;

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
        CreateMap<PostDatabaseViewModel, PostDatabaseRequest>();
        CreateMap<UpdateDatabaseViewModel, UpdateDatabaseRequest>();
        CreateMap<PostTargetViewModel, PostTargetRequest>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => FormFileHelper.GetAsByteArray(src.Image)))
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => FormFileHelper.GetAsMetadata(src.Metadata)));
        CreateMap<UpdateTargetImageViewModel, UpdateTargetRequest>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => FormFileHelper.GetAsByteArray(src.Image)));
        CreateMap<UpdateTargetNameViewModel, UpdateTargetRequest>();
        CreateMap<UpdateTargetWidthViewModel, UpdateTargetRequest>();
        CreateMap<UploadTargetMetadataViewModel, UpdateTargetRequest>()
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => FormFileHelper.GetAsMetadata(src.Metadata)));
    }
}