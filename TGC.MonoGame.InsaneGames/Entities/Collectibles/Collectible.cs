using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    abstract class Collectible : Entity 
    {
        abstract public string ModelName { get; }
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

        private Effect Effect { get; set; }
        private Texture2D Texture { get; set; }


        protected Collectible(Matrix spawnPoint, float scaling)
        {
            SpawnPoint = spawnPoint;

            UpVertex = SpawnPoint.Translation + HitboxSize * scaling;
            BottomVertex = SpawnPoint.Translation - HitboxSize * scaling;

        }
        public override void Load()
        {
            if (Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);

            Texture = ((BasicEffect)Model.Meshes.FirstOrDefault()?.MeshParts.FirstOrDefault()?.Effect)?.Texture;

            Effect = ContentManager.Instance.LoadEffect("Collectible");

        }
        public override void Update(GameTime gameTime)
        {
           time = (float)gameTime.TotalGameTime.TotalSeconds;
           Rotation = Matrix.CreateRotationY(time * 2);
           World = Scale * initialRotation * Rotation * SpawnPoint;
        }
        public override void Draw(GameTime gameTime)
        {
            if (!Collected) {
                var mesh = Model.Meshes.FirstOrDefault();
                var view = Maps.MapRepo.CurrentMap.Camera.View;
                var projection = Maps.MapRepo.CurrentMap.Camera.Projection;

                if (mesh != null)
                {
                    foreach (var part in mesh.MeshParts)
                    {
                        part.Effect = Effect;
                        Effect.Parameters["World"].SetValue(World * mesh.ParentBone.Transform);
                        Effect.Parameters["View"].SetValue(view);
                        Effect.Parameters["Projection"].SetValue(projection);
                        Effect.Parameters["ModelTexture"]?.SetValue(Texture);
                        Effect.Parameters["Time"].SetValue(time);
                    }

                    mesh.Draw();
                }
            }

        }
    }
}
