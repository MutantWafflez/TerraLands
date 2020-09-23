using TerraLands.Items.Weapons.Guns;
using TerraLands.Items.Weapons.Guns.Legendaries;
using TerraLands.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraLands.NPCs
{
    public class TLGlobalNPC : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                TLUtils.NewTLItem(npc.getRect(), ModContent.ItemType<KemptSharell>());
            }
        }
    }
}
