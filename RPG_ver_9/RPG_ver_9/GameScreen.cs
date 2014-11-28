using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG_ver_9
{
    /// <summary>
    /// To jest g��wny ekran rozgrywki.
    /// </summary>
    public class GameScreen : IScreen
    {
        private List<SingleLine> lines;

        public GameScreen()
        {
            NewGame();
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
        public void Update(GameTime gameTime)
        {
            if (!GameOver)
            {
                foreach (var singleLine in lines)
                {
                    singleLine.Update(gameTime);
                }
            }
            else
            {
                //elementy aktualizowane w stanie ko�ca gry
            }
        }

        /// <summary>
        /// Funkcja rysuj�ca obiekty na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        /// <param name="spriteBatch">Umo�liwia rysowanie na ekranie.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
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
        }

        /// <summary>
        /// Funkcja wywo�ywana je�li naci�ni�to lewy przycisk myszy.
        /// </summary>
        /// <param name="p">Aktualna pozycja kursora myszki.</param>
        public void CheckClick(Vector2 p)
        {
            if (!GameOver)
            {
                var mouse = new GameObject();
                mouse.Position = p;
                foreach (var singleLine in lines)
                {
                    if (mouse.CollidesWith(singleLine.Character))
                    {
                        singleLine.Jump();
                    }
                }
            }
            else
            {
                //obs�uga myszy/dotyku przy game over
            }
        }

        /// <summary>
        /// Funkcja wywo�ywana je�li dotkni�to ekranu dotykowego.
        /// </summary>
        /// <param name="p">Lista punkt�w dotkni�cia.</param>
        public void CheckTouchPoints(List<Vector2> p)
        {
            if (GameOver)
            {
                if (p.Count >= 2)
                    NewGame();
            }
        }

        /// <summary>
        /// Funkcja wywo�ywana je�l wci�ni�to konkretny klawisz.
        /// </summary>
        /// <param name="key">Wci�ni�ty klawisz na klawiaturze.</param>
        public void ButtonPressed(Keys key)
        {
            if (!GameOver)
            {
                if (key == Keys.Q)
                {
                    lines[0].Jump();
                }
                if (key == Keys.W)
                {
                    lines[1].Jump();
                }
                if (key == Keys.E)
                {
                    lines[2].Jump();
                }
            }
            else
            {
                if (key == Keys.Space)
                {
                    NewGame();
                }
            }
        }
    }
}
