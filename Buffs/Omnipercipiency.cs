using Terraria;
using Terraria.ModLoader;

namespace TerraLands.Buffs {
    public class Omnipercipiency : ModBuff {
        public override void SetDefaults() {
            DisplayName.SetDefault("Omnipercipiency");
            Description.SetDefault("I... see... EVERYTHING!!!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.dangerSense = true;
            player.findTreasure = true;
            player.nightVision = true;
        }
    }
}
