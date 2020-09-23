using TerraLands.Utils;
using Terraria;

namespace TerraLands.TLPrefixes
{
    public class TLSuffix
    {
        public readonly int suffixID;

        public readonly string suffixText;

        public readonly SuffixRarity suffixRarity;

        public delegate void PlayerUpdateDelegate(Player player);

        /// <summary>
        /// Method that can be called at the end of Player.Update() if the Suffix has some kind of unique ability,
        /// such as giving a buff.
        /// </summary>
        public PlayerUpdateDelegate playerUpdateMethod;

        public TLSuffix(int suffixID, string suffixText, SuffixRarity suffixRarity, PlayerUpdateDelegate playerUpdateMethod = null)
        {
            this.suffixID = suffixID;
            this.suffixText = suffixText;
            this.suffixRarity = suffixRarity;
            this.playerUpdateMethod = playerUpdateMethod;
        }
    }
}
