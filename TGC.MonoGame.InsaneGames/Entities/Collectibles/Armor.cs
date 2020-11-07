using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    class Armor : Collectible
    {
        private const string ModelName = "collectibles/armor/shield/Shield";
        private float RecoveryAmount;
        private static readonly Vector3 hitboxSize = new Vector3(15, 5, 15);
        override protected Vector3 HitboxSize { get { return hitboxSize; }}
        override public Vector3 Position => SpawnPoint.Translation;
        public Armor(Matrix spawnPoint, float scaling = 1, float recoveryAmount = 10) : base(spawnPoint, scaling)
        {
            Scale = Matrix.CreateScale(0.3f);
            RecoveryAmount = recoveryAmount;
        }
        public override void Load()
        {
            if (Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);
        }

        public override void CollidedWith(Player player)
        {
            player.AddToArmor(RecoveryAmount);
            Collected = true;
        }
    }
}
