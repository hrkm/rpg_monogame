using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = RPG_ver_5.IDrawable;
using IUpdateable = RPG_ver_5.IUpdateable;

namespace RPG_ver_5
{
    class SingleLine : IUpdateable, IDrawable
    {
        /// <summary>
        /// Postać sterowana przez gracza.
        /// </summary>
        public GameObject Character;
        /// <summary>
        /// Skok podczepiony pod postać gracza.
        /// </summary>
        private JumpBehaviour jump;

        /// <summary>
        /// Czy jest już koniec gry?
        /// </summary>
        public bool GameOver;
        /// <summary>
        /// Aktualny wynik uzyskany dla tego fragmentu gry.
        /// </summary>
        public int Score;

        /// <summary>
        /// Lista przeszkód i gwiazdek.
        /// </summary>
        private List<GameObject> obstacles = new List<GameObject>();

        /// <summary>
        /// Kiedy wygenerowano ostatni obiekt?
        /// </summary>
        private float timeFromLastObject;

        /// <summary>
        /// Po ilu sekundach wygenerować następny obiekt?
        /// </summary>
        private float timeForGeneratingNextObject;

        /// <summary>
        /// Wektor przesunięcia obiektu SingleLine na ekranie głównym gry.
        /// </summary>
        private Vector2 _offset;

        /// <summary>
        /// Tworzymy nowy obiekt klasy SingleLine podając jego pozycję na ekranie.
        /// Wszystkie pozycje dla umieszczanych obiektów muszą być przesunięte o ten
        /// wektor. Dodatkowo ustawiamy kolor postaci na unikatowy dla danej linii.
        /// </summary>
        /// <param name="offset">Pozycja na ekranie.</param>
        /// <param name="color">Kolor postaci sterowanej przez gracza.</param>
        public SingleLine(Vector2 offset, Color color)
        {
            _offset = offset;

            Character = GameObjectFactory.CreateCharacter(_offset + new Vector2(240, 120));
            Character.Color = color;
            Character.Rotation = 45*(float) Math.PI / 180;

            jump = BehaviourFactory.CreateJumpBehaviour(Character, 100, 100);
            Character.AddBehaviour(jump);

            var emitParticles = new EmitParticlesBehaviour();
            emitParticles.Lifespan = 2;
            emitParticles.ParticlesPerSpawn = 3;
            emitParticles.AddRandomBehaviour = true;
            emitParticles.ParticleTemplate = Character.Clone();
            emitParticles.ParticleTemplate.Alpha = 0.5f;
            Character.AddBehaviour(emitParticles);

            timeForGeneratingNextObject = Game1.random.Next(10, 30)/10.0f;
        }

        /// <summary>
        /// Wywołanie instrukcji skoku na zachowaniu skoku.
        /// </summary>
        public void Jump()
        {
            jump.Jump();
        }

        /// <summary>
        /// Jeśli jest koniec gry, to nic nie rób.
        /// W przeciwnym wypadku zaktualizuj wszystkie obiekty i
        /// sprawdź kolizje.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public void Update(GameTime gameTime)
        {
            if (GameOver)
                return;

            timeFromLastObject += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeFromLastObject > timeForGeneratingNextObject)
            {
                timeFromLastObject = 0;
                timeForGeneratingNextObject = Game1.random.Next(10, 30) / 10.0f;
                if (Game1.random.Next(2) == 0)
                {
                    var star = GameObjectFactory.CreateStar(_offset + new Vector2(0, 0));
                    var emitParticles = new EmitParticlesBehaviour();
                    emitParticles.ParticleTemplate = GameObjectFactory.CreateParticle(new Vector2(0,0));
                    emitParticles.ParticleTemplate.Alpha = 0.5f;
                    star.AddBehaviour(emitParticles);
                    obstacles.Add(star);
                }
                else
                {
                    var obstacle = GameObjectFactory.CreateObstacle(_offset + new Vector2(0, 170));
                    obstacles.Add(obstacle);
                }
            }

            Character.Update(gameTime);

            foreach (var gameObject in obstacles)
            {
                gameObject.Update(gameTime);
            }

            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                var obstacle = obstacles[i];
                if (!obstacle.Active)
                    obstacles.RemoveAt(i);
                else if (Character.CollidesWith(obstacle))
                {
                    //TODO: poniższy kod prowadzi do bad smells, w kolejnej wersji należy go zrefaktoryzować!
                    if (obstacle.Color == Color.Black)
                    {
                        GameOver = true;
                        AssetManager.Instance.Hit.Play();
                    }
                    else
                    {
                        obstacles.RemoveAt(i);
                        Score += 1;
                        AssetManager.Instance.PickUp.Play();
                    }
                }
            }
        }

        /// <summary>
        /// Funkcja rysująca wszystkie elementy składowe na ekranie.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        /// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var gameObject in obstacles)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
            Character.Draw(gameTime, spriteBatch);
        }
    }
}
