using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace RPG_ver_9
{
    /// <summary>
    /// Klasa do zarządzania zasobami.
    /// </summary>
    public class AssetManager
    {
        private static AssetManager _instance;

        /// <summary>
        /// Korzystając ze wzorca Singleton dostęp do klasy AssetManager
        /// mamy dzięki temu statycznemu polu.
        /// </summary>
        public static AssetManager Instance
        {
            get { return _instance ?? (_instance = new AssetManager()); }
        }

        public SpriteFont Font;
        public Texture2D Square;
        public Texture2D Circle;
        public Texture2D Star;
        public Texture2D Mouse;
        public Song BackgroundMusic;
        public SoundEffect Hit;
        public SoundEffect PickUp;
        public SoundEffect Jump;

        public List<Texture2D> Trees; 

        /// <summary>
        /// Prywatny konstruktor, ponieważ realizujemy wzorzec Singleton.
        /// </summary>
        private AssetManager()
        {
        }

        /// <summary>
        /// Funkcja wczytująca tekstury, dźwięki, czcionki oraz
        /// tworzące wszystkie inne dynamiczne obiekty.
        /// </summary>
        /// <param name="content">Zarządca zasobów MonoGame.</param>
        public void LoadContent(ContentManager content)
        {
            Circle = content.Load<Texture2D>("circle");
            Square = content.Load<Texture2D>("square");
            Font = content.Load<SpriteFont>("font");
            Star = content.Load<Texture2D>("star");
            Mouse = content.Load<Texture2D>("mouse");
#if WINDOWS_PHONE || ANDROID
            BackgroundMusic = content.Load<Song>("Rolemusic_-_pl4y1ng");
#endif
            Hit = content.Load<SoundEffect>("hit");
            Jump = content.Load<SoundEffect>("jump");
            PickUp = content.Load<SoundEffect>("pickup");
        }

        /// <summary>
        /// Jeśli stworzymy obiekty dynamicznie z poziomu aplikacji,
        /// to tutaj powinniśmy zwolnić zasoby przez nie zajęte.
        /// </summary>
        public void UnloadContent()
        {
            //TODO: zwolnij dodatkowe zasoby nie wczytane przy pomocy ContentManager
            foreach (var texture2D in Trees)
            {
                texture2D.Dispose();
            }
        }

        /// <summary>
        /// Przygotuj unikalne tekstury przedstawiające drzewa.
        /// </summary>
        /// <param name="spriteBatch">Obiekt umożliwiający rysowanie.</param>
        public void PrepareTrees(SpriteBatch spriteBatch)
        {
            Trees = new List<Texture2D>();
            for (int i = 0; i < 20; i++)
            {
                var renderTarget = new RenderTarget2D(spriteBatch.GraphicsDevice, 160, 160);
                //ustaw docelowe miejsce renderowania w pamięci
                spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
                //wyczyść cały obszar kolorem przezroczystym
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                //narysuj losowe drzew
                spriteBatch.Begin();
                spriteBatch.Draw(Square, new Vector2(80, 135), null, Color.Brown, 0, new Vector2(50, 50), 0.5f, SpriteEffects.None, 0);
                //narysuj 10 losowych "gałęzi"
                spriteBatch.Draw(Circle, new Vector2(80, 80), null, Color.DarkGreen, 0, new Vector2(50, 50), 1, SpriteEffects.None, 0);
                for (int j = 0; j < 10; j++)
                {
                    spriteBatch.Draw(Circle, new Vector2(80 + Game1.random.Next(-40, 40), 80 + Game1.random.Next(-30, 30)), null, Color.DarkGreen, 0, new Vector2(50, 50), Game1.random.Next(40, 80) / 100f, SpriteEffects.None, 0);
                }
                spriteBatch.End();
                //dodaje gotowe drzewo do wzorców drzew
                Trees.Add(renderTarget);
            }
            //zresetuj urządzenie aby renderowało grafikę na ekran
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
