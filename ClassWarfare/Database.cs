using System;
using System.Data;
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
        public static List<ArenaData> Arenas = new List<ArenaData>();
        public static List<PlayerData> Players = new List<PlayerData>();

        public static IDbConnection Connection { get; set; }

        public static void ReloadDatabase()
        {
            if (TShock.Config.StorageType.ToLower() == "sqlite")
            { 
            }
            else if (TShock.Config.StorageType.ToLower() == "mysql")
            {
            }
            else
            {
                // Yes, TShock handles this.
                TSPlayer.Server.SendErrorMessage("You've formatted your database wrong. Use \"mysql\" or \"sqlite\" instead.");
                throw new ArgumentException("Invalid storage type.");
            }
        }
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
