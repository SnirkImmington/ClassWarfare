using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace ClassWarfare
{
    static class Utils
    {
        public static void TellPlayersAndObservers(CWGame game, string message, Color color)
        {
            message = "CW @" + game.Arena.Name + ": " + message;

            foreach (var ply in game.Players)
                TShock.Players[ply].SendMessage(message, color);

            foreach (var ply in game.Observers)
                TShock.Players[ply].SendMessage(message, color);
        }
        public static void TellPlayersAndObservers(CWGame game, string message, Color color, params object[] format)
        {
            TellPlayersAndObservers(game, string.Format(message, format), color);
        }

        public static Item FromDB(DBItem item)
        {
            var ytem = TShock.Utils.GetItemById(item.ID);
            ytem.Prefix(item.Prefix); ytem.stack = item.Stack;
            return ytem;
        }

        internal static string WeaponSpeed(int animation)
        {
            if (animation <= 8)
                return "Insanely fast"; // 6

            if (animation <= 20)
                return "Very fast"; // 7

            if (animation <= 25)
                return "Fast speed"; // 8

            if (animation <= 30)
                return "Average speed"; // 9

            if (animation <= 35)
                return "Slow speed"; // 10

            if (animation <= 45)
                return "Very slow"; // 11

            if (animation <= 55)
                return "Extremely slow"; // 12

            return "Snail speed"; // 13
        }

        public static string[] ClassInformation(CWClass klass)
        {
            #region Save stats
            var turn = new List<string>();
            int health = 100, defense = 0, mana = 0;
            var weapons = new List<Item>();
            #endregion

            #region Loop n add
            for (int i = 0; i < klass.Items.Length; i++)
            {
                var item = FromDB(klass.Items[i]);

                if (item.defense > 0)
                    defense += item.defense;

                else if (item.netID == 29) health += 20;
                else if (item.netID == 109) mana += 20;

                else if (item.damage > 0)
                    weapons.Add(item);
            }
            #endregion

            #region Create

            turn.Add(string.Format("{0} | {1} | {2} HP |{3}{4} defense",
                klass.Name, klass.Difficulty, health, (mana == 0 ? "" : mana + " mana | "), defense));

            for (int i = 0; i < 4; i++)
            {
                turn.Add(string.Format("{0} | {1} damage | {2}", weapons[i].AffixName(), weapons[i].damage, WeaponSpeed(weapons[i].useAnimation)));
            }

            turn.Add(string.Format("{0} mobility | {1}", klass.Mobility, klass.Info);

            #endregion

            return turn.ToArray();
        }
    }
}
