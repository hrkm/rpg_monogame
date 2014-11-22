using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_2
{
    /// <summary>
    /// Klasa bazowa dla obiektów gry.
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Czy obiekt jest aktywny?
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// Promień koła do testowania kolizji.
        /// </summary>
        public float Radius = 50;

        /// <summary>
        /// Aktualna pozycja obiektu.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Punkt względem którego wykonywane są wszystkie
        /// operacje przesunięcia/obracania/skalowania.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// Aktualny rozmiar obiektu.
        /// </summary>
        public float Scale = 1f;

        /// <summary>
        /// Aktualny obrót obiektu.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// Aktualny kolor obiektu. Biały powoduje rysowanie
        /// obiektu w oryginalnym kolorze.
        /// </summary>
        public Color Color = Color.White;

        /// <summary>
        /// Lista zachowań dodanych do danego obiektu.
        /// </summary>
        public List<IBehaviour> Behaviours = new List<IBehaviour>();

        /// <summary>
        /// Tekstura wykorzystywana do rysowania obiektu.
        /// </summary>
        private Texture2D _texture;
        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                if (_texture != null)
                    Origin = new Vector2((float)_texture.Width/2, (float)_texture.Height/2);
            }
        }

        /// <summary>
        /// Funkcja update służy do aktualizacji stanu obiektu.
        /// W tym wypadku polega to na zastosowaniu wszystkich
        /// zachowań przypiętych do tego obiektu.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public void Update(GameTime gameTime)
        {
            foreach (var behaviour in Behaviours)
            {
                behaviour.Update(gameTime);
                behaviour.Apply(this);
            }
        }

        /// <summary>
        /// Funkcja rysująca obiekt na ekranie zgodnie z jego stanem.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        /// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Metoda sprawdzająca czy ten obiekt koliduje z innym.
        /// </summary>
        /// <param name="gameObject">Drugi obiekt w kolizji.</param>
        /// <returns>Prawda jeśli obiekty ze sobą kolidują, przeciwnie fałsz.</returns>
        public bool CollidesWith(GameObject gameObject)
        {
            if (Vector2.Distance(gameObject.Position, Position) <= gameObject.Radius + Radius)
                return true;
            return false;
        }
    }
}
