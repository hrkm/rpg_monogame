using Microsoft.Xna.Framework;

namespace RPG_ver_7
{
    /// <summary>
    /// Zachowanie definiujące prosty skok w pionie.
    /// </summary>
    public class JumpBehaviour : Behaviour
    {
        /// <summary>
        /// Prędkość z jaką postać zacznie zwalniać/opadać.
        /// </summary>
        public float Speed = 50;

        /// <summary>
        /// Siła wyskoku.
        /// </summary>
        public float Power = 100;

        /// <summary>
        /// Czy jesteśmy już w powietrzu?
        /// </summary>
        public bool IsJumping;

        /// <summary>
        /// Aktualny poziom wyskoku.
        /// </summary>
        private Vector2 _offset;

        /// <summary>
        /// Pozycja, z której startowaliśmy.
        /// </summary>
        private readonly Vector2 _originalPosition;

        /// <summary>
        /// Prędkość wznoszenia/opadania.
        /// </summary>
        private float _dy;

        /// <summary>
        /// Konstruktor ustawiający pozycję wyjściową na pozycję danego obiektu.
        /// </summary>
        /// <param name="parent">Dostarcza informacje na temat czasu.</param>
        public JumpBehaviour(GameObject parent)
        {
            _originalPosition = parent.Position;
        }

        /// <summary>
        /// Aktualizacja fazy skoku.
        /// </summary>
        /// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
        public override void Update(GameTime gameTime)
        {
            //zwiększamy prędkość opadania
            _dy += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;
            //przesuwamy pozycję
            _offset.Y += (float)gameTime.ElapsedGameTime.TotalSeconds*_dy;

            //jeśli aktualna faza skoku posiada przesunięcie w Y dodatnie
            //to skok się zakończył i należy wyzerować wszystkie wartości
            if (_offset.Y > 0)
            {
                _offset.Y = 0;
                IsJumping = false;
                _dy = 0;
            }
        }

        /// <summary>
        /// Zastosowanie danego zachowania do obiektu.
        /// </summary>
        /// <param name="gameObject">Obiekt na którym działamy.</param>
        public override void Apply(GameObject gameObject)
        {
            gameObject.Position = _originalPosition + _offset;
            base.Apply(gameObject);
        }

        /// <summary>
        /// Wywołanie polecenia skoku. Jeśli jesteśmy już w powietrzu
        /// to nic się nie stanie.
        /// </summary>
        public void Jump()
        {
            if (IsJumping) return;
            AssetManager.Instance.Jump.Play();
            IsJumping = true;
            _dy = -Power;
        }
    }
}
