using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_5
{
    /// <summary>
    /// System obsługi cząsteczek, który jest również obiektem klasy GameObject.
    /// </summary>
    public class ParticleSystem : GameObject
    {
        /// <summary>
        /// Lista do przechowywania aktywnych cząstek.
        /// </summary>
        private readonly List<GameObject> _particles = new List<GameObject>();
        /// <summary>
        /// Lista do przetrzymywania pozostałej długości życia każdej z cząstek.
        /// </summary>
        private readonly List<float> _lifespans = new List<float>(); 

        /// <summary>
        /// Dodanie wskazanej cząstki do systemu cząsteczek.
        /// </summary>
        /// <param name="particle">Dodawana cząsteczka.</param>
        /// <param name="lifespan">Długość życia wyrażona w sekundach.</param>
        public void AddParticle(GameObject particle, float lifespan)
        {
            _particles.Add(particle);
            _lifespans.Add(lifespan);
        }

        /// <summary>
        /// Aktualizacja systemu cząsteczek. Skrócenie życia każdej z cząstek
        /// i sprawdzenie czy powinna już zniknąć czy jeszcze zostać.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var particle = _particles[i];
                _lifespans[i] -= elapsedSeconds;
                if (_lifespans[i] < 0)
                {
                    _particles.RemoveAt(i);
                    _lifespans.RemoveAt(i);
                }
                else
                {
                    particle.Update(gameTime);   
                }
            }
        }

        /// <summary>
        /// Rysowanie każdej cząsteczki dodanej do systemu na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        /// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var particle in _particles)
            {
                particle.Draw(gameTime, spriteBatch);
            }
        }
    }
}
