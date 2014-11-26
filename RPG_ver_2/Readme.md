#RPG_ver_2 - zarządzanie teksturami, fabryka obiektów, fabryka zachowań, pierwsza wersja gry

##Zarządzanie teksturami

Z praktycznego punktu widzenia możemy posiadać wiele miejsc w naszej aplikacji, w których będziemy potrzebowali danej tekstury. Stąd trzymanie wszystkiego w klasie `Game1` jest trochę kłopotliwe. Z tego powodu proponuję użyć następującej klasy typu Singleton do zarządzania teksturami oraz innymi zasobami:

```
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
```

Zgodnie z powyższą implementacją powinniśmy wywołać metody `LoadContent`/`UnloadContent` w ich odpowiednikach klasy `Game1`. Zwróćcie uwagę że przenieśliśmy zmienne `Font` oraz `Square` i utworzyliśmy zmienną do przechowywania tekstury z gwiazdą.

Aby skorzystać z tak utworzonej klasy możemy odwołać się do niej na przykład tak:

```
character.Texture = AssetManager.Instance.Square;
```

##Fabryka obiektów

Do tej pory nowe obiekty gry tworzyliśmy w głównej klasie `Game1`, gdzie ustawialiśmy wszystkie istotne wartości pól. Z lenistwa niektórym z nich ustawialiśmy również wartości domyślne, żeby nie trzeba było ich modyfikować. Wyobraźmy sobie sytuację, gdzie w różnych miejscach aplikacji będziemy tworzyć obiekty danego typu (np. gwiazdki do zbierania) - zarządzanie kodem stało by się strasznie uciążliwe. Dlatego stworzymy klasę pomocniczą `GameObjectFactory`, która będzie posiadała metody tworzące poszczególne obiekty, np. postać główną, gwiazdki czy przeszkody, które dodamy później.

```
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
		star.Behaviours.Add(new HorizontalMoveBehaviour(star.Position.X));
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
		obstacle.Behaviours.Add(new HorizontalMoveBehaviour(obstacle.Position.X));
		obstacle.Color = Color.Black;
		obstacle.Scale = 0.5f;
		obstacle.Radius = 25;
		return obstacle;
	}
}
```

Zauważcie, że metody wytwórcze tej klasy (`Create*`) są metodami statycznymi, więc nie trzeba tworzyć obiektów klasy `GameObjectFactory`.

Przy okazji skorzystaliśmy z tego, że przy rysowaniu w klasie `GameObject` uwzględniamy kolor - ponieważ nasze tekstury są białe, to rysowanie ich z innym kolorem spowoduje, że będą w wybranym przez nas kolorze.

Wykorzystując powyższą klasę nasz kod głównej aplikacji przy tworzeniu obiektów może wyglądać następująco:

```
AssetManager.Instance.LoadContent(Content);

character = GameObjectFactory.CreateCharacter(new Vector2(240, 400));

jump = new JumpBehaviour(character);
character.Behaviours.Add(jump);

obstacles = new List<GameObject>();
for (int i = 0; i < 3; i++)
{
	var star = GameObjectFactory.CreateStar(new Vector2(160*i, 250));
	obstacles.Add(star);

	var obstacle = GameObjectFactory.CreateObstacle(new Vector2(160*i, 550));
	obstacles.Add(obstacle);
}
```

##Fabryka zachowań

Czy zwróciliście uwagę, że obiekty `IBehaviour` są dodawane w różnych miejscach aplikacji? Możemy również dla nich utworzyć fabrykę abstrakcyjną.

```
public static class BehaviourFactory
{
	/// <summary>
	/// Utwórz nowe zachowanie skoku.
	/// </summary>
	/// <param name="parent">Obiekt, który ma mieć możliwość skoku.</param>
	/// <param name="power">Siła wyskoku.</param>
	/// <param name="speed">Prędkość opadania.</param>
	/// <returns>Obiekt zachowania skoku.</returns>
	public static JumpBehaviour CreateJumpBehaviour(GameObject parent, float power, float speed)
	{
		var jump = new JumpBehaviour(parent);
		jump.Power = power;
		jump.Speed = speed;
		return jump;
	}

	/// <summary>
	/// Utwórz nowe zachowanie przesuwania w lewo.
	/// </summary>
	/// <param name="x">Początkowa wartość współrzędnej X.</param>
	/// <returns>Obiekt przesuwania w lewo.</returns>
	public static HorizontalMoveBehaviour CreateHorizontalMoveBehaviour(float x)
	{
		var horizontalMove = new HorizontalMoveBehaviour(x);
		return horizontalMove;
	}
}
```

##Pierwsza wersja gry

Mając już podstawy możemy utworzyć pierwszy wariant gry. Zasady gry są następujące - kierujemy postacią, której możemy wydać polecenie skoku. Postać musi unikać przeszkód w postaci czarnych gwiazdek, ale powinna zbierać żółte gwiazdki. Za każdą zebraną żółtą gwiazdkę otrzymujemy 1 punkt. Wpadnięcie w przeszkodę kończy grę.

W tym celu zmodyfikujemy wpierw `HorizontalMoveBehaviour`, tak aby obiekty zaczynały zawsze z prawej strony ekranu (lekko poza ekranem) i po zniknięciu z lewej strony aby ulegały dezaktywacji. Obiekty, które będą niewatne będą następnie usuwane z listy w funkcji `Update` klasy `Game1`.

