using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using TrainingProject.Application.Mappers;

namespace TrainingProject.Application.Tests.Unit.Helpers;

public class AutoMapperFixture
{
    public IMapper Mapper { get; }

    public AutoMapperFixture()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
        },
        NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();
        Mapper = configuration.CreateMapper();
    }
}
