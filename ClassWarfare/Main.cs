using System;
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
    public class Main : TerrariaPlugin
    {
        #region Properties

        public static List<CWGame> RunningGames { get; set; }

        public static CWPlayer[] Players { get; set; }

        #endregion

        #region Initialize

        public override void Initialize()
        {
            // TODO Load configuration

            NetHooks.GreetPlayer += OnGreet;
            GetDataHandlers.ItemDrop += OnItem;
            GameHooks.Initialize += OnInit;
            ServerHooks.Leave += OnLeave;

            Commands.ChatCommands.Add(new Command("cw", Com, "cw", "classwarfare"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NetHooks.GreetPlayer -= OnGreet;
                GetDataHandlers.ItemDrop -= OnItem;
                GameHooks.Initialize -= OnInit;
                ServerHooks.Leave -= OnLeave;

                Commands.ChatCommands.Add(new Command("cw", Com, "cw", "classwarfare"));
            }
            base.Dispose(disposing);
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
            */
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
