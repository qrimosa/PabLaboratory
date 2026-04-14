using AutoMapper;
using AppCore.Dto;
using AppCore.Models;
using AppCore.ValueObjects;

namespace AppCore.Mapper;

public class ContactsMappingProfile : Profile
{
    public ContactsMappingProfile()
    {
        CreateMap<Person, PersonDto>().ReverseMap();

        CreateMap<Address, AddressDto>()
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.ZipCode))
            .ReverseMap()
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.PostalCode));

        CreateMap<Note, NoteDto>();

        CreateMap<CreatePersonDto, Person>()
            .DisableCtorValidation()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdatePersonDto, Person>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<CreateNoteDto, Note>();
        CreateMap<Address, AddressDto>().DisableCtorValidation();
    }
}