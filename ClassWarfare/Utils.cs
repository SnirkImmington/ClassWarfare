using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;

namespace ClassWarfare
{
    static class Utils
    {
        public static void TellPlayersAndObservers(CWGame game, string message, Color color, params object[] format)
        {
            message = "CW @ " + game.Arena.Name + ": " + string.Format(message, format);

            foreach (var ply in game.Players)
                TShock.Players[ply].SendMessage(message, color);

            foreach (var ply in game.Observers)
                TShock.Players[ply].SendMessage(message, color);
        }
    }
}
