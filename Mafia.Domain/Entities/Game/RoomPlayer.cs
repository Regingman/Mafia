using System.ComponentModel.DataAnnotations;

namespace Mafia.Domain.Entities.Game
{
    public class RoomPlayer
    {
        [Key]
        public int Id { get; set; }

        public Room Room { get; set; }
        public int RoomId { get; set; }

        public ApplicationUser Player { get; set; }
        public string PlayerId { get; set; }

        public string PlayerName { get; set; }
        public string PlayerPhoto { get; set; }
        public Gender? PlayerGender { get; set; }
        public int? PlayerAge { get; set; }
        public RoomRole? RoomRole { get; set; }
        public bool RoomEnabled { get; set; } = true;
    }

    public enum Gender
    {
        Female = 1,
        Male = 2,
    }

    public enum RoomRole
    {
        Civilian,
        Putana,
        Commisar,
        Doctor,
        Mafia,
        Default
    }
}
