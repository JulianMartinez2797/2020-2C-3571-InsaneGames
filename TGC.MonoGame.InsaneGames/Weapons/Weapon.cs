using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using TGC.MonoGame.InsaneGames.Maps;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    abstract class Weapon : IDrawable
    { 
        protected Model Model;
        protected string ModelName;
        protected Effect Effect { get; set; }
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

            // Seteo constantes y colores para iluminacion tipo BlinnPhong
            Effect.Parameters["KAmbient"]?.SetValue(1f);
            Effect.Parameters["KDiffuse"]?.SetValue(0.4f);
            Effect.Parameters["KSpecular"]?.SetValue(0.5f);
            Effect.Parameters["shininess"]?.SetValue(16.0f);

            MapRepo.CurrentMap.AddIluminationParametersToEffect(Effect);

        }
    }
}