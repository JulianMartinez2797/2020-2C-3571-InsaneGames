using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace TGC.MonoGame.InsaneGames.Entities.Obstacles
{
    public class Obstacle : Entity
    {
        private string ModelName;
        private Model Model;
        private Matrix Misalignment;
        private Matrix SpawnPoint;
        private readonly Vector3 HitboxSize = new Vector3(20, 5, 20);
        public void PrintHitbox() 
        {
            Debug.WriteLine("UpVertex: "+UpVertex);
            Debug.WriteLine("BottomVertex: "+BottomVertex);
        }
        public Obstacle(string modelName, Matrix spawnPoint, Matrix scaling)
        {
            Misalignment = Matrix.CreateTranslation(0, 0, 0);
            SpawnPoint = Misalignment *
                        scaling *
                        spawnPoint;
            ModelName = modelName;
            UpVertex = SpawnPoint.Translation + HitboxSize;
            BottomVertex = SpawnPoint.Translation - HitboxSize;
        }
        public override void Load()
        {
            Model = ContentManager.Instance.LoadModel(ModelName);
        }
        public override void Draw(GameTime gameTime)
        {
            Model.Draw(SpawnPoint, Game.Camera.View, Game.Camera.Projection);
        }

    }
}