using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Maps;
using System.Linq;

namespace TGC.MonoGame.InsaneGames.Entities.Enemies
{
    class TGCito : Enemy
    {
        private Matrix CurPosition, PrevPosition;
        private Player playerReference { set; get; }
        private const string ModelName = "tgcito/tgcito-classic";
        static private Model Model;
        private readonly Vector3 HitboxSize = new Vector3(10, 16, 10);
        private readonly float TimePerHit = 2, TimeToRespawn = 10;
        private Matrix Misalignment { get; }
        private Boolean Death = false, PosSet = false;
        private float TimeSinceLastHit = 0, TimeSinceDeath = 0;
        private Effect Effect { get; set; }
        private Texture2D Texture { get; set; }

        private float time = 0;

        override public Vector3 Position 
        {
            get { return CurPosition.Translation; }
            set { 
                    CurPosition = Misalignment * Matrix.CreateTranslation(value); 
                    PosSet = true;
                }
        }
        public TGCito(Player player, Matrix? spawnPoint = null, Matrix? scaling = null, float life = 100, float damage = 5)
        {
            playerReference = player;
            Misalignment = Matrix.CreateTranslation(0, 44.5f, 0) * scaling.GetValueOrDefault(Matrix.CreateScale(0.2f));
            if(spawnPoint.HasValue)
            {
                CurPosition = Misalignment * spawnPoint.Value;
                UpVertex = spawnPoint.Value.Translation + HitboxSize / 2;
                BottomVertex = spawnPoint.Value.Translation - HitboxSize / 2;
                PosSet = true;
            }
            floorEnemy = true;
            TotalLife = life;
            CurrentLife = TotalLife;
            Damage = damage;
        }

        public bool isPlayerNear()
        {
            float detection_distance = 150f;
            Vector3 playerPosition = playerReference.NewPosition;
            Vector3 enemyPosition = this.CurPosition.Translation;
            return Vector3.Distance(playerPosition, enemyPosition) < detection_distance;
        }


        // Esto se tiene que cambiar por CreateLookAt
        private float getAngleBetweenVectorsInPlaneXZ(Vector3 vecA, Vector3 vecB)
        {
            Vector2 vecA2 = new Vector2(vecA.X,vecA.Z);
            Vector2 vecB2 = new Vector2(vecB.X,vecB.Z);

            float dotResult = Vector2.Dot(vecA2, vecB2);
            float vecA2Length = vecA2.Length();
            float vecB2Length = vecB2.Length();
            if (vecA2Length == 0 || vecB2Length == 0)
                return 0f;
            double angle = Math.Acos(dotResult / (vecA2Length * vecB2Length));
            if (angle == double.NaN)
                return 0f;
            Vector3 cross = Vector3.Cross(vecA,vecB);
            if(cross.Y < 0)
                angle *= -1;
            float angleF = Convert.ToSingle(angle);
            return angleF;
        }
        private bool _mirandoPlayer = false;
        public override void Update(GameTime gameTime)
        {
            if(Death)
            {
                IfDeathUpdate(gameTime);
                return;
            }
            else if (isPlayerNear())
            {   
                PrevPosition = CurPosition;
                //Detectado
                Vector3 vec_to_player = playerReference.NewPosition - CurPosition.Translation;
                vec_to_player.Y = 0;
                // Rotar el tgcito
                float rot_speed = MathHelper.ToRadians(200f * Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds));
                float angle = getAngleBetweenVectorsInPlaneXZ(CurPosition.Backward, playerReference.NewPosition - CurPosition.Translation);
                if (float.IsNaN(angle))
                    angle = 0f;
                if (Math.Abs(angle) < rot_speed)
                {
                    rot_speed = angle;
                    _mirandoPlayer = true;
                }
                else
                {
                    rot_speed *= angle / Math.Abs(angle);
                }
                float angle_in_degrees = MathHelper.ToDegrees(angle);
                CurPosition = Matrix.CreateRotationY(rot_speed) * CurPosition;

                // Empezar a mover el tgcito
                if (_mirandoPlayer)
                {
                    vec_to_player.Normalize();
                    float enemy_speed = 70f * Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
                    CurPosition = CurPosition * Matrix.CreateTranslation(vec_to_player * enemy_speed);
                }
                UpVertex = CurPosition.Translation + HitboxSize / 2;
                BottomVertex = CurPosition.Translation - HitboxSize / 2;
            } 
            else 
            {
                _mirandoPlayer = false;
            }
            TimeSinceLastHit += (float) gameTime.ElapsedGameTime.TotalSeconds;

            var cameraPosition = MapRepo.CurrentMap.Camera.Position;
            var lightPosition = new Vector3(cameraPosition.X, 0, cameraPosition.Z);
            Effect.Parameters["lightPosition"]?.SetValue(lightPosition);
            Effect.Parameters["eyePosition"]?.SetValue(cameraPosition);
        }

        public override void Load(GraphicsDevice gd)
        {
            if(Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);

            Texture = ((BasicEffect)Model.Meshes.FirstOrDefault()?.MeshParts.FirstOrDefault()?.Effect)?.Texture;

            Effect = ContentManager.Instance.LoadEffect("Ilumination");

            MapRepo.CurrentMap.AddIluminationParametersToEffect(Effect);

            // Seteo constantes y colores para iluminacion tipo BlinnPhong
            Effect.Parameters["KAmbient"].SetValue(0.3f);
            Effect.Parameters["KDiffuse"].SetValue(0.6f);
            Effect.Parameters["KSpecular"].SetValue(0.1f);
            Effect.Parameters["shininess"].SetValue(8.0f);

        }
        public override void Draw(GameTime gameTime)
        {

            var world = CurPosition;
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
                Effect.Parameters["World"].SetValue(worldMatrix);
                Effect.Parameters["View"].SetValue(view);
                Effect.Parameters["Projection"].SetValue(projection);
                // InverseTransposeWorld is used to rotate normals
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                Effect.Parameters["ModelTexture"]?.SetValue(Texture);
                Effect.Parameters["Time"]?.SetValue(time);

                modelMesh.Draw();
            }

            base.Draw(gameTime);

        }
        public override void CollidedWith(Player player)
        {
            if(Death) return;
            
            CurPosition = PrevPosition;
            if(TimeSinceLastHit > TimePerHit)
            {
                base.CollidedWith(player);
                TimeSinceLastHit = 0;
            }

        }
        public override void CollidedWith(Obstacles.Obstacle obstacle)
        {
            CurPosition = PrevPosition;
        }
        public override void CollidedWith(Maps.Wall wall)
        {
            CurPosition = PrevPosition;
        }
        public override void RemoveFromLife(float amount) 
        {
            base.RemoveFromLife(amount);
            Death = CurrentLife == 0;
        }
        public override bool PositionSet()
        {
            return PosSet;
        }
        private void IfDeathUpdate(GameTime gameTime)
        {
            TimeSinceDeath += (float) gameTime.ElapsedGameTime.TotalSeconds;
            if(TimeSinceDeath > TimeToRespawn)
            {
                MapRepo.CurrentMap.SetPositionOfEnemy(this);
                TimeSinceDeath = 0;
                CurrentLife = TotalLife;
                Death = false;
            }
        }
    }
}