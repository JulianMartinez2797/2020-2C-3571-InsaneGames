using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.InsaneGames.Cameras;
using TGC.MonoGame.InsaneGames.Maps;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;
using System;
using TGC.MonoGame.InsaneGames.Weapons;
namespace TGC.MonoGame.InsaneGames.Entities
{
    class Player : Entity, ObstacleCollisionable
    {
        public float Life { get; private set; } = 50;
        public float Armor { get; private set; } = 0;
        public Camera Camera { get; set; }
        /// <summary>
        ///     Aspect ratio, defined as view space width divided by height.
        /// </summary>
        public float AspectRatio { get; set; }

        /// <summary>
        ///     Distance to the far view plane.
        /// </summary>
        public float FarPlane { get; set; }

        /// <summary>
        ///     Field of view in the y direction, in radians.
        /// </summary>
        public float FieldOfView { get; set; }

        /// <summary>
        ///     Distance to the near view plane.
        /// </summary>
        public float NearPlane { get; set; }

        /// <summary>
        ///     Direction where the camera is looking.
        /// </summary>
        public Vector3 FrontDirection { get; set; }

        /// <summary>
        ///     The perspective projection matrix.
        /// </summary>
        public Matrix Projection { get; set; }

        /// <summary>
        ///     Position where the camera is located.
        /// </summary>
        public Vector3 NewPosition { get; set; }
        public Vector3 LastPosition { get; set; }

        /// <summary>
        ///     Represents the positive x-axis of the camera space.
        /// </summary>
        public Vector3 RightDirection { get; set; }

        /// <summary>
        ///     Vector up direction (may differ if the camera is reversed).
        /// </summary>
        public Vector3 UpDirection { get; set; }

        /// <summary>
        ///     The created view matrix.
        /// </summary>
        public Matrix View { get; set; }

        private float pitch;
        public List<Weapon> Weapons {get;set;}
        public Weapon CurrentWeapon {get;set;}
        // Angles
        private float yaw = -90f;
        public float MovementSpeed { get; set; } = 100f;
        public float MouseSensitivity { get; set; } = 5f;
        
        public Vector3 CameraCorrection { get; set; }
        public Vector3 LastBottomVertex { get; set;}
        public Vector3 LastUpVertex { get; set;}
        private readonly Vector3 HitboxSize = new Vector3(15, 19, 15);
        public Player(TGCGame game, Camera camera, float yPosition = 0, Matrix? scaling = null)
        {
            this.Camera = camera;
            NewPosition = Camera.Position + new Vector3(0, yPosition, 0);
            UpVertex = NewPosition + HitboxSize;
            BottomVertex = NewPosition - HitboxSize;

            // Remover
            Weapons = new List<Weapon>();
            Weapon machineGun = new MachineGun();
            Weapon handgun = new Handgun();
            Weapon rpg7 = new Rpg7();
            Weapon shotgun = new Shotgun();
            Weapons.Add(handgun);
            Weapons.Add(machineGun);
            Weapons.Add(shotgun);
            Weapons.Add(rpg7);
            CurrentWeapon = Weapons[0];
        }

        private void CalculateView()
        {
            View = Matrix.CreateLookAt(NewPosition, NewPosition + FrontDirection, UpDirection);
        }

        /// <inheritdoc />
        public override void Load() 
        {
            foreach (Weapon weapon in Weapons)
            {
                weapon.Load();
            }
        }
        public override void Initialize(TGCGame game) 
        {
            foreach (Weapon weapon in Weapons)
            {
                weapon.Initialize(game);
            }
            base.Initialize(game);
        }

        bool explosion_happening = false;
        public override void Update(GameTime gameTime)
        {

            LastPosition = NewPosition;
            LastBottomVertex = BottomVertex;
            LastUpVertex = UpVertex;    
            
            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ProcessKeyboard(elapsedTime);

            
            this.Camera.Update(gameTime);

            //Les saco el componente Y para que no se mueva verticalmente
            FrontDirection = Vector3.Normalize(Camera.FrontDirection * new Vector3(1, 0, 1));
            RightDirection = Vector3.Normalize(Camera.RightDirection * new Vector3(1, 0, 1));
            UpDirection = Vector3.Normalize(Camera.UpDirection);

            CalculateView();

            Camera.Position = NewPosition + CameraCorrection;
            if (explosion_happening) {
                explosionEffect(Camera, gameTime, RightDirection);
            }
            //UpdatePlayerVectors();

            BottomVertex = NewPosition - HitboxSize;
            UpVertex = NewPosition + HitboxSize;
            CurrentWeapon.Update(gameTime, Camera.FrontDirection, Camera.Position);
        }

