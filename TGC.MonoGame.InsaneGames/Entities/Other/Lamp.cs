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
        private Effect BloomEffect;
        private Effect IluminationEffect;
        private Effect WhiteEffect;
        private Effect BlackEffect;
        public Lamp(Matrix spawnPoint)
        {
            Translation = spawnPoint;
            Scale = Matrix.CreateScale(0.15f);
            Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
        }

        public override void Load()
        {
            Model = ContentManager.Instance.LoadModel("others/street_lamp");

            World = Scale * Rotation * Translation;

            BloomEffect = ContentManager.Instance.LoadEffect("Bloom");

            IluminationEffect = ContentManager.Instance.LoadEffect("Ilumination");
            
            IluminationEffect.Parameters["KAmbient"].SetValue(1f);
            IluminationEffect.Parameters["KDiffuse"].SetValue(0.4f);
            IluminationEffect.Parameters["KSpecular"].SetValue(0.5f);
            IluminationEffect.Parameters["shininess"].SetValue(8.0f);

            MapRepo.CurrentMap.AddIluminationParametersToEffect(IluminationEffect);

            WhiteEffect = ContentManager.Instance.LoadEffect("ColorShader");

            BlackEffect = ContentManager.Instance.LoadEffect("BlackShader");

        }

        public override void Draw(GameTime gameTime)
        {
            var view = MapRepo.CurrentMap.Camera.View;
            var projection = MapRepo.CurrentMap.Camera.Projection;

            WhiteEffect.Parameters["colorTarget"].SetValue(Color.White.ToVector4());
            
            foreach (var modelMesh in Model.Meshes)
            {
                modelMesh.MeshParts[0].Effect = IluminationEffect;
                modelMesh.MeshParts[1].Effect = WhiteEffect;
                var worldMatrix = World;

                WhiteEffect.Parameters["World"].SetValue(worldMatrix);
                WhiteEffect.Parameters["View"].SetValue(view);
                WhiteEffect.Parameters["Projection"].SetValue(projection);

                IluminationEffect.CurrentTechnique = IluminationEffect.Techniques["IluminationWithoutTexture"];
                IluminationEffect.Parameters["modelColor"].SetValue(Color.Black.ToVector4());
                IluminationEffect.Parameters["World"].SetValue(worldMatrix);
                IluminationEffect.Parameters["View"].SetValue(view);
                IluminationEffect.Parameters["Projection"].SetValue(projection);
                IluminationEffect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                modelMesh.Draw();
            }
             
        }

        public void DrawBloom(GameTime gameTime)
        {
            var view = MapRepo.CurrentMap.Camera.View;
            var projection = MapRepo.CurrentMap.Camera.Projection;

            WhiteEffect.Parameters["colorTarget"].SetValue(Color.White.ToVector4());

            foreach (var modelMesh in Model.Meshes)
            {
                modelMesh.MeshParts[0].Effect = BlackEffect;
                modelMesh.MeshParts[1].Effect = WhiteEffect;
                var worldMatrix = World;

                WhiteEffect.Parameters["World"].SetValue(worldMatrix);
                WhiteEffect.Parameters["View"].SetValue(view);
                WhiteEffect.Parameters["Projection"].SetValue(projection);

                BlackEffect.Parameters["World"].SetValue(worldMatrix);
                BlackEffect.Parameters["View"].SetValue(view);
                BlackEffect.Parameters["Projection"].SetValue(projection);

                modelMesh.Draw();
            }
        }
        public override void Update(GameTime gameTime)
        {
            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(IluminationEffect);
        }
    }
}