using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_5
{
    /// <summary>
    /// Klasa bazowa dla obiektów gry.
    /// </summary>
    public class GameObject : IUpdateable, IDrawable
    {
        /// <summary>
        /// Współczynnik przezroczystości obiektu.
        /// </summary>
        public float Alpha = 1.0f;

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
        private readonly List<IBehaviour> _behaviours = new List<IBehaviour>();

        /// <summary>
        /// Lista zawierająca wszystkie podczepione elementy IUpdateable.
        /// </summary>
        private List<IUpdateable> _updateables = new List<IUpdateable>(); 

        /// <summary>
        /// Lista zawierająca wszystkie podczepione elementy IDrawable.
        /// </summary>
        private List<IDrawable> _drawables = new List<IDrawable>(); 

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
        /// Dodaje zachowanie do obiektu gry.
        /// </summary>
        /// <param name="behaviour">Zachowanie do dodania.</param>
        public void AddBehaviour(IBehaviour behaviour)
        {
            _behaviours.Add(behaviour);
            _updateables.Add(behaviour);
            
            if (behaviour is IDrawable)
                _drawables.Add(behaviour as IDrawable);
        }

        /// <summary>
        /// Funkcja update służy do aktualizacji stanu obiektu.
        /// W tym wypadku polega to na zastosowaniu wszystkich
        /// zachowań przypiętych do tego obiektu.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public virtual void Update(GameTime gameTime)
        {
            foreach (var updateable in _updateables)
            {
                updateable.Update(gameTime);
            }
            foreach (var behaviour in _behaviours)
            {
                behaviour.Apply(this);
            }
        }

        /// <summary>
        /// Funkcja rysująca obiekt na ekranie zgodnie z jego stanem.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        /// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var drawable in _drawables)
            {
                drawable.Draw(gameTime, spriteBatch);
            }
            spriteBatch.Draw(Texture, Position, null, Color * Alpha, Rotation, Origin, Scale, SpriteEffects.None, 0);
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

        /// <summary>
        /// Metoda wykonująca płytką kopię obiektu gry, bez przenoszenia zachowań.
        /// </summary>
        /// <returns>Płytka kopia obiektu gry.</returns>
        public GameObject Clone()
        {
            var o = new GameObject();
            o.Alpha = Alpha;
            o.Active = Active;
            o.Radius = Radius;
            o.Rotation = Rotation;
            o.Scale = Scale;
            o.Position = Position;
            o.Color = Color;
            o.Texture = Texture;
            return o;
        }
    }
}
