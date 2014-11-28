using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_9
{
    /// <summary>
    /// Klasa odpowiedzialna za dodanie zachowania emitowania cząsteczek do obiektu.
    /// </summary>
    public class EmitParticlesBehaviour : Behaviour, IDrawable
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
        public override void Update(GameTime gameTime)
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
                    var alpha = new InterpolationBehaviour(0, 1, Lifespan, InterpolationBehaviour.InterpolationParameter.Alpha);
                    particle.AddBehaviour(alpha);
                    if (AddRandomBehaviour)
                    {
                        var move = new InterpolationBehaviour(-50, particle.Position.X, (particle.Position.X + 50)/100, InterpolationBehaviour.InterpolationParameter.PositionX);
                        move.Applied += (sender, o) => o.Rotation += 0.1f;
                        particle.AddBehaviour(move);
                    }
                    _particleSystem.AddParticle(particle, Lifespan);
                }
            }
        }

        /// <summary>
        /// Zastosowanie danego zachowania do obiektu.
        /// W tym wypadku przepisujemy pozycję obiektu do systemu cząsteczek.
        /// </summary>
        /// <param name="gameObject">Obiekt na którym działamy.</param>
        public override void Apply(GameObject gameObject)
        {
            _particleSystem.Position = gameObject.Position;
            base.Apply(gameObject);
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
