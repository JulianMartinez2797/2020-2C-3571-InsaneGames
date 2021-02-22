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

        private BasicEffect BasicEffect;

        public Lamp(Matrix spawnPoint)
        {
            Translation = spawnPoint;
            Scale = Matrix.CreateScale(0.1f);
            Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
        }

        public override void Load()
        {
            Model = ContentManager.Instance.LoadModel("others/street_lamp");

            BasicEffect = (BasicEffect)Model.Meshes[0].Effects[0];

            Effect = ContentManager.Instance.LoadEffect("LampBloom");

            World = Scale * Rotation * Translation;
        }

        public override void Draw(GameTime gameTime)
        {

            Effect.CurrentTechnique = Effect.Techniques["BloomPass"];
            Effect.Parameters["baseTexture"]?.SetValue(BasicEffect.Texture);

            /*
            foreach (var modelMesh in Model.Meshes)
                foreach (var meshPart in modelMesh.MeshParts)
                    meshPart.Effect = Effect;

            foreach (var modelMesh in Model.Meshes)
            {
                // We set the main matrices for each mesh to draw
                var worldMatrix = World;
                var view = MapRepo.CurrentMap.Camera.View;
                var projection = MapRepo.CurrentMap.Camera.Projection;
                // World is used to transform from model space to world space
                Effect.Parameters["WorldViewProjection"].SetValue(worldMatrix * view * projection);
                //Effect.Parameters["View"].SetValue(view);
                //Effect.Parameters["View"].SetValue(view);
                //Effect.Parameters["Projection"].SetValue(projection);
                // InverseTransposeWorld is used to rotate normals
                //Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                //Effect.Parameters["ModelTexture"].SetValue(Texture);
                //Effect.Parameters["Time"].SetValue(time);

                modelMesh.Draw();
            }
            */
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
 
            //Model.Draw(World, view, projection);
        }
    }
}
