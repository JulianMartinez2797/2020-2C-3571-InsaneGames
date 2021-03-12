using System;
using System.Collections.Generic;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using TGC.MonoGame.InsaneGames.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Cameras;
using TGC.MonoGame.InsaneGames.Entities.Bullets;
using TGC.MonoGame.InsaneGames.Entities.Other;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class Map : IDrawable
    {
        private Room[] Rooms;
        private Enemy[] Enemies;
        private Random Random;
        public Player Player;
        private List<Bullet> Bullets;
        private InfoUI UI;
        public bool keyFound = false;
        public Camera Camera => Player.Camera;

        private GraphicsDevice GraphicsDevice;
        private RenderTarget2D MainSceneRenderTarget;
        private RenderTarget2D FirstPassBloomRenderTarget;
        private RenderTarget2D SecondPassBloomRenderTarget;
        private FullScreenQuad FullScreenQuad;
        private Effect IntegrateEffect { get; set; }
        private Effect BlurEffect { get; set; }

        private const int PassCount = 2;

        private Lamp Lamp { get; set; }
        private Lamp Lamp2 { get; set; }
        private TGCGame _game;

        public Map(Room[] rooms, Enemy[] enemies, Player player) 
        {
            Rooms = rooms;
            Enemies = enemies;
            Random = new Random();
            Player = player;
            Bullets = new List<Bullet>();
            UI = new InfoUI();
            Lamp = new Lamp(Matrix.CreateTranslation(1200, 0, 50));
            Lamp2 = new Lamp(Matrix.CreateTranslation(1380, -10, 180));
        }

        public override void Initialize(TGCGame game)
        {
            _game = game;
            base.Initialize(game);

            Player.Initialize(game);

            foreach (var room in Rooms)
                room.Initialize(game);

            Array.ForEach(Enemies, (enemy) => {
                enemy.Initialize(game);
                if(enemy.PositionSet()) return;
                this.SetPositionOfEnemy(enemy);
            });

            UI.Initialize(game);

        }

        public override void Draw(GameTime gameTime)
        {
            #region Pass 1

            // Use the default blend and depth configuration
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Set the main render target, here we'll draw the base scene
            GraphicsDevice.SetRenderTarget(MainSceneRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            Player.Draw(gameTime);

            foreach (var room in Rooms)
                room.Draw(gameTime);

            foreach (var bullet in Bullets)
                if (bullet.isInitialized())
                    bullet.Draw(gameTime);
                else {
                    bullet.Initialize(_game);
                    bullet.Load();
                }
            
            foreach (var enemy in Enemies)
                enemy.Draw(gameTime);
            
            Lamp.Draw(gameTime);

            Lamp2.Draw(gameTime);

            UI.Draw(gameTime, Player);

            #endregion

            #region Pass 2

            // Set the render target as our bloomRenderTarget, we are drawing the bloom color into this texture
            GraphicsDevice.SetRenderTarget(FirstPassBloomRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

            if (Game.godMode)
            {
                Player.Draw(gameTime);
            }
            else
            {
                Player.DrawBlack(gameTime);
            }

            foreach (var room in Rooms)
                room.DrawBlack(gameTime);
            
            foreach (var enemy in Enemies)
                enemy.DrawBlack(gameTime);
            
            Lamp.DrawBloom(gameTime);

            Lamp2.DrawBloom(gameTime);

            UI.Draw(gameTime, Player);

            #endregion

            #region Multipass Bloom

            // Now we apply a blur effect to the bloom texture
            // Note that we apply this a number of times and we switch
            // the render target with the source texture
            // Basically, this applies the blur effect N times
            BlurEffect.CurrentTechnique = BlurEffect.Techniques["Blur"];

            var bloomTexture = FirstPassBloomRenderTarget;
            var finalBloomRenderTarget = SecondPassBloomRenderTarget;

            for (var index = 0; index < PassCount; index++)
            {
                //Exchange(ref SecondaPassBloomRenderTarget, ref FirstPassBloomRenderTarget);

                // Set the render target as null, we are drawing into the screen now!
                GraphicsDevice.SetRenderTarget(finalBloomRenderTarget);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

                BlurEffect.Parameters["baseTexture"].SetValue(bloomTexture);
                FullScreenQuad.Draw(BlurEffect);

                if (index != PassCount - 1)
                {
                    var auxiliar = bloomTexture;
                    bloomTexture = finalBloomRenderTarget;
                    finalBloomRenderTarget = auxiliar;
                }
            }

            #endregion

            #region Final Pass

            // Set the depth configuration as none, as we don't use depth in this pass
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            // Set the render target as null, we are drawing into the screen now!
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            // Set the technique to our blur technique
            // Then draw a texture into a full-screen quad
            // using our rendertarget as texture
            IntegrateEffect.CurrentTechnique = IntegrateEffect.Techniques["Integrate"];
            IntegrateEffect.Parameters["baseTexture"]?.SetValue(MainSceneRenderTarget);
            IntegrateEffect.Parameters["bloomTexture"]?.SetValue(FirstPassBloomRenderTarget);
            FullScreenQuad.Draw(IntegrateEffect);

            #endregion

        }
        public override void Load(GraphicsDevice gd) 
        {
            GraphicsDevice = gd;

            IntegrateEffect = ContentManager.Instance.LoadEffect("Bloom");

            // Create a full screen quad to post-process
            FullScreenQuad = new FullScreenQuad(GraphicsDevice);

            // Load the blur effect to blur the bloom texture
            BlurEffect = ContentManager.Instance.LoadEffect("GaussianBlur");
            BlurEffect.Parameters["screenSize"]
                .SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));


            // Create render targets. 
            // MainRenderTarget is used to store the scene color
            // BloomRenderTarget is used to store the bloom color and switches with MultipassBloomRenderTarget
            // depending on the pass count, to blur the bloom color
            MainSceneRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0,
                RenderTargetUsage.DiscardContents);
            FirstPassBloomRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0,
                RenderTargetUsage.DiscardContents);
            SecondPassBloomRenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None, 0,
                RenderTargetUsage.DiscardContents);

            UI.Load(gd);

            Lamp.Load();

            Lamp2.Load();

            Player.Load();

            foreach (var room in Rooms)
                room.Load();

            foreach (var enemy in Enemies)
                enemy.Load(gd);
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var room in Rooms)
            {
                room.Update(gameTime);
                Boolean playerInRoom = false;
                if(room.IsInRoom(Player.NewPosition))
                {
                    
                    Player.Update(gameTime);
                    
                    var collidedWall = room.CollidesWithWall(Player.BottomVertex, Player.UpVertex);
                    if (!Game.godMode)
                    {
                        Player.CollidedWith(collidedWall);
                    }
                    room.CheckCollectiblesCollision(Player);
                    room.CheckObstacleCollision(Player);
                    playerInRoom = true;
                }

                var bullets = Bullets.FindAll(bullet => room.IsInRoom(bullet.BottomVertex) || room.IsInRoom(bullet.UpVertex));
                var enemies = Array.FindAll(Enemies, enemy => room.IsInRoom(enemy.Position));
                bullets.ForEach(b => b.Update(gameTime));
                foreach(var enemy in enemies)
                {
                    enemy.Update(gameTime);
                    var collidedWall = room.CollidesWithWall(enemy.BottomVertex, enemy.UpVertex);
                    if(!(collidedWall is null)) enemy.CollidedWith(collidedWall);
                    room.CheckObstacleCollision(enemy);
                    if(playerInRoom && Player.CollidesWith(enemy.BottomVertex, enemy.UpVertex) && !Game.godMode)
                    { 
                        enemy.CollidedWith(Player);
                    }
                    var collidedBullets = bullets.FindAll(bullets => bullets.CollidesWith(enemy.BottomVertex, enemy.UpVertex));
                    collidedBullets.ForEach(b => b.CollidedWith(enemy));
                }
                bullets.FindAll(b => room.CollidesWithWall(b.BottomVertex, b.UpVertex) != null).ForEach(b => b.CollidedWith());
                bullets.ForEach(b => room.CheckObstacleCollision(b));
            }
            Lamp.Update(gameTime);
            Lamp2.Update(gameTime);
        }
        public void AddBullet(Bullet bullet)
        {
            Bullets.Add(bullet);
        }

        public bool playerIsDead()
        {
            return Player.Life <= 0;
        }
        public void playerFoundKey()
        {
            keyFound = true;
        }

        public void SetPositionOfEnemy(Enemy enemy, int maxTries = 0)
        {
            var infiteTries = maxTries == 0; 
            int i = 0;
            for(; infiteTries || i < maxTries; i++)
            {
                var room = Rooms[Random.Next(0, Rooms.Length)];
                if(!room.Spawnable)
                    continue;
                var spawn = room.SpawnableSpace().GetSpawnPoint(enemy.floorEnemy);
                
                if(room.CollidesWithWall(enemy.BottomVertex + spawn, enemy.UpVertex + spawn) is null)
                    enemy.Position = spawn;
                else
                    continue;
                break;
            }
            if(!infiteTries && i > maxTries) throw new Exception("Position of enemy could not be set in amount tries");
        }

        public void Reset()
        {
            keyFound = false;
            Player.NewPosition = new Vector3(1125, 20, 125);
            Player.LastPosition = new Vector3(1125, 20, 125);
            Player.Reset();
            Array.ForEach(Enemies, (enemy) => {
                enemy.Reset();
                this.SetPositionOfEnemy(enemy);
            });
        }

        public Effect AddIluminationParametersToEffect(Effect effect)
        {
            effect.Parameters["ambientColor"].SetValue(new Vector3(1f, 1f, 1f));
            effect.Parameters["diffuseColor"].SetValue(new Vector3(1f, 1f, 1f));
            effect.Parameters["specularColor"].SetValue(new Vector3(1f, 1f, 1f));

            effect.Parameters["constant"]?.SetValue(1.0f);
            effect.Parameters["linearTerm"]?.SetValue(0.007f);
            effect.Parameters["quadratic"]?.SetValue(0.0002f);

            return effect;
        }

        public Effect UpdateIluminationParametersInEffect(Effect effect)
        {
            var pointsLightsPosition = new Vector3[3];
            var cameraPosition = MapRepo.CurrentMap.Camera.Position;
            var lightPlayerPosition = new Vector3(cameraPosition.X, 0, cameraPosition.Z);
            var lightLampPosition = new Vector3(1200, -20, 50);
            var lightLampPosition2 = new Vector3(1380, -20, 180);
            pointsLightsPosition[0] = lightPlayerPosition;
            pointsLightsPosition[1] = lightLampPosition;
            pointsLightsPosition[2] = lightLampPosition2;
            effect.Parameters["lightsPositions"]?.SetValue(pointsLightsPosition);
            effect.Parameters["eyePosition"]?.SetValue(cameraPosition);
            return effect;
        }
    }
}