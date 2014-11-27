using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace RPG_ver_8
{
    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        /// <summary>
        /// Docelowa szarokość ekranu.
        /// </summary>
        public const int Width = 480;
        /// <summary>
        /// Docelowa wysokość ekranu.
        /// </summary>
        public const int Height = 800;

        /// <summary>
        /// Obiekt przedstawiający kursor.
        /// </summary>
        private GameObject mouse;

        /// <summary>
        /// Renderowanie gry niezależnie od rozdzielczości ekranu.
        /// </summary>
        private ResolutionIndependentRenderer renderer;

        /// <summary>
        /// Obiekt do losowania liczb losowych.
        /// </summary>
        public static Random random = new Random();

        /// <summary>
        /// Urządzenie rysujące grafikę.
        /// </summary>
        GraphicsDeviceManager graphics;
        /// <summary>
        /// Klasa umożliwiająca rysowanie.
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// Aktualny stan gry, w którym jesteśmy.
        /// </summary>
        public static GameState CurrentState { get; set; }

        /// <summary>
        /// Aktualnie wyświetlany ekran gry.
        /// </summary>
        private IScreen CurrentScreen
        {
            get { return Screens[CurrentState]; }
        }

        /// <summary>
        /// Lista zawierająca wszystkie ekrany gry.
        /// </summary>
        public static Dictionary<GameState, IScreen> Screens = new Dictionary<GameState, IScreen>();

        /// <summary>
        /// Dostępne stany gry, w których możemy się znaleźć.
        /// </summary>
        public enum GameState
        {
            Gameplay
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.IsFullScreen = true;
            //ustawiamy rozmiar okna dla aplikacji desktopowej
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
        }

        /// <summary>
        /// Umożliwia aplikacji wykonanie niezbędnych inicjalizacji przed rozpoczęciem działania.
        /// Można tutaj odpytać zewnętrzne serwisy, wczytać zasoby nie powiązane z grafiką etc.
        /// Wywołanie base.Initialize spowoduje wyliczenie wszystkich komponentów i ich inicjalizację.
        /// </summary>
        protected override void Initialize()
        {
            renderer = new ResolutionIndependentRenderer(GraphicsDevice, Width, Height, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            renderer.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// Ta funkcja jest wywoływana jednokrotnie na początku działania
        /// aplikacji i powinna służyć do wczytania wszystkich zewnętrznych
        /// zasobów, np. tekstur, dźwięków, czcionek.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.Instance.LoadContent(Content);
            AssetManager.Instance.PrepareTrees(spriteBatch);
            mouse = GameObjectFactory.CreateMouseCursor(new Vector2(0, 0));

            //dla każdego stanu gry dodaj odpowiedni ekran
            Screens.Add(GameState.Gameplay, new GameScreen());
            CurrentState = GameState.Gameplay;

#if WINDOWS_PHONE || ANDROID
            MediaPlayer.Play(AssetManager.Instance.BackgroundMusic);
#endif
        }

        /// <summary>
        /// Ta funkcja jest wywoływana jednokrotnie na koniec działania
        /// aplikacji i powinna służyć do zwolnienia zasobów nie wczytanych
        /// przy pomocy obiektu ContentManager
        /// </summary>
        protected override void UnloadContent()
        {
            AssetManager.Instance.UnloadContent();
        }

        /// <summary>
        /// Logika gry może być aktualizowana w tej funkcji, np.
        /// sprawdzanie kolizji, odtwarzanie dźwięku, obsługa sterowania.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Update(GameTime gameTime)
        {
            CheckInput(gameTime);
            CurrentScreen.Update(gameTime);

            base.Update(gameTime);
        }

#if !WINDOWS_PHONE && !ANDROID
        private MouseState previous;
        private MouseState current;
#endif

        /// <summary>
        /// Sprawdza wszystkie źródła sterowania i przekazuje informacje
        /// o wciśniętych klawiszach/dotkniętych punktach na ekranie do
        /// aktualnie renderowanego ekranu IScreen.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected void CheckInput(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //obsługa ekranu dotykowego
            List<Vector2> points = new List<Vector2>();
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
                {
                    Vector2 p = renderer.ScaleMouseToScreenCoordinates(tl.Position);
                    points.Add(p);
                }
                //jeśli dopiero został wciśnięty, to potraktuj to również jak kliknięcie myszką
                if (tl.State == TouchLocationState.Pressed)
                {
                    Vector2 p = renderer.ScaleMouseToScreenCoordinates(tl.Position);
                    CurrentScreen.CheckClick(p);
                }
            }
            CurrentScreen.CheckTouchPoints(points);

#if !WINDOWS_PHONE && !ANDROID
            //obsługa myszki (poza platformami mobilnymi)
            previous = current;
            current = Mouse.GetState();
            mouse.Position = renderer.ScaleMouseToScreenCoordinates(new Vector2(current.X, current.Y));
            mouse.Update(gameTime);
            if (previous.Position != current.Position) mouse.Active = true;
            if (current.LeftButton == ButtonState.Pressed)
            {
                Vector2 p = renderer.ScaleMouseToScreenCoordinates(new Vector2(current.X, current.Y));
                if (previous.LeftButton == ButtonState.Released)
                {
                    CurrentScreen.CheckClick(p);
                }
                //przytrzymanie wciśniętego przycisku myszki jest traktowane jak palec na ekranie dotykowym
                CurrentScreen.CheckTouchPoints(new List<Vector2>(1) { p });
            }
#endif

            //obsługa klawiatury
            var keyboard = Keyboard.GetState();
            foreach (var pressedKey in keyboard.GetPressedKeys())
            {
                CurrentScreen.ButtonPressed(pressedKey);
            }
        }

        /// <summary>
        /// Funkcja rysująca obiekty na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Draw(GameTime gameTime)
        {
            renderer.BeginDraw();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null,
                             null, null, null, renderer.GetTransformationMatrix());

            if (CurrentScreen != null)
                CurrentScreen.Draw(gameTime, spriteBatch);

#if !WINDOWS_PHONE && !ANDROID
            mouse.Draw(gameTime, spriteBatch);
#endif

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}