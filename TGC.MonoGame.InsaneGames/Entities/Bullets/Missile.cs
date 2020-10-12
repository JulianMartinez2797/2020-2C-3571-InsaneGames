using Microsoft.Xna.Framework;
using TGC.MonoGame.InsaneGames.Entities.Enemies;

namespace TGC.MonoGame.InsaneGames.Entities.Bullets
{
    class Missile : BasicBullet
    {
        protected Vector3 ExplosionSize;
        public Missile(float baseDamage, Vector3 speed, Vector3 initialPos, Vector3 hitboxSize, Vector3 explosionSize) : base(baseDamage, speed, initialPos, hitboxSize)
        {
            ExplosionSize = explosionSize;
        }
        public override void CollidedWith(Enemy enemy)
        {
            CollidedWith();
        }
        public override void CollidedWith()
        {
            Collided = true;
            Maps.MapRepo.CurrentMap.AddBullet(new FragmentBullet(Damage, new Vector3(0, 1, 0), CurrentPosition, ExplosionSize, false));
        }
    }
}