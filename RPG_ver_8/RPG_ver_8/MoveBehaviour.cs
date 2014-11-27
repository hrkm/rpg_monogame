using System;
using Microsoft.Xna.Framework;

namespace RPG_ver_8
{
    /// <summary>
    /// Zachowanie polegające na przesuwaniu obiektu między dwoma punktami.
    /// </summary>
    public class MoveBehaviour : Behaviour
    {
        /// <summary>
        /// Aktualna pozycja.
        /// </summary>
        public Vector2 CurrentPosition;
        /// <summary>
        /// Pozycja docelowa.
        /// </summary>
        public Vector2 TargetPosition;
        /// <summary>
        /// Prędkość przesuwania obiektu wyrażona w pikselach na sekundę.
        /// </summary>
        protected Vector2 dv;

        /// <summary>
        /// Konstruktor zachowania przesunięcia.
        /// </summary>
        /// <param name="targetPosition">Pozycja docelowa.</param>
        /// <param name="startingPosition">Pozycja początkowa.</param>
        /// <param name="duration">Liczba sekund przeznaczona na wykonanie przesunięcia.</param>
        public MoveBehaviour(Vector2 targetPosition, Vector2 startingPosition, float duration)
        {
            TargetPosition = targetPosition;
            CurrentPosition = startingPosition;
            //ustawienie czasu wykonania na zero przesunie obiekt natychmiast
            if (duration == 0)
            {
                CurrentPosition = targetPosition;
                dv = new Vector2(0, 0);
            }
            else
            {
                dv = (TargetPosition - CurrentPosition)/duration;
            }
        }

        /// <summary>
        /// Aktualizacja stanu zachowania poprzez aktualizację obecnej pozycji.
        /// Jeśli na X lub Y pozycja docelowa została już osiągnięta/przekroczona,
        /// to poniższa funkcja zagwarantuje, że obiekt zostanie ustawiony dokładnie
        /// we wskazanym punkcie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public override void Update(GameTime gameTime)
        {
            CurrentPosition += dv*(float) gameTime.ElapsedGameTime.TotalSeconds;
            if (dv.X > 0)
            {
                if (CurrentPosition.X > TargetPosition.X) CurrentPosition.X = TargetPosition.X;
            }
            else
            {
                if (CurrentPosition.X < TargetPosition.X) CurrentPosition.X = TargetPosition.X;
            }
            if (dv.Y > 0)
            {
                if (CurrentPosition.Y > TargetPosition.Y) CurrentPosition.Y = TargetPosition.Y;
            }
            else
            {
                if (CurrentPosition.Y < TargetPosition.Y) CurrentPosition.Y = TargetPosition.Y;
            }
            if (CurrentPosition.X == TargetPosition.X && CurrentPosition.Y == TargetPosition.Y)
            {
                OnFinished();
            }
        }

        /// <summary>
        /// Zastosowanie zachowania do wskazanego obiektu gry.
        /// Odbywa się to poprzez przepisanie obecnej pozycji z zachowania jako pozycja obiektu.
        /// </summary>
        /// <param name="gameObject">Obiekt, który ma być zmodyfikowany.</param>
        public override void Apply(GameObject gameObject)
        {
            gameObject.Position = CurrentPosition;
            base.Apply(gameObject);
        }
    }
}