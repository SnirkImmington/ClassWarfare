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
        public static List<CWClass> Classes { get; set; }

        private static IDbConnection Db { get; set; }

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
            Classes = new List<CWClass>();

            #region Create tables, etc.

            var creator = new SqlTableCreator(Db,
                Db.GetSqlType() == SqlType.Sqlite ?
                (IQueryBuilder)new SqliteQueryCreator() :
                new MysqlQueryCreator());

            #region Arenas Table

            var table = new SqlTable("Arenas",
                new SqlColumn("Name", MySqlDbType.String, 20) { Primary = true },
                new SqlColumn("MineTileID", MySqlDbType.Int16),
                //new SqlColumn("Region", MySqlDbType.Text),

                new SqlColumn("RedTilesRegionsNames", MySqlDbType.MediumText),
                new SqlColumn("BlueTilesRegionsNames", MySqlDbType.MediumText),

                new SqlColumn("RedSpawn", MySqlDbType.TinyText),
                new SqlColumn("BlueSpawn", MySqlDbType.TinyText),

                new SqlColumn("ObservationPoint", MySqlDbType.TinyText),
                new SqlColumn("WirePoint", MySqlDbType.TinyText));

            #endregion

            creator.EnsureExists(table);

            // TODO Check config for whether to track player stats.

            #region Player Stats Table
            table = new SqlTable("PlayerStats",
                new SqlColumn("AccountID", MySqlDbType.Int32) { Primary = true },

                new SqlColumn("AverageKills", MySqlDbType.UInt16),
                new SqlColumn("AverageDeaths", MySqlDbType.UInt16),
                new SqlColumn("AverageTiles", MySqlDbType.UInt16),

                new SqlColumn("TotalPvPGames", MySqlDbType.UInt16),
                new SqlColumn("TotalMinerGames", MySqlDbType.UInt16),

                new SqlColumn("GamesWon", MySqlDbType.UInt16),
                new SqlColumn("GamesLost", MySqlDbType.UInt16),
                new SqlColumn("GamesDrawn", MySqlDbType.UInt16));
            #endregion

            creator.EnsureExists(table);

            #region Classes Table
            table = new SqlTable("CWClasses",
                new SqlColumn("Name", MySqlDbType.String, 20) { Primary = true },
                new SqlColumn("Type", MySqlDbType.String, 10),
                new SqlColumn("Difficulty", MySqlDbType.String, 10),
                new SqlColumn("Information", MySqlDbType.Text, 50));
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
                            WorldID = reader.Get<int>("WorldID"),
                            MineTileID = reader.Get<int>("MineTileID"),
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
                            AccountID = reader.Get<int>("AccountID"),
                            AverageKills = reader.Get<UInt16>("AverageKills"),
                            AverageDeaths = reader.Get<UInt16>("AverageDeaths"),
                            AverageTiles = reader.Get<UInt16>("AverageTiles"),
                            TotalPvPGames = reader.Get<UInt16>("TotalPvPGames"),
                            TotalMineGames = reader.Get<UInt16>("TotalMinerGames"),
                            GamesWon = reader.Get<UInt16>("GamesWon"),
                            GamesLost = reader.Get<UInt16>("GamesLost"),
                            GamesDrawn = reader.Get<UInt16>("GamesDrawn")
                        });
                    }
                }
            }
            catch (Exception ex) { Log.Error("Class Warfare: Error loading player stats from database: " + ex.ToString()); }
            #endregion

            #region Classes
            try
            {
                using (var reader = Db.QueryReader("SELECT * FROM CWAreanas"))
                {
                    Classes.Add(new CWClass()
                    {
                        Name = reader.Get<string>("Name"),
                        Difficulty = reader.Get<string>("Difficulty"),
                        Info = reader.Get<string>("Info"),
                        Type = reader.Get<string>("Type"),
                        Items = reader.Get<string>("Items").Split(';').ToList().ConvertAll(t => new DBItem(t)).ToArray()
                    });

                }
            }
            catch (Exception ex) { Log.Error("Class Warfare: Error loading class data from database :" + ex.ToString()); }
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
        public int WorldID { get; set; }

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
