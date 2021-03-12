using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Entities;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class InfoUI : IDrawable
    {
        private SpriteBatch SpriteBatch;
        private SpriteFont Font;
        private GraphicsDevice GraphicsDevice;
        private Texture2D Life;
        private Texture2D Armor;

        public void Load(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = new SpriteBatch(graphicsDevice);
            Font = ContentManager.Instance.LoadSpriteFont("Basic");
            Life = ContentManager.Instance.LoadTexture2D("life/heart");
            Armor = ContentManager.Instance.LoadTexture2D("Armor/shield");
        }
        public void Draw(GameTime gameTime, Player player)
        {
            Vector2 lifeTexturePosition = new Vector2(20, GraphicsDevice.Viewport.Height - 50);
            Vector2 lifeStringPosition = new Vector2(80, GraphicsDevice.Viewport.Height - 50);
            Vector2 armorTexturePosition = new Vector2(160, GraphicsDevice.Viewport.Height - 50);
            Vector2 armorStringPosition = new Vector2(210, GraphicsDevice.Viewport.Height - 50);
            Vector2 origin = new Vector2(0, 0);

            Rectangle lifeSourceRectangle = new Rectangle(0, 0, Life.Width, Life.Height);
            Rectangle armorSourceRectangle = new Rectangle(0, 0, Armor.Width, Armor.Height);

            float lifeScale = 0.05f;
            float armorScale = 0.08f;

            SpriteBatch.Begin();
            SpriteBatch.Draw(Life, lifeTexturePosition, lifeSourceRectangle, Color.White, 0f, origin, lifeScale, SpriteEffects.None, 0f);
            SpriteBatch.DrawString(Font, $"{player.Life}", lifeStringPosition, Color.Red);
            SpriteBatch.Draw(Armor, armorTexturePosition, armorSourceRectangle, Color.White, 0f, origin, armorScale, SpriteEffects.None, 0f);
            SpriteBatch.DrawString(Font, $"{player.Armor}", armorStringPosition, Color.DarkBlue);
            if(Game.godMode)
                SpriteBatch.DrawString(Font, "GOD_MODE", new Vector2(GraphicsDevice.Viewport.Width - 200, 10), Color.Green);
            SpriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        }
    }
}