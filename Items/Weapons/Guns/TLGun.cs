using System.Collections.Generic;
using TerraLands.Enums;
using TerraLands.TLPrefixes;
using Terraria.ID;

namespace TerraLands.Items.Weapons.Guns {
    public abstract class TLGun : TLWeapon {
        public override void AutoDefaults() {
            base.AutoDefaults();
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.ranged = true;
            item.noMelee = true;
        }

        public override void GetAvailablePrefixes() {
            List<TLPrefix> returnedPrefixes = TLPrefixList.GetListOfPrefixesWithType(PrefixType.Ranged);
            foreach (TLPrefix prefix in returnedPrefixes) {
                availablePrefixes.Add(prefix, prefix.weight);
            }
        }
    }
}
