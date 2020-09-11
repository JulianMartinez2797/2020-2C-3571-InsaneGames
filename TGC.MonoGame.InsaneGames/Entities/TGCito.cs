using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Entities
{
    public class TGCito : DrawableGameComponent, IEnemy
    {
        private const string ModelName = "tgcito/tgcito-classic";
        static private Model Model;
        static private Matrix Misalignment;
        private Matrix SpawnPoint;
        private new TGCGame Game;

        public TGCito(TGCGame game) : base(game)
        {
            Game = game;
        }
        public void Initialize(Matrix spawnPoint, Matrix? scaling = null)
        {
            if(Model is null)
            {
                Misalignment = Matrix.CreateTranslation(0, 44.5f, 0);
            }
            SpawnPoint = Misalignment * 
                        scaling.GetValueOrDefault(Matrix.CreateScale(0.2f)) * 
                        spawnPoint;
        }
        public void Load()
        {
            if(Model is null)
                Model = Game.Content.Load<Model>(TGCGame.ContentFolder3D + ModelName);
        }
        public override void Update(GameTime gameTime)
        {

        }
        public override void Draw(GameTime gameTime)
        {
            Model.Draw(SpawnPoint, Game.Camera.View, Game.Camera.Projection);
        }
    }
}