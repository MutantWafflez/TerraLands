using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;

namespace TerraLands.Projectiles.UniqueProj {
    public class KemptSharrellProj : TLProjectile {

        public float HasSplit {
            get { return projectile.ai[0]; }
            set { projectile.ai[0] = value; }
        }

        public float SplitTimer {
            get { return projectile.ai[1]; }
            set { projectile.ai[1] = value;  }
        }

        public override void AI() {
            if (HasSplit == 0f) {
                if (++SplitTimer > 45) {
                    HasSplit = 1;

                    float angleVariation = 5f;
                    int newProj = Projectile.NewProjectile(projectile.position, projectile.velocity.RotatedBy(MathHelper.ToRadians(-angleVariation)), projectile.type, projectile.damage, projectile.knockBack, projectile.owner);

                    KemptSharrellProj newProjInst = (KemptSharrellProj)Main.projectile[newProj].modProjectile;
                    newProjInst.projectileElement = ((KemptSharrellProj)projectile.modProjectile).projectileElement;
                    newProjInst.HasSplit = HasSplit;
                    newProjInst.SplitTimer = SplitTimer;
                    projectile.netUpdate = true;
                    newProjInst.projectile.netUpdate = true;
                }
            }
        }
    }
}
