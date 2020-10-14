using Microsoft.Xna.Framework;
using TerraLands.Enums;
using TerraLands.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraLands.Projectiles {
    public abstract class TLProjectile : ModProjectile {
        public ElementType projectileElement = ElementType.Default;

        public int explosiveTimer = 0;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.Bullet;

        public override void SetDefaults() {
            projectile.CloneDefaults(ProjectileID.Bullet);
            projectile.arrow = false;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.knockBack = 1f;
            projectile.alpha = 0;
            aiType = ProjectileID.Bullet;
        }

        public override void Kill(int timeLeft) {
            if (projectileElement == ElementType.Explosive && explosiveTimer < 5) {
                ExplodeProjectile();
                explosiveTimer++;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
            target.GetGlobalNPC<TLGlobalNPC>().elementHitBy = projectileElement;
        }

        /// <summary>
        /// Method that adds the visual effect of the explosion as well as sound if need be.
        /// </summary>
        /// <param name="projectile">Projectile that is currently exploding.</param>
        /// <param name="addSound">Whether or not to play sound.</param>
        public void AddExplosiveFlair(Projectile projectile, bool addSound = false) {
            if (addSound) {
                Main.PlaySound(SoundID.Item62.WithVolume(0.5f), projectile.Center);
            }
            for (int i = 0; i < 10; i++) {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Dirt);
            }
        }

        private void ExplodeProjectile() {
            projectile.position = projectile.Center;
            projectile.width = 60;
            projectile.height = 60;
            projectile.Center = projectile.position;
            projectile.alpha = 255;
            projectile.velocity = Vector2.Zero;
            AddExplosiveFlair(projectile, explosiveTimer == 0);
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)TLPacketType.SyncProjectileExplosion);
                packet.Write(projectile.whoAmI);
                packet.Send();
            }
        }
    }
}
