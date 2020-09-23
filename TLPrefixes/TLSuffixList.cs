using TerraLands.Utils;
using Terraria;
using System.Linq;
using static TerraLands.TLPrefixes.TLSuffix;
using Terraria.ID;

namespace TerraLands.TLPrefixes
{
    public static class TLSuffixList
    {
        public static readonly TLSuffix Default = new TLSuffix(
            suffixID: -1,
            "Defaulcy",
            suffixRarity: SuffixRarity.Common,
            delegate(Player player)
            {
                player.AddBuff(BuffID.Ironskin, 60);
            }
            );
    }
}
