using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace ClassWarfare
{
    public class Main
    {
        #region properties

        public static List<CWGame> RunningGames { get; set; }

        public static CWPlayer[] Players { get; set; }

        #endregion

        #region Initialize
        #endregion

        #region Command
        #endregion

        #region Hooks
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
