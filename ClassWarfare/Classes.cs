using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace ClassWarfare
{
    /// <summary>
    /// Contains data for player classes.
    /// </summary>
    class CWClass
    {
        public string Name { get; set; }

        public DBItem[] Items { get; set; }

        public string Type { get; set; }
    }

    /// <summary>
    /// Item storage throughout the plugin. More efficient than Redigit's items.
    /// </summary>
    class DBItem
    {
        public int ID { get; set; }
        public int Stack { get; set; }
        public byte Prefix { get; set; }

        public DBItem(int iD, int stack, byte prefix)
        {
            ID = iD; Stack = stack; Prefix = prefix;
        }

        public DBItem(Item it)
        {
            ID = it.netID; Stack = it.stack; Prefix = it.prefix;
        }
    }

    /// <summary>
    /// Stores Terrarian point-based data.
    /// </summary>
    class SpawnPoint
    {
        /// <summary>
        /// The X coordinate of the point.
        /// </summary>
        public int TileX { get; set; }
        /// <summary>
        /// The Y coordinate of the point.
        /// </summary>
        public int TileY { get; set; }

        /// <summary>
        /// To be used to make sure the creator from the DB didn't fail.
        /// </summary>
        public bool isNull { get { return TileY != TileX && TileX != -1; } }

        /// <summary>
        /// The string for writing a SpawnPoint to the database.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return TileX.ToString() + ',' + TileY.ToString();
        }

        /// <summary>
        /// Creates a SpawnPoint from 2 integers.
        /// </summary>
        /// <param name="X">X coordinate</param>
        /// <param name="Y">Y coordinate</param>
        public SpawnPoint(int X, int Y)
        {
            TileX = X; TileY = Y;
        }

        /// <summary>
        /// Creates a SpawnPoint from a Vector2
        /// </summary>
        /// <param name="Vector">The Vector2 holding X and Y.</param>
        public SpawnPoint(Vector2 Vector)
        {
            TileX = (int)Vector.X;
            TileY = (int)Vector.Y;
        }

        /// <summary>
        /// Creates a SpawnPoint from a Warp.
        /// </summary>
        /// <param name="Warp">The Warp whose coordinates are used.</param>
        public SpawnPoint(TShockAPI.DB.Warp Warp)
        {
            TileX = (int)Warp.WarpPos.X;
            TileY = (int)Warp.WarpPos.Y;
        }

        /// <summary>
        /// The constructor from the database.
        /// </summary>
        /// <param name="dbString">String containing the X and Y values.</param>
        public SpawnPoint(string dbString)
        {
            var subs = dbString.Split(',');
            int x = 0, y = 0;
            if (int.TryParse(subs[0], out x) && int.TryParse(subs[1], out y))
            {
                TileX = x; TileY = y;
            }
            else TileY = TileX = -1;
        }

        /// <summary>
        /// Creates a spawnpoint from a player's left shoe!
        /// </summary>
        /// <param name="ply"></param>
        public SpawnPoint(TSPlayer ply)
        {
            TileX = ply.TileX; TileY = ply.TileY;
        }
    }

    /// <summary>
    /// Contains data for each player.
    /// </summary>
    class CWPlayer
    {
        // PvP data
        public bool? ForcePvP { get; set; }
        public byte? ForceTeam { get; set; }

        // Game Data
        public byte GameNumber { get; set; }
        public PlayerLevel Level { get; set; }

        // Local Scores
        public uint GameKills { get; set; }
        public uint GameDeaths { get; set; }
        public uint GameMines { get; set; }

        // Game Scores
        public PlayerData Statistics { get; set; }
    }

    /// <summary>
    /// Game Progress Data
    /// </summary>
    enum PlayerLevel : byte
    {
        /// <summary>
        /// Player is not in CW/no games available.
        /// </summary>
        None = 0,
        /// <summary>
        /// Used /cw join. Waiting for tp to class choose.
        /// </summary>
        Joined,
        /// <summary>
        /// CW game is set, team pick mode, player choosing CW class.
        /// </summary>
        ChoosingClass,
        /// <summary>
        /// Player chosen CW class, waiting for tp to arena.
        /// </summary>
        ChosenClass,
        /// <summary>
        /// Is in game.
        /// </summary>
        PlayingGame,
    }

    enum GameLevel : byte
    {
    }

    class CWGame
    {
        public byte Host { get; set; }
        // Unique Identifier

        public List<byte> Players { get; set; }
        public List<byte> Observers { get; set; }

        public ArenaData Arena { get; set; }
    }
}