        public void generateExplosionEffect()
        {
            explosion_happening = true;
        }
        private void ChangeWeapon(int weaponIdx)
        {
            CurrentWeapon = Weapons[weaponIdx];
        }

        private void ProcessKeyboard(float elapsedTime)
        {
            var keyboardState = Keyboard.GetState();

            var currentMovementSpeed = MovementSpeed;
            if (keyboardState.IsKeyDown(Keys.LeftShift))
                currentMovementSpeed *= 5f;

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                NewPosition -= RightDirection * currentMovementSpeed * elapsedTime;
            } 
            else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                NewPosition += RightDirection * currentMovementSpeed * elapsedTime;
            }
            else if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                NewPosition += FrontDirection * currentMovementSpeed * elapsedTime;
            }
            else if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                NewPosition -= FrontDirection * currentMovementSpeed * elapsedTime;
            }
            if (keyboardState.IsKeyDown(Keys.D1))
            {
                ChangeWeapon(0); // Pistol
            }
            else if (keyboardState.IsKeyDown(Keys.D2))
            {
                ChangeWeapon(1); // Rifle
            }
            else if (keyboardState.IsKeyDown(Keys.D3))
            {
                ChangeWeapon(2); // Shotgun
            }
            else if (keyboardState.IsKeyDown(Keys.D4))
            {
                ChangeWeapon(3); // RPG
            }
        }

        Vector3 cameraOffset = new Vector3(0,0,0);
        float explosionTime = 0f;
        public void explosionEffect(Camera cam, GameTime time, Vector3 rDirection)
        {  
            float timeSeconds = (float)time.ElapsedGameTime.TotalSeconds;
            explosionTime += timeSeconds;
            float bounceSpeed = 35f;
            float bounceForce = 2.8f;
            float explosionDuration = 0.6f;
            
            if (MathF.Floor(explosionTime*bounceSpeed) % 2 == 0)
            {
                cameraOffset += bounceForce * rDirection;
            } 
            else {
                cameraOffset -= bounceForce * rDirection;
            }
            cam.Position += cameraOffset; 
            if (explosionTime > explosionDuration) {
                explosion_happening = false;
                explosionTime = 0f;
            }
        }

        public void CollidedWith(Wall wall)
        {
            if(wall is null)
                return;
            ResetPosition();
        }
        public void CollidedWith(Obstacle obstacle)
        {
            if(obstacle is null)
                return;
            ResetPosition();
        }

        public void ResetPosition(){
            NewPosition = LastPosition;
            BottomVertex = LastBottomVertex;
            UpVertex = LastUpVertex;
        }

        private void UpdatePlayerVectors()
        {
            // Calculate the new Front vector
            Vector3 tempFront;
            tempFront.X = MathF.Cos(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch));
            tempFront.Y = MathF.Sin(MathHelper.ToRadians(pitch));
            tempFront.Z = MathF.Sin(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch));

            FrontDirection = Vector3.Normalize(tempFront);

            // Also re-calculate the Right and Up vector
            // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            RightDirection = Vector3.Normalize(Vector3.Cross(FrontDirection, Vector3.Up));
            UpDirection = Vector3.Normalize(Vector3.Cross(RightDirection, FrontDirection));
        }
        public override void Draw(GameTime gameTime)
        {
            // Draw gun
            CurrentWeapon.Draw(gameTime);
        }
        public override void DrawBlack(GameTime gameTime)
        {
            // Draw gun
            CurrentWeapon.DrawBlack(gameTime);
        }
        public void AddToLife(float amount)
        {
            Life = Math.Min(Life + amount, 100);
        }
        public void RemoveFromLife(float amount)
        {
            Life = Math.Max(Life - amount, 0);
        }
        public void AddToArmor(float amount)
        {
            Armor = Math.Min(Armor + amount, 100);
        }
        public void RemoveFromArmor(float amount)
        {
            Armor = Math.Max(Armor - amount, 0);
        }
        public void BeAttacked(float damage)
        {
            if(Armor < damage)
                RemoveFromLife(damage - Armor);
            if(Armor != 0)
                RemoveFromArmor(damage);
        }
        public void Reset()
        {
            Life = 50;
            UpVertex = NewPosition + HitboxSize;
            BottomVertex = NewPosition - HitboxSize;
        }
    }
}