Lista potrzebnych modyfikacji:
* dodajemy pole `Active` do klasy `GameObject`:

```
/// <summary>
/// Czy obiekt jest aktywny?
/// </summary>
public bool Active = true;
```

* modyfikujemy implementację klasy `HorizontalMoveBehaviour`, i oczywiście aktualizujemy wszelkie odwołania do niej (mam nadzieję, że z tym sobie poradzicie):

```
public class HorizontalMoveBehaviour : IBehaviour
{
	private float _x = 480 + 50;

	/// <summary>
	/// Aktualizacja fazy przesunięcia.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public void Update(GameTime gameTime)
	{
		_x -= (float) gameTime.ElapsedGameTime.TotalSeconds*100;
	}

	/// <summary>
	/// Zastosowanie danego zachowania do obiektu.
	/// </summary>
	/// <param name="gameObject">Obiekt na którym działamy.</param>
	public void Apply(GameObject gameObject)
	{
		gameObject.Position.X = _x;
		if (_x < -50)
			gameObject.Active = false;
	}
}
```

	Zauważcie, że obiekty będą ustawiane o 50 pikseli dalej niż prawa krawędź ekranu i ulegną dezaktywacji dopiero jak znajdą się o 50 pikseli dalej niż lewa krawędź ekranu. Wynika to z tego, że moje obiekty mają oryginalny rozmiar 100x100, a punkt względem którego obiekty te ulegają transformacji jest dokładnie na środku tekstury, czyli (50,50).

* w głównej pętli aplikacji (a więc funkcja `Update` klasy `Game1`):

```
for (int i = obstacles.Count - 1; i >= 0; i--)
{
	var obstacle = obstacles[i];
	if (!obstacle.Active)
		obstacles.RemoveAt(i);
	else if (character.CollidesWith(obstacle))
		obstacles.RemoveAt(i);
}
```

Teraz musimy zająć się generowaniem obiektów przeszkód i gwiazdek w określonych przedziałach czasowych oraz w takim położeniu na osi Y, aby postać była w stanie wejść w kolizję z danym obiektem (bo będzie się on przesuwał z prawej strony na lewą). W tym celu dodamy pomocnicze pole w klasie `Game1` do zliczania ile czasu upłynęło od ostatnio wygenerowanego obiektu:

```
private float timeFromLastObject;
```

Następnie w funkcji `Update` będziemy generować nowy losowy obiekt jeśli wartość przechowywana w tym polu przekroczy 3 sekundy:

```
timeFromLastObject += (float) gameTime.ElapsedGameTime.TotalSeconds;
if (timeFromLastObject > 3)
{
	timeFromLastObject = 0;
	if (random.Next(2) == 0)
	{
		var star = GameObjectFactory.CreateStar(new Vector2(0, 280));
		obstacles.Add(star);
	}
	else
	{
		var obstacle = GameObjectFactory.CreateObstacle(new Vector2(0, 450));
		obstacles.Add(obstacle);
	}
}
```

Żeby gra mogła nazywać się grą musimy jeszcze wprowadzić dwa elementy - zliczanie punktów oraz stan przegranej, czyli "Game Over".

Dodajemy zatem dwa dodatkowe pola, które umożliwią nam obsługę tych dwóch elementów:

```
private bool gameOver;
private int score;
```

W funkcji sprawdzającej kolizje zweryfikujemy czy to kolizja z gwiazdką czy przeszkodą na podstawie koloru. Przeszkoda ma kolor czarny i w wyniku zderzenia następuje koniec gry. Gwiazdka ma kolor żółty i w wyniku zderzenia zwiększamy liczbę punktów o 1.

```
for (int i = obstacles.Count - 1; i >= 0; i--)
{
	var obstacle = obstacles[i];
	if (!obstacle.Active)
		obstacles.RemoveAt(i);
	else if (character.CollidesWith(obstacle))
	{
		//TODO: poniższy kod prowadzi do bad smells, w kolejnej wersji należy go zrefaktoryzować!
		if (obstacle.Color == Color.Black)
			gameOver = true;
		else
		{
			obstacles.RemoveAt(i);
			score += 1;
		}
	}
}
```

Liczbę punktów możemy wypisać na ekranie zamiast tekstu "Ahoj Przygodo".

Jeśli gra jest w stanie "Game Over", to kolejne wciśnięcie klawisza spacji ma umożliwić rozpoczęcie nowej gry. W tym celu wprowadźmy funkcję ustawiającą wszystkie elementy w pozycjach wyjściowych.

```
private void NewGame()
{
	obstacles = new List<GameObject>();
	score = 0;
	gameOver = false;
}
```

Ponadto w funkcji `Update` nie aktualizujmy obiektów gry jeśli obecny stan to koniec gry.

```
protected override void Update(GameTime gameTime)
{
	// TODO: dodaj logikę gry w tym miejscu
	if (!gameOver)
	{
		//dotychczasowa logika gry
	}
	else
	{
		var state = Keyboard.GetState();
		if (state.IsKeyDown(Keys.Space))
		{
			NewGame();
		}
	}

	base.Update(gameTime);
}
```

Obecnie funkcja `Draw` może również informować o stanie końca gry poprzez dodanie poniższych dwóch linijek:

```
if (gameOver)
	spriteBatch.DrawString(AssetManager.Instance.Font, "Game Over", new Vector2(6, 300), Color.White);
```