using System.ComponentModel.DataAnnotations;

namespace Mafia.Domain.Entities.Game
{
    public class RoomStagePlayer
    {
        [Key]
        public int Id { get; set; }
        public RoomStage Room { get; set; }
        public int RoomId { get; set; }
        public RoomPlayer Player { get; set; }
        public int PlayerId { get; set; }

        public bool Day { get; set; } = false;
        public int DayCount { get; set; } = 0;
        public int MafiaCount { get; set; } = 0;
        public bool Nigth { get; set; } = false;
        public bool Mafia { get; set; } = false;
        public bool Doctor { get; set; } = false;
        public bool Putana { get; set; } = false;
        public bool Commisar_whore { get; set; } = false;
    }
}
