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

        public override void Load()
        {
            Model = ContentManager.Instance.LoadModel(ModelName);
        }
    }
}