using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_2
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
