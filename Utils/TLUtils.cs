using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace TerraLands.Utils
{
    public static class TLUtils
    {
        /// <summary>
        /// Turns a item.rare into its respective TerraLands rarity color. Returns Color.White by default.
        /// </summary>
        public static Color ItemRareToTLRareColor(int itemRare)
        {
            if (itemRare == ItemRarityID.Green)
                return Colors.RarityGreen;
            else if (itemRare == ItemRarityID.Blue)
                return Colors.RarityBlue;
            else if (itemRare == ItemRarityID.Purple)
                return new Color(179, 34, 253); //Copy of the colour used for the DARK purple items. Colors.RarityPurple isn't quite dark enough.
            else if (itemRare == ItemRarityID.Quest)
                return Colors.RarityAmber;
            else if (itemRare == ItemRarityID.Pink)
                return Colors.RarityPink;
            else if (itemRare == ItemRarityID.Cyan)
                return Colors.RarityCyan;
            else if (itemRare == ItemRarityID.Expert)
                return Main.DiscoColor;
            else
                return Color.White;
        }

        /// <summary>
        /// Turns a item.rare into its respective TerraLands rarity name. Returns "Common" by default.
        /// </summary>
        public static string ItemRareToTLRareName(int itemRare)
        {
            if (itemRare == ItemRarityID.Green)
                return "Uncommon";
            else if (itemRare == ItemRarityID.Blue)
                return "Rare";
            else if (itemRare == ItemRarityID.Purple)
                return "Epic";
            else if (itemRare == ItemRarityID.Quest)
                return "Legendary";
            else if (itemRare == ItemRarityID.Pink)
                return "Seraph";
            else if (itemRare == ItemRarityID.Cyan)
                return "Pearlescent";
            else if (itemRare == ItemRarityID.Expert)
                return "Effervescent";
            else
                return "Common";
        }

        /// <summary>
        /// Converts a given ElementType to its Color counterpart. If an Default or None is passed, Color.White is returned.
        /// </summary>
        public static Color ItemElementToColor(ElementType element)
        {
            if (element == ElementType.Explosive)
                return Colors.RarityYellow;
            else if (element == ElementType.Fire)
                return Colors.RarityOrange;
            else if (element == ElementType.Shock)
                return Colors.RarityBlue;
            else if (element == ElementType.Corrosive)
                return Colors.RarityGreen;
            else if (element == ElementType.Cryo)
                return Colors.RarityCyan;
            else if (element == ElementType.Radiated)
                return Colors.RarityLime;
            return Color.White;
        }

        /// <summary>
        /// Little helper method to make sure TerraLands weapons spawn and get their random prefix (Since Item.NewItem doesn't have PrefixGiven set to -1)
        /// </summary>
        public static int NewTLItem(Rectangle spawnRectangle, int Type, int Stack = 1, bool noBroadcast = false, int prefixGiven = -1, bool noGrabDelay = false, bool reverseLookup = false)
        {
            return Item.NewItem(spawnRectangle, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
        }

        /// <summary>
        /// Method that returns the threshold needed to be passed to level up.
        /// Up until level 30, uses the equation 45x^2.75 - 45.
        /// For level 30 and beyond, uses the eqatuion 45x^2.75 - 45 - 45x^1.67.
        /// </summary>
        /// <param name="currentLevel"></param>
        public static int XPRequiredToLevelUp(int currentLevel)
        {
            return (int)(currentLevel < 30 ? (45f * Math.Pow(currentLevel + 1f, 2.75f)) - 45f : (45f * Math.Pow(currentLevel + 1f, 2.75f)) - 45f - (45f * Math.Pow(currentLevel + 1f, 1.67f)));
        }
    }
}
