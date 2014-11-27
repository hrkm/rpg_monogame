using Microsoft.Xna.Framework;

namespace RPG_ver_8
{
    /// <summary>
    /// Klasa służąca do tworzenia obiektów klasy IBehaviour.
    /// </summary>
    public static class BehaviourFactory
    {
        /// <summary>
        /// Utwórz nowe zachowanie skoku.
        /// </summary>
        /// <param name="parent">Obiekt, który ma mieć możliwość skoku.</param>
        /// <param name="power">Siła wyskoku.</param>
        /// <param name="speed">Prędkość opadania.</param>
        /// <returns>Obiekt zachowania skoku.</returns>
        public static JumpBehaviour CreateJumpBehaviour(GameObject parent, float power, float speed)
        {
            var jump = new JumpBehaviour(parent);
            jump.Power = power;
            jump.Speed = speed;
            return jump;
        }

        /// <summary>
        /// Utwórz nowe zachowanie przesuwania w lewo.
        /// </summary>
        /// <param name="x">Początkowa wartość współrzędnej X.</param>
        /// <returns>Obiekt przesuwania w lewo.</returns>
        public static MoveBehaviour CreateMoveBehaviour(Vector2 position, Vector2 startingPosition, float duration)
        {
            var horizontalMove = new MoveBehaviour(position, startingPosition, duration);
            return horizontalMove;
        }
    }
}
