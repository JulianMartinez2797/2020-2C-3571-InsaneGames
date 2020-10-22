using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    class Life : Collectible
    {
        private const string ModelName = "collectibles/life/heart/heart";
        static private Model Model;
        static private Matrix Misalignment;
        private Matrix SpawnPoint;
        private float RecoveryAmount;
        private static readonly Vector3 HitboxSize = new Vector3(15, 2, 15);
        override public Vector3 Position => SpawnPoint.Translation;
        public Life(Matrix spawnPoint, Matrix? scaling = null, float recoveryAmount = 10)
        {
            if (Model is null)
            {
                Misalignment = Matrix.CreateTranslation(0, 0, 0);
            }
            SpawnPoint = Misalignment *
                        scaling.GetValueOrDefault(Matrix.CreateScale(30.0f)) *
                        spawnPoint;
            RecoveryAmount = recoveryAmount;
            UpVertex = SpawnPoint.Translation + HitboxSize;
            BottomVertex = SpawnPoint.Translation - HitboxSize;
        }
        public override void Load()
        {
            if (Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);
        }
        public override void Draw(GameTime gameTime)
        {
            if(!Collected)
                Model.Draw(SpawnPoint, Maps.MapRepo.CurrentMap.Camera.View, Maps.MapRepo.CurrentMap.Camera.Projection);
        }
        public override void CollidedWith(Player player)
        {
            player.AddToLife(RecoveryAmount);
            Collected = true;
        }
    }
}
