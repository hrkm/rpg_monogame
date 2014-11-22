using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG_ver_2
{
    /// <summary>
    /// To jest g��wna klasa aplikacji
    /// </summary>
    public class Game1 : Game
    {
        Random random = new Random();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private GameObject character;
        private JumpBehaviour jump;

        private List<GameObject> obstacles;

        private float timeFromLastObject;
        private bool gameOver;
        private int score;

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

            character = GameObjectFactory.CreateCharacter(new Vector2(240, 400));

            jump = BehaviourFactory.CreateJumpBehaviour(character, 100, 100);
            character.Behaviours.Add(jump);

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

        private void NewGame()
        {
            obstacles = new List<GameObject>();
            score = 0;
            gameOver = false;
        }

        /// <summary>
        /// Logika gry mo�e by� aktualizowana w tej funkcji, np.
        /// sprawdzanie kolizji, odtwarzanie d�wi�ku, obs�uga sterowania.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: dodaj logik� gry w tym miejscu
            if (!gameOver)
            {
                var state = Keyboard.GetState();
                if (state.IsKeyDown(Keys.Space))
                    jump.Jump();

                timeFromLastObject += (float) gameTime.ElapsedGameTime.TotalSeconds;
                if (timeFromLastObject > 3)
                {
                    timeFromLastObject = 0;
                    if (random.Next(2) == 0)
                    {
                        var star = GameObjectFactory.CreateStar(new Vector2(0, 280));
                        obstacles.Add(star);
                    }
                    else
                    {
                        var obstacle = GameObjectFactory.CreateObstacle(new Vector2(0, 450));
                        obstacles.Add(obstacle);
                    }
                }

                character.Update(gameTime);

                foreach (var gameObject in obstacles)
                {
                    gameObject.Update(gameTime);
                }

                for (int i = obstacles.Count - 1; i >= 0; i--)
                {
                    var obstacle = obstacles[i];
                    if (!obstacle.Active)
                        obstacles.RemoveAt(i);
                    else if (character.CollidesWith(obstacle))
                    {
                        //TODO: poni�szy kod prowadzi do bad smells, w kolejnej wersji nale�y go zrefaktoryzowa�!
                        if (obstacle.Color == Color.Black)
                            gameOver = true;
                        else
                        {
                            obstacles.RemoveAt(i); 
                            score += 1;
                        }
                    }
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
            foreach (var gameObject in obstacles)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
            character.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(AssetManager.Instance.Font, score.ToString(), new Vector2(6,6), Color.White);
            if (gameOver)
                spriteBatch.DrawString(AssetManager.Instance.Font, "Game Over", new Vector2(6, 300), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
