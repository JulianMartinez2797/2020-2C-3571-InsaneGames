using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Maps;

namespace TGC.MonoGame.InsaneGames.Entities.Other
{
    class Lamp : IDrawable
    {
        private Model Model;
        private Matrix World;
        private Matrix Scale;
        private Matrix Translation;
        private Matrix Rotation = Matrix.Identity;

        public Lamp(Matrix spawnPoint)
        {
            Translation = spawnPoint;
            Scale = Matrix.CreateScale(0.05f);
        }

        public override void Load()
        {
            Model = ContentManager.Instance.LoadModel("others/street_lamp");

            World = Scale * Rotation * Translation;
        }

        public override void Draw(GameTime gameTime)
        {
            var view = MapRepo.CurrentMap.Camera.View;
            var projection = MapRepo.CurrentMap.Camera.Projection;
            Model.Draw(World, view, projection);
        }
    }
}
