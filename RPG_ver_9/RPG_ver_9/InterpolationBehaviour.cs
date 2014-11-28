using System;
using Microsoft.Xna.Framework;

namespace RPG_ver_9
{
    /// <summary>
    /// Zachowanie polegające na modyfikacji parametry liczbowego obiektu w sposób liniowy.
    /// </summary>
    public class InterpolationBehaviour : Behaviour
    {
        public enum InterpolationParameter
        {
            PositionX,
            PositionY,
            Alpha,
            Scale,
            Rotation
        }

        public InterpolationParameter Parameter;

        /// <summary>
        /// Aktualna wartość.
        /// </summary>
        public float CurrentValue;
        /// <summary>
        /// Pozycja docelowa.
        /// </summary>
        public float TargetValue;
        /// <summary>
        /// Prędkość zmiany wartości liczbowej wyrażona w jednostkach na sekundę.
        /// </summary>
        protected float dv;

        /// <summary>
        /// Konstruktor zachowania interpolacji liniowej.
        /// </summary>
        /// <param name="targetValue">Pozycja docelowa.</param>
        /// <param name="startingValue">Pozycja początkowa.</param>
        /// <param name="duration">Liczba sekund przeznaczona na wykonanie przesunięcia.</param>
        /// <param name="parameter">Interpolowany parametr liczbowy.</param>
        public InterpolationBehaviour(float targetValue, float startingValue, float duration, InterpolationParameter parameter)
        {
            Parameter = parameter;
            TargetValue = targetValue;
            CurrentValue = startingValue;
            //ustawienie czasu wykonania na zero to natychmiastowe przypisanie wartości docelowej
            if (duration == 0)
            {
                CurrentValue = targetValue;
                dv = 0;
            }
            else
            {
                dv = (TargetValue - CurrentValue)/duration;
            }
        }

        /// <summary>
        /// Aktualizacja stanu zachowania poprzez aktualizację obecnej wartości.
        /// Jeśli wartość docelowa została już osiągnięta/przekroczona,
        /// to poniższa funkcja zagwarantuje, że zostanie ona ustawiona dokładnie
        /// na wskazaną wartość docelową.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public override void Update(GameTime gameTime)
        {
            CurrentValue += dv*(float) gameTime.ElapsedGameTime.TotalSeconds;
            if (dv > 0)
            {
                if (CurrentValue > TargetValue) CurrentValue = TargetValue;
            }
            else
            {
                if (CurrentValue < TargetValue) CurrentValue = TargetValue;
            }
            if (CurrentValue == TargetValue && CurrentValue == TargetValue)
            {
                OnFinished();
            }
        }

        /// <summary>
        /// Zastosowanie zachowania do wskazanego obiektu gry.
        /// Odbywa się to poprzez przepisanie wartości z zachowania do odpowiedniego
        /// pola obiektu docelowego.
        /// </summary>
        /// <param name="gameObject">Obiekt, który ma być zmodyfikowany.</param>
        public override void Apply(GameObject gameObject)
        {
            switch (Parameter)
            {
                case InterpolationParameter.Alpha:
                    gameObject.Alpha = CurrentValue;
                    break;
                case InterpolationParameter.Rotation:
                    gameObject.Rotation = CurrentValue;
                    break;
                case InterpolationParameter.PositionX:
                    gameObject.Position.X = CurrentValue;
                    break;
                case InterpolationParameter.PositionY:
                    gameObject.Position.Y = CurrentValue;
                    break;
                case InterpolationParameter.Scale:
                    gameObject.Scale = CurrentValue;
                    break;
            }
            base.Apply(gameObject);
        }
    }
}