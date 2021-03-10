using TGC.MonoGame.InsaneGames.Entities.Enemies;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.InsaneGames.Entities.Bullets
{
    abstract class Bullet : Entity
    {
        abstract public bool Remove { get; }
        abstract public void CollidedWith(Enemy enemy);
        abstract public void CollidedWith();
        abstract public bool isInitialized();
    }
}