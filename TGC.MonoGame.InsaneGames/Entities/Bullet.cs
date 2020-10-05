using System;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.InsaneGames.Entities
{
    class Bullet : Entity
    {
        protected Vector3 Speed, InitialPos;
        public Boolean Remove { get; protected set; }
        public Vector3 LastPosition { get; protected set; } 
        public Vector3 CurrentPosition { get; protected set; }
        protected float Damage;
        override public Vector3 BottomVertex 
        { 
            get { return LastPosition; }
        }
        override public Vector3 UpVertex 
        { 
            get { return CurrentPosition; }
        }
        public Bullet(float damage, Vector3 speed, Vector3 initialPos)
        {
            Damage = damage;
            Speed = speed;
            InitialPos = initialPos;
            LastPosition = initialPos;
        }

        public override void Update(GameTime gameTime)
        {
            LastPosition = CurrentPosition;
            var time = gameTime.ElapsedGameTime.TotalSeconds;
            CurrentPosition = Speed * (float) time + InitialPos;
        }

        public void Collided()
        {
            this.Remove = true;
        }
    }
}