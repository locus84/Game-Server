using AutoMapper;
using GameServer.Core.ChatAggregate;
using GameServer.Core.GameAggregate;
using GameServer.Core.MessageAggregate;
using GameServer.Core.PlayerAggregate;
using GameServer.Core.SettingsAggregate;
using GameServer.Core.StatisticsAggregate;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Web;

public class DTOsProfile : Profile
{
  public DTOsProfile()
  {
    CreateMap<Game, GameDTO>().ReverseMap();
    CreateMap<Player, PlayerDTO>().ReverseMap();
    CreateMap<MultiplayerGame, MultiplayerGameDTO>().ReverseMap();
    CreateMap<PushNotification, PushNotificationDTO>().ReverseMap();
    CreateMap<Chat, ChatDTO>().ReverseMap();
    CreateMap<Inbox, InboxDTO>().ReverseMap();
    CreateMap<ShopItem, ShopItemDTO>().ReverseMap();
    CreateMap<AdminSettings, AdminSettingsDTO>().ReverseMap();
    CreateMap<AppSettings, AppSettingsDTO>().ReverseMap();
    CreateMap<Statistics, StatisticsDTO>().ReverseMap();
    CreateMap<Chest, ChestDTO>().ReverseMap();
    CreateMap<FriendRequest, FriendRequestDTO>().ReverseMap();
    CreateMap<Report, ReportDTO>().ReverseMap();
  }
}
