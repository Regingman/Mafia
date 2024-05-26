using Mafia.Domain.Entities.Game;
using Mafia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafia.Application.Paggination;

namespace Mafia.Application.Services.Mafia
{
    public interface IMafiaService
    {
        Task<int> UserCreate(string userId, string roomNumber, string roomPassword, string name, int age, Gender gender, string photo);
        Task<int> UserRefresh(string userId, string roomNumber, string roomPassword);
        Task<PaginatedList<RoomResponse>> ListRoomAsync(int page, int size);
        RoomResponse CreateRoom(string adminId, int roomMafia, int playerCount);
        void DisablePlayer(string playerId);
        void StartGame(int roomId);
        List<PlayerStatus> GetAllPlayerStatusLive(int roomId);
        List<PlayerStatus> GetAllPlayerStatusLiveUser(int roomId);
        Task RoomStageUpdate(int roomId, RoomStageUpdateType stageUpdateType);
        void PlayerVote(int roomId, string playerId);
        Task<GameStatus> UpdateGameStatus(int roomId);
        Task<bool> MafiaVote(int roomId, string playerId);
        Task<bool> CommisarVote(int roomId, string playerId);
        Task<bool> PutanaVote(int roomId, string playerId);
        Task<bool> DoctorVote(int roomId, string playerId);
    }

    public enum RoomStageUpdateType
    {
        StartNight,
        NightMafia,
        PostKill,
        GetKill,
        NightDoctor,
        PostMedical,
        GetMedical,
        NightWhore,
        PostPutana,
        GetPutana,
        CommisarWhore,
        PostCommisar,
        StartDay
    }

    public class PlayerStatus
    {
        public string PlayerId { get; set; }
        public string PlayerUserName { get; set; }
        public string PlayerName { get; set; }
        public string RoomNumber { get; set; }
        public bool? IsAlive { get; set; }
        public RoomRole? Role { get; set; }
    }

    public class GameStatus
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool MafiaWin { get; set; }
        public bool CivilianWin { get; set; }
    }
}
