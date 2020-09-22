using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraLands
{
    public class TerraLands : Mod
    {
        internal static ILog TLLogger = LogManager.GetLogger("TerraLands");

        public override void Load()
        {
            #region Shaders
            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> nisRef = new Ref<Effect>(GetEffect("Effects/Items/NewItemSheen"));
                GameShaders.Misc["NewItemSheen"] = new MiscShaderData(nisRef, "NewItemSheen");
            }
            #endregion
        }

    }
}