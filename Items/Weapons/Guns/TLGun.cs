using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using TerraLands.Projectiles;
using TerraLands.Projectiles.UniqueProj;
using TerraLands.TLPrefixes;
using TerraLands.Utils;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraLands.Items.Weapons.Guns
{
    public abstract class TLGun : TLWeapon
    {
        public override void AutoDefaults()
        {
            base.AutoDefaults();
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.ranged = true;
            item.noMelee = true;
        }

        public override void GetAvailablePrefixes()
        {
            List<TLPrefix> returnedPrefixes = TLPrefixList.GetListOfPrefixesWithType(PrefixType.Ranged);
            foreach (TLPrefix prefix in returnedPrefixes)
            {
                availablePrefixes.Add(prefix, prefix.weight);
            }
        }
    }
}
