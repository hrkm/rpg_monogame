using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_8
{
    /// <summary>
    /// Umożliwia renderowanie obrazu niezależnie od natywnej rozdzielczości.
    /// </summary>
    public class ResolutionIndependentRenderer
    {
        /// <summary>
        /// Macierz przeskalowania ekranu.
        /// </summary>
        private static Matrix _scaleMatrix;
        /// <summary>
        /// Urządzenie rysujące naszą grę.
        /// </summary>
        private readonly GraphicsDevice _graphics;
        /// <summary>
        /// Kolor czyszczenia ekranu.
        /// </summary>
        public Color BackgroundColor = Color.CornflowerBlue;
        /// <summary>
        /// Aktualna wysokość ekranu.
        /// </summary>
        public int ScreenHeight;
        /// <summary>
        /// Aktualna szerokość ekranu.
        /// </summary>
        public int ScreenWidth;
        /// <summary>
        /// Docelowa wysokość ekranu.
        /// </summary>
        public int VirtualHeight;
        /// <summary>
        /// Docelowa szerokość ekranu;
        /// </summary>
        public int VirtualWidth;
        /// <summary>
        /// Czy musimy ponownie przeliczyć macierze?
        /// </summary>
        private bool _dirtyMatrix = true;
        /// <summary>
        /// Współczynnik skalowania wzdłuż osi X.
        /// </summary>
        private float _ratioX;
        /// <summary>
        /// Współczynnik skalowania wzdłuż osi Y.
        /// </summary>
        private float _ratioY;
        /// <summary>
        /// Okno w ramach którego rysujemy obiekty.
        /// </summary>
        private Viewport _viewport;

        /// <summary>
        /// Konstruktor w którym ustawiamy parametry dotyczące rozdzielczości
        /// wirtualnej i rzeczywistej.
        /// </summary>
        /// <param name="graphics">Urządzenie graficzne wykorzystywane do rysowania.</param>
        /// <param name="virtualWidth">Wirtualna docelowa szerokość ekranu.</param>
        /// <param name="virtualHeight">Wirtualna docelowa wysokość ekranu.</param>
        /// <param name="screenWidth">Rzeczywista szerokość ekranu.</param>
        /// <param name="screenHeight">Rzeczywista wysokość ekranu.</param>
        public ResolutionIndependentRenderer(GraphicsDevice graphics, int virtualWidth, int virtualHeight, int screenWidth, int screenHeight)
        {
            _graphics = graphics;
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        /// <summary>
        /// Inicjalizacja dla klasy.
        /// </summary>
        public void Initialize()
        {
            SetupVirtualScreenViewport();

            _ratioX = (float)_viewport.Width / VirtualWidth;
            _ratioY = (float)_viewport.Height / VirtualHeight;

            _dirtyMatrix = true;
        }

        /// <summary>
        /// Zresetowanie okna w ramach którego rysujemy
        /// do domyślnego (0,0,1,1) dla rozdzielczości
        /// rzeczywistej urządzenia.
        /// </summary>
        public void SetupFullViewport()
        {
            var vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = ScreenWidth;
            vp.Height = ScreenHeight;
            _graphics.Viewport = vp;
            _dirtyMatrix = true;
        }

        /// <summary>
        /// Ustawienie wirtualnego okna w ramach którego
        /// rysujemy zgodnie z docelową rozdzielczością
        /// rysowania.
        /// </summary>
        public void SetupVirtualScreenViewport()
        {
            float targetAspectRatio = VirtualWidth / (float)VirtualHeight;
            //wylicz największy możliwy wymiar okna do renderowania mieszczący się
            //w rzeczywistej rozdzielczości ekranu
            int width = ScreenWidth;
            var height = (int)(width / targetAspectRatio + .5f);
            if (height > ScreenHeight)
            {
                height = ScreenHeight;
                //rysowanie czarnych pasów po bokach ekranu
                width = (int)(height * targetAspectRatio + .5f);
            }

            //wycentruj oknow do rysowania w buforze rysowania
            _viewport = new Viewport
            {
                X = (ScreenWidth / 2) - (width / 2),
                Y = (ScreenHeight / 2) - (height / 2),
                Width = width,
                Height = height
            };
            _graphics.Viewport = _viewport;
        }

        /// <summary>
        /// Uzyskaj macierz skalowania ekranu.
        /// Jeśli coś się zmieniło to funkcja ta
        /// przeliczy ją na nowo.
        /// </summary>
        /// <returns>Macierz skalowania ekranu.</returns>
        public Matrix GetTransformationMatrix()
        {
            if (_dirtyMatrix)
            {
                var ratioX = (float) ScreenWidth/VirtualWidth;
                var ratioY = (float) ScreenHeight/VirtualHeight;
                var ratio = (ratioX > ratioY ? ratioY : ratioX);
                Matrix.CreateScale(ratio, ratio, 1f, out _scaleMatrix);
                _dirtyMatrix = false;
            }
            return _scaleMatrix;
        }

        /// <summary>
        /// Przelicz współrzędne punktu na ekranie urządzenia
        /// na współrzędne w ramach wirtualnego okna po którym
        /// rysujemy naszą grę.
        /// </summary>
        /// <param name="screenPosition">Rzeczywisty punkt na ekranie.</param>
        /// <returns>Punkt na wirtualnym ekranie odpowiadający punktowi rzeczywistemu.</returns>
        public Vector2 ScaleMouseToScreenCoordinates(Vector2 screenPosition)
        {
            float realX = screenPosition.X - _viewport.X;
            float realY = screenPosition.Y - _viewport.Y;

            var mouse = new Vector2(realX/_ratioX, realY/_ratioY);
            return mouse;
        }

        /// <summary>
        /// Rozpocznij rysowanie - ustaw okno rysowania
        /// zgodnie z podaną rozdzielczością docelową.
        /// </summary>
        public void BeginDraw()
        {
            //zresetuj okno do (0,0,1,1)
            SetupFullViewport();
            //wyczyść ekran
            _graphics.Clear(BackgroundColor);
            //przelicz współrzędne wirtualnego okna
            SetupVirtualScreenViewport();
            //poniżej można wywołać ponownie Clear z innym kolorem,
            //żeby uzyskać efekt ramki, inaczej pozostanie kolor
            //z poprzedniego czyszczenia ekranu
        }
    }
}