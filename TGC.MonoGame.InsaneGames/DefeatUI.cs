﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames
{
    class DefeatUI
    {
        private SpriteBatch SpriteBatch;
        private SpriteFont Font;
        private GraphicsDevice GraphicsDevice;
        private Texture2D BackgroundTexture;
        public void Load(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = new SpriteBatch(graphicsDevice);
            Font = ContentManager.Instance.LoadSpriteFont("Basic");
            BackgroundTexture = ContentManager.Instance.LoadTexture2D("Menu/background2");
        }

        public void Draw(GameTime gameTime)
        {

            //SpriteBatch.Begin();
            //SpriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            //SpriteBatch.End();

            DrawCenterTextY("YOU LOSE!", 200, 2);
        }
        private void DrawCenterTextY(string msg, float Y, float escala)
        {
            var W = GraphicsDevice.Viewport.Width;
            var H = GraphicsDevice.Viewport.Height;
            var size = Font.MeasureString(msg) * escala;
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                Matrix.CreateScale(escala) * Matrix.CreateTranslation((W - size.X) / 2, Y, 0));
            SpriteBatch.DrawString(Font, msg, new Vector2(0, 0), Color.Red);
            SpriteBatch.End();
        }

    }


}