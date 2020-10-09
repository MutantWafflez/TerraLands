using TerraLands.Enums;
using TerraLands.Projectiles.UniqueProj;
using TerraLands.TLPrefixes;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraLands.Items.Weapons.Guns.Legendaries {
    public class KemptSharell : TLGun {
        public override string Texture => "Terraria/Item_" + ItemID.Handgun;

        public override void GetAvailablePrefixes() {
            availablePrefixes.Add(TLPrefixList.Implicit);
        }

        public override void GetAvailableElements() {
            availableElements.Add(ElementType.Explosive);
        }

        public override void SetDefaults() {
            item.damage = 50;
            item.rare = (int)ItemRarities.Legendary;
            item.shoot = ModContent.ProjectileType<KemptSharrellProj>();
            item.shootSpeed = 4f;
            item.useTime = 20;
            item.useAnimation = 20;
            projectileAccuracy = 0.67f;
            additionalShots = 2;
            fixedSpread = 10f;
            flavorText = "In no way related to anyone named Harold";
        }
    }
}
