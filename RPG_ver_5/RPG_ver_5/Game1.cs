using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace RPG_ver_5
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
        private GameObject mouse;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Umo�liwia aplikacji wykonanie niezb�dnych inicjalizacji przed rozpocz�ciem dzia�ania.
        /// Mo�na tutaj odpyta� zewn�trzne serwisy, wczyta� zasoby nie powi�zane z grafik� etc.
        /// Wywo�anie base.Initialize spowoduje wyliczenie wszystkich komponent�w i ich inicjalizacj�.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: dodaj logik� inicjalizuj�c� dzia�anie aplikacji
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

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
            mouse = GameObjectFactory.CreateMouseCursor(new Vector2(0, 0));

            NewGame();

#if WINDOWS_PHONE
            MediaPlayer.Play(AssetManager.Instance.BackgroundMusic);
#endif
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
            var mouseState = Mouse.GetState();
            mouse.Position = new Vector2(mouseState.X, mouseState.Y);
            mouse.Update(gameTime);

            if (!GameOver)
            {
                var state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Q))
                {
                    lines[0].Jump();
                }
                if (state.IsKeyDown(Keys.W))
                {
                    lines[1].Jump();
                }
                if (state.IsKeyDown(Keys.E))
                {
                    lines[2].Jump();
                }

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    foreach (var singleLine in lines)
                    {
                        if (mouse.CollidesWith(singleLine.Character))
                        {
                            singleLine.Jump();
                        }
                    }
                }

                TouchCollection touchCollection = TouchPanel.GetState();
                foreach (TouchLocation tl in touchCollection)
                {
                    if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
                    {
                        var touchLocation = new GameObject();
                        touchLocation.Position = tl.Position;
                        foreach (var singleLine in lines)
                        {
                            if (touchLocation.CollidesWith(singleLine.Character))
                            {
                                singleLine.Jump();
                            }
                        }
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

                int count = 0;
                TouchCollection touchCollection = TouchPanel.GetState();
                foreach (TouchLocation tl in touchCollection)
                {
                    if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
                    {
                        count++;
                    }
                }

                if (count >= 2)
                    NewGame();
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

            var m = AssetManager.Instance.Font.MeasureString(Score.ToString());
            spriteBatch.DrawString(AssetManager.Instance.Font, Score.ToString(), new Vector2(240 - m.X/2, 6), Color.White);
            if (GameOver)
            {
                m = AssetManager.Instance.Font.MeasureString("Game Over");
                spriteBatch.DrawString(AssetManager.Instance.Font, "Game Over", new Vector2(240 - m.X/2, 300), Color.White);
            }

#if !WINDOWS_PHONE
            mouse.Draw(gameTime, spriteBatch);
#endif

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
