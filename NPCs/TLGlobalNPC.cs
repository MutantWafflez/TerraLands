using TerraLands.Items.Weapons.Guns.Legendaries;
using TerraLands.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraLands.NPCs
{
    public class TLGlobalNPC : GlobalNPC
    {
        public ElementType elementHitBy = ElementType.Default;

        public override bool InstancePerEntity => true;

        public override void NPCLoot(NPC npc)
        {
            if (npc.type == NPCID.BlueSlime)
            {
                TLUtils.NewTLItem(npc.getRect(), ModContent.ItemType<KemptSharell>());
            }
        }

        public override void PostAI(NPC npc)
        {
            elementHitBy = ElementType.Default;
        }
    }
}
