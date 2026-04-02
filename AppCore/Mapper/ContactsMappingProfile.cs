using AutoMapper;
using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Mapper;

public class ContactsMappingProfile : Profile
{
    public ContactsMappingProfile()
    {
        CreateMap<Person, PersonDto>().ReverseMap();
        CreateMap<Address, AddressDto>().ReverseMap();
        CreateMap<Note, NoteDto>();

        CreateMap<CreatePersonDto, Person>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdatePersonDto, Person>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<CreateNoteDto, Note>();
        CreateMap<Address, AddressDto>().DisableCtorValidation();
        CreateMap<CreatePersonDto, Person>().DisableCtorValidation();
    }
}