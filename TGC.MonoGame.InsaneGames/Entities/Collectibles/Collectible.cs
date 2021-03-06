using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using TGC.MonoGame.InsaneGames.Maps;

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
        private Effect BlackEffect;
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

            Effect = ContentManager.Instance.LoadEffect("Ilumination");

            BlackEffect = ContentManager.Instance.LoadEffect("ColorShader");

            // Seteo constantes y colores para iluminacion tipo BlinnPhong
            Effect.Parameters["KAmbient"].SetValue(1f);
            Effect.Parameters["KDiffuse"].SetValue(0.4f);
            Effect.Parameters["KSpecular"].SetValue(0.5f);
            Effect.Parameters["shininess"].SetValue(16.0f);

            MapRepo.CurrentMap.AddIluminationParametersToEffect(Effect);

        }
        public override void Update(GameTime gameTime)
        {
            time = (float)gameTime.TotalGameTime.TotalSeconds;
            Rotation = Matrix.CreateRotationY(time * 2);
            World = Scale * initialRotation * Rotation * SpawnPoint;

            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(Effect);

        }
        public override void Draw(GameTime gameTime)
        {

            if (!Collected)
            {
                var view = Maps.MapRepo.CurrentMap.Camera.View;
                var projection = Maps.MapRepo.CurrentMap.Camera.Projection;

                // We assign the effect to each one of the models
                foreach (var modelMesh in Model.Meshes)
                    foreach (var meshPart in modelMesh.MeshParts)
                        meshPart.Effect = Effect;

                foreach (var modelMesh in Model.Meshes)
                {
                    // We set the main matrices for each mesh to draw
                    var worldMatrix = World;
                    // World is used to transform from model space to world space
                    Effect.CurrentTechnique = Effect.Techniques["Ilumination"];
                    Effect.Parameters["World"].SetValue(worldMatrix);
                    Effect.Parameters["View"].SetValue(view);
                    Effect.Parameters["Projection"].SetValue(projection);
                    // InverseTransposeWorld is used to rotate normals
                    Effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                    Effect.Parameters["ModelTexture"]?.SetValue(Texture);
                    Effect.Parameters["Time"]?.SetValue(time);

                    modelMesh.Draw();
                }               
            }
        }

        public override void DrawBlack(GameTime gameTime)
        {

            if (!Collected)
            {
                var view = Maps.MapRepo.CurrentMap.Camera.View;
                var projection = Maps.MapRepo.CurrentMap.Camera.Projection;

                BlackEffect.Parameters["colorTarget"].SetValue(Color.Black.ToVector4());

                // We assign the effect to each one of the models
                foreach (var modelMesh in Model.Meshes)
                    foreach (var meshPart in modelMesh.MeshParts)
                        meshPart.Effect = BlackEffect;

                foreach (var modelMesh in Model.Meshes)
                {
                    // We set the main matrices for each mesh to draw
                    var worldMatrix = World;
                    // World is used to transform from model space to world space
                    BlackEffect.Parameters["World"].SetValue(worldMatrix);
                    BlackEffect.Parameters["View"].SetValue(view);
                    BlackEffect.Parameters["Projection"].SetValue(projection);
                    modelMesh.Draw();
                }
            }
        }

    }
}
