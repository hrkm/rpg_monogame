using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG_ver_10
{
    /// <summary>
    /// To jest ekran koñca gry.
    /// </summary>
    public class EndGameScreen : IScreen
    {
        /// <summary>
        /// Przycisk do rozpoczêcia nowej gry.
        /// </summary>
        private GameObject newGameButton;

        /// <summary>
        /// Wynik uzyskany w ostatniej grze.
        /// </summary>
        public int Score;

        public EndGameScreen()
        {
            newGameButton = GameObjectFactory.CreateButton(new Vector2(Game1.Width/2, Game1.Height/2));
        }

        /// <summary>
        /// Logika gry mo¿e byæ aktualizowana w tej funkcji, np.
        /// sprawdzanie kolizji, odtwarzanie dŸwiêku, obs³uga sterowania.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public void Update(GameTime gameTime)
        {
            newGameButton.Update(gameTime);
        }

        /// <summary>
        /// Funkcja rysuj¹ca obiekty na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        /// <param name="spriteBatch">Umo¿liwia rysowanie na ekranie.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var m = AssetManager.Instance.Font.MeasureString("Zagraj ponownie");
            spriteBatch.DrawString(AssetManager.Instance.Font, "Zagraj ponownie", new Vector2(240 - m.X / 2, 300), Color.White);
            var pointsText = "Wynik to " + Score + " punkty!";
            m = AssetManager.Instance.Font.MeasureString(pointsText);
            spriteBatch.DrawString(AssetManager.Instance.Font, pointsText, new Vector2(240 - m.X / 2, 150), Color.White);
            newGameButton.Draw(gameTime, spriteBatch);

            newGameButton.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Funkcja wywo³ywana jeœli naciœniêto lewy przycisk myszy.
        /// </summary>
        /// <param name="p">Aktualna pozycja kursora myszki.</param>
        public void CheckClick(Vector2 p)
        {
            var point = new GameObject();
            point.Position = p;
            if (point.CollidesWith(newGameButton))
            {
                Game1.CurrentState = Game1.GameState.Gameplay;
            }
        }

        /// <summary>
        /// Funkcja wywo³ywana jeœli dotkniêto ekranu dotykowego.
        /// </summary>
        /// <param name="p">Lista punktów dotkniêcia.</param>
        public void CheckTouchPoints(List<Vector2> p)
        {
        }

        /// <summary>
        /// Funkcja wywo³ywana jeœl wciœniêto konkretny klawisz.
        /// </summary>
        /// <param name="key">Wciœniêty klawisz na klawiaturze.</param>
        public void ButtonPressed(Keys key)
        {
        }
    }
}
