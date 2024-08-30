using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Mafia.Application.Mappings;
using Mafia.Application.Paggination;
using Mafia.Application.Services.Interfaces;
using Mafia.Domain.Data.Adapters;
using Mafia.Domain.Entities;
using Mafia.Domain.Entities.Game;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Application.Services.Mafia
{
    public class MafiaService : IMafiaService
    {
        private readonly MafiaDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MafiaService(MafiaDbContext context, IHubContext<ChatHub> hubContext,
        IMapper mapper, ICurrentUserService currentUserService)
        {
            _context = context;
            _hubContext = hubContext;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }


        public async Task<int> UserCreate(string userId, string roomNumber, string name, int age, Gender gender, string photo)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(e => e.RoomNumber == roomNumber);
            if (room == null)
            {
                throw new InvalidOperationException("Room not found");
            }
            var roomP = new RoomPlayer
            {
                PlayerName = name,
                PlayerAge = age,
                PlayerGender = gender,
                PlayerPhoto = photo,
                RoomId = room.Id,
                PlayerId = userId
            };

            _context.RoomPlayers.Add(roomP);
            _context.SaveChanges();
            // Подключаем пользователя к группе комнаты
            //await _hubContext.Clients.User(userId).SendAsync("JoinRoomGroup", room.RoomNumber);

            // Уведомляем всех игроков в комнате о новом участнике
            await _hubContext.Clients.Group(room.RoomNumber).SendAsync("UserJoined", name);
            foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
            {
                await _hubContext.Clients.User(temp).SendAsync("UserJoined", $"" +
                $" user connected {temp}");
            }
            return room.Id;
        }

        public async Task<PaginatedList<RoomResponse>> ListRoomAsync(int page, int size)
        {
            return await _context.Rooms.Include(r => r.Players).Include(r => r.Stages)
                     .ProjectTo<RoomResponse>(_mapper.ConfigurationProvider)
                     .PaginatedListAsync(page, size); ;
        }

        string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                char randomChar = chars[random.Next(chars.Length)];
                stringBuilder.Append(randomChar);
            }

            return stringBuilder.ToString();
        }

        public RoomResponse CreateRoom(string adminId, int roomMafia, int playerCount)
        {

            var room = new Room
            {
                UserId = adminId,
                RoomNumber = GenerateRandomText(8),
                RoomPassword = GenerateRandomText(8),
                CreateDate = DateTime.UtcNow,
                Status = Status.game_start,
                CurrentStageNumber = 1,
                MafiaCount = roomMafia,
                PlayerCount = playerCount,
                PlayerCurrentCount = 0,
                Players = new List<RoomPlayer>(),
                Stages = new List<RoomStage>()
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();
            var record = _mapper.Map<Room, RoomResponse>(room);
            return record;
        }

        public void DisablePlayer(string playerId)
        {
            var player = _context.RoomPlayers.FirstOrDefault(e => e.PlayerId == playerId && (e.Room.Status != Status.mafia_win || e.Room.Status != Status.citizen_win));
            if (player != null)
            {
                player.RoomEnabled = false;
                _context.RoomPlayers.Update(player);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Player not found");
            }
        }

        public void StartGame(int roomId)
        {
            Console.WriteLine("Start Game");
            var room = _context.Rooms.Include(r => r.Players).Include(e => e.Stages).FirstOrDefault(r => r.Id == roomId);
            if (room != null /*&& room.Players.Count == room.PlayerCount && room.Stages.Count == 0*/)
            {
                var roles = new List<RoomRole>();
                // Add the specified number of mafia roles
                for (int i = 0; i < room.MafiaCount; i++)
                {
                    roles.Add(RoomRole.Mafia);
                }

                // Add one of each special role
                roles.Add(RoomRole.Commisar);
                roles.Add(RoomRole.Doctor);
                if (room.Players.Count > 10)
                {
                    roles.Add(RoomRole.Putana);
                }

                // Fill the remaining roles with civilians
                while (roles.Count < room.Players.Count)
                {
                    roles.Add(RoomRole.Civilian);
                }

                var random = new Random();
                foreach (var player in room.Players)
                {
                    int index = random.Next(roles.Count);
                    player.RoomRole = roles[index];
                    roles.RemoveAt(index);
                }

                room.Status = Status.winner_not;
                room.CurrentStageNumber = 1;

                var stage = new RoomStage
                {
                    RoomId = roomId,
                    Day = true,
                    Stage = room.CurrentStageNumber
                };

                room.Stages.Add(stage);
                _context.SaveChanges();

                foreach (var temp in room.Players)
                {
                    var roomStage = new RoomStagePlayer()
                    {
                        PlayerId = temp.Id,
                        RoomId = stage.Id,
                    };
                    _context.RoomStagePlayers.Add(roomStage);
                    _context.SaveChanges();
                }
            }
            else
            {
                throw new InvalidOperationException("Room not found or player count does not match the required player count or Game is started");
            }
        }

        public List<PlayerStatus> GetAllPlayerStatusLive(int roomId)
        {
            var user = _currentUserService.ApplicationUser.Id;
            Console.WriteLine(_currentUserService.ApplicationUser.Id);
            var room = _context.Rooms.Include(r => r.Players).ThenInclude(e => e.Player).FirstOrDefault(r => r.Id == roomId);
            if (room != null)
            {
                return room.Players.Select(p => new PlayerStatus
                {
                    IsYou = p.PlayerId.Contains(user) ? true : false,
                    PlayerId = p.PlayerId,
                    PlayerUserName = p.Player.UserName,
                    PlayerName = p.PlayerName,
                    IsAlive = p.RoomEnabled,
                    Role = p.RoomRole == null ? RoomRole.Default : p.RoomRole,
                    PlayerPhoto = p.PlayerPhoto,
                    RoomNumber = p.Room.RoomNumber,
                    RoomEnabled = p.RoomEnabled
                }).ToList();
            }
            else
            {
                throw new InvalidOperationException("Room not found");
            }
        }

        private List<int> GetAllPlayerStatusLiveCheck(int roomId)
        {
            var room = _context.Rooms.Include(r => r.Players).ThenInclude(e => e.Player).FirstOrDefault(r => r.Id == roomId);
            if (room != null)
            {
                return room.Players.Where(e => e.RoomEnabled).Select(p => p.Id).ToList();
            }
            else
            {
                throw new InvalidOperationException("Room not found");
            }
        }

        public List<PlayerStatus> GetAllPlayerStatusLiveUser(int roomId)
        {
            var user = _currentUserService.ApplicationUser.Id;
            Console.WriteLine(_currentUserService.ApplicationUser.Id);
            var room = _context.Rooms.Include(r => r.Players).FirstOrDefault(r => r.Id == roomId);
            if (room != null)
            {
                return room.Players.Select(p => new PlayerStatus
                {
                    IsYou = p.PlayerId.Contains(user) ? true : false,
                    PlayerId = p.PlayerId,
                    PlayerName = p.PlayerName,
                    IsAlive = p.RoomEnabled,
                    PlayerPhoto = p.PlayerPhoto,
                    RoomEnabled = p.RoomEnabled,
                }).ToList();
            }
            else
            {
                throw new InvalidOperationException("Room not found");
            }
        }

        public async Task RoomStageUpdate(int roomId, RoomStageUpdateType stageUpdateType)
        {
            var room = _context.Rooms.Include(r => r.Stages).FirstOrDefault(r => r.Id == roomId);
            if (room == null)
            {
                throw new InvalidOperationException("Room not found");
            }

            var currentStage = room.Stages.OrderByDescending(e => e.Stage).FirstOrDefault();

            //Функция для проверки завершения всех этапов ночи
            //bool IsNightComplete(RoomStage stage)
            //{
            //    return stage.Mafia && stage.Doctor && stage.Putana && stage.Commisar_whore;
            //}

            // Если начинается ночь, и все предыдущие ночные этапы завершены, создаем новую стадию
            if (stageUpdateType == RoomStageUpdateType.StartNight)
            {
                //if (currentStage != null && IsNightComplete(currentStage))
                //{
                var newStage = new RoomStage
                {
                    Stage = currentStage.Stage + 1
                    // Инициализируем другие поля, если нужно
                };
                room.Stages.Add(newStage);
                currentStage = newStage;
                _context.SaveChanges();

                var players = GetAllPlayerStatusLiveCheck(room.Id);

                Console.WriteLine("Start Nigth RoomStagePlayer");
                foreach (var temp in players)
                {
                    var roomStage = new RoomStagePlayer()
                    {
                        PlayerId = temp,
                        RoomId = currentStage.Id,
                    };
                    _context.RoomStagePlayers.Add(roomStage);
                    _context.SaveChanges();
                }
                //}
                //else if (currentStage == null || !IsNightComplete(currentStage))
                //{
                //    throw new InvalidOperationException("Cannot start a new night until the previous night stages are completed.");
                //}
            }
            Console.WriteLine($"{stageUpdateType} stage, room: {room.Id}");
            switch (stageUpdateType)
            {
                case RoomStageUpdateType.StartNight:
                    currentStage.Nigth = true;
                    room.Status = Status.nigth;

                    foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                    {
                        await _hubContext.Clients.User(temp).SendAsync("NightTime", $"" +
                        $" It's nighttime. Roles take your actions.");
                    }
                    //await _hubContext.Clients.Group(room.RoomNumber).SendAsync("NightTime", $"" +
                    //    $" It's nighttime. Roles take your actions.");

                    break;
                case RoomStageUpdateType.NightMafia:
                    currentStage.Mafia = true;
                    var listM = GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName);
                    foreach (var temp in listM)
                    {
                        await _hubContext.Clients.User(temp).SendAsync("MafiaTurn", "It's your turn, Mafia");
                    }
                    // Implement mafia logic
                    break;
                case RoomStageUpdateType.NightDoctor:
                    currentStage.Doctor = true;
                    var listD = GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName);
                    foreach (var temp in listD)
                    {
                        await _hubContext.Clients.User(temp).SendAsync("DoctorTurn", "It's your turn, Doctor");
                    }
                    // Implement doctor logic
                    break;
                case RoomStageUpdateType.NightWhore:
                    currentStage.Putana = true;
                    var listP = GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName);
                    foreach (var temp in listP)
                    {
                        await _hubContext.Clients.User(temp).SendAsync("PutanaTurn", "It's your turn, Putana");
                    }
                    // Implement whore logic
                    break;
                case RoomStageUpdateType.CommisarWhore:
                    currentStage.Commisar_whore = true;
                    var listC = GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName);
                    foreach (var temp in listC)
                    {
                        await _hubContext.Clients.User(temp).SendAsync("CommisarTurn", "It's your turn, Commisar");
                    }
                    // Implement commisar whore logic
                    break;
                case RoomStageUpdateType.StartDay:
                    try
                    {
                        Console.WriteLine($"DayTime stage, start method!");
                        currentStage.Day = true;
                        room.Status = Status.day;

                        var userD = _context.RoomStagePlayers
                            .OrderByDescending(e => e.Mafia)
                            .Include(e => e.Room)
                            .Include(e => e.Player)
                            .Include(e => e.Player.Player)
                            .FirstOrDefault(e => e.Room.Stage == currentStage.Stage && e.Mafia && e.Room.RoomId == roomId);
                        if (userD != null)
                        {
                            var player = await _context.RoomPlayers.FirstOrDefaultAsync(e => e.Id == userD.PlayerId);
                            player.RoomEnabled = false;
                            _context.Entry(player).State = EntityState.Modified;
                            await _context.SaveChangesAsync();


                            foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                            {
                                await _hubContext.Clients.User(temp).SendAsync("KillNigth", $"{userD.Player.Player.Id}");
                            }

                            if (userD.Mafia && !userD.Doctor && !userD.Putana)
                            {
                                foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                                {
                                    await _hubContext.Clients.User(temp).SendAsync("UserKill", $"Ночью не выжил: {userD.Player.PlayerName}.");
                                }
                            }

                            if (userD.Mafia && userD.Doctor)
                            {
                                foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                                {
                                    await _hubContext.Clients.User(temp).SendAsync("UserKill", $"Ночью мафии не удалось убить никого, врач спас жертву");
                                }
                            }

                            if (userD.Mafia && userD.Putana)
                            {
                                foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                                {
                                    await _hubContext.Clients.User(temp).SendAsync("UserKill", $"Ночью мафии не удалось убить никого, путана спасла жертву");
                                }
                            }
                        }
                        foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                        {
                            await _hubContext.Clients.User(temp).SendAsync("DayTime", $"It's DayTime. Roles take your actions.");
                        }
                        Console.WriteLine($"DayTime stage, start sended!");
                        //await _hubContext.Clients.Group(room.RoomNumber).SendAsync("DayTime", $"Ночью не выжил: {userD.Player.PlayerName}. It's DayTime. Roles take your actions.");
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex);
                    }
                    break;
                default: Console.WriteLine($"default {stageUpdateType}"); break;
            }

            _context.SaveChanges();
        }

        private async Task NotifyPlayers(int roomId, string message)
        {
            await _hubContext.Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", message);
        }

        private async Task NotifyRole(int roomId, string role, string message)
        {
            await _hubContext.Clients.Group($"{roomId}_{role}").SendAsync("ReceiveMessage", message);
        }

        public void PlayerVote(int roomId, string playerId)
        {
            var room = _context.Rooms.Include(r => r.Players).FirstOrDefault(r => r.Id == roomId);
            if (room != null)
            {
                var stagePlayers = _context.RoomStagePlayers
                    .Include(e => e.Room)
                    .Include(e => e.Player)
                    .FirstOrDefault(e => e.Room.Stage == room.CurrentStageNumber && e.Player.PlayerId == playerId);

                stagePlayers.Day = true;
                stagePlayers.DayCount += 1;
                _context.Update(stagePlayers);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Room not found");
            }
        }

        public async Task<GameStatus> UpdateGameStatus(int roomId)
        {
            try
            {
                var room = _context.Rooms.Include(r => r.Players).FirstOrDefault(r => r.Id == roomId);
                var mafiaCount = room.Players.Count(p => p.RoomRole == RoomRole.Mafia && p.RoomEnabled == true);
                var civilianCount = room.Players.Count(p => p.RoomRole != RoomRole.Mafia && p.RoomEnabled == true);

                var result = new GameStatus();
                bool mafiawin = false;
                bool civilianwin = false;
                if (mafiaCount == 0)
                {
                    civilianwin = true;
                }
                else if (mafiaCount >= civilianCount)
                {
                    mafiawin = true;
                }
                var user = _context.RoomStagePlayers.OrderByDescending(e => e.DayCount)
                            .Include(e => e.Room)
                            .Include(e => e.Player)
                            .OrderByDescending(e => e.DayCount)
                            .FirstOrDefault(e => e.Room.Stage == room.CurrentStageNumber && e.DayCount != 0);
                if (user != null)
                {
                    var player = await _context.RoomPlayers.FirstOrDefaultAsync(e => e.Id == user.PlayerId);
                    player.RoomEnabled = false;
                    _context.Entry(player).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    result.PlayerId = user.Player.PlayerId;
                    result.PlayerName = user.Player.PlayerName;
                }
                result.MafiaWin = mafiawin;
                result.CivilianWin = civilianwin;


                if (result.MafiaWin)
                {
                    room.Status = Status.mafia_win;
                    room.EndDate = DateTime.Now;

                    foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                    {
                        await _hubContext.Clients.User(temp).SendAsync("GameStatus", $"Мафия выиграла");
                    }
                }
                else if (result.CivilianWin)
                {
                    room.Status = Status.citizen_win;
                    room.EndDate = DateTime.Now;

                    foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                    {
                        await _hubContext.Clients.User(temp).SendAsync("GameStatus", $"Мирные выиграли");
                    }
                }
                else
                {
                    room.Status = Status.winner_not;
                    if (user != null)
                    {
                        foreach (var temp in GetAllPlayerStatusLive(room.Id).Select(e => e.PlayerUserName))
                        {
                            await _hubContext.Clients.User(temp).SendAsync("UserKill", $"Днем жители принесли в жертву: {user.Player.PlayerName}.");
                        }
                    }

                }
                _context.SaveChanges();
                string jsonString = JsonConvert.SerializeObject(result, Formatting.Indented);
                Console.WriteLine(jsonString);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public async Task<int> UserRefresh(string userId, string roomNumber)
        {
            var user = await _context.RoomPlayers.Include(e => e.Room).FirstOrDefaultAsync(e => e.PlayerId == userId && e.Room.RoomNumber == roomNumber);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }
            // Подключаем пользователя к группе комнаты
            //await _hubContext.Clients.User(userId).SendAsync("JoinRoomGroup", roomNumber);

            foreach (var temp in GetAllPlayerStatusLive(user.RoomId).Select(e => e.PlayerUserName))
            {
                await _hubContext.Clients.User(temp).SendAsync("UserJoined", $"" +
                $" user connected {user.PlayerName}");
            }

            // Уведомляем всех игроков в комнате о новом участнике
            //await _hubContext.Clients.Group(roomNumber).SendAsync("UserJoined", user.PlayerName);

            return user.RoomId;
        }

        public async Task<bool> MafiaVote(int roomId, string playerId)
        {
            try
            {
                var vote = await _context.RoomStagePlayers.Include(e => e.Player).Include(e => e.Room).FirstOrDefaultAsync(e => e.Player.PlayerId == playerId && e.Room.RoomId == roomId);
                vote.Mafia = true;
                _context.Entry(vote).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                return true;
            }
        }

        public async Task<bool> CommisarVote(int roomId, string playerId)
        {
            try
            {
                var vote = await _context.RoomStagePlayers.Include(e => e.Player).Include(e => e.Room).FirstOrDefaultAsync(e => e.Player.PlayerId == playerId && e.Room.RoomId == roomId);
                vote.Commisar_whore = true;
                _context.Entry(vote).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                return true;
            }
        }

        public async Task<bool> PutanaVote(int roomId, string playerId)
        {
            try
            {
                var vote = await _context.RoomStagePlayers.Include(e => e.Player).Include(e => e.Room).FirstOrDefaultAsync(e => e.Player.PlayerId == playerId && e.Room.RoomId == roomId);
                vote.Putana = true;
                _context.Entry(vote).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                return true;
            }
        }

        public async Task<bool> DoctorVote(int roomId, string playerId)
        {
            try
            {
                var vote = await _context.RoomStagePlayers.Include(e => e.Player).Include(e => e.Room).FirstOrDefaultAsync(e => e.Player.PlayerId == playerId && e.Room.RoomId == roomId);
                vote.Doctor = true;
                _context.Entry(vote).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                return true;
            }
        }
    }
}
