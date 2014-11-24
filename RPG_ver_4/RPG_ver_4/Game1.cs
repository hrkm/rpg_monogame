using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG_ver_4
{
    /// <summary>
    /// To jest g³ówna klasa aplikacji
    /// </summary>
    public class Game1 : Game
    {
        public static Random random = new Random();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private List<SingleLine> lines;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
        }

        /// <summary>
        /// Umo¿liwia aplikacji wykonanie niezbêdnych inicjalizacji przed rozpoczêciem dzia³ania.
        /// Mo¿na tutaj odpytaæ zewnêtrzne serwisy, wczytaæ zasoby nie powi¹zane z grafik¹ etc.
        /// Wywo³anie base.Initialize spowoduje wyliczenie wszystkich komponentów i ich inicjalizacjê.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Ta funkcja jest wywo³ywana jednokrotnie na pocz¹tku dzia³ania
        /// aplikacji i powinna s³u¿yæ do wczytania wszystkich zewnêtrznych
        /// zasobów, np. tekstur, dŸwiêków, czcionek.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: u¿yj obiektu Content aby wczytaæ zasoby
            AssetManager.Instance.LoadContent(Content);

            NewGame();
        }

        /// <summary>
        /// Ta funkcja jest wywo³ywana jednokrotnie na koniec dzia³ania
        /// aplikacji i powinna s³u¿yæ do zwolnienia zasobów nie wczytanych
        /// przy pomocy obiektu ContentManager
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: zwolnij zasoby wczytane poza obiektem ContentManager
            AssetManager.Instance.UnloadContent();
        }

        /// <summary>
        /// Jeœli na co najmniej jednej linii trwa jeszcze
        /// rozgrywka, zwróæ fa³sz. Przeciwnie zwróæ prawdê.
        /// </summary>
        public bool GameOver
        {
            get
            {
                foreach (var singleLine in lines)
                {
                    if (!singleLine.GameOver)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Jaki jest ³¹czny wynik punktowy na wszystkich liniach?
        /// </summary>
        public int Score
        {
            get
            {
                int total = 0;
                foreach (var singleLine in lines)
                {
                    total += singleLine.Score;
                }
                return total;
            }
        }

        private void NewGame()
        {
            lines = new List<SingleLine>();
            lines.Add(new SingleLine(new Vector2(0, 60), Color.Red));
            lines.Add(new SingleLine(new Vector2(0, 320), Color.Green));
            lines.Add(new SingleLine(new Vector2(0, 580), Color.Blue));
        }

        /// <summary>
        /// Logika gry mo¿e byæ aktualizowana w tej funkcji, np.
        /// sprawdzanie kolizji, odtwarzanie dŸwiêku, obs³uga sterowania.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: dodaj logikê gry w tym miejscu
            if (!GameOver)
            {
                var state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Space))
                {
                    foreach (var singleLine in lines)
                    {
                        singleLine.Jump();
                    }
                }


                foreach (var singleLine in lines)
                {
                    singleLine.Update(gameTime);
                }
            }
            else
            {
                var state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Space))
                {
                    NewGame();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Funkcja rysuj¹ca obiekty na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: poni¿ej umieœæ instrukcje rysowania
            spriteBatch.Begin();
            foreach (var singleLine in lines)
            {
                singleLine.Draw(gameTime, spriteBatch);   
            }
            spriteBatch.DrawString(AssetManager.Instance.Font, Score.ToString(), new Vector2(6,6), Color.White);
            if (GameOver)
                spriteBatch.DrawString(AssetManager.Instance.Font, "Game Over", new Vector2(6, 300), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
