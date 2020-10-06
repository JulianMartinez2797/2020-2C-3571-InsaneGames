using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    class Armor : Collectible
    {
        private const string ModelName = "collectibles/armor/shield/Shield";
        static private Model Model;
        static private Matrix Misalignment;
        private Matrix SpawnPoint;
        private float RecoveryAmount;
        public Armor(Matrix spawnPoint, Matrix? scaling = null, float recoveryAmount = 10)
        {
            if (Model is null)
            {
                Misalignment = Matrix.CreateTranslation(0, 0, 0);
            }
            SpawnPoint = Misalignment *
                        scaling.GetValueOrDefault(Matrix.CreateScale(0.3f)) *
                        spawnPoint;
            RecoveryAmount = recoveryAmount;
        }
        public override void Load()
        {
            if (Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);
        }
        public override void Draw(GameTime gameTime)
        {
            if(!Collected)
                Model.Draw(SpawnPoint, Game.Camera.View, Game.Camera.Projection);
        }
        public override void CollidedWith(Player player)
        {
            player.AddToArmor(RecoveryAmount);
            Collected = true;
        }
    }
}
