using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace ClassWarfare
{
    static class Database
    {
        public static List<ArenaData> Arenas { get; set; }
        public static List<PlayerData> Players { get; set; }

        public static IDbConnection Db { get; set; }

        public static void LoadDatabase()
        {
            #region Set up IDbConnection Connection
            if (TShock.Config.StorageType.ToLower() == "sqlite")
            {
                Db = new SqliteConnection(string.Format("uri=file://{0},Version=3", Path.Combine(TShock.SavePath, "Class Warfate.sqlite")));
            }
            else if (TShock.Config.StorageType.ToLower() == "mysql")
            {
                try
                {
                    var hostport = TShock.Config.MySqlHost.Split(':');
                    Db = new MySqlConnection()
                    { 
                        ConnectionString = 
                        string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                        hostport[0], hostport.Length > 1 ? hostport[1] : "3306",
                        TShock.Config.MySqlDbName, TShock.Config.MySqlUsername, TShock.Config.MySqlPassword)
                    };
                }
                catch (MySqlException ex)
                {
                    Log.Error("Class Warfare - error setting up database: "+ex.ToString());
                    throw new Exception("Database did not set up correctly.");
                }
            }
            else
            {
                // Yes, TShock handles this before we do.
                TSPlayer.Server.SendErrorMessage("You've formatted your database wrong. Use \"mysql\" or \"sqlite\" instead.");
                throw new ArgumentException("Invalid storage type.");
            }
            #endregion

            Arenas = new List<ArenaData>();
            Players = new List<PlayerData>();

            #region Create tables, etc.

            var creator = new SqlTableCreator(Db,
                Db.GetSqlType() == SqlType.Sqlite ?
                (IQueryBuilder)new SqliteQueryCreator() :
                new MysqlQueryCreator());

            #region Arenas Table

            var table = new SqlTable("Arenas",
                new SqlColumn("Name", MySqlDbType.String, 20) { Primary = true },
                new SqlColumn("MineTile ID", MySqlDbType.Int16),
                //new SqlColumn("Region", MySqlDbType.Text),

                new SqlColumn("Red Tiles Regions Names", MySqlDbType.MediumText),
                new SqlColumn("Blue Tiles Regions Names", MySqlDbType.MediumText),

                new SqlColumn("Red Spawn", MySqlDbType.TinyText),
                new SqlColumn("Blue Spawn", MySqlDbType.TinyText),

                new SqlColumn("Observation Point", MySqlDbType.TinyText),
                new SqlColumn("Wire Point", MySqlDbType.TinyText));

            #endregion

            creator.EnsureExists(table);

            // TODO Check config for whether to track player stats.

            #region Player Stats Table
            table = new SqlTable("PlayerStats",
                new SqlColumn("Account ID", MySqlDbType.Int32) { Primary = true },

                new SqlColumn("Average Kills", MySqlDbType.UInt16),
                new SqlColumn("Average Deaths", MySqlDbType.UInt16),
                new SqlColumn("Average Tiles", MySqlDbType.UInt16),

                new SqlColumn("Total PvP Games", MySqlDbType.UInt16),
                new SqlColumn("Total Miner Games", MySqlDbType.UInt16),

                new SqlColumn("Games Won", MySqlDbType.UInt16),
                new SqlColumn("Games Lost", MySqlDbType.UInt16),
                new SqlColumn("Games Drawn", MySqlDbType.UInt16));
            #endregion

            creator.EnsureExists(table);

            #endregion

            #region Copy Values into memory

            #region Arenas
            try
            {
                using (var reader = Db.QueryReader("SELECT * FROM Arenas"))
                {
                    while (reader.Read())
                    {
                        // Create memory objects from the database
                        Arenas.Add(new ArenaData()
                        {
                            Name = reader.Get<string>("Name"),
                            MineTileID = reader.Get<int>("MineTile ID"),
                            MT_Red = reader.Get<string>("Red Tiles Regions Names").Split(';').ToList().ConvertAll(n => TShock.Regions.ZacksGetRegionByName(n)).ToArray(),
                            MT_Blue = reader.Get<string>("Blue Tiles Regions Names").Split(';').ToList().ConvertAll(n => TShock.Regions.ZacksGetRegionByName(n)).ToArray(),
                            SpawnRed = new SpawnPoint(reader.Get<string>("Red Spawn")),
                            SpawnBlue = new SpawnPoint(reader.Get<string>("Blue Spawn")),
                            ObservePoint = new SpawnPoint(reader.Get<string>("Observation Point")),
                            TripWire = new SpawnPoint(reader.Get<string>("Wire Point"))
                        });
                    }
                }
            }
            catch (Exception ex) { Log.Error("Class Warfare: Error loading arenas from database: " + ex.ToString()); }
            #endregion

            #region Player Statistics
            // TODO if statement
            try
            {
                using (var reader = Db.QueryReader("SELECT * FROM PlayerStats"))
                {
                    while (reader.Read())
                    {
                        Players.Add(new PlayerData()
                        {
                            AccountID = reader.Get<int>("Account ID"),
                            AverageKills = reader.Get<UInt16>("Average Kills"),
                            AverageDeaths = reader.Get<UInt16>("Average Deaths"),
                            AverageTiles = reader.Get<UInt16>("Average Tiles"),
                            TotalPvPGames = reader.Get<UInt16>("Total PvP Games"),
                            TotalMineGames = reader.Get<UInt16>("Total Miner Games"),
                            GamesWon = reader.Get<UInt16>("Games Won"),
                            GamesLost = reader.Get<UInt16>("Games Lost"),
                            GamesDrawn = reader.Get<UInt16>("Games Drawn")
                        });
                    }
                }
            }
            catch (Exception ex) { Log.Error("Class Warfare: Error loading player stats from database: " + ex.ToString()); }
            #endregion

            #endregion
        }

        public static void SaveDatabase()
        {
            // TODO Writes things to the file.
        }
    }

    class PlayerData
    {
        public int AccountID { get; set; }

        public UInt16 AverageKills { get; set; }
        public UInt16 AverageDeaths { get; set; }

        public UInt16 AverageTiles { get; set; }
        public UInt16 TotalPvPGames { get; set; }

        public UInt16 TotalMineGames { get; set; }

        public UInt16 GamesWon { get; set; }
        public UInt16 GamesLost { get; set; }
        public UInt16 GamesDrawn { get; set; }
    }

    class ArenaData
    {
        public string Name { get; set; }
        public int MineTileID { get; set; }
        //public Region Region { get; set; }

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
