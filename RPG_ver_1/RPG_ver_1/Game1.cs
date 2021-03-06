using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG_ver_1
{
    /// <summary>
    /// To jest g��wna klasa aplikacji
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private SpriteFont font;
        private Texture2D square;
        private GameObject character;
        private JumpBehaviour jump;

        private List<GameObject> obstacles;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
        }

        /// <summary>
        /// Umo�liwia aplikacji wykonanie niezb�dnych inicjalizacji przed rozpocz�ciem dzia�ania.
        /// Mo�na tutaj odpyta� zewn�trzne serwisy, wczyta� zasoby nie powi�zane z grafik� etc.
        /// Wywo�anie base.Initialize spowoduje wyliczenie wszystkich komponent�w i ich inicjalizacj�.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Ta funkcja jest wywo�ywana jednokrotnie na pocz�tku dzia�ania
        /// aplikacji i powinna s�u�y� do wczytania wszystkich zewn�trznych
        /// zasob�w, np. tekstur, d�wi�k�w, czcionek.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: u�yj obiektu Content aby wczyta� zasoby
            square = Content.Load<Texture2D>("square");
            font = Content.Load<SpriteFont>("font");

            character = new GameObject();
            character.Texture = square;
            character.Position = new Vector2(240, 400);

            jump = new JumpBehaviour(character);
            character.Behaviours.Add(jump);

            var starTexture = Content.Load<Texture2D>("star");
            obstacles = new List<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                var star = new GameObject();
                star.Texture = starTexture;
                star.Position = new Vector2(160 * i, 250);
                star.Behaviours.Add(new HorizontalMoveBehaviour(star.Position.X));
                obstacles.Add(star);
            }
        }

        /// <summary>
        /// Ta funkcja jest wywo�ywana jednokrotnie na koniec dzia�ania
        /// aplikacji i powinna s�u�y� do zwolnienia zasob�w nie wczytanych
        /// przy pomocy obiektu ContentManager
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: zwolnij zasoby wczytane poza obiektem ContentManager
        }

        /// <summary>
        /// Logika gry mo�e by� aktualizowana w tej funkcji, np.
        /// sprawdzanie kolizji, odtwarzanie d�wi�ku, obs�uga sterowania.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: dodaj logik� gry w tym miejscu
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Space))
                jump.Jump();

            character.Update(gameTime);

            foreach (var gameObject in obstacles)
            {
                gameObject.Update(gameTime);
            }

            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                if (character.CollidesWith(obstacles[i]))
                    obstacles.RemoveAt(i);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Funkcja rysuj�ca obiekty na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: poni�ej umie�� instrukcje rysowania
            spriteBatch.Begin();
            foreach (var gameObject in obstacles)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
            character.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(font, "Ahoj przygodo!", new Vector2(6,6), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
