using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.InsaneGames.Entities.Enemies
{
    class TGCito : Enemy
    {
        private Player playerReference { set; get; }
        private const string ModelName = "tgcito/tgcito-classic";
        static private Model Model;
        private readonly Vector3 HitboxSize = new Vector3(10, 40, 10);
        private readonly float TimePerHit = 2;
        private Matrix Misalignment { get; }
        private Boolean Death = false;
        private float TimeSinceLastHit = 0;
        public TGCito(Player player, Matrix? spawnPoint = null, Matrix? scaling = null, float life = 100, float damage = 5)
        {
            playerReference = player;
            Misalignment = Matrix.CreateTranslation(0, 44.5f, 0) * scaling.GetValueOrDefault(Matrix.CreateScale(0.2f));
            if(spawnPoint.HasValue)
            {
                position = spawnPoint.Value;
                UpVertex = spawnPoint.Value.Translation + new Vector3(HitboxSize.X / 2, HitboxSize.Y, HitboxSize.Z / 2);
                BottomVertex = spawnPoint.Value.Translation - new Vector3(HitboxSize.X / 2, 0, HitboxSize.Z / 2);
            }
            floorEnemy = true;
            Life = life;
            Damage = damage;
        }

        public bool isPlayerNear()
        {
            float detection_distance = 100f;
            Vector3 playerPosition = playerReference.NewPosition;
            Vector3 enemyPosition = this.position.Value.Translation;
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
                //Logica de respawn
                return;
            }
            else if (isPlayerNear())
            {   //Detectado
                Vector3 vec_to_player = playerReference.NewPosition - this.position.Value.Translation;
                vec_to_player.Y = 0;
                // Rotar el tgcito
                float rot_speed = MathHelper.ToRadians(200f * Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds));
                float angle = getAngleBetweenVectorsInPlaneXZ(this.position.Value.Backward, playerReference.NewPosition - this.position.Value.Translation);
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
                position = Matrix.CreateRotationY(rot_speed) * position.Value;

                // Empezar a mover el tgcito
                if (_mirandoPlayer)
                {
                    vec_to_player.Normalize();
                    float enemy_speed = 70f * Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
                    position = position * Matrix.CreateTranslation(vec_to_player * enemy_speed);
                }
                UpVertex = position.Value.Translation + new Vector3(HitboxSize.X / 2, HitboxSize.Y, HitboxSize.Z / 2);
                BottomVertex = position.Value.Translation - new Vector3(HitboxSize.X / 2, 0, HitboxSize.Z / 2);
            } 
            else 
            {
                _mirandoPlayer = false;
            }
            TimeSinceLastHit += (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Load()
        {
            if(Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);
        }
        public override void Draw(GameTime gameTime)
        {
            if(!position.HasValue)
                throw new System.Exception("The position of the TGCito was not set");
            var world = Misalignment * position.Value; 
            Model.Draw(world, Maps.MapRepo.CurrentMap.Camera.View, Maps.MapRepo.CurrentMap.Camera.Projection);
        }
        public override void CollidedWith(Player player)
        {
            if(Death) return;
            
            if(TimeSinceLastHit > TimePerHit)
            {
                base.CollidedWith(player);
                TimeSinceLastHit = 0;
            }

        }
        public override void CollidedWith(Obstacles.Obstacle obstacle)
        {
        }
        public override void RemoveFromLife(float amount) 
        {
            base.RemoveFromLife(amount);
            Death = Life == 0;
        }
    }
}