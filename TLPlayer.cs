using TerraLands.Enums;
using TerraLands.Items.Weapons;
using TerraLands.Utils;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerraLands {
    public class TLPlayer : ModPlayer {
        #region Stats

        internal int Level = 1;

        internal int Experience = 0;

        #endregion

        #region Netcode
        public override void clientClone(ModPlayer clientClone) {
            TLPlayer clone = (TLPlayer)clientClone;

            clone.Level = Level;
            clone.Experience = Experience;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)TLPacketType.SyncJoiningPlayer);
            packet.Write((byte)player.whoAmI);
            packet.Write(Level);
            packet.Write(Experience);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer) {
            TLPlayer clone = (TLPlayer)clientPlayer;
            if (clone.Level != Level || clone.Experience != Experience) {
                var packet = mod.GetPacket();
                packet.Write((byte)TLPacketType.SyncLevelData);
                packet.Write((byte)player.whoAmI);
                packet.Write(Level);
                packet.Write(Experience);
                packet.Send();
            }
        }
        #endregion

        #region I/O
        public override TagCompound Save() {
            return new TagCompound
            {
                {"Level", Level },
                {"Experience", Experience }
            };
        }

        public override void Load(TagCompound tag) {
            Level = tag.GetInt("Level");
            Experience = tag.GetInt("Experience");
        }
        #endregion

        #region Update Overrides
        public override void PostUpdate() {
            if (player.HeldItem.modItem != null) {
                ((TLWeapon)player.HeldItem.modItem).itemSuffix.playerUpdateMethod?.Invoke(player);
            }

            if (Experience >= TLUtils.XPRequiredToLevelUp(Level)) {
                Level++;
            }
        }
        #endregion
    }
}
