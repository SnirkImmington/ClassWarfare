using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using MySql.Data;
using TShockAPI;
using TShockAPI.DB;

namespace ClassWarfare
{
    static class Database
    {
        
    }

    class PlayerData
    {
        public string AccountName { get; set; }

        public uint AverageKills { get; set; }
        public uint AverageDeaths { get; set; }

        public uint AverageTiles { get; set; }
        public uint TotalPvPGames { get; set; }

        public uint TotalMineGames { get; set; }

        public uint GamesWon { get; set; }
        public uint GamesLost { get; set; }
        public uint GamesDrawn { get; set; }
    }

    class ArenaData
    {
        public string Name { get; set; }
        public int MineTileID { get; set; }
        public Region Region { get; set; }

        public Region[] MT_Red { get; set; }
        public Region[] MT_Blue { get; set; }

        public SpawnPoint SpawnRed { get; set; }
        public SpawnPoint SpawnBlue { get; set; }

        public SpawnPoint ObservePoint { get; set; }
        public SpawnPoint TripWire { get; set; }
    }

    class MainData
    {
        public SpawnPoint WaitingRoomPoint { get; set; }
        public SpawnPoint ChoosingRoomPoint { get; set; }
    }
}
