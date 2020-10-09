using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TerraLands.Enums;
using TerraLands.NPCs;
using TerraLands.UI;
using TerraLands.Utils;
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
            }
            #endregion

            #region Detours
            On.Terraria.NPC.StrikeNPC += NPC_StrikeNPC;
            #endregion

            #region UI
            levelBar = new LevelBar();
            levelBar.Activate();
            levelInterface = new UserInterface();
            levelInterface.SetState(levelBar);
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

        private double NPC_StrikeNPC(On.Terraria.NPC.orig_StrikeNPC orig, NPC self, int Damage, float knockBack, int hitDirection, bool crit, bool noEffect, bool fromNet) {
            bool playerInteracted = Main.netMode == NetmodeID.SinglePlayer;
            FieldInfo playerInteractionsField = typeof(NPC).GetField("ignorePlayerInteractions", BindingFlags.NonPublic | BindingFlags.Static);
            if (playerInteracted && (int)playerInteractionsField.GetValue(null) > 0) {
                playerInteractionsField.SetValue(null, (int)playerInteractionsField.GetValue(null) - 1);
                playerInteracted = false;
            }
            if (!self.active || self.life <= 0) {
                return 0.0;
            }
            double damagePlaceholder = Damage;
            int defensePlaceholder = self.defense;
            if (self.ichor) {
                defensePlaceholder -= 20;
            }
            if (self.betsysCurse) {
                defensePlaceholder -= 40;
            }
            if (defensePlaceholder < 0) {
                defensePlaceholder = 0;
            }
            if (NPCLoader.StrikeNPC(self, ref damagePlaceholder, defensePlaceholder, ref knockBack, hitDirection, ref crit)) {
                damagePlaceholder = Main.CalculateDamage((int)damagePlaceholder, defensePlaceholder);
                if (crit) {
                    damagePlaceholder *= 2.0;
                }
                if (self.takenDamageMultiplier > 1f) {
                    damagePlaceholder *= self.takenDamageMultiplier;
                }
            }
            if ((self.takenDamageMultiplier > 1f || Damage != 9999) && self.lifeMax > 1) {
                if (self.friendly) {
                    //Color color = crit ? CombatText.DamagedFriendlyCrit : CombatText.DamagedFriendly;
                    Color color = TLUtils.ItemElementToColor(self.GetGlobalNPC<TLGlobalNPC>().elementHitBy);
                    CombatText.NewText(new Rectangle((int)self.position.X, (int)self.position.Y, self.width, self.height), color, (int)damagePlaceholder, crit, false);
                }
                else {
                    /*Color color2 = crit ? CombatText.DamagedHostileCrit : CombatText.DamagedHostile;
					if (fromNet)
					{
						color2 = (crit ? CombatText.OthersDamagedHostileCrit : CombatText.OthersDamagedHostile);
					}*/
                    Color color2 = TLUtils.ItemElementToColor(self.GetGlobalNPC<TLGlobalNPC>().elementHitBy);
                    CombatText.NewText(new Rectangle((int)self.position.X, (int)self.position.Y, self.width, self.height), color2, (int)damagePlaceholder, crit, false);
                }
            }
            if (damagePlaceholder >= 1.0) {
                if (playerInteracted) {
                    self.PlayerInteraction(Main.myPlayer);
                }
                self.justHit = true;
                if (self.townNPC) {
                    if (self.aiStyle == 7 && (self.ai[0] == 3f || self.ai[0] == 4f || self.ai[0] == 16f || self.ai[0] == 17f)) {
                        NPC chosenNPC = Main.npc[(int)self.ai[2]];
                        if (chosenNPC.active) {
                            chosenNPC.ai[0] = 1f;
                            chosenNPC.ai[1] = 300 + Main.rand.Next(300);
                            chosenNPC.ai[2] = 0f;
                            chosenNPC.localAI[3] = 0f;
                            chosenNPC.direction = hitDirection;
                            chosenNPC.netUpdate = true;
                        }
                    }
                    self.ai[0] = 1f;
                    self.ai[1] = 300 + Main.rand.Next(300);
                    self.ai[2] = 0f;
                    self.localAI[3] = 0f;
                    self.direction = hitDirection;
                    self.netUpdate = true;
                }
                if (self.aiStyle == 8 && Main.netMode != NetmodeID.MultiplayerClient) {
                    if (self.type == NPCID.RuneWizard) {
                        self.ai[0] = 450f;
                    }
                    else if (self.type == NPCID.Necromancer || self.type == NPCID.NecromancerArmored) {
                        if (Main.rand.Next(2) == 0) {
                            self.ai[0] = 390f;
                            self.netUpdate = true;
                        }
                    }
                    else if (self.type == NPCID.DesertDjinn) {
                        if (Main.rand.Next(3) != 0) {
                            self.ai[0] = 181f;
                            self.netUpdate = true;
                        }
                    }
                    else {
                        self.ai[0] = 400f;
                    }
                    self.TargetClosest(true);
                }
                if (self.aiStyle == 97 && Main.netMode != NetmodeID.MultiplayerClient) {
                    self.localAI[1] = 1f;
                    self.TargetClosest(true);
                }
                if (self.type == NPCID.DetonatingBubble) {
                    damagePlaceholder = 0.0;
                    self.ai[0] = 1f;
                    self.ai[1] = 4f;
                    self.dontTakeDamage = true;
                }
                if (self.type == NPCID.SantaNK1 && self.life >= self.lifeMax * 0.5 && self.life - damagePlaceholder < self.lifeMax * 0.5) {
                    Gore.NewGore(self.position, self.velocity, 517, 1f);
                }
                if (self.type == NPCID.SpikedIceSlime) {
                    self.localAI[0] = 60f;
                }
                if (self.type == NPCID.SlimeSpiked) {
                    self.localAI[0] = 60f;
                }
                if (self.type == NPCID.SnowFlinx) {
                    self.localAI[0] = 1f;
                }
                if (!self.immortal) {
                    if (self.realLife >= 0) {
                        Main.npc[self.realLife].life -= (int)damagePlaceholder;
                        self.life = Main.npc[self.realLife].life;
                        self.lifeMax = Main.npc[self.realLife].lifeMax;
                    }
                    else {
                        self.life -= (int)damagePlaceholder;
                    }
                }
                if (knockBack > 0f && self.knockBackResist > 0f) {
                    float num3 = knockBack * self.knockBackResist;
                    if (num3 > 8f) {
                        float num4 = num3 - 8f;
                        num4 *= 0.9f;
                        num3 = 8f + num4;
                    }
                    if (num3 > 10f) {
                        float num5 = num3 - 10f;
                        num5 *= 0.8f;
                        num3 = 10f + num5;
                    }
                    if (num3 > 12f) {
                        float num6 = num3 - 12f;
                        num6 *= 0.7f;
                        num3 = 12f + num6;
                    }
                    if (num3 > 14f) {
                        float num7 = num3 - 14f;
                        num7 *= 0.6f;
                        num3 = 14f + num7;
                    }
                    if (num3 > 16f) {
                        num3 = 16f;
                    }
                    if (crit) {
                        num3 *= 1.4f;
                    }
                    int num8 = (int)damagePlaceholder * 10;
                    if (Main.expertMode) {
                        num8 = (int)damagePlaceholder * 15;
                    }
                    if (num8 > self.lifeMax) {
                        if (hitDirection < 0 && self.velocity.X > 0f - num3) {
                            if (self.velocity.X > 0f) {
                                self.velocity.X = self.velocity.X - num3;
                            }
                            self.velocity.X = self.velocity.X - num3;
                            if (self.velocity.X < 0f - num3) {
                                self.velocity.X = 0f - num3;
                            }
                        }
                        else if (hitDirection > 0 && self.velocity.X < num3) {
                            if (self.velocity.X < 0f) {
                                self.velocity.X = self.velocity.X + num3;
                            }
                            self.velocity.X = self.velocity.X + num3;
                            if (self.velocity.X > num3) {
                                self.velocity.X = num3;
                            }
                        }
                        if (self.type == NPCID.SnowFlinx) {
                            num3 *= 1.5f;
                        }
                        num3 = (self.noGravity ? (num3 * -0.5f) : (num3 * -0.75f));
                        if (self.velocity.Y > num3) {
                            self.velocity.Y = self.velocity.Y + num3;
                            if (self.velocity.Y < num3) {
                                self.velocity.Y = num3;
                            }
                        }
                    }
                    else {
                        if (!self.noGravity) {
                            self.velocity.Y = (0f - num3) * 0.75f * self.knockBackResist;
                        }
                        else {
                            self.velocity.Y = (0f - num3) * 0.5f * self.knockBackResist;
                        }
                        self.velocity.X = num3 * hitDirection * self.knockBackResist;
                    }
                }
                if ((self.type == NPCID.WallofFlesh || self.type == NPCID.WallofFleshEye) && self.life <= 0) {
                    for (int i = 0; i < 200; i++) {
                        if (Main.npc[i].active && (Main.npc[i].type == NPCID.WallofFlesh || Main.npc[i].type == NPCID.WallofFleshEye)) {
                            Main.npc[i].HitEffect(hitDirection, damagePlaceholder);
                        }
                    }
                }
                else {
                    self.HitEffect(hitDirection, damagePlaceholder);
                }
                if (self.HitSound != null) {
                    Main.PlaySound(self.HitSound, self.position);
                }
                if (self.realLife >= 0) {
                    Main.npc[self.realLife].checkDead();
                }
                else {
                    self.checkDead();
                }
                return damagePlaceholder;
            }
            return 0.0;
        }
    }
}