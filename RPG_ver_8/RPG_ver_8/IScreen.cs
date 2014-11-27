using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG_ver_8
{
    public interface IScreen
    {
        /// <summary>
        /// Funkcja update służy do aktualizacji stanu obiektu.
        /// W tym wypadku polega to na zastosowaniu wszystkich
        /// zachowań przypiętych do tego obiektu.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Funkcja rysująca obiekt na ekranie zgodnie z jego stanem.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        /// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        /// <summary>
        /// Funkcja wywoływana jeśli naciśnięto lewy przycisk myszy.
        /// </summary>
        /// <param name="p">Aktualna pozycja kursora myszki.</param>
        void CheckClick(Vector2 p);

        /// <summary>
        /// Funkcja wywoływana jeśli dotknięto ekranu dotykowego.
        /// </summary>
        /// <param name="p">Lista punktów dotknięcia.</param>
        void CheckTouchPoints(List<Vector2> p);

        /// <summary>
        /// Funkcja wywoływana jeśl wciśnięto konkretny klawisz.
        /// </summary>
        /// <param name="key">Wciśnięty klawisz na klawiaturze.</param>
        void ButtonPressed(Keys key);
    }
}