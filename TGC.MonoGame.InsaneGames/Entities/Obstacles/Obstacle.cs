using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Linq;
using TGC.MonoGame.InsaneGames.Maps;

namespace TGC.MonoGame.InsaneGames.Entities.Obstacles
{
    public class Obstacle : Entity
    {
        private string ModelName;
        private Model Model;
        private Matrix Misalignment;
        private Matrix SpawnPoint;
        private readonly Vector3 HitboxSize = new Vector3(5, 10, 5);
        private Effect Effect { get; set; }
        private Texture2D Texture { get; set; }
        public Vector3 Position => SpawnPoint.Translation;
        public void PrintHitbox() 
        {
            Debug.WriteLine("UpVertex: "+UpVertex);
            Debug.WriteLine("BottomVertex: "+BottomVertex);
        }
        public Obstacle(string modelName, Matrix spawnPoint, Matrix scaling)
        {
            Misalignment = Matrix.CreateTranslation(0, 0, 0);
            SpawnPoint = Misalignment *
                        scaling *
                        spawnPoint;
            ModelName = modelName;
            UpVertex = SpawnPoint.Translation + new Vector3(HitboxSize.X / 2, HitboxSize.Y, HitboxSize.Z / 2);
            BottomVertex = SpawnPoint.Translation - new Vector3(HitboxSize.X / 2, 0, HitboxSize.Z / 2);
        }
        public override void Load()
        {
            if (Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);

            Texture = ((BasicEffect)Model.Meshes.FirstOrDefault()?.MeshParts.FirstOrDefault()?.Effect)?.Texture;

            Effect = ContentManager.Instance.LoadEffect("Ilumination");

            // Seteo constantes y colores para iluminacion tipo BlinnPhong
            Effect.Parameters["KAmbient"]?.SetValue(1f);
            Effect.Parameters["KDiffuse"]?.SetValue(0.4f);
            Effect.Parameters["KSpecular"]?.SetValue(0.5f);
            Effect.Parameters["shininess"]?.SetValue(16.0f);

            MapRepo.CurrentMap.AddIluminationParametersToEffect(Effect);

        }
        public override void Update(GameTime gameTime)
        {
            var cameraPosition = MapRepo.CurrentMap.Camera.Position;
            var lightPosition = new Vector3(cameraPosition.X, 0, cameraPosition.Z);
            Effect.Parameters["lightPosition"]?.SetValue(lightPosition);
            Effect.Parameters["eyePosition"]?.SetValue(cameraPosition);
        }
        public override void Draw(GameTime gameTime)
        {
            var world = SpawnPoint;
            var view = Maps.MapRepo.CurrentMap.Camera.View;
            var projection = Maps.MapRepo.CurrentMap.Camera.Projection;

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

    }
}