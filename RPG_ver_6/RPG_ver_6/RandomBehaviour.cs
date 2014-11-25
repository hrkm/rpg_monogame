using Microsoft.Xna.Framework;

namespace RPG_ver_6
{
    /// <summary>
    /// Losowe zachowanie - przesuwa obiekt w lewo i obraca.
    /// </summary>
    public class RandomBehaviour : IBehaviour
    {
        private float _x = 480 + 50;

        public RandomBehaviour(float x)
        {
            _x = x;
        }

        /// <summary>
        /// Aktualizacja fazy przesunięcia.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public void Update(GameTime gameTime)
        {
            _x -= (float) gameTime.ElapsedGameTime.TotalSeconds*100;
        }

        /// <summary>
        /// Zastosowanie danego zachowania do obiektu.
        /// </summary>
        /// <param name="gameObject">Obiekt na którym działamy.</param>
        public void Apply(GameObject gameObject)
        {
            gameObject.Position.X = _x;
            gameObject.Rotation += 0.1f;
        }
    }
}
