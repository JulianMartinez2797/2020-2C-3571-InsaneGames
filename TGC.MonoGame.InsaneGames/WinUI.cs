using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames
{
    class WinUI
    {
        private SpriteBatch SpriteBatch;
        private SpriteFont Font;
        private GraphicsDevice GraphicsDevice;
        private Texture2D BackgroundTexture;
        private Effect Effect;

        public void Load(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = new SpriteBatch(graphicsDevice);
            Font = ContentManager.Instance.LoadSpriteFont("Basic");
            BackgroundTexture = ContentManager.Instance.LoadTexture2D("Menu/background2");
            Effect = ContentManager.Instance.LoadEffect("DefeatAndWin");
        }

        public void Draw(GameTime gameTime)
        {
            // TODO: Add some texture
            //SpriteBatch.Begin();
            //SpriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            //SpriteBatch.End();

            Effect.Parameters["Time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);

            DrawCenterTextY("YOU WIN!", 200, 2, Color.Blue);
            DrawCenterTextY("Press R to restart", 300, 0.7f, Color.White);

        }
        private void DrawCenterTextY(string msg, float Y, float escala, Color color)
        {
            var W = GraphicsDevice.Viewport.Width;
            var H = GraphicsDevice.Viewport.Height;
            var size = Font.MeasureString(msg) * escala;
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, Effect,
                Matrix.CreateScale(escala) * Matrix.CreateTranslation((W - size.X) / 2, Y, 0));
            SpriteBatch.DrawString(Font, msg, new Vector2(0, 0), color);
            SpriteBatch.End();
        }

    }


}
