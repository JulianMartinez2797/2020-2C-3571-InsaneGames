using System;
using Microsoft.Xna.Framework;
using TGC.MonoGame.InsaneGames.Entities.Enemies;

namespace TGC.MonoGame.InsaneGames.Entities
{
    class Bullet : Entity
    {
        protected Vector3 Speed, InitialPos;
        public Boolean Remove { get; protected set; }
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
        public Bullet(float damage, Vector3 speed, Vector3 initialPos, Vector3 hitboxSize)
        {
            Damage = damage;
            Speed = speed;
            InitialPos = initialPos;
            CurrentPosition = initialPos;
            LastPosition = CurrentPosition;
            HitboxSize = hitboxSize;
            Remove = false;
        }

        public override void Update(GameTime gameTime)
        {
            LastPosition = CurrentPosition;
            var time = gameTime.ElapsedGameTime.TotalSeconds;
            CurrentPosition = Speed * (float) time + LastPosition;
            Console.Write(LastPosition);
            Console.Write(CurrentPosition);
            Console.WriteLine("");
        }

        public void Collided()
        {
            this.Remove = true;
        }
        public void CollidedWith(Enemy enemy)
        {
            enemy.RemoveFromLife(Damage);
            Collided();
        }
    }
}