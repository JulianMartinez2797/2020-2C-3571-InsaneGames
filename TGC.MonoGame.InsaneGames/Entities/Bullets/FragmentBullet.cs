using System;
using Microsoft.Xna.Framework;
using TGC.MonoGame.InsaneGames.Entities.Enemies;

namespace TGC.MonoGame.InsaneGames.Entities.Bullets
{
    class FragmentBullet : Bullet
    {
        protected bool remove;
        public override bool Remove { get { return remove; } }
        protected float BaseDamage;
        protected Vector3 Position;
        protected Vector3 FrontRadius;
        private double TimeSinceShooted = 0;
        private bool destroy_bullet = false;
        protected float Radius;
        protected bool HalfExplosion;
        public override void Draw(GameTime gameTime)
        {}
        public override void Load()
        {}
        public override bool isInitialized()
        {
            return true;
        }
        public FragmentBullet(float baseDamage, Vector3 direction, Vector3 position, Vector3 hitboxSize, bool halfExplosion = true)
        {
            BaseDamage = baseDamage;
            Position = position;
            BottomVertex = position - hitboxSize;
            UpVertex = position + hitboxSize;
            Radius = (UpVertex - BottomVertex).Length();
            FrontRadius = direction * Radius;
            HalfExplosion = halfExplosion;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            TimeSinceShooted += gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeSinceShooted > 0.5)
                destroy_bullet = true;
            remove = true;
        }
        public override bool CollidesWith(Vector3 bVertex, Vector3 uVertex)
        {
            if(base.CollidesWith(bVertex, uVertex)) return true;
            if(HalfExplosion)
            {
                //Es feo pero va a mas o menos funcionar
                var middle = uVertex - bVertex;
                var distanciaHitbox = Vector3.Distance(FrontRadius, middle);
                if(distanciaHitbox > Radius) return false;
            }
            // get box closest point to sphere center by clamping
            var x = Math.Max(bVertex.X, Math.Min(Position.X, uVertex.X));
            var y = Math.Max(bVertex.Y, Math.Min(Position.Y, uVertex.Y));
            var z = Math.Max(bVertex.Z, Math.Min(Position.Z, uVertex.Z));

            // this is the same as isPointInsideSphere
            var distance = Math.Sqrt((x - Position.X) * (x - Position.X) +
                                    (y - Position.Y) * (y - Position.Y) +
                                    (z - Position.Z) * (z - Position.Z));
            
            return distance < Radius; 
        }
        public override void CollidedWith(Enemy enemy)
        {
            if (destroy_bullet)
                return;
            var mult = (1 - (enemy.Position - Position).Length() / Radius);
            enemy.RemoveFromLife(BaseDamage * mult);
        }
        public override void CollidedWith()
        {

        }
    }
}