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
        private Matrix Misalignment { get; }
        public TGCito(Player player, Matrix? spawnPoint = null, Matrix? scaling = null, float life = 100, float damage = 5)
        {
            playerReference = player;
            Misalignment = Matrix.CreateTranslation(0, 44.5f, 0) * scaling.GetValueOrDefault(Matrix.CreateScale(0.2f));
            if(spawnPoint.HasValue)
                position = spawnPoint.Value;
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
            // Calculo manual
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
            //Math.Atan2(b.Y - a.Y,b.X - a.X);
            // NO FUNCIONO double angle = Math.Atan2(vecB2.Y - vecA2.Y, vecB2.X - vecA2.X);
            // NO FUNCIONO double angle = Math.Atan2(vecA2.Y - vecB2.Y, vecA2.X - vecB2.X);
            // NO FUNCIONO double angle = Math.Atan2(vecB2.X - vecA2.X, vecB2.Y - vecA2.Y);
           // double angle = Math.Atan2(vecA2.X * vecB2.Y - vecA2.Y * vecB2.X, vecA2.X * vecB2.X + vecA2.Y * vecB2.Y);
            float angleF = Convert.ToSingle(angle);
            // angle = MathHelper.ToRadians(angleF);
            return angleF;

// cross = crossProduct(Va, Vb);
// if (dotProduct(Vn, cross) < 0) { // Or > 0
//   angle = -angle;
// }
            //float cross = Vector2.Multiply(vecA2, vecB2);

            //return angleF;
        }
        public override void Update(GameTime gameTime)
        {
            if (isPlayerNear())
            {   //Detectado
                Vector3 vec_to_player = playerReference.NewPosition - this.position.Value.Translation;
                
                // Rotar el tgcito
                float rot_speed = MathHelper.ToRadians(200f * Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds));
                float angle = getAngleBetweenVectorsInPlaneXZ(this.position.Value.Backward, playerReference.NewPosition - this.position.Value.Translation);
                if (float.IsNaN(angle))
                    angle = 0f;
                if (Math.Abs(angle) < rot_speed)
                    rot_speed = angle;
                else
                    rot_speed *= angle / Math.Abs(angle);
                float angle_in_degrees = MathHelper.ToDegrees(angle);
                position = Matrix.CreateRotationY(rot_speed) * position.Value;

                // Empezar a mover el tgcito
                vec_to_player.Normalize();
                float enemy_speed = 70f * Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
                position = position * Matrix.CreateTranslation(vec_to_player * enemy_speed);
            }
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
            Model.Draw(world, Game.Camera.View, Game.Camera.Projection);
        }

    }
}