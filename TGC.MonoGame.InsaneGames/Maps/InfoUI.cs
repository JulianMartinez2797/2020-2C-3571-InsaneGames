using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Entities;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class InfoUI 
    {
        private SpriteBatch SpriteBatch;
        private SpriteFont Font;
        private GraphicsDevice GraphicsDevice;
        public void Load(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = new SpriteBatch(graphicsDevice);
            Font = ContentManager.Instance.LoadSpriteFont("Basic");
        }
        public void Draw(GameTime gameTime, Player player)
        {
            Vector2 lifeStringPosition = new Vector2(10, GraphicsDevice.Viewport.Height - 50);
            Vector2 armorStringPosition = new Vector2(170, GraphicsDevice.Viewport.Height - 50);

            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, $"LIFE: {player.Life}", lifeStringPosition, Color.Red);
            SpriteBatch.DrawString(Font, $"ARMOR: {player.Armor}", armorStringPosition, Color.DarkBlue);
            SpriteBatch.End();

        }
    }
}