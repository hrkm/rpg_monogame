using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_5
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

        /// <summary>
        /// Czas życia pojedynczej cząsteczki.
        /// </summary>
        public float Lifespan = 0.5f;

        private readonly ParticleSystem _particleSystem = new ParticleSystem();

        /// <summary>
        /// Wzorzec cząsteczki.
        /// </summary>
        public GameObject ParticleTemplate;

        public bool AddRandomBehaviour;

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
            if (ParticleTemplate == null)
                return;

            _particleSystem.Update(gameTime);

            timeFromLastParticle += (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (timeFromLastParticle > ParticleSpawnDelay)
            {
                timeFromLastParticle = 0;
                for (int i = 0; i < ParticlesPerSpawn; i++)
                {
                    var particle = ParticleTemplate.Clone();
                    particle.Position = _particleSystem.Position;
                    particle.Position.Y += Game1.random.Next(-20, 20);
                    particle.Position.X += Game1.random.Next(-20, 20);
                    particle.Scale = Game1.random.Next(20, 30) / 100.0f;
                    //kąt w radianach = kąt w stopniach * PI / 180
                    particle.Rotation = Game1.random.Next(360) * (float)Math.PI / 180;
                    if (AddRandomBehaviour)
                        particle.AddBehaviour(new RandomBehaviour(particle.Position.X));
                    _particleSystem.AddParticle(particle, Lifespan);
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
