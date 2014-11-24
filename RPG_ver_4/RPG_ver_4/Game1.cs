using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG_ver_4
{
    /// <summary>
    /// To jest g��wna klasa aplikacji
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
            AssetManager.Instance.LoadContent(Content);

            NewGame();
        }

        /// <summary>
        /// Ta funkcja jest wywo�ywana jednokrotnie na koniec dzia�ania
        /// aplikacji i powinna s�u�y� do zwolnienia zasob�w nie wczytanych
        /// przy pomocy obiektu ContentManager
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: zwolnij zasoby wczytane poza obiektem ContentManager
            AssetManager.Instance.UnloadContent();
        }

        /// <summary>
        /// Je�li na co najmniej jednej linii trwa jeszcze
        /// rozgrywka, zwr�� fa�sz. Przeciwnie zwr�� prawd�.
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
        /// Jaki jest ��czny wynik punktowy na wszystkich liniach?
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
        /// Logika gry mo�e by� aktualizowana w tej funkcji, np.
        /// sprawdzanie kolizji, odtwarzanie d�wi�ku, obs�uga sterowania.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: dodaj logik� gry w tym miejscu
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
        /// Funkcja rysuj�ca obiekty na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: poni�ej umie�� instrukcje rysowania
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
