using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.Samples.Cameras;
using TGC.MonoGame.InsaneGames.Maps;
using TGC.MonoGame.InsaneGames.Entities;
using TGC.MonoGame.InsaneGames.Weapons;
using TGC.MonoGame.InsaneGames.Obstacles;
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

        private Weapon Weapon { get; set; }
        private SpriteBatch spriteBatch;
        private SpriteFont font;


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

            Camera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 20, 60), center_point);            
            Map = CreateMap();

            Map.Initialize(this);
            Weapon = new MachineGun();
            Weapon.Initialize(this);
            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el
        ///     procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            Map.Load();
            Weapon.Load();

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = ContentManager.Instance.LoadSpriteFont("Basic");

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

            Camera.Update(gameTime);

            Map.Update(gameTime);
            Weapon.Update(gameTime);

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

            Map.Draw(gameTime);
            Weapon.Draw(gameTime);


            Vector2 lifeStringPosition = new Vector2(10, GraphicsDevice.Viewport.Height - 50);
            Vector2 armorStringPosition = new Vector2(170, GraphicsDevice.Viewport.Height - 50);

            spriteBatch.Begin();
            // TODO: Take life and armor from player
            spriteBatch.DrawString(font, "LIFE: 100", lifeStringPosition, Color.Red);
            spriteBatch.DrawString(font, "ARMOR: 100", armorStringPosition, Color.DarkBlue);
            spriteBatch.End();

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
        private Map CreateMap()
        {

            List<Room> rooms = CreateRooms();

            List<Obstacle> obstacles = CreateObstacles();

            List<Enemy> enemies = CreateEnemies();

            Point center_point;
            center_point.Y = Graphics.GraphicsDevice.Viewport.Height / 2;
            center_point.X = Graphics.GraphicsDevice.Viewport.Width / 2;

            var player = new Player(Camera, Matrix.CreateTranslation(1, 0, 60) );

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

            var life = new Life(Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(370, 0, 70));
            var armor = new Armor(Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(370, 5, 90));

            rooms.Add(new Box(initialDict, new Vector3(250, 100, 250), new Vector3(0, 50, 0), new Collectible[] { }));
            rooms.Add(new Box(dictCorridorInZ, new Vector3(250, 100, 250), new Vector3(0, 50, -250), new Collectible[] { }));
            rooms.Add(new Box(dictCorner1, new Vector3(250, 100, 250), new Vector3(0, 50, -500), new Collectible[] { }));
            rooms.Add(new Box(dictCorridorInX, new Vector3(250, 100, 250), new Vector3(250, 50, -500), new Collectible[] { }));
            rooms.Add(new Box(dictCorridorInX, new Vector3(250, 100, 250), new Vector3(500, 50, -500), new Collectible[] { }));
            rooms.Add(new Box(dictCorner2, new Vector3(250, 100, 250), new Vector3(750, 50, -500), new Collectible[] { }));
            rooms.Add(new Box(dictCorridorInZ, new Vector3(250, 100, 250), new Vector3(750, 50, -250), new Collectible[] { }));
            rooms.Add(new Box(dictCorner3, new Vector3(250, 100, 250), new Vector3(750, 50, 0), new Collectible[] { }));
            rooms.Add(new Box(dictCorridorInX, new Vector3(250, 100, 250), new Vector3(500, 50, 0), new Collectible[] { }));
            rooms.Add(new Box(dictCorner4, new Vector3(250, 100, 250), new Vector3(250, 50, 0), new Collectible[] { life, armor }));
            rooms.Add(new Box(finalDict, new Vector3(250, 100, 250), new Vector3(250, 50, -250), new Collectible[] { }));

            return rooms;
        }

        private List<Enemy> CreateEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();

            enemies.Add(new TGCito(Matrix.CreateRotationY(-90f) * Matrix.CreateTranslation(80, 0, -200)));

            //enemies.Add(new TGCito(Matrix.CreateRotationY(-90f) * Matrix.CreateTranslation(80, 0, -200)));
            //enemies.Add(new TGCito(Matrix.CreateRotationY(90f) * Matrix.CreateTranslation(-80, 0, -400)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -25)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -50)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -75)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -100)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -125)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -150)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -175)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -200)));
            //enemies.Add(new TGCito(Matrix.CreateTranslation(0, 0, -225)));
            enemies.Add(new TGCito(Matrix.CreateTranslation(200, 0, -250)));
            enemies.Add(new TGCito(Matrix.CreateTranslation(225, 0, -225)));
            enemies.Add(new TGCito(Matrix.CreateTranslation(250, 0, -250)));
            enemies.Add(new TGCito(Matrix.CreateTranslation(275, 0, -225)));
            enemies.Add(new TGCito(Matrix.CreateTranslation(300, 0, -250)));


            return enemies;
        }

        private List<Obstacle> CreateObstacles()
        {
            List<Obstacle> obstacles = new List<Obstacle>();

            // Boxes
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(3, Matrix.CreateTranslation(-50, 0, -50), true)).ToList();
            obstacles.Add(new BoxObstacle(Matrix.CreateTranslation(-25, 25, -50)));

            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(2, Matrix.CreateTranslation(80, 0, -150), true)).ToList();
            obstacles.Add(new BoxObstacle(Matrix.CreateTranslation(105, 25, -150)));

            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(3, Matrix.CreateTranslation(-112, 0, -350), true)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(3, Matrix.CreateTranslation(-112, 25, -350), true)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBoxesObstaclesInLine(3, Matrix.CreateTranslation(-112, 50, -350), true)).ToList();

            // Barriers
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBarriersObstaclesInLine(3, Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(200, 0, -475), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBarriersObstaclesInLine(3, Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(350, 0, -605), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBarriersObstaclesInLine(3, Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(500, 0, -475), false)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainBarriersObstaclesInLine(3, Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(650, 0, -605), false)).ToList();

            // Sawhorses
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainSawhorsesObstaclesInLine(2, Matrix.CreateTranslation(665, 0, -300), true)).ToList();
            obstacles = obstacles.Union(ObstaclesBuilder.ObtainSawhorsesObstaclesInLine(2, Matrix.CreateTranslation(760, 0, -175), true)).ToList();

            // Cones
            //obstacles = obstacles.Union(ObstaclesBuilder.ObtainConesObstaclesInLine(6, Matrix.CreateTranslation(0, 0, -200), true)).ToList();

            return obstacles;
        }
    }
}