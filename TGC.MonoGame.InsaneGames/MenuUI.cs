using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames
{
    class MenuUI
    {
        private SpriteBatch SpriteBatch;
        private SpriteFont Font;
        private GraphicsDevice GraphicsDevice;
        private Texture2D BackgroundTexture;
        private float Height;
        public void Load(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = new SpriteBatch(graphicsDevice);
            Font = ContentManager.Instance.LoadSpriteFont("Basic");
            BackgroundTexture = ContentManager.Instance.LoadTexture2D("Menu/background2");
            Height = graphicsDevice.Viewport.Height;
        }

        public void Draw(GameTime gameTime)
        {

            SpriteBatch.Begin();
            SpriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            SpriteBatch.End();

            var stringHeight = Height / 7;

            DrawCenterTextY("TGC Shooter", stringHeight * 0, 1);
            DrawCenterTextY("W A S D -> Movement", stringHeight * 1, 1);
            DrawCenterTextY("Left click -> Shoot", stringHeight * 2, 1);
            DrawCenterTextY("To Win -> Find the Key!", stringHeight * 3, 1);
            DrawCenterTextY("SpaceBar -> Start game", stringHeight * 4, 1);
            DrawCenterTextY("G -> God mode (you don't lose life and you can cross walls)", stringHeight * 5, 0.8f);
            DrawCenterTextY("Esc -> Exit game", stringHeight * 1, 1);
        }
        private void DrawCenterTextY(string msg, float Y, float escala)
        {
            var W = GraphicsDevice.Viewport.Width;
            var H = GraphicsDevice.Viewport.Height;
            var size = Font.MeasureString(msg) * escala;
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                Matrix.CreateScale(escala) * Matrix.CreateTranslation((W - size.X) / 2, Y, 0));
            SpriteBatch.DrawString(Font, msg, new Vector2(0, 0), Color.Black);
            SpriteBatch.End();
        }

    }


}
