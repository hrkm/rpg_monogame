#RPG_ver_1 - wprowadzenie klasy `GameObject`, ulepszenie skakania, więcej obiektów na ekranie, prosty system kolizji

Uwaga: projekt RPG_ver_1 korzysta z folderu "packages" poprzedniego projektu w celu zaoszczędzenia miejsca. W związku z tym zaleca się ściągnięcie całego repozytorium lub dodanie MonoGame przy pomocy NuGeta.

##Klasa `GameObject`

Zauważyliśmy być może w RPG_ver_0, że w celu narysowania czegoś na ekranie musimy podać całkiem sporo różnych parametrów. W związku z tym ułatwimy sobie to zadanie w przyszłości i utworzymy klasę `GameObject`:

```
public class GameObject
{
	/// <summary>
	/// Aktualna pozycja obiektu.
	/// </summary>
	public Vector2 Position;

	/// <summary>
	/// Punkt względem którego wykonywane są wszystkie
	/// operacje przesunięcia/obracania/skalowania.
	/// </summary>
	public Vector2 Origin;

	/// <summary>
	/// Aktualny rozmiar obiektu.
	/// </summary>
	public float Scale = 1f;

	/// <summary>
	/// Aktualny obrót obiektu.
	/// </summary>
	public float Rotation;

	/// <summary>
	/// Aktualny kolor obiektu. Biały powoduje rysowanie
	/// obiektu w oryginalnym kolorze.
	/// </summary>
	public Color Color = Color.White;

	/// <summary>
	/// Tekstura wykorzystywana do rysowania obiektu.
	/// </summary>
	private Texture2D _texture;
	public Texture2D Texture
	{
		get { return _texture; }
		set
		{
			_texture = value;
			if (_texture != null)
				Origin = new Vector2((float)_texture.Width/2, (float)_texture.Height/2);
		}
	}

	/// <summary>
	/// Funkcja rysująca obiekt na ekranie zgodnie z jego stanem.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	/// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
	public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, 0);
	}
}
```

Klasa ta posiada wszystkie potrzebne informacje do narysowania tekstury na ekranie w określonym punkcie, obróconej o podany kąt i przeskalowanej wg podanego parametru.

Musimy teraz w naszej głównej aplikacji podmienić zmienną `position` na obiekt klasy `GameObject`:

```
private GameObject character;
```

A następnie utworzyć ten obiekt w funkcji `LoadContent`:

```
square = Content.Load<Texture2D>("square");
character = new GameObject();
character.Texture = square;
character.Position = new Vector2(240, 400);
```

Wówczas funkcja `Draw` w głównej aplikacji wyglądać będzie następująco:

```
GraphicsDevice.Clear(Color.CornflowerBlue);

spriteBatch.Begin();
character.Draw(gameTime, spriteBatch);
spriteBatch.DrawString(font, "Ahoj przygodo!", new Vector2(6,6), Color.White);
spriteBatch.End();

base.Draw(gameTime);
```

##Ulepszenie skakania

Skorzystajmy z nowo zdefiniowanej klasy `GameObject` i rozszerzmy jej możliwości. W tym celu wpierw dodamy interfejs określający zachowanie się obiektu:

```
public interface IBehaviour
{
	void Update(GameTime gameTime);
	void Apply();
}
```

Interfejs ten definiuje tylko dwie metody. `Update` ma na celu aktualizacje stanu zachowania, z kolei `Apply` powoduje zastosowanie zachowania do obiektu. Dopiero jednak implementacja specyficznego określi co to oznacza.

Zatem chcemy teraz utworzyć zachowanie określające skok postaci (którą obecnie jest wystąpienie klasy `GameObject`). W tym celu dodajmy poniższą klasę:

```
public class JumpBehaviour : IBehaviour
{
	/// <summary>
	/// Prędkość z jaką postać zacznie zwalniać/opadać.
	/// </summary>
	private const float Speed = 50;

	/// <summary>
	/// Siła wyskoku.
	/// </summary>
	private const float Power = 100;

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
	/// <param name="parent"></param>
	public JumpBehaviour(GameObject parent)
	{
		_originalPosition = parent.Position;
	}

	/// <summary>
	/// Aktualizacja fazy skoku.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public void Update(GameTime gameTime)
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
	public void Apply(GameObject gameObject)
	{
		gameObject.Position = _originalPosition + _offset;
	}

	/// <summary>
	/// Wywołanie polecenia skoku. Jeśli jesteśmy już w powietrzu
	/// to nic się nie stanie.
	/// </summary>
	public void Jump()
	{
		if (IsJumping) return;
		IsJumping = true;
		_dy = -Power;
	}
}
```

Powyższa implementacja definiuje w pełni prosty mechanizm skakania. Oczywiście można uzyskać coś lepszego jeśli skorzystamy z silników symulacji fizyki, ale na chwilę obecną wystarczy nam klasa `JumpBehaviour`. Uwagę należy zwrócić na implementację metody `Apply`, w której następuje aktualizacja pozycji tego obiektu przechowywanej w polu `Position`.

W celu aktywacji zachowania skoku należy wywołać metodę `Jump` obiektu `JumpBehaviour`, co uczynimy za chwilę w kodzie głównym aplikacji.

To jeszcze nie koniec. Musimy teraz zagwarantować, że to zachowanie (lub dowolne inne dodane w przyszłości) będzie stale aktualizowane oraz stosowane do danego obiektu. W tym celu dodamy listę zachowań w klasie `GameObject`:

