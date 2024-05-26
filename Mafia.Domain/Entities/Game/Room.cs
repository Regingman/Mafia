using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mafia.Domain.Entities.Game
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Status? Status { get; set; }

        public string RoomNumber { get; set; }
        public string RoomPassword { get; set; }
        public int CurrentStageNumber { get; set; }
        public int MafiaCount { get; set; }
        public int PlayerCount { get; set; }
        public int PlayerCurrentCount { get; set; }

        public List<RoomPlayer> Players { get; set; }
        public List<RoomStage> Stages { get; set; }
    }
    
    public class RoomResponse
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Status? Status { get; set; }

        public string RoomNumber { get; set; }
        public string RoomPassword { get; set; }
        public int CurrentStageNumber { get; set; }
        public int MafiaCount { get; set; }
        public int PlayerCount { get; set; }
        public int PlayerCurrentCount { get; set; }

        public List<RoomPlayerResponse> Players { get; set; }
        public List<RoomStageResponse> Stages { get; set; }
    }

    public class RoomStageResponse
    {
        [Key]
        public int Id { get; set; }
        public int RoomId { get; set; }
        public bool Day { get; set; } = false;
        public bool Nigth { get; set; } = false;
        public bool Mafia { get; set; } = false;
        public bool Doctor { get; set; } = false;
        public bool Putana { get; set; } = false;
        public bool Commisar_whore { get; set; } = false;
        public int Stage { get; set; } = 1;
    }

    public class RoomPlayerResponse
    {
        [Key]
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerPhoto { get; set; }
        public Gender? PlayerGender { get; set; }
        public int? PlayerAge { get; set; }
        public RoomRole? RoomRole { get; set; }
        public bool RoomEnabled { get; set; } = true;
    }

    public enum Status
    {
        game_start = 0,
        winner_not = 1,
        citizen_win = 2,
        mafia_win = 3,
        day = 4,
        nigth = 5
    }
}
