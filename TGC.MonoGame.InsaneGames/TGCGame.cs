using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.InsaneGames.Maps;
using TGC.MonoGame.InsaneGames.Entities;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using Microsoft.Xna.Framework.Media;


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
            Graphics.IsFullScreen = true;
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";

            ContentManager.MakeInstance(Content);
        }

        private GraphicsDeviceManager Graphics { get; }
        private Map Map { get; set; }

        private MenuUI MenuUI { get; set; }

        public bool godMode = false;
        private DefeatUI DefeatUI { get; set; }
        private WinUI WinUI { get; set; }

        public const int ST_MENU = 0;
        public const int ST_LEVEL_1 = 1;
        public const int ST_DEFEAT = 2;
        public const int ST_WIN = 3;
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

            MapRepo.CurrentMap = MapRepo.Level1(this, GraphicsDevice);
            Map = MapRepo.CurrentMap;
            Map.Initialize(this);
            MenuUI = new MenuUI();
            DefeatUI = new DefeatUI();
            WinUI = new WinUI();

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
            MenuUI.Load(GraphicsDevice);
            DefeatUI.Load(GraphicsDevice);
            WinUI.Load(GraphicsDevice);

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
                case ST_DEFEAT:
                case ST_WIN:
                    if (Keyboard.GetState().IsKeyDown(Keys.R))
                    {
                        Map.Reset();
                        status = ST_MENU;
                    }
                    break;
                case ST_MENU:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        status = ST_LEVEL_1;
                    break;

                case ST_LEVEL_1:
                    Map.Update(gameTime);
                    if (Keyboard.GetState().IsKeyDown(Keys.G))
                        godMode = !godMode;
                    if (Map.playerIsDead())
                    {
                        status = ST_DEFEAT;
                        MediaPlayer.Volume = 1f;
                        MediaPlayer.Play(DefeatUI.Song);
                    }
                    if (Map.keyFound)
                    {
                        status = ST_WIN;
                        MediaPlayer.Volume = 0.3f;
                        MediaPlayer.Play(WinUI.Song);
                    }
                    break;
            }

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
                case ST_DEFEAT:
                    DefeatUI.Draw(gameTime);
                    break;
                case ST_WIN:
                    WinUI.Draw(gameTime);
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
    }
}