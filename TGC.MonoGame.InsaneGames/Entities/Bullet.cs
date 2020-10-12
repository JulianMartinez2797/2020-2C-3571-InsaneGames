using System;
using Microsoft.Xna.Framework;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;
namespace TGC.MonoGame.InsaneGames.Entities
{
    class Bullet : Entity
    {
        protected Vector3 Speed, InitialPos;
        public Boolean Collided { get; protected set; } = false;
        public Vector3 LastPosition { get; protected set; } 
        public Vector3 CurrentPosition { get; protected set; }
        protected float Damage;
        protected Vector3 HitboxSize;
        override public Vector3 BottomVertex 
        { 
            get { return LastPosition - HitboxSize / 2; }
        }
        override public Vector3 UpVertex 
        { 
            get { return CurrentPosition + HitboxSize / 2; }
        }
        public Boolean Remove 
        {
            get { return Collided; }
        }
        public Bullet(float damage, Vector3 speed, Vector3 initialPos, Vector3 hitboxSize)
        {
            Damage = damage;
            Speed = speed;
            InitialPos = initialPos;
            CurrentPosition = initialPos;
            LastPosition = CurrentPosition;
            HitboxSize = hitboxSize;
        }

        public override void Update(GameTime gameTime)
        {
            LastPosition = CurrentPosition;
            var time = gameTime.ElapsedGameTime.TotalSeconds;
            CurrentPosition = Speed * (float) time + LastPosition;
        }
        public void CollidedWith(Enemy enemy)
        {
            if(!Collided)
            {
                enemy.RemoveFromLife(Damage);
                CollidedWith();
            }
        }
        public void CollidedWith()
        {
            Collided = true;
        }
        public void CollidedWith(Obstacle obstacle)
        {
            CollidedWith();
        }
        public override Boolean CollidesWith(Vector3 bVertex, Vector3 uVertex)
        {
            return !Collided && base.CollidesWith(bVertex, uVertex);
        }
    }
}