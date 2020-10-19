using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.Samples.Cameras;
using TGC.MonoGame.InsaneGames.Maps;
using TGC.MonoGame.InsaneGames.Entities;
using TGC.MonoGame.InsaneGames.Weapons;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;
using TGC.MonoGame.InsaneGames.Utils;
using System.Linq;
using TGC.MonoGame.InsaneGames.Entities.Collectibles;
using TGC.MonoGame.InsaneGames.Entities.Enemies;

namespace TGC.MonoGame.InsaneGames
{
    /// <summary>
    ///     Esta es la clase principal  del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);
            // Descomentar para que el juego sea pantalla completa.
            //Graphics.IsFullScreen = true;
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";

            ContentManager.MakeInstance(Content);
        }

        private GraphicsDeviceManager Graphics { get; }
        public Camera Camera { get; private set; }

        private Map Map { get; set; }
        private Player Player { get; set; }

        private Weapon Weapon { get; set; }

        private MenuUI MenuUI { get; set; }

        public const int ST_MENU = 0;
        public const int ST_LEVEL_1 = 1;
        public int status = ST_MENU;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: todo procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.
            IsMouseVisible = true;
            // Apago el backface culling.
            // Esto se hace por un problema en el diseno del modelo del logo de la materia.
            // Una vez que empiecen su juego, esto no es mas necesario y lo pueden sacar.
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            // Seria hasta aca.

            
            Point center_point;
            center_point.Y = Graphics.GraphicsDevice.Viewport.Height / 2;
            center_point.X = Graphics.GraphicsDevice.Viewport.Width / 2;

            Camera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 20, 0), center_point);            
            Player = new Player(this, Camera, Matrix.CreateTranslation(1, 5, 60));
            Map = CreateMap(Player);
            MapRepo.CurrentMap = Map;
            Map.Initialize(this);
            // Weapon = new MachineGun();
            // Weapon = new Shotgun(); No funciona
            // Weapon = new Handgun();
            // Weapon.Initialize(this);
            Player.Initialize(this);

            MenuUI = new MenuUI();

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el
        ///     procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            Map.Load(GraphicsDevice);
            Player.Load();
            //Weapon.Load();

            MenuUI.Load(GraphicsDevice);

            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.

            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Salgo del juego.
                Exit();

            switch (status)
            {
                case ST_MENU:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        status = ST_LEVEL_1;
                    break;

                case ST_LEVEL_1:
                    Camera.Update(gameTime);
                    Map.Update(gameTime);
                    break;
            }

            //Camera.Update(gameTime);

            //Map.Update(gameTime);
            //Weapon.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logia de renderizado del juego.
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            switch (status)
            {
                case ST_MENU:
                    MenuUI.Draw(gameTime);
                    break;
                case ST_LEVEL_1:
                    Map.Draw(gameTime);
                    break;
            }

            //Weapon.Draw(gameTime);
            base.Draw(gameTime);
        }


        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }

        //Temporal
        private Map CreateMap(Player player)
        {

            List<Room> rooms = CreateRooms();

            List<Obstacle> obstacles = CreateObstacles();

            List<Enemy> enemies = CreateEnemies();

            Point center_point;
            center_point.Y = Graphics.GraphicsDevice.Viewport.Height / 2;
            center_point.X = Graphics.GraphicsDevice.Viewport.Width / 2;

            return new Map(rooms.ToArray(), enemies.ToArray(), obstacles.ToArray(), player);
        }

        private List<Room> CreateRooms()
        {
            List<Room> rooms = new List<Room>();

            var wallsEffect = new BasicEffect(GraphicsDevice);
            wallsEffect.TextureEnabled = true;
            wallsEffect.Texture = ContentManager.Instance.LoadTexture2D("Wall/Wall2");
            var floorEffect = new BasicEffect(GraphicsDevice);
            floorEffect.TextureEnabled = true;
            floorEffect.Texture = ContentManager.Instance.LoadTexture2D("Checked-Floor/Checked-Floor");
            var ceilingEffect = new BasicEffect(GraphicsDevice);
            ceilingEffect.TextureEnabled = true;
            ceilingEffect.Texture = ContentManager.Instance.LoadTexture2D("ceiling/Ceiling");
            var initialDict = new Dictionary<WallId, BasicEffect> { { WallId.Ceiling, ceilingEffect }, { WallId.Floor, floorEffect }, { WallId.Right, wallsEffect }, { WallId.Left, wallsEffect }, { WallId.Back, wallsEffect } };
            var dictCorridorInZ = new Dictionary<WallId, BasicEffect> { { WallId.Ceiling, ceilingEffect }, { WallId.Floor, floorEffect }, { WallId.Right, wallsEffect }, { WallId.Left, wallsEffect } };
            var dictCorridorInX = new Dictionary<WallId, BasicEffect> { { WallId.Ceiling, ceilingEffect }, { WallId.Floor, floorEffect }, { WallId.Back, wallsEffect }, { WallId.Front, wallsEffect } };
            var dictCorner1 = new Dictionary<WallId, BasicEffect> { { WallId.Ceiling, ceilingEffect }, { WallId.Floor, floorEffect }, { WallId.Left, wallsEffect }, { WallId.Front, wallsEffect } };
            var dictCorner2 = new Dictionary<WallId, BasicEffect> { { WallId.Ceiling, ceilingEffect }, { WallId.Floor, floorEffect }, { WallId.Right, wallsEffect }, { WallId.Front, wallsEffect } };
            var dictCorner3 = new Dictionary<WallId, BasicEffect> { { WallId.Ceiling, ceilingEffect }, { WallId.Floor, floorEffect }, { WallId.Right, wallsEffect }, { WallId.Back, wallsEffect } };
            var dictCorner4 = new Dictionary<WallId, BasicEffect> { { WallId.Ceiling, ceilingEffect }, { WallId.Floor, floorEffect }, { WallId.Left, wallsEffect }, { WallId.Back, wallsEffect } };
            var finalDict = new Dictionary<WallId, BasicEffect> { { WallId.Ceiling, ceilingEffect }, { WallId.Floor, floorEffect }, { WallId.Left, wallsEffect }, { WallId.Right, wallsEffect }, { WallId.Front, wallsEffect } };
            //var textRepet = new Dictionary<WallId, (float, float)> { { WallId.Front, (2, 1) } };

            var life = new Life(Matrix.CreateTranslation(10, 0, -500));
            var armor = new Armor(Matrix.CreateTranslation(40, 5, -500));
            var life2 = new Life(Matrix.CreateTranslation(840, 0, -320));
            var armor2 = new Armor(Matrix.CreateRotationY(MathHelper.ToRadians(180f)) * Matrix.CreateTranslation(810, 5, -320));
            var life3 = new Life(Matrix.CreateTranslation(750, 0, -50));
            var armor3 = new Armor(Matrix.CreateRotationY(MathHelper.ToRadians(180f)) * Matrix.CreateTranslation(780, 5, -50));
            var life4 = new Life(Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(350, 0, 70));
            var armor4 = new Armor(Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(350, 5, 90));

            rooms.Add(new Box(initialDict, new Vector3(250, 100, 250), new Vector3(0, 50, 0), new Collectible[] { }, false));
            rooms.Add(new Box(dictCorridorInZ, new Vector3(250, 100, 250), new Vector3(0, 50, -250), new Collectible[] { }));
            rooms.Add(new Box(dictCorner1, new Vector3(250, 100, 250), new Vector3(0, 50, -500), new Collectible[] { life, armor }));
            rooms.Add(new Box(dictCorridorInX, new Vector3(250, 100, 250), new Vector3(250, 50, -500), new Collectible[] { }));
            rooms.Add(new Box(dictCorridorInX, new Vector3(250, 100, 250), new Vector3(500, 50, -500), new Collectible[] { }));
            rooms.Add(new Box(dictCorner2, new Vector3(250, 100, 250), new Vector3(750, 50, -500), new Collectible[] { life2, armor2 }));
            rooms.Add(new Box(dictCorridorInZ, new Vector3(250, 100, 250), new Vector3(750, 50, -250), new Collectible[] { }));
            rooms.Add(new Box(dictCorner3, new Vector3(250, 100, 250), new Vector3(750, 50, 0), new Collectible[] { life3, armor3 }));
            rooms.Add(new Box(dictCorridorInX, new Vector3(250, 100, 250), new Vector3(500, 50, 0), new Collectible[] { }));
            rooms.Add(new Box(dictCorner4, new Vector3(250, 100, 250), new Vector3(250, 50, 0), new Collectible[] { life4, armor4 }));
            rooms.Add(new Box(finalDict, new Vector3(250, 100, 250), new Vector3(250, 50, -250), new Collectible[] { }));

            return rooms;
        }

        private List<Enemy> CreateEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();

            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));
            enemies.Add(new TGCito(Player));

            return enemies;
        }

        private List<Obstacle> CreateObstacles()
        {
            List<Obstacle> obstacles = new List<Obstacle>();

            // Boxes
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(3, Matrix.CreateTranslation(-50, 0, -50), true)).ToList();
            obstacles.Add(ObstaclesFactory.WoodenCrate(Matrix.CreateTranslation(-25, 25, -50)));

            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(2, Matrix.CreateTranslation(87, 0, -200), true)).ToList();
            obstacles.Add(ObstaclesFactory.WoodenCrate(Matrix.CreateTranslation(112, 25, -200)));

            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(3, Matrix.CreateTranslation(-112, 0, -350), true)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(3, Matrix.CreateTranslation(-112, 25, -350), true)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(3, Matrix.CreateTranslation(-112, 50, -350), true)).ToList();

            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(7, Matrix.CreateTranslation(363, 0, -113), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(7, Matrix.CreateTranslation(363, 25, -113), false)).ToList();

            // Barriers
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBarriersObstaclesInLine(3, Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(200, 0, -475), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBarriersObstaclesInLine(3, Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(350, 0, -605), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBarriersObstaclesInLine(3, Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(500, 0, -475), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBarriersObstaclesInLine(3, Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(650, 0, -605), false)).ToList();

            // Sawhorses
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainSawhorsesObstaclesInLine(2, Matrix.CreateTranslation(665, 0, -300), true)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainSawhorsesObstaclesInLine(2, Matrix.CreateTranslation(760, 0, -175), true)).ToList();

            // Cones
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainConesObstaclesInLine(8, Matrix.CreateTranslation(620, 0, -57), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainConesObstaclesInLine(8, Matrix.CreateTranslation(540, 0, -115), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainConesObstaclesInLine(8, Matrix.CreateTranslation(460, 0, -57), false)).ToList();

            return obstacles;
        }
    }
}