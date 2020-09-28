using TerraLands.Items.Weapons;
using Terraria.ModLoader;

namespace TerraLands
{
    public class TLPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (player.HeldItem.modItem != null)
            {
                ((TLWeapon)player.HeldItem.modItem).itemSuffix.playerUpdateMethod?.Invoke(player);
            }
        }
    }
}
