using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using TerraLands.Enums;
using TerraLands.Projectiles;
using TerraLands.UI;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraLands {
    public class TerraLands : Mod {
        internal static ILog TLLogger = LogManager.GetLogger("TerraLands");

        #region UI
        internal GameTime lastGameTime;

        internal LevelBar levelBar;
        internal UserInterface levelInterface;
        #endregion

        public override void Load() {
            #region Shaders
            if (Main.netMode != NetmodeID.Server) {
                Ref<Effect> nisRef = new Ref<Effect>(GetEffect("Effects/Items/NewItemSheen"));
                GameShaders.Misc["NewItemSheen"] = new MiscShaderData(nisRef, "NewItemSheen");

                Ref<Effect> tooltipSheenRef = new Ref<Effect>(GetEffect("Effects/Items/ItemTooltipSheen"));
                GameShaders.Misc["ItemTooltipSheen"] = new MiscShaderData(tooltipSheenRef, "ItemTooltipSheen");
            }
            #endregion

            #region Detours/IL

            #endregion

            #region UI
            if (Main.netMode != NetmodeID.Server) {
                levelBar = new LevelBar();
                levelBar.Activate();
                levelInterface = new UserInterface();
                levelInterface.SetState(levelBar);
            }
            #endregion

        }

        #region Packet Management
        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            TLPacketType packetRecieved = (TLPacketType)reader.ReadByte();
            switch (packetRecieved) {
                case TLPacketType.SyncJoiningPlayer:
                    byte playerIndex = reader.ReadByte();
                    TLPlayer modPlayer = Main.player[playerIndex].GetModPlayer<TLPlayer>();
                    modPlayer.Level = reader.ReadInt32();
                    modPlayer.Experience = reader.ReadInt32();
                    break;
                case TLPacketType.SyncLevelData:
                    playerIndex = reader.ReadByte();
                    modPlayer = Main.player[playerIndex].GetModPlayer<TLPlayer>();
                    modPlayer.Level = reader.ReadInt32();
                    modPlayer.Experience = reader.ReadInt32();
                    if (Main.netMode == NetmodeID.Server) {
                        var packet = GetPacket();
                        packet.Write((byte)TLPacketType.SyncLevelData);
                        packet.Write(playerIndex);
                        packet.Write(modPlayer.Level);
                        packet.Write(modPlayer.Experience);
                        packet.Send(-1, playerIndex);
                    }
                    break;
                case TLPacketType.SyncProjectileExplosion:
                    if (Main.netMode == NetmodeID.Server) {
                        int projectileIndex = reader.ReadInt32();
                        ModPacket packet = GetPacket();
                        packet.Write((byte)TLPacketType.SyncProjectileExplosion);
                        packet.Write(projectileIndex);
                        packet.Send(ignoreClient: whoAmI);
                    }
                    else {
                        int projectileIndex = reader.ReadInt32();
                        Projectile projectileAtIndex = Main.projectile[projectileIndex];
                        (projectileAtIndex.modProjectile as TLProjectile).AddExplosiveFlair(projectileAtIndex, true);
                    }
                    break;
                default:
                    TLLogger.WarnFormat($"Unrecognized packet recieved: {packetRecieved}");
                    break;
            }
        }
        #endregion

        #region UI Hooks
        public override void UpdateUI(GameTime gameTime) {
            lastGameTime = gameTime;
            levelInterface?.Update(gameTime);

        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (mouseTextIndex != -1) {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    $"{nameof(TerraLands)}: Player Level",
                    delegate {
                        if (lastGameTime != null) {
                            levelInterface.Draw(Main.spriteBatch, lastGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        #endregion
    }
}