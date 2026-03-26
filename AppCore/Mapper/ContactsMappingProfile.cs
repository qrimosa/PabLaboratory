using AutoMapper;
using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Mapper;

public class ContactsMappingProfile : Profile
{
    public ContactsMappingProfile()
    {
        CreateMap<Person, PersonDto>().ReverseMap();
        CreateMap<CreatePersonDto, Person>();
        CreateMap<UpdatePersonDto, Person>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Address, AddressDto>().ReverseMap();
        CreateMap<Note, NoteDto>();
        CreateMap<CreateNoteDto, Note>();
    }
}