﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using TerraLands.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraLands.Projectiles
{
    public abstract class TLProjectile : ModProjectile
    {
        public ElementType projectileElement = ElementType.Default;

        public int explosiveTimer = 0;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.Bullet;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Bullet);
            projectile.arrow = false;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
            projectile.knockBack = 1f;
            projectile.alpha = 0;
            aiType = ProjectileID.Bullet;
        }

        public override void Kill(int timeLeft)
        {
            if (projectileElement == ElementType.Explosive && explosiveTimer < 5)
            {
                ExplodeProjectile();
                explosiveTimer++;
            }
        }

        private void ExplodeProjectile()
        {
            if (explosiveTimer == 0)
            {
                Main.PlaySound(SoundID.Item62.WithVolume(0.5f), projectile.Center);
            }
            projectile.width = 60;
            projectile.height = 60;
            projectile.hide = true;
            projectile.velocity = Vector2.Zero;
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Dirt);
            }
        }
    }
}
