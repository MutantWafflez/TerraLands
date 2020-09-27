using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerraLands.Projectiles;
using TerraLands.TLPrefixes;
using TerraLands.Utils;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace TerraLands.Items.Weapons
{
    //Main abstract class for TerraLands Weapons
    public abstract class TLWeapon : ModItem
    {
        private int rarityAlphaValue = 175;
        private int rarityAlphaDirection = 1;

        public string flavorText = "";

        public TLPrefix itemPrefix = TLPrefixList.Default;
        public WeightedRandom<TLPrefix> availablePrefixes = new WeightedRandom<TLPrefix>();

        public TLSuffix itemSuffix = TLSuffixList.Default;

        public ElementType itemElement = ElementType.Default;
        public WeightedRandom<ElementType> availableElements = new WeightedRandom<ElementType>();

        /// <summary>
        /// How many additional projectiles of type item.shoot are fired.
        /// Defaults to 0.
        /// </summary>
        public int additionalShots = 0;
        /// <summary>
        /// Accuracy on a scale of 0f to 1f. 0f (0%) is complete inaccuracy (30 degree spread) and 1f (100%) is complete accuracy (no spread).
        /// Defaults to 100% accuracy.
        /// </summary>
        public float projectileAccuracy = 1f;

        /// <summary>
        /// If the weapon has projectiles, setting this value to anything other than 0 will make that value the min/max angle spread for the weapon.
        /// </summary>
        public float fixedSpread = 0f;

        #region Cloning
        public override bool CloneNewInstances => true;

        //Cloning is CRUCIAL in making this mod work; if there is no cloning, all of the guns will have the same exact prefix/parts. No good.
        public override ModItem Clone(Item item)
        {
            TLWeapon wepClone = (TLWeapon)base.Clone(item);
            if (!wepClone.availablePrefixes.elements.Any())
            {
                wepClone.GetAvailablePrefixes();
            }
            if (!wepClone.availableElements.elements.Any())
            {
                wepClone.GetAvailableElements();
            }
            wepClone.additionalShots = 0;
            wepClone.fixedSpread = 0f;
            if (wepClone.itemPrefix == TLPrefixList.Default)
            {
                wepClone.itemPrefix = availablePrefixes.Get();
                wepClone.item.SetNameOverride(GetAdjustedName());
            }
            if (wepClone.itemSuffix == TLSuffixList.Default)
            {
                wepClone.itemSuffix = TLSuffixList.GetRandomSuffix();
                wepClone.item.SetNameOverride(GetAdjustedName());
            }
            if (wepClone.itemElement == ElementType.Default)
            {
                wepClone.itemElement = availableElements.Get();
            }
            return wepClone;
        }
        #endregion

        #region Weapon Defaults

        //Since all inheriting members will be weapons, no stacking, and define a base for shoot speed
        public override void AutoDefaults()
        {
            base.AutoDefaults();
            item.maxStack = 1;
            item.shootSpeed = 8f;
        }

        //Don't want Vanilla Prefixes, will have Prefixes of our own that are more like Borderlands' Prefixes
        public override bool? PrefixChance(int pre, UnifiedRandom rand)
        {
            if (pre == -1)
            {
                GetAvailablePrefixes();
                if (itemPrefix == TLPrefixList.Default)
                {
                    itemPrefix = availablePrefixes.Get();
                    item.SetNameOverride(GetAdjustedName());
                }
                if (itemSuffix == TLSuffixList.Default)
                {
                    itemSuffix = TLSuffixList.GetRandomSuffix();
                    item.SetNameOverride(GetAdjustedName());
                }
                GetAvailableElements();
                if (itemElement == ElementType.Default)
                {
                    itemElement = availableElements.Get();
                }
                return false;
            }

            else if (pre == -3)
            {
                return false;
            }

            return null;
        }

        public override bool CanBurnInLava() => false;
        #endregion

        #region Projectiles
        /* Real quick run down on how the shooting mechanics of this mod work:
         * Every projectile has the weapon's stats applied onto it: Weapon Element (WIP), Prefix, and any other specific stats.
         * The direction of the projectile is then rotated by a random value between the negative accuracy and positive accuracy extremes.
         * The negative accuracy and positive accuracy extremes at 0% accuracy are -30 and 30 degrees, respectively.
         * Increasing accuracy above 0% makes the spread smaller and smaller, until it is pin-point accurate at 100% accuracy. Anything above 100% accuracy does nothing.
         * Decreasing accuracy below 0% makes will not increase the spread unless the item specifically overrides this cap.
         */
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (item.shoot > ProjectileID.None)
            {
                if (HoldoutOffset() != null)
                {
                    position += (Vector2)HoldoutOffset();
                }

                if (fixedSpread != 0f)
                {
                    for (float i = -fixedSpread; i <= fixedSpread; i += fixedSpread / (additionalShots - 1))
                    {
                        Vector2 firedVelocity = new Vector2(speedX, speedY);

                        firedVelocity = firedVelocity.RotatedBy(MathHelper.ToRadians(i));
                        firedVelocity *= itemPrefix.projVelocityMultiplier;

                        int projectile = Projectile.NewProjectile(position, firedVelocity, type, damage, knockBack, player.whoAmI);
                        if (Main.projectile[projectile].modProjectile?.GetType().BaseType == typeof(TLProjectile))
                        {
                            ((TLProjectile)Main.projectile[projectile].modProjectile).projectileElement = itemElement;
                        }


                        Main.projectile[projectile].scale *= itemPrefix.scaleMultiplier;
                    }
                }
                else
                {
                    float maxSpreadAngle = 30;
                    for (int i = 0; i <= additionalShots; i++)
                    {
                        Vector2 firedVelocity = new Vector2(speedX, speedY);

                        float accuracy = 1f - (projectileAccuracy * itemPrefix.accuracyMultiplier);
                        MathHelper.Clamp(accuracy, 0f, 1f);

                        firedVelocity = firedVelocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-maxSpreadAngle * accuracy, maxSpreadAngle * accuracy)));
                        firedVelocity *= itemPrefix.projVelocityMultiplier;

                        int projectile = Projectile.NewProjectile(position, firedVelocity, type, damage, knockBack, player.whoAmI);
                        if (Main.projectile[projectile].modProjectile?.GetType() == typeof(TLProjectile))
                        {
                            ((TLProjectile)Main.projectile[projectile].modProjectile).projectileElement = itemElement;
                        }


                        Main.projectile[projectile].scale *= itemPrefix.scaleMultiplier;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Prefix Stat Changes
        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat) => mult *= itemPrefix.damageMultiplier;

        public override void GetWeaponCrit(Player player, ref int crit) => crit += itemPrefix.critChanceModification;

        public override float UseTimeMultiplier(Player player) => itemPrefix.useSpeedMultiplier;

        public override float MeleeSpeedMultiplier(Player player) => item.melee ? itemPrefix.useSpeedMultiplier : 1f;

        public override void GetWeaponKnockback(Player player, ref float knockback) => knockback *= itemPrefix.knockbackMultiplier;
        #endregion

        #region I/O
        //Since we can't save our itemPrefix field directly, gotta save/load with its ID.
        public override TagCompound Save()
        {
            return new TagCompound
            {
                {"WeaponPrefix", itemPrefix.TLPrefixID},
                {"WeaponElement", (int)itemElement },
                {"WeaponSuffix", itemSuffix.suffixID }
            };
        }

        public override void Load(TagCompound tag)
        {
            itemPrefix = TLPrefixList.GetInstanceWithID(tag.GetInt("WeaponPrefix"));
            itemElement = (ElementType)tag.GetInt("WeaponElement");
            itemSuffix = TLSuffixList.GetInstanceWithID(tag.GetInt("WeaponSuffix"));
        }

        //Just for some sweet sweet multiplayer compatability :-) (AKA send the item prefix data upon throwing the item into the world, for example)
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(itemPrefix.TLPrefixID);
            writer.Write((int)itemElement);
            writer.Write(itemSuffix.suffixID);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            itemPrefix = TLPrefixList.GetInstanceWithID(reader.ReadInt32());
            itemElement = (ElementType)reader.ReadInt32();
            itemSuffix = TLSuffixList.GetInstanceWithID(reader.ReadInt32());
        }
        #endregion

        #region Per Tick Hooks

        public override void PostUpdate() => item.SetNameOverride(GetAdjustedName());

        public override void UpdateInventory(Player player) => item.SetNameOverride(GetAdjustedName());

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawRarityLight(spriteBatch);
            rarityAlphaValue += rarityAlphaDirection;
            if (rarityAlphaValue > 225 || rarityAlphaValue < 175)
            {
                rarityAlphaDirection *= -1;
            }
            if (item.newAndShiny && Main.netMode != NetmodeID.Server)
            {
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                GameShaders.Misc["NewItemSheen"].UseColor(TLUtils.ItemRareToTLRareColor(item.rare)).Apply();
            }
            return true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine TLRarity = new TooltipLine(mod, "TLRarity", $"[{TLUtils.ItemRareToTLRareName(item.rare)}]")
            {
                overrideColor = TLUtils.ItemRareToTLRareColor(item.rare)
            };

            TooltipLine nameLine = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.mod == "Terraria");
            if (nameLine == null)
            {
                tooltips.Add(TLRarity);
            }
            else
            {
                tooltips.Insert(tooltips.IndexOf(nameLine) + 1, TLRarity);
            }

            if (!string.IsNullOrEmpty(flavorText))
            {
                TooltipLine flavorLine = new TooltipLine(mod, "TLFlavor", flavorText)
                {
                    overrideColor = Color.Red
                };
                TooltipLine rarityLine = tooltips.FirstOrDefault(x => x.Name == "TLRarity" && x.mod == "TerraLands");
                tooltips.Insert(tooltips.IndexOf(rarityLine) + 1, flavorLine);
            }

            if (itemElement != ElementType.None)
            {
                TooltipLine vanillaDamage = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
                if (vanillaDamage == null)
                {
                    tooltips.Add(new TooltipLine(mod, "TLElement", itemElement.ToString()));
                }
                else
                {
                    string[] splitText = vanillaDamage.text.Split(' ');
                    string damageString = splitText.Last();

                    damageString = string.Concat(itemElement.ToString().ToLower(), " ", damageString);
                    splitText[splitText.Length - 1] = damageString;

                    vanillaDamage.text = splitText.First();
                    for (int i = 1; i < splitText.Length; i++)
                    {
                        vanillaDamage.text = string.Concat(vanillaDamage.text, " ", splitText[i]);
                    }
                }
            }

            #region Stat Tooltips

            string accuracyText = "Completely accurate";
            float accuracyStat = projectileAccuracy * itemPrefix.accuracyMultiplier;
            if (accuracyStat <= 0.2f)
                accuracyText = "Ludicrously inaccurate";
            else if (accuracyStat <= 0.4f)
                accuracyText = "Extremely inaccurate";
            else if (accuracyStat <= 0.6f)
                accuracyText = "Moderately inaccurate";
            else if (accuracyStat <= 0.8f)
                accuracyText = "Mostly accurate";
            else if (accuracyStat <= 0.9f)
                accuracyText = "Moderately accurate";
            else if (accuracyStat <= 0.99f)
                accuracyText = "Highly accurate";

            TooltipLine accuracyStatLine = new TooltipLine(mod, "TLAccuracyStat", accuracyText);
            tooltips.Add(accuracyStatLine);

            if (!(Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift)))
            {
                TooltipLine TLHideStats = new TooltipLine(mod, "TLHideStats", "Hold Shift to see prefix stats")
                {
                    overrideColor = Colors.RarityTrash
                };
                tooltips.Add(TLHideStats);
            }
            else
            {
                List<TooltipLine> StatList = new List<TooltipLine>
                {
                    new TooltipLine(mod, "TLPrefixDmgStat", Math.Round((itemPrefix.damageMultiplier - 1f) * 100) + "% damage"),
                    new TooltipLine(mod, "TLPrefixCritChanceStat", itemPrefix.critChanceModification + "% critical strike chance"),
                    new TooltipLine(mod, "TLPrefixCritDmgStat", Math.Round((itemPrefix.critDamageMultiplier - 1f) * 100) + "% critical strike damage"),
                    new TooltipLine(mod, "TLPrefixDUseSpeedStat", Math.Round((itemPrefix.useSpeedMultiplier - 1f) * 100) + "% usage speed"),
                    new TooltipLine(mod, "TLPrefixVelocityStat", Math.Round((itemPrefix.projVelocityMultiplier - 1f) * 100) + "% projectile velocity"),
                    new TooltipLine(mod, "TLPrefixScaleStat", Math.Round((itemPrefix.scaleMultiplier - 1f) * 100) + "% projectile scale"),
                    new TooltipLine(mod, "TLPrefixKnockStat", Math.Round((itemPrefix.knockbackMultiplier - 1f) * 100) + "% knockback"),
                    new TooltipLine(mod, "TLPrefixAccStat", Math.Round((itemPrefix.accuracyMultiplier - 1f) * 100) + "% accuracy")
                };
                foreach (TooltipLine line in StatList)
                {
                    if (line.text[0] == '0') //So that the stats with 0% values (AKA no change) don't show up
                    {
                        continue;
                    }
                    line.isModifier = true;
                    if (line.text.Contains("-"))
                    {
                        line.isModifierBad = true;
                    }
                    else
                    {
                        line.text = string.Concat("+", line.text);
                    }

                    tooltips.Add(line);
                }
            }
            #endregion
        }

        private void DrawRarityLight(SpriteBatch spriteBatch)
        {
            Texture2D glowTexture = mod.GetTexture("Items/Weapons/RarityLightBase");
            Color rarityColor = TLUtils.ItemRareToTLRareColor(item.rare);
            rarityColor.A = (byte)rarityAlphaValue;
            Vector2 glowPos = item.Center - Main.screenPosition + new Vector2(0, -glowTexture.Height);

            spriteBatch.Draw(glowTexture, glowPos, rarityColor);
        }
        #endregion

        #region Usable Methods

        /// <summary>
        /// Method called in TLWeapon's Clone() method to fill the availablePrefixes list.
        /// Needed for each damage type, as each one has different prefixes.
        /// Can be overriden in a specific weapon class for specific prefixes, regardless of type, if needed.
        /// </summary>
        public abstract void GetAvailablePrefixes();

        /// <summary>
        /// Method called in TLWeapon's Clone() method to fill the availablePrefixes list.
        /// All elements are allowed on all weapons by default.
        /// Can be overriden in a specific weapon class for specific elements, regardless of type, if needed.
        /// </summary>
        public virtual void GetAvailableElements()
        {
            availableElements.Add(ElementType.None, 1.25f);
            availableElements.Add(ElementType.Explosive, 0.75f);
            availableElements.Add(ElementType.Fire, 0.75f);
            availableElements.Add(ElementType.Shock, 0.75f);
            availableElements.Add(ElementType.Corrosive, 0.75f);
            availableElements.Add(ElementType.Cryo, 0.75f);
            availableElements.Add(ElementType.Radiated, 0.75f);
        }

        /// <summary>
        /// Returns the base item's name with the proper prefix and suffix attached.
        /// </summary>
        public string GetAdjustedName()
        {
            item.ClearNameOverride();
            string clearedName = item.Name;
            return string.Concat(itemPrefix.prefixText + " ", clearedName, " of ", itemSuffix.suffixText);
        }
        #endregion
    }
}