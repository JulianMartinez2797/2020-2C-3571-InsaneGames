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
        private Texture2D Texture;
        private BasicEffect BasicEffect1;
        private BasicEffect BasicEffect2;

        public Lamp(Matrix spawnPoint)
        {
            Translation = spawnPoint;
            Scale = Matrix.CreateScale(0.05f);
            //Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
        }

        public override void Load()
        {
            Model = ContentManager.Instance.LoadModel("others/street_lamp");

            World = Scale * Rotation * Translation;

            Texture = ((BasicEffect)Model.Meshes.FirstOrDefault()?.MeshParts.FirstOrDefault()?.Effect)?.Texture;

            BasicEffect1 = (BasicEffect)Model.Meshes[0].Effects[0];
            BasicEffect1.LightingEnabled = true;
            BasicEffect1.EmissiveColor = Color.Black.ToVector3();
            BasicEffect2 = (BasicEffect)Model.Meshes[0].Effects[1];
            BasicEffect2.LightingEnabled = true;
            BasicEffect2.EmissiveColor = Color.White.ToVector3();
            BloomEffect = ContentManager.Instance.LoadEffect("LampBloom");
        }

        public override void Draw(GameTime gameTime)
        {
            var view = MapRepo.CurrentMap.Camera.View;
            var projection = MapRepo.CurrentMap.Camera.Projection;
            ((BasicEffect)Model.Meshes[0].Effects[1]).LightingEnabled = true;
            ((BasicEffect)Model.Meshes[0].Effects[1]).EmissiveColor = Color.White.ToVector3();
            Model.Draw(World, view, projection);
        }

        public void DrawBloom(GameTime gameTime)
        {
            var view = MapRepo.CurrentMap.Camera.View;
            var projection = MapRepo.CurrentMap.Camera.Projection;
            ((BasicEffect)Model.Meshes[0].Effects[0]).LightingEnabled = true;
            ((BasicEffect)Model.Meshes[0].Effects[0]).EmissiveColor = Color.Black.ToVector3();
            Model.Draw(World, view, projection);
        }
    }
}