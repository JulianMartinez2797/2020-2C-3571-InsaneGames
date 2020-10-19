using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames
{
    class MenuUI
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

        public void Draw(GameTime gameTime)
        {
            DrawCenterTextY("TGC Shooter", 0, 1);
            DrawCenterTextY("W A S D -> Movement", 100, 1);
            DrawCenterTextY("Left click -> Shoot", 200, 1);
            DrawCenterTextY("SpaceBar -> Start game", 300, 1);
            DrawCenterTextY("Esc -> Exit game", 400, 1);
        }
        private void DrawCenterTextY(string msg, float Y, float escala)
        {
            var W = GraphicsDevice.Viewport.Width;
            var H = GraphicsDevice.Viewport.Height;
            var size = Font.MeasureString(msg) * escala;
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                Matrix.CreateScale(escala) * Matrix.CreateTranslation((W - size.X) / 2, Y, 0));
            SpriteBatch.DrawString(Font, msg, new Vector2(0, 0), Color.YellowGreen);
            SpriteBatch.End();
        }

    }


}
