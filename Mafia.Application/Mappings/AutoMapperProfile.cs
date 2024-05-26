using AutoMapper;
using Mafia.Domain.Entities;
using Mafia.Domain.Entities.Game;
using System;

namespace Mafia.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Country, Country>();
            CreateMap<Country, Country>();

            CreateMap<RoomPlayer, RoomPlayerResponse>();
            CreateMap<RoomPlayerResponse, RoomPlayer>();

            CreateMap<RoomStage, RoomStageResponse>();
            CreateMap<RoomStageResponse, RoomStage>();

            CreateMap<Room, RoomResponse>();
            CreateMap<RoomResponse, Room>();

        }
    }
}