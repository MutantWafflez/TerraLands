using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;

namespace TerraLands.Projectiles.UniqueProj
{
    public class KemptSharrellProj : TLProjectile
    {
        private int splitTimer = 0;

        public bool hasSplit = false;

        public override void AI()
        {
            if (!hasSplit)
            {
                if (++splitTimer > 45 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    hasSplit = true;

                    float angleVariation = 5f;
                    int newProj = Projectile.NewProjectile(projectile.position, projectile.velocity.RotatedBy(MathHelper.ToRadians(-angleVariation)), projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                    //projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(angleVariation));

                    KemptSharrellProj newProjInst = (KemptSharrellProj)Main.projectile[newProj].modProjectile;
                    newProjInst.projectileElement = ((KemptSharrellProj)projectile.modProjectile).projectileElement;
                    newProjInst.hasSplit = true;
                    projectile.netUpdate = true;
                    newProjInst.projectile.netUpdate = true;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(splitTimer);
            writer.Write(hasSplit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            splitTimer = reader.ReadInt32();
            hasSplit = reader.ReadBoolean();
        }
    }
}
