using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_4
{
    /// <summary>
    /// Klasa odpowiedzialna za dodanie zachowania emitowania cząsteczek do obiektu.
    /// </summary>
    public class EmitParticlesBehaviour : IBehaviour, IDrawable
    {
        /// <summary>
        /// Częstotliwość generowania cząsteczek.
        /// </summary>
        public float ParticleSpawnDelay = 0.2f;

        /// <summary>
        /// Liczba wygenerowanych cząsteczek co określony okres.
        /// </summary>
        public float ParticlesPerSpawn = 7;

        private readonly ParticleSystem _particleSystem = new ParticleSystem();

        /// <summary>
        /// Zmienna pomocnicza do liczenia ile czasu upłynęło od ostatniej generacji cząsteczek.
        /// </summary>
        private float timeFromLastParticle;

        /// <summary>
        /// Aktualizacja systemu cząsteczek, plus sprawdzenie
        /// czy nie trzeba dodać nowych cząsteczek.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public void Update(GameTime gameTime)
        {
            _particleSystem.Update(gameTime);

            timeFromLastParticle += (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (timeFromLastParticle > ParticleSpawnDelay)
            {
                timeFromLastParticle = 0;
                for (int i = 0; i < ParticlesPerSpawn; i++)
                {
                    var particle = GameObjectFactory.CreateParticle(_particleSystem.Position);
                    _particleSystem.AddParticle(particle, 0.5f);
                }
            }
        }

        /// <summary>
        /// Zastosowanie danego zachowania do obiektu.
        /// W tym wypadku przepisujemy pozycję obiektu do systemu cząsteczek.
        /// </summary>
        /// <param name="gameObject">Obiekt na którym działamy.</param>
        public void Apply(GameObject gameObject)
        {
            _particleSystem.Position = gameObject.Position;
        }

        /// <summary>
        /// Rysowanie systemu cząsteczek na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        /// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _particleSystem.Draw(gameTime, spriteBatch);
        }
    }
}
