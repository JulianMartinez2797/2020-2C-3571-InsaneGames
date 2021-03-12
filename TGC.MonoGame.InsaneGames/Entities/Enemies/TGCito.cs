using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Maps;
using System.Linq;

namespace TGC.MonoGame.InsaneGames.Entities.Enemies
{
    class TGCito : Enemy
    {
        private Effect DeathEffect { get; set; }

        private Effect BlackEffect;

        private Texture2D Texture { get; set; }
        private Matrix CurPosition, PrevPosition;
        private Player playerReference { set; get; }
        private const string ModelName = "tgcito/tgcito-classic";
        protected Model Model { get; set; }
        private readonly Vector3 HitboxSize = new Vector3(10, 16, 10);
        private readonly float TimePerHit = 2, TimeToRespawn = 4;
        private Matrix Misalignment { get; }
        private Boolean Death = false, PosSet = false;
        private float TimeSinceLastHit = 0, TimeSinceDeath = 0, AnimationTime = 0;

        override public Vector3 Position
        {
            get { return CurPosition.Translation; }
            set
            {
                CurPosition = Misalignment * Matrix.CreateTranslation(value);
                hitbox_new_value = value;
                hitboxNeedsInitialize = true;
                PosSet = true;
            }
        }
        private void setHitboxDimensions(Vector3 val){
            UpVertex = CurPosition.Translation + HitboxSize / 2;
            BottomVertex = CurPosition.Translation - HitboxSize / 2;
            hitboxNeedsInitialize = false;
        }
        public TGCito(Player player, Matrix? spawnPoint = null, Matrix? scaling = null, float life = 100, float damage = 5)
        {
            playerReference = player;
            Misalignment = Matrix.CreateTranslation(0, 44.5f, 0) * scaling.GetValueOrDefault(Matrix.CreateScale(0.2f));
            if (spawnPoint.HasValue)
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
        public bool isPlayerEnoughNearToDraw()
        {
            float detection_distance = 700f;
            Vector3 playerPosition = playerReference.NewPosition;
            Vector3 enemyPosition = this.CurPosition.Translation;
            return Vector3.Distance(playerPosition, enemyPosition) < detection_distance;
        }


        // Esto se tiene que cambiar por CreateLookAt
        private float getAngleBetweenVectorsInPlaneXZ(Vector3 vecA, Vector3 vecB)
        {
            Vector2 vecA2 = new Vector2(vecA.X, vecA.Z);
            Vector2 vecB2 = new Vector2(vecB.X, vecB.Z);

            float dotResult = Vector2.Dot(vecA2, vecB2);
            float vecA2Length = vecA2.Length();
            float vecB2Length = vecB2.Length();
            if (vecA2Length == 0 || vecB2Length == 0)
                return 0f;
            double angle = Math.Acos(dotResult / (vecA2Length * vecB2Length));
            if (angle == double.NaN)
                return 0f;
            Vector3 cross = Vector3.Cross(vecA, vecB);
            if (cross.Y < 0)
                angle *= -1;
            float angleF = Convert.ToSingle(angle);
            return angleF;
        }
        private bool _mirandoPlayer = false;
        private bool hitboxNeedsInitialize = false;
        private Vector3 hitbox_new_value;
        public override void Update(GameTime gameTime)
        {
            
            if (Death)
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
            TimeSinceLastHit += (float)gameTime.ElapsedGameTime.TotalSeconds;

            MapRepo.CurrentMap.UpdateIluminationParametersInEffect(DeathEffect);
            if (hitboxNeedsInitialize){
                setHitboxDimensions(hitbox_new_value);
            }
        }

        public override void Load(GraphicsDevice gd)
        {
            if (Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);

            Texture = ((BasicEffect)Model.Meshes.FirstOrDefault()?.MeshParts.FirstOrDefault()?.Effect)?.Texture;
            DeathEffect = ContentManager.Instance.LoadEffect("DeathDissolve");

            BlackEffect = ContentManager.Instance.LoadEffect("ColorShader");

            MapRepo.CurrentMap.AddIluminationParametersToEffect(DeathEffect);
            DeathEffect.Parameters["KAmbient"].SetValue(0.3f);
            DeathEffect.Parameters["KDiffuse"].SetValue(0.6f);
            DeathEffect.Parameters["KSpecular"].SetValue(0.1f);
            DeathEffect.Parameters["shininess"].SetValue(8.0f);


        }
        float time = 0;
        public override void Draw(GameTime gameTime)
        {
            if (isPlayerEnoughNearToDraw())
            {
                float minY = float.MaxValue;
                float maxY = float.MinValue;
                var world = CurPosition;
                var view = Maps.MapRepo.CurrentMap.Camera.View;
                var projection = Maps.MapRepo.CurrentMap.Camera.Projection;
                var mesh = Model.Meshes.FirstOrDefault();
                if (mesh != null)
                {
                    foreach (var part in mesh.MeshParts)
                    {
                        int count = part.NumVertices;
                        VertexPositionNormalTexture[] positions = new VertexPositionNormalTexture[count];
                        part.VertexBuffer.GetData(positions);
                        foreach (var position in positions)
                        {
                            if (position.Position.Z < minY)
                                minY = position.Position.Z;
                            if (position.Position.Z > maxY)
                                maxY = position.Position.Z;
                        }
                        part.Effect = DeathEffect;
                        var worldMatrix = world;
                        DeathEffect.Parameters["World"].SetValue(worldMatrix);
                        DeathEffect.Parameters["View"].SetValue(view);
                        DeathEffect.Parameters["Projection"].SetValue(projection);
                        DeathEffect.Parameters["ModelTexture"].SetValue(Texture);
                        DeathEffect.Parameters["Time"]?.SetValue(AnimationTime);
                        DeathEffect.Parameters["minY"]?.SetValue(minY);
                        DeathEffect.Parameters["maxY"]?.SetValue(maxY);
                        DeathEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                    }
                    mesh.Draw();
                }
            }
        }

        public override void DrawBlack(GameTime gameTime)
        {
            if (isPlayerEnoughNearToDraw())
            {
                BlackEffect.Parameters["colorTarget"].SetValue(Color.Black.ToVector4());
                float minY = float.MaxValue;
                float maxY = float.MinValue;
                var world = CurPosition;
                var view = Maps.MapRepo.CurrentMap.Camera.View;
                var projection = Maps.MapRepo.CurrentMap.Camera.Projection;
                var mesh = Model.Meshes.FirstOrDefault();
                if (mesh != null)
                {
                    foreach (var part in mesh.MeshParts)
                    {
                        int count = part.NumVertices;
                        VertexPositionNormalTexture[] positions = new VertexPositionNormalTexture[count];
                        part.VertexBuffer.GetData(positions);
                        foreach (var position in positions)
                        {
                            if (position.Position.Z < minY)
                                minY = position.Position.Z;
                            if (position.Position.Z > maxY)
                                maxY = position.Position.Z;
                        }
                        part.Effect = BlackEffect;
                        var worldMatrix = world;
                        BlackEffect.Parameters["World"].SetValue(worldMatrix);
                        BlackEffect.Parameters["View"].SetValue(view);
                        BlackEffect.Parameters["Projection"].SetValue(projection);
                    }
                    mesh.Draw();
                }
            }
        }
        public override void CollidedWith(Player player)
        {
            if (Death) return;

            CurPosition = PrevPosition;
            if (TimeSinceLastHit > TimePerHit)
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
            TimeSinceDeath += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Rotate TGCito
            if (TimeSinceDeath < 0.85f)
            {
                CurPosition = Matrix.CreateRotationX(-0.03f) * CurPosition;
                CurPosition = CurPosition * Matrix.CreateTranslation(0, -0.1f, 0);
            }
            //Start timer for Shader: DeathDissolve
            if (TimeSinceDeath > 1.3f)
            {
                AnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (TimeSinceDeath > TimeToRespawn)
            {
                MapRepo.CurrentMap.SetPositionOfEnemy(this);
                TimeSinceDeath = 0;
                CurrentLife = TotalLife;
                Death = false;
            }
        }
    }
}