```
public List<IBehaviour> Behaviours = new List<IBehaviour>();
```

A następnie zdefiniujemy w tej klasie funkcję `Update`, która będzie aktualizowała wszystkie dodane zachowania do danego obiektu:

```
public void Update(GameTime gameTime)
{
	foreach (var behaviour in Behaviours)
	{
		behaviour.Update(gameTime);
		behaviour.Apply(this);
	}
}
```

W kodzie głównej aplikacji musimy teraz zainicjalizować `JumpBehaviour`:

```
private JumpBehaviour jump;
```

Oraz powiązać to zachowanie z obiektem postaci w funkcji `LoadContent` (bo tam tworzymy postać):

```
jump = new JumpBehaviour(character);
character.Behaviours.Add(jump);
```

Żeby całość zadziałała dodajemy wywołanie funkcji `Update` na obiekcie `character` w funkcji o tej samej nazwie (czyli `Update`) znajdującej się w głównej aplikacji. Dodatkowo usuwamy poprzedni kod obsługi klawiatury i zastępujemy go poniższym:

```
var state = Keyboard.GetState();
if (state.IsKeyDown(Keys.Space))
	jump.Jump();
character.Update(gameTime);
```

##Więcej obiektów na ekranie

Na razie nasza gra jest dość uboga. Dodajmy zatem trochę więcej poruszających się obiektów. W tym celu skorzystamy z klasy `GameObject` i po prostu utworzymy listę dodatkowych elementów, które będziemy przesuwać z prawej strony ekranu na lewą.

Zdefiniujmy wpierw nowe zachowanie umożliwiające przesuwanie obiektów od prawej do lewej:

```
public class HorizontalMoveBehaviour : IBehaviour
{
	private float _x;
	public HorizontalMoveBehaviour(float x)
	{
		_x = x;
	}

	/// <summary>
	/// Aktualizacja fazy przesunięcia.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public void Update(GameTime gameTime)
	{
		_x -= (float) gameTime.ElapsedGameTime.TotalSeconds*100;
		if (_x < 0)
			_x = 480;
	}

	/// <summary>
	/// Zastosowanie danego zachowania do obiektu.
	/// </summary>
	/// <param name="gameObject">Obiekt na którym działamy.</param>
	public void Apply(GameObject gameObject)
	{
		gameObject.Position.X = _x;
	}
}
```

Dodajmy do projektu plik "star.xnb".

W klasie `Game1` dodajemy listę do przechowywania obiektów:

```
private List<GameObject> obstacles;
```

którą zainicializujemy w funkcji `LoadContent`:

```
var starTexture = Content.Load<Texture2D>("star");
obstacles = new List<GameObject>();
for (int i = 0; i < 3; i++)
{
	var star = new GameObject();
	star.Texture = starTexture;
	star.Position = new Vector2(160 * i, 250);
	star.Behaviours.Add(new HorizontalMoveBehaviour(star.Position.X));
	obstacles.Add(star);
}
```

Adekwatnie musimy dodać kod do funkcji `Update`:

```
foreach (var gameObject in obstacles)
{
	gameObject.Update(gameTime);
}
```

oraz do funkcji `Draw`:

```
foreach (var gameObject in obstacles)
{
	gameObject.Draw(gameTime, spriteBatch);
}
```

##Prosty system kolizji

Najprościej kolizcje można sprawdzać działając na obiektach typu prostokąty lub koła. W naszej aplikacji zastosujemy kolizcję stosując te drugie figury.

Na początek dodajmy do klasy `GameObject` pole definiujące promień koła, który będzie wykorzystany przy teście na kolizję:

```
public float Radius = 50;
```

Następnie definiujemy test kolizji w poniższy sposób:

```
/// <summary>
/// Metoda sprawdzająca czy ten obiekt koliduje z innym.
/// </summary>
/// <param name="gameObject">Drugi obiekt w kolizji.</param>
/// <returns>Prawda jeśli obiekty ze sobą kolidują, przeciwnie fałsz.</returns>
public bool CollidesWith(GameObject gameObject)
{
	if (Vector2.Distance(gameObject.Position, Position) <= gameObject.Radius + Radius)
		return true;
	return false;
}
```

Jak widać test porównuje punkty w których znadują się dwa obiekty, a następnie sprawdza czy odległość między nimi nie jest przypadkiem mniejsza od sumy promieni okręgów obiektów, które ze sobą testujemy. Jeśli tak jest, to wóczas obiekty kolidują ze sobą (innymi słowy dwa okręgi reprezentujące te obiekty nakładają się na siebie). W przeciwnym wypadku nie zachodzi kontakt między obiektami i należy zwrócić fałsz.

Skorzystajmy z tego testu i dodajmy możliwość zbierania gwiazdek w naszej aplikacji. W tym celu w funkcji `Update` musimy dodać poniższy kod:

```
for (int i = obstacles.Count - 1; i >= 0; i--)
{
	if (character.CollidesWith(obstacles[i]))
		obstacles.RemoveAt(i);
}
```

Pętla ta sprawdza czy występuje kolizja między naszą postacią i danym obiektem. Jeśli tak, to obiekt ten jest usuwany z listy, więc nie będzie już aktualizowany ani rysowany na ekranie. W efekcie mamy wrażenie, że został zebrany przez naszą postać.

Zwrócić należy uwagę, że usuwamy elementy od końca - dzięki temu nie musimy się zastanawiać czy powinniśmy inkrementować zmienną `i` czy nie.