using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace RPG_ver_5
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
        public Texture2D Star;
        public Texture2D Mouse;
        public Song BackgroundMusic;
        public SoundEffect Hit;
        public SoundEffect PickUp;
        public SoundEffect Jump;

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
        }
    }
}
