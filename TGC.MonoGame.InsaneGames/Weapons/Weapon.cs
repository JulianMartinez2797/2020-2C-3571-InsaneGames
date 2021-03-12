using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using TGC.MonoGame.InsaneGames.Maps;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    abstract class Weapon : IDrawable
    {
        protected Matrix World;
        protected Model Model;
        protected string ModelName;
        protected Effect Effect { get; set; }
        protected Effect BlackEffect { get; set; }
        protected Texture2D Texture { get; set; }
        abstract public SoundEffect SoundEffect { get; }

        public Weapon(string modelName)
        {
            ModelName = modelName;
        }

        public virtual void Update(GameTime gameTime, Vector3 direction, Vector3 playerPosition)
        {
            this.Update(gameTime);
        }

        public override void Load()
        {
            Model = ContentManager.Instance.LoadModel(ModelName);

            Texture = ((BasicEffect)Model.Meshes.FirstOrDefault()?.MeshParts.FirstOrDefault()?.Effect)?.Texture;

            Effect = ContentManager.Instance.LoadEffect("Ilumination");
            
            BlackEffect = ContentManager.Instance.LoadEffect("ColorShader");

            // Seteo constantes y colores para iluminacion tipo BlinnPhong
            Effect.Parameters["KAmbient"]?.SetValue(1f);
            Effect.Parameters["KDiffuse"]?.SetValue(0.4f);
            Effect.Parameters["KSpecular"]?.SetValue(0.5f);
            Effect.Parameters["shininess"]?.SetValue(16.0f);

            MapRepo.CurrentMap.AddIluminationParametersToEffect(Effect);

        }
        public override void Draw(GameTime gameTime)
        {

            var world = World;
            var view = MapRepo.CurrentMap.Camera.View;
            var projection = MapRepo.CurrentMap.Camera.Projection;

            //Model.Draw(world, view, projection);


            // We assign the effect to each one of the models
            foreach (var modelMesh in Model.Meshes)
                foreach (var meshPart in modelMesh.MeshParts)
                    meshPart.Effect = Effect;

            foreach (var modelMesh in Model.Meshes)
            {
                // We set the main matrices for each mesh to draw
                var worldMatrix = world;
                // World is used to transform from model space to world space
                Effect.CurrentTechnique = Effect.Techniques["Ilumination"];
                Effect.Parameters["World"].SetValue(worldMatrix);
                Effect.Parameters["View"].SetValue(view);
                Effect.Parameters["Projection"].SetValue(projection);
                // InverseTransposeWorld is used to rotate normals
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                Effect.Parameters["ModelTexture"]?.SetValue(Texture);
                //Effect.Parameters["Time"]?.SetValue(time);

                modelMesh.Draw();
            }

        }
        public override void DrawBlack(GameTime gameTime)
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