using System;
using Microsoft.Xna.Framework;
using RPG_ver_9;

namespace RPG_ver_9
{
    /// <summary>
    /// Klasa abstrakcyjna opisująca dodatkowe zachowanie się obiektu.
    /// </summary>
    public abstract class Behaviour : IUpdateable
    {
        /// <summary>
        /// Event rzucany w momencie zakończenia działania zachowania.
        /// </summary>
        public event EventHandler Finished;

        /// <summary>
        /// Event rzucany w momencie zakończenia działania zachowania.
        /// </summary>
        public event EventHandler<GameObject> Applied;

        /// <summary>
        /// Funkcja pomocnicza do rzucenia wydarzenia Finished.
        /// </summary>
        protected void OnFinished()
        {
            if (Finished != null)
                Finished(this, new EventArgs());
        }

        /// <summary>
        /// Funkcja pomocnicza do rzucenia wydarzenia Applied.
        /// </summary>
        /// <param name="gameObject">Obiekt, który jest pod wpływem zachowania.</param>
        protected virtual void OnApplied(GameObject gameObject)
        {
            if (Applied != null)
                Applied(this, gameObject);
        }

        /// <summary>
        /// Aktualizacja stanu zachowania.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Zastosowanie zachowania do wskazanego obiektu gry.
        /// </summary>
        /// <param name="gameObject">Obiekt, który ma być zmodyfikowany.</param>
        public virtual void Apply(GameObject gameObject)
        {
            OnApplied(gameObject);
        }
    }
}