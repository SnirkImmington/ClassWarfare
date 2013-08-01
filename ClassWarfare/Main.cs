using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using Hooks;

namespace ClassWarfare
{
    [APIVersion(1,12)]
    public class PluginMain : TerrariaPlugin
    {
        #region Properties

        private static List<CWGame> RunningGames;
        public static CWPlayer[] Players { get; set; }
        private static List<QuitterData> Quitters;

        #region Arena Setting

        // Spawnpoints
        private static Point ArenaRedSpawn  = Point.Zero;
        private static Point ArenaBlueSpawn = Point.Zero;

        // Arena observation point
        private static Point ArenaObserve = Point.Zero;

        // Names of minetile regions
        private static List<string> ArenaRedRegions  = new List<string>();
        private static List<string> ArenaBlueRegions = new List<string>();

        // Arena MineTileID
        private static int ArenaMineTile = 0;

        // Arena Tripwire point
        private static Point ArenaTripWire = Point.Zero;

        #endregion

        #region Overrides

        public override string Name { get { return "Class Warfare Plugin"; } }
        public override string Author { get { return "Snirk Immington"; } }
        public override string Description { get { return "As of TShock 4.1, \"description\" token ain't used."; } }
        public override Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        #endregion

        #endregion

        #region Initialize

        public PluginMain(Main game) : base(game)
        {
            Order = 1;
            Players = new CWPlayer[255];
            RunningGames = new List<CWGame>();
            Quitters = new List<QuitterData>();
        }

