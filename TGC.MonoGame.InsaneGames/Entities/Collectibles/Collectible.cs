using Microsoft.Xna.Framework;
namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    abstract class Collectible : Entity 
    {
        public bool Collected { get; protected set; } = false;
        protected Matrix SpawnPoint;
        abstract public Vector3 Position { get; }
        abstract public void CollidedWith(Player player);
        abstract protected Vector3 HitboxSize { get; }
        protected Collectible(Matrix spawnPoint, float scaling)
        {
            SpawnPoint = Matrix.CreateScale(scaling) * spawnPoint;

            UpVertex = SpawnPoint.Translation + HitboxSize * scaling;
            BottomVertex = SpawnPoint.Translation - HitboxSize * scaling;
        }
    }
}
