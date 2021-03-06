using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.InsaneGames.Maps;

namespace TGC.MonoGame.InsaneGames.Weapons
{
    class Handgun : Weapon
    {
        protected Matrix World;
        private bool Shooting = false;
        static readonly float Damage = 20;
        static readonly Vector3 BulletSize = new Vector3(1, 1, 1);
        static protected Matrix RotationMatrix = Matrix.CreateScale(0.02f) *
                                                /*Matrix.CreateTranslation(0, -0.5f, 0)* */
                                                Matrix.CreateRotationX(MathHelper.ToRadians(-3f)) * 
                                                Matrix.CreateRotationY(MathHelper.ToRadians(-178f));

        public Handgun () : base("armas/pistol/m1911-handgun") {}
        public override void Initialize(TGCGame game) {
            World = Matrix.CreateScale(0.1f);
            base.Initialize(game);
        } 
        public override void Update(GameTime gameTime)
        {
            Matrix cameraWorld = Matrix.Invert(MapRepo.CurrentMap.Camera.View);
            Matrix weaponWorld = cameraWorld;   //gives your weapon a matrix that is co-located and co-rotated with camera

            float distInFrontOfCam = 2f;
            float amountWeaponIsLoweredFromCenterOfScreen = 0.8f;
            float leftOrRightOffset = 0.4f;
            
            weaponWorld.Translation += (cameraWorld.Forward * distInFrontOfCam) +                       //set to taste. moves the weapon slightly in front of cam
                               (cameraWorld.Down * amountWeaponIsLoweredFromCenterOfScreen) +   //set to taste. moves the weapon from the center of the screen to the lower part
                               (cameraWorld.Right * leftOrRightOffset);

            World = RotationMatrix * weaponWorld;

            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(Effect);

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
        public override void Update(GameTime gameTime, Vector3 direction, Vector3 playerPosition)
        {
            Update(gameTime);
            var mouseState = Mouse.GetState();
            if(!Shooting && mouseState.LeftButton == ButtonState.Pressed)
            {
                SoundEffect.CreateInstance().Play();
                Shooting = true;
                MapRepo.CurrentMap.AddBullet(new Entities.Bullets.BasicBullet(Damage, direction * 1000, playerPosition, BulletSize));
            }
            else if(mouseState.LeftButton == ButtonState.Released)
            {
                Shooting = false;
            }
        }

        public override SoundEffect SoundEffect
        {
            get { return ContentManager.Instance.LoadSoundEffect("handgun-shot"); }
        }
    }
}