using AutoMapper;
using DiscordDomain.Models;
using ApplicationDiscord.Dto;

namespace DiscordWebClient.MapperConfig;

public class MapperConfig
{
    public static Mapper InitializeAutomapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, InfoUser>();
            cfg.CreateMap<User, InfoFriends>();
        });

        var mapper = new Mapper(config);
        return mapper;
    }
}
