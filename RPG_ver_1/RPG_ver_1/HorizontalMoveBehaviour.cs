using Microsoft.Xna.Framework;

namespace RPG_ver_1
{
    public class HorizontalMoveBehaviour : IBehaviour
    {
        private float _x;
        public HorizontalMoveBehaviour(float x)
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
            if (_x < 0)
                _x = 480;
        }

        /// <summary>
        /// Zastosowanie danego zachowania do obiektu.
        /// </summary>
        /// <param name="gameObject">Obiekt na którym działamy.</param>
        public void Apply(GameObject gameObject)
        {
            gameObject.Position.X = _x;
        }
    }
}
