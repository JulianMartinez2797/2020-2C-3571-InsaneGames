using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    class Key : Collectible
    {
        private static readonly Vector3 hitboxSize = new Vector3(15, 5, 15);
        override protected Vector3 HitboxSize { get { return hitboxSize; } }

        override public Vector3 Position => SpawnPoint.Translation;
        public Key(Matrix spawnPoint, float scaling = 1) : base(spawnPoint, scaling)
        {
            Scale = Matrix.CreateScale(1f);
            initialRotation = Matrix.CreateRotationZ(MathHelper.ToRadians(90f));
        }
        public override string ModelName
        {
            get { return "collectibles/key/Key_B_02"; }
        }
        public override void CollidedWith(Player player)
        {
            Maps.MapRepo.CurrentMap.playerFoundKey();
            // Collected = true;
        }
    }
}
