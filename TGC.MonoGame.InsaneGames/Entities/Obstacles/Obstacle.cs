using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Entities.Obstacles
{
    public class Obstacle : Entity
    {
        private string ModelName;
        static private Model Model;
        static private Matrix Misalignment;
        private Matrix SpawnPoint;

        public Obstacle(string modelName, Matrix spawnPoint, Matrix scaling)
        {
            if (Model is null)
            {
                Misalignment = Matrix.CreateTranslation(0, 0, 0);
            }
            SpawnPoint = Misalignment *
                        scaling *
                        spawnPoint;
            ModelName = modelName;
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