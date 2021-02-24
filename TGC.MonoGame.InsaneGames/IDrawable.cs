using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TGC.MonoGame.InsaneGames
{
    public abstract class IDrawable 
    {
        protected TGCGame Game { get; set; }
        virtual public void Initialize(TGCGame game) 
        {
            Game = game;
        }
        virtual public void Draw(GameTime gameTime) {}
        virtual public void Update(GameTime gameTime) {}
        virtual public void Load() {}

        virtual public void Load(GraphicsDevice gd) { }
        virtual public void DrawBlack(GameTime gameTime) { }


    }
}