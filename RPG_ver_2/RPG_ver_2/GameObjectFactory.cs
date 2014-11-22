using Microsoft.Xna.Framework;

namespace RPG_ver_2
{
    /// <summary>
    /// Klasa służąca do tworzenia obiektów klasy GameObject.
    /// </summary>
    public static class GameObjectFactory
    {
        /// <summary>
        /// Utwórz obiekt postaci. Postać posiada teksturę Square.
        /// </summary>
        /// <param name="position">Pozycja w której umieszczamy postać.</param>
        /// <returns>Obiekt z postacią.</returns>
        public static GameObject CreateCharacter(Vector2 position)
        {
            var character = new GameObject();
            character.Texture = AssetManager.Instance.Square;
            character.Position = position;
            return character;
        }

        /// <summary>
        /// Utwórz obiekt gwiazdki. Gwiazdka posiada teksturę Star
        /// oraz żółty kolor.
        /// </summary>
        /// <param name="position">Pozycja w której umieszczamy gwiazdkę.</param>
        /// <returns>Obiekt z gwiazdką.</returns>
        public static GameObject CreateStar(Vector2 position)
        {
            var star = new GameObject();
            star.Texture = AssetManager.Instance.Star;
            star.Position = position;
            star.Behaviours.Add(BehaviourFactory.CreateHorizontalMoveBehaviour());
            star.Color = Color.Yellow;
            return star;
        }

        /// <summary>
        /// Utwórz obiekt przeszkody. Przeszkoda posiada teksturę Star,
        /// czarny kolor oraz jest dwukrotnie mniejsza od gwiazdki.
        /// </summary>
        /// <param name="position">Pozycja w której umieszczamy przeszkodę.</param>
        /// <returns>Obiekt z przeszkodą.</returns>
        public static GameObject CreateObstacle(Vector2 position)
        {
            var obstacle = new GameObject();
            obstacle.Texture = AssetManager.Instance.Star;
            obstacle.Position = position;
            obstacle.Behaviours.Add(BehaviourFactory.CreateHorizontalMoveBehaviour());
            obstacle.Color = Color.Black;
            obstacle.Scale = 0.5f;
            obstacle.Radius = 25;
            return obstacle;
        }
    }
}
