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
        private readonly Vector3 HitboxSize = new Vector3(5, 10, 5);
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
            UpVertex = SpawnPoint.Translation + new Vector3(HitboxSize.X / 2, HitboxSize.Y, HitboxSize.Z / 2);
            BottomVertex = SpawnPoint.Translation - new Vector3(HitboxSize.X / 2, 0, HitboxSize.Z / 2);
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