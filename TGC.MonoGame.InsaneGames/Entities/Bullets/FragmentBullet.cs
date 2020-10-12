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
        protected float Radius;
        public FragmentBullet(float baseDamage, Vector3 direction, Vector3 position, Vector3 hitboxSize)
        {
            BaseDamage = baseDamage;
            Position = position;
            BottomVertex = position - hitboxSize;
            UpVertex = position + hitboxSize;
            Radius = (UpVertex - BottomVertex).Length();
            FrontRadius = direction * Radius;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            remove = true;
        }
        public override bool CollidesWith(Vector3 bVertex, Vector3 uVertex)
        {
            if(base.CollidesWith(bVertex, uVertex))
                return true;
            if(uVertex.X <= Position.X || FrontRadius.X <= bVertex.X) return false;
            if(uVertex.Y <= Position.Y || FrontRadius.Y <= bVertex.Y) return false;
            if(uVertex.Z <= Position.Z || FrontRadius.Z <= bVertex.Z) return false;
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
            var mult = (1 - (enemy.position.Value.Translation - Position).Length() / Radius);
            enemy.RemoveFromLife(BaseDamage * mult);
        }
        public override void CollidedWith()
        {

        }
    }
}