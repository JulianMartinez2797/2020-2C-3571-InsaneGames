using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    abstract class Weapon : IDrawable
    { 
        protected Model Model;
        protected string ModelName;

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
        }
    }
}