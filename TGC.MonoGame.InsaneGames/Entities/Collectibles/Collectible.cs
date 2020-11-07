using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    abstract class Collectible : Entity 
    {
        public bool Collected { get; protected set; } = false;
        protected Matrix SpawnPoint;
        protected Matrix World;
        protected Model Model { get; set; }

        abstract public Vector3 Position { get; }
        abstract public void CollidedWith(Player player);
        abstract protected Vector3 HitboxSize { get; }
        private float time = 0;
        protected Matrix Rotation;
        // Para aquellos modelos que requieran primero una rotacion
        protected Matrix initialRotation = Matrix.Identity;

        protected Matrix Scale;


        protected Collectible(Matrix spawnPoint, float scaling)
        {
            SpawnPoint = spawnPoint;

            UpVertex = SpawnPoint.Translation + HitboxSize * scaling;
            BottomVertex = SpawnPoint.Translation - HitboxSize * scaling;
        }
        public override void Update(GameTime gameTime)
        {
           time = (float)gameTime.TotalGameTime.TotalSeconds;
           Rotation = Matrix.CreateRotationY(time * 2);
           World = Scale * initialRotation * Rotation * SpawnPoint;
        }
        public override void Draw(GameTime gameTime)
        {
            if (!Collected)
                Model.Draw(World, Maps.MapRepo.CurrentMap.Camera.View, Maps.MapRepo.CurrentMap.Camera.Projection);
        }
    }
}
