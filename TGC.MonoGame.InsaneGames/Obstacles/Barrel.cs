﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

<<<<<<< HEAD:TGC.MonoGame.InsaneGames/Obstacles/Barrel.cs
namespace TGC.MonoGame.InsaneGames.Obstacles
=======
namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
>>>>>>> master:TGC.MonoGame.InsaneGames/Entities/Collectibles/Heart.cs
{
    class Barrel : Obstacle
    {
        private const string ModelName = "obstacles/barrel/barrel";
        static private Model Model;
        static private Matrix Misalignment;
        private Matrix SpawnPoint;

        public Barrel(Matrix spawnPoint, Matrix? scaling = null)
        {
            if (Model is null)
            {
                Misalignment = Matrix.CreateTranslation(0, 0, 0);
            }
            SpawnPoint = Misalignment *
                        scaling.GetValueOrDefault(Matrix.CreateScale(10.2f)) *
                        spawnPoint;
        }
        public override void Load()
        {
            if (Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);
        }
        public override void Draw(GameTime gameTime)
        {
            Model.Draw(SpawnPoint, Game.Camera.View, Game.Camera.Projection);
        }
    }
}
