using Microsoft.Xna.Framework;

namespace RPG_ver_10
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
        /// Utwórz nowe zachowanie interpolacji parametru liczbowego.
        /// </summary>
        /// <returns>Obiekt interpolacji parametru liczbowego.</returns>
        public static InterpolationBehaviour CreateInterpolationBehaviour(float targetValue, float startingValue, float duration, InterpolationBehaviour.InterpolationParameter parameter)
        {
            var interpolation = new InterpolationBehaviour(targetValue, startingValue, duration, parameter);
            return interpolation;
        }
    }
}
