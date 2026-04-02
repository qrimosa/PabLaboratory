using AppCore.Mapper;
using AppCore.Validators;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging; 
using Microsoft.Extensions.Logging.Abstractions; 

namespace AppCore.Module;

public static class ContactsModule
{
    public static IServiceCollection AddContactsModule(this IServiceCollection services, IConfiguration configuration)
    {
        ILoggerFactory dummyLoggerFactory = NullLoggerFactory.Instance;

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ContactsMappingProfile>();
        }, dummyLoggerFactory); 

        IMapper mapper = new AutoMapper.Mapper(mappingConfig);
        services.AddSingleton<IMapper>(mapper);

        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        services.AddFluentValidationAutoValidation();

        return services;
    }
}