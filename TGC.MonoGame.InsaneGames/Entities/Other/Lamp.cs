using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using TGC.MonoGame.InsaneGames.Maps;

namespace TGC.MonoGame.InsaneGames.Entities.Other
{
    class Lamp : IDrawable
    {
        private Model Model;
        private Matrix World;
        private Matrix Scale;
        private Matrix Translation;
        private Matrix Rotation = Matrix.Identity;
        private Effect Effect { get; set; }
        private Effect IluminationEffect { get; set; }

        private BasicEffect BasicEffect;
        private Texture2D Texture { get; set; }


        public Lamp(Matrix spawnPoint)
        {
            Translation = spawnPoint;
            Scale = Matrix.CreateScale(0.5f);
            //Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
        }

        public override void Load(GraphicsDevice gd)
        {
            //Model = ContentManager.Instance.LoadModel("others/street_lamp");
            Model = ContentManager.Instance.LoadModel("obstacles/cone/TrafficCone");

            BasicEffect = (BasicEffect)Model.Meshes[0].Effects[0];

            Effect = ContentManager.Instance.LoadEffect("LampBloom");

            World = Scale * Rotation * Translation;

            Texture = ((BasicEffect)Model.Meshes.FirstOrDefault()?.MeshParts.FirstOrDefault()?.Effect)?.Texture;

            IluminationEffect = ContentManager.Instance.LoadEffect("Ilumination");

            // Seteo constantes y colores para iluminacion tipo BlinnPhong
            IluminationEffect.Parameters["KAmbient"]?.SetValue(1f);
            IluminationEffect.Parameters["KDiffuse"]?.SetValue(0.4f);
            IluminationEffect.Parameters["KSpecular"]?.SetValue(0.5f);
            IluminationEffect.Parameters["shininess"]?.SetValue(16.0f);

            MapRepo.CurrentMap.AddIluminationParametersToEffect(IluminationEffect);
        }

        public override void Draw(GameTime gameTime)
        {

            var view = MapRepo.CurrentMap.Camera.View;
            var projection = MapRepo.CurrentMap.Camera.Projection;

            // We assign the effect to each one of the models
            foreach (var modelMesh in Model.Meshes)
                foreach (var meshPart in modelMesh.MeshParts)
                    meshPart.Effect = IluminationEffect;

            foreach (var modelMesh in Model.Meshes)
            {
                // We set the main matrices for each mesh to draw
                var worldMatrix = World;
                // World is used to transform from model space to world space
                IluminationEffect.Parameters["World"].SetValue(worldMatrix);
                IluminationEffect.Parameters["View"].SetValue(view);
                IluminationEffect.Parameters["Projection"].SetValue(projection);
                // InverseTransposeWorld is used to rotate normals
                IluminationEffect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                IluminationEffect.Parameters["ModelTexture"]?.SetValue(Texture);

                modelMesh.Draw();
            }
        }

        public void DrawBloom(GameTime gameTime)
        {

            Effect.CurrentTechnique = Effect.Techniques["BloomPass"];
            Effect.Parameters["baseTexture"]?.SetValue(BasicEffect.Texture);

            var worldMatrix = World;
            var view = MapRepo.CurrentMap.Camera.View;
            var projection = MapRepo.CurrentMap.Camera.Projection;
            var mesh = Model.Meshes.FirstOrDefault();
            
            if (mesh != null)
            {
                foreach (var part in mesh.MeshParts)
                {
                    part.Effect = Effect;
                    Effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * view * projection);
                }
                mesh.Draw();
            }
         }
        public override void Update(GameTime gameTime)
        {
            var cameraPosition = MapRepo.CurrentMap.Camera.Position;
            var lightPosition = new Vector3(cameraPosition.X, 0, cameraPosition.Z);
            IluminationEffect.Parameters["lightPosition"]?.SetValue(lightPosition);
            IluminationEffect.Parameters["eyePosition"]?.SetValue(cameraPosition);
        }

    }
}
