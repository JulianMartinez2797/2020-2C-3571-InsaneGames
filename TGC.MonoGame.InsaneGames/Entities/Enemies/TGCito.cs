using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.MonoGame.InsaneGames.Entities.Enemies
{
    class TGCito : Enemy
    {
        private Matrix CurPosition, PrevPosition;
        private Player playerReference { set; get; }
        private const string ModelName = "tgcito/tgcito-classic";
        static private Model Model;
        private readonly Vector3 HitboxSize = new Vector3(10, 16, 10);
        private readonly float TimePerHit = 2;
        private Matrix Misalignment { get; }
        private Boolean Death = false, PosSet = false;
        private float TimeSinceLastHit = 0;
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
            Life = life;
            Damage = damage;
        }

        public bool isPlayerNear()
        {
            float detection_distance = 100f;
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
                //Logica de respawn
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
        }

        public override void Load()
        {
            if(Model is null)
                Model = ContentManager.Instance.LoadModel(ModelName);
        }
        public override void Draw(GameTime gameTime)
        {
            var world = CurPosition; 
            Model.Draw(world, Maps.MapRepo.CurrentMap.Camera.View, Maps.MapRepo.CurrentMap.Camera.Projection);
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
            Death = Life == 0;
        }
        public override bool PositionSet()
        {
            return PosSet;
        }
    }
}