        public override void Initialize()
        {
            // TODO Load configuration

            NetHooks.GreetPlayer += OnGreet;
            GetDataHandlers.ItemDrop += OnItem;
            GetDataHandlers.ChestOpen += OnChest;
            GameHooks.Initialize += OnInit;
            ServerHooks.Leave += OnLeave;

            AppDomain.CurrentDomain.UnhandledException += OnFailure;

            Commands.ChatCommands.Add(new Command("cw", Com, "cw", "classwarfare"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NetHooks.GreetPlayer -= OnGreet;
                GetDataHandlers.ItemDrop -= OnItem;
                GetDataHandlers.ChestOpen -= OnChest;
                GameHooks.Initialize -= OnInit;
                ServerHooks.Leave -= OnLeave;

                AppDomain.CurrentDomain.UnhandledException -= OnFailure;

                Database.SaveDatabase();
            }
            base.Dispose(disposing);
        }

        private static void OnFailure(object weeeee, UnhandledExceptionEventArgs e)
        {
            Database.SaveDatabase();
        }

        private static void OnInit()
        {
            // TODO Set up the database
        }

        #endregion

        #region Command

        private static void Com(CommandArgs com)
        {
            /* 
             * /cw host <arena name> - cw,cwhost - hosts a game to that arena - anytime
             * /cw join <arena name|player name> - cw - joins a game of cw -    if one is available
             * /cw observe <arena name|ply name> - cw - observes a game of cw - if one is in session
             * 
             * /cw stats <player> - cw - gets the stats of a player - anytime/if game more
             * 
             * /cw chooseclass <class name> - cw - chooses a class for the game - if game ready
             * /cw classinfo <class name> - cw - info on a class (make it awesome)
             * /cw classlist [type] - cw - classes of a type - 
             * 
             * /cw newclass <name> <difficulty> <type> <description> - cwadmin
             * /cw modclass <type> [new data] - cwadmin - support chest thing
             * 
             * /cw arenaset [type] - cwadmin
             * /cw arenadef <name> - cwadmin
            */
            bool isAdmin = com.Player.Group.HasPermission("cwadmin");
            var player = Players[com.Player.Index];
            if (com.Parameters.Count == 0 || com.Parameters[0].ToLower() == "help")
            {
                com.Player.SendInfoMessage("This is the global command for Class Warfare. Here is some of what you can do right now:");
                if (isAdmin) com.Player.SendInfoMessage("For administrative commands, use /cw adminhelp.");
                switch (player.Level)
                {
                    case PlayerLevel.None:
                        if (com.Player.Group.HasPermission("cwhost"))
                            com.Player.SendInfoMessage("/cw host <arena name> - host a game of Class Warfare at that arena (/cw arenalist).");
                        if (RunningGames.Count > 0)
                            com.Player.SendInfoMessage("/cw join <arena name|host name> - joins a game of Class Warfare (/cw listgames)!");
                            com.Player.SendInfoMessage("The /cw rules and class information commands are available anytime!");
                        break;

                    case PlayerLevel.ChoosingClass:
                    case PlayerLevel.Joined:
                        com.Player.SendInfoMessage("/cw classlist [type] | /cw classinfo|classitems <name> - learn about classes!");
                        break;

                    case PlayerLevel.ChosenClass:
                    case PlayerLevel.PlayingGame:
                        com.Player.SendInfoMessage("/cw stats <player> - see how much you're losing by!");
                        break;
                }
            }

            else switch (com.Parameters[0].ToLower())
                {
                    case "adminhelp":
                        break;

                    case "rules":
                        break;

                    case "host":
                        break;

                    case "join":
                        break;

                    case "chooseclass":
                        break;

                    case "classinfo":
                        break;

                    case "classitems":
                        break;

                    case "newclass":
                        break;

                    case "modclass":
                        break;
                }
        }

        #endregion

        #region Hooks

        private static void OnGreet(int who, HandledEventArgs args)
        {
            // TODO get data
        }

        private static void OnLeave(int who)
        {
            // TODO save data
            // TODO Resolve host

            if (Players[who].Level != PlayerLevel.None)
            {
                
            }
        }

        private static void OnTile(object thisAintUsed, GetDataHandlers.TileEditEventArgs args)
        {
            if (Players[args.Player.Index].GonnaMakeAClass == true)
            {
                // Find the chest there
                var ply = Players[args.Player.Index];

                for (int i = 0; i < Main.chest.Length; i++)
                {
                    if (Main.chest[i].x == args.X && Main.chest[i].y == args.Y)
                    {
                        // Chest found, create the CWClass.
                        // Checks for the params are in the command.

                        args.Handled = true;

                        var Class = new CWClass(ply.ClassName, Main.chest[i].item, ply.ClassType, ply.ClassDiff, ply.ClassMob, ply.ClassBlurb);

                        args.Player.SendSuccessMessage(string.Format("Created {0} class {1}: {2}", Class.Type, Class.Name, Class.Info));

                        
                    }
                }
            }
        }

        #endregion

        #region UpdateThread

        private static void UpdateThread()
        {
            while (true)
            {
                // Announcements
                if (RunningGames.Count == 0)
                {
                    
                }
                else // There are games running. 
                    foreach (var game in RunningGames)
                    {
                        #region Switch Game Level
                        switch (game.Level)
                        {
                            case GameLevel.Announced:
                                // Broadcast information to the players who can receive it.
                                break;

                            case GameLevel.ChoosingClasses:
                                // Tell the players registered to hurry up with class choosing already.
                                // also have class countdown timers, for second delays between info.
                                break;
                                
                            case GameLevel.InProgress: // Check for cheaters, etc.

                                #region Check cheaters/respawn/reduce counts

                                foreach (var num in game.Players)
                                {
                                    var ply = Players[num];
                                    var tsply = TShock.Players[num];

                                    // Check inventory
                                        // kick

                                    // Respawn? Heal?
                                }

                                #endregion

                                #region Check counts

                                #endregion

                                #region Check progress

                                #endregion

                                break;

                            case GameLevel.GameOver: // Clean up and award points

                                #region Game Clean Up

                                foreach (var num in game.Players)
                                {
                                    var ply = Players[num];
                                    var tsply = Players[num];

                                    #region Apply Points
                                    if (ply.Class.Name == "Miner")
                                    {
                                    }
                                    else // player is not a miner
                                    {
                                        
                                    }
                                    #endregion

                                    #region 
                                    #endregion
                                }

                                #endregion

                                #region Award Points
                                #endregion

                                break;
                        }
                        #endregion
                    }

                Thread.Sleep(1000);
            }
        }

        #endregion
    }
}
