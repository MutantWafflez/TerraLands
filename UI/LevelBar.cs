using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraLands.Utils;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraLands.UI {
    public class LevelBar : UIState {
        private UIImage levelFrame;
        private UIImage levelProgress;
        private UIElement levelProgressConstraint;
        private UIText levelText;

        public override void OnInitialize() {
            levelFrame = new UIImage(ModContent.GetTexture(nameof(TerraLands) + "/UI/Textures/LevelFrame"));
            levelFrame.Left.Set(-(levelFrame.Width.Pixels / 2f), 0.5f);
            levelFrame.Top.Set(-levelFrame.Height.Pixels, 0.95f);

            levelText = new UIText("Level 1", 0.9f);
            levelText.Left.Set(0, 0.44f);
            levelText.Top.Set(0f, 1f);
            levelFrame.Append(levelText);

            levelProgressConstraint = new UIElement();
            levelProgressConstraint.Width = levelFrame.Width;
            levelProgressConstraint.Height = levelFrame.Height;
            levelProgressConstraint.OverflowHidden = true;
            levelFrame.Append(levelProgressConstraint);

            levelProgress = new UIImage(ModContent.GetTexture(nameof(TerraLands) + "/UI/Textures/LevelProgress"));
            levelProgressConstraint.Append(levelProgress);

            Append(levelFrame);
        }

        protected override void DrawChildren(SpriteBatch spriteBatch) {
            TLPlayer localPlayer = Main.LocalPlayer.GetModPlayer<TLPlayer>();
            levelProgressConstraint.Width.Set(0, localPlayer.Experience / (float)TLUtils.XPRequiredToLevelUp(localPlayer.Level));
            base.DrawChildren(spriteBatch);
        }

        public override void Update(GameTime gameTime) {
            levelText.SetText($"Level {Main.LocalPlayer.GetModPlayer<TLPlayer>().Level}");
            base.Update(gameTime);
        }
    }
}
