#RPG_ver_7 - refaktoryzacja zachowań, renderowanie do tekstury, efekt paralaksy

##Refaktoryzacja zachowań

Dotychczas chcąc dodać kolejny rodzaj zachowań tworzyliśmy klasę, która implementowała by dane zachowanie. W ten sposób powstały klasy `HorizontalMoveBehaviour` oraz `RandomBehaviour`, które mają dość podobną funkcjonalność przesuwania obiektów z jednego końca planszy na drugi, prawda? Wynika z tego, że niektóre zachowania mogą składać się z serii innych zachowań, jak na przykład zachowanie umożliwiające przesuwanie obiektu z jednego punktu do drugiego. Przy okazji być może warto wiedzieć kiedy takie zachowanie zakończy swoje działanie, żeby wykonać jakąś akcję z tym powiązaną. Dlatego zmodyfikujemy teraz interfejs `IBehaviour` i zamienimy go na klasę abstrakcyjną z dodatkową obsługę wydarzenia na zakończenie działania danego zachowania.

```
public abstract class Behaviour : IUpdateable
{
	/// <summary>
	/// Event rzucany w momencie zakończenia działania zachowania.
	/// </summary>
	public event EventHandler Finished;

	/// <summary>
	/// Funkcja pomocnicza do rzucenia wydarzenia Finished.
	/// </summary>
	protected void OnFinished()
	{
		if (Finished != null)
			Finished(this, new EventArgs());
	}

	/// <summary>
	/// Aktualizacja stanu zachowania.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public abstract void Update(GameTime gameTime);
	/// <summary>
	/// Zastosowanie zachowania do wskazanego obiektu gry.
	/// </summary>
	/// <param name="gameObject">Obiekt, który ma być zmodyfikowany.</param>
	public abstract void Apply(GameObject gameObject);
}
```

Następnie zamiast zachowania `HorizontalMoveBehaviour`, utworzymy klasę `MoveBehaviour` umożliwiającą przesuwanie obiektu w sposób liniowy między dwoma punktami.

```
public class MoveBehaviour : Behaviour
{
	/// <summary>
	/// Aktualna pozycja.
	/// </summary>
	public Vector2 CurrentPosition;
	/// <summary>
	/// Pozycja docelowa.
	/// </summary>
	public Vector2 TargetPosition;
	/// <summary>
	/// Prędkość przesuwania obiektu wyrażona w pikselach na sekundę.
	/// </summary>
	protected Vector2 dv;

	/// <summary>
	/// Konstruktor zachowania przesunięcia.
	/// </summary>
	/// <param name="targetPosition">Pozycja docelowa.</param>
	/// <param name="startingPosition">Pozycja początkowa.</param>
	/// <param name="duration">Liczba sekund przeznaczona na wykonanie przesunięcia.</param>
	public MoveBehaviour(Vector2 targetPosition, Vector2 startingPosition, float duration)
	{
		TargetPosition = targetPosition;
		CurrentPosition = startingPosition;
		//ustawienie czasu wykonania na zero przesunie obiekt natychmiast
		if (duration == 0)
		{
			CurrentPosition = targetPosition;
			dv = new Vector2(0, 0);
		}
		else
		{
			dv = (TargetPosition - CurrentPosition)/duration;
		}
	}

	/// <summary>
	/// Aktualizacja stanu zachowania poprzez aktualizację obecnej pozycji.
	/// Jeśli na X lub Y pozycja docelowa została już osiągnięta/przekroczona,
	/// to poniższa funkcja zagwarantuje, że obiekt zostanie ustawiony dokładnie
	/// we wskazanym punkcie.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public override void Update(GameTime gameTime)
	{
		CurrentPosition += dv*(float) gameTime.ElapsedGameTime.TotalSeconds;
		if (dv.X > 0)
		{
			if (CurrentPosition.X > TargetPosition.X) CurrentPosition.X = TargetPosition.X;
		}
		else
		{
			if (CurrentPosition.X < TargetPosition.X) CurrentPosition.X = TargetPosition.X;
		}
		if (dv.Y > 0)
		{
			if (CurrentPosition.Y > TargetPosition.Y) CurrentPosition.Y = TargetPosition.Y;
		}
		else
		{
			if (CurrentPosition.Y < TargetPosition.Y) CurrentPosition.Y = TargetPosition.Y;
		}
		if (CurrentPosition.X == TargetPosition.X && CurrentPosition.Y == TargetPosition.Y)
		{
			OnFinished();
		}
	}

	/// <summary>
	/// Zastosowanie zachowania do wskazanego obiektu gry.
	/// Odbywa się to poprzez przepisanie obecnej pozycji z zachowania jako pozycja obiektu.
	/// </summary>
	/// <param name="gameObject">Obiekt, który ma być zmodyfikowany.</param>
	public override void Apply(GameObject gameObject)
	{
		gameObject.Position = CurrentPosition;
	}
}
```

Może wówczas usunąć klasę `HorizontalMoveBehaviour` i zamiast niej w `GameObjectFactory` dodawać zachowanie `MoveBehaviour` w następujący sposób:

```
var move = BehaviourFactory.CreateHorizontalMoveBehaviour(new Vector2(-50, position.Y), new Vector2(480 + 50, position.Y), (480 + 50 + 50) / 100);
move.Finished += (sender, args) =>
{
	obstacle.Active = false;
};
obstacle.AddBehaviour(move);
```

Czyli wpierw tworzmy nowe `MoveBehaviour`, ustawiając współrzędną X na 480 + 50, czyli 50 pikseli poza prawą stroną ekranu. Następnie pozycja docelowa posiada współrzędną X ustawioną na 50 pikseli poza lewą stroną ekranu. Czas trwania przesuwania w oryginalnej klasie `HorizontalMoveBehaviour` wynosił 100 pikseli na sekundę, dlatego możemy wyliczyć ile sekund powinna trwać cała animacja dzieląc liczbę przebytych pikseli przez 100, aby uzyskać taką samą prędkość przy pomocy klasy `MoveBehaviour`. Dodatkowo pod koniec swojego działania pierwotna klasa deaktywowała przesuwany obiekt - tutaj zostało to osiągnięte poprzez użycie wydarzenia `Finished` z klasy `Behaviour`, które jest wywoływane w momencie gdy obiekt zakończy ruch, i wtedy obiekt staje się nieaktywny. W ostatniej linijce oczywiście dodajemy zachowanie do obiektu.

Drugim zachowaniem, które chcielibyśmy zastąpić przy pomocy naszej nowej klasy jest `RandomBehaviour`. Jednak w tym zachowaniu oprócz przesuwania obiektów występuje również obracanie obiektu. Jest to na tyle jednorazowa i unikatowa dla tego zachowania operacja, że chcielibyśmy uniknąć potrzeby tworzenia nowej klasy i móc zdefiniować to zachowanie w sposób dynamiczny w kodzie. Możemy uzyskać ten efekt poprzez dodanie dodatkowego zdarzenia wywoływanego w momencie wywołania funkcji `Apply` w klasie `Behaviour`:

```
/// <summary>
/// Event rzucany w momencie zakończenia działania zachowania.
/// </summary>
public event EventHandler<GameObject> Applied;

/// <summary>
/// Funkcja pomocnicza do rzucenia wydarzenia Applied.
/// </summary>
/// <param name="gameObject">Obiekt, który jest pod wpływem zachowania.</param>
protected virtual void OnApplied(GameObject gameObject)
{
	if (Applied != null)
		Applied(this, gameObject);
}
```

Zmodyfikujemy jeszcze implementację funkcji `Apply` w tej klasie:

```
/// <summary>
/// Zastosowanie zachowania do wskazanego obiektu gry.
/// </summary>
/// <param name="element">Obiekt, który ma być zmodyfikowany.</param>
public virtual void Apply(GameObject element)
{
	OnApplied(element);
}
```

Oraz w każdej klasie dziedziczącej po `Behaviour` musimy na końcu funkcji `Apply` wywołać wersję z klasy bazowej w następujący sposób:

```
base.Apply(gameObject);
```

Wówczas możemy usunąć klasę `RandomBehaviour` i dodać w miejscu jej tworzenia w klasie `EmitParticlesBehaviour` następujący fragment kodu:

```
if (AddRandomBehaviour)
{
	var move = new MoveBehaviour(new Vector2(-50, particle.Position.Y), particle.Position,
		(particle.Position.X + 50)/100);
	move.Applied += (sender, o) => o.Rotation += 0.1f;
	particle.AddBehaviour(move);
}
```

Czyli dodajemy zachowanie przesuwające obiekt w poziomie od obecnej pozycji do pozycji znajdującej się 50 pikseli poza lewą krawędzią ekranu, a za każdym razem gdy zostanie wywołana funkcja `Apply`, dodatkowo obracamy element o 0.1 radiana. W efekcie uzyskujemy dokładnie takie samo zachowanie jak w oryginale, a zamieniliśmy tylko dwie klasy na jedną trochę modyfikując klasę bazową `Behaviour` (która w poprzednich wersjach była tylko interfejsem).

Oczywiście `JumpBehaviour` oraz `EmitParticlesBehaviour` są na tyle specyficzne i odmienne od zachowania ruchu, że muszą być wyróżnione jako osobne klasy.

##Renderowanie do tekstury

Chcemy urozmaicić trochę grę generując w sposób dynamiczny obrazki reprezentujące drzewa. Chcemy to zrobić programistycznie, żeby efekt za każdym razem był losowy i dzięki temu unikalny. W związku z tym musimy wykonać rendering do tekstury zamiast na ekran. Aby to osiągnąć wpierw dodajemy listę do przechowywania tych tekstur:

```
public List<Texture2D> Trees; 
```

Następnie dodajemy metodę `PrepareTrees`, która służy do narysowania drzewek w sposób dynamiczne już w trakcie działania gry.

```
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
			spriteBatch.Draw(Circle, new Vector2(80 + Game1.random.Next(-40,40), 80 + Game1.random.Next(-30,30)), null, Color.DarkGreen, 0, new Vector2(50, 50), Game1.random.Next(40,80)/100f, SpriteEffects.None, 0);
		}
		spriteBatch.End();
		//dodaje gotowe drzewo do wzorców drzew
		Trees.Add(renderTarget);
	}
	//zresetuj urządzenie aby renderowało grafikę na ekran
	spriteBatch.GraphicsDevice.SetRenderTarget(null);
}
```

Funkcja ta jest wywołana zaraz po `LoadContent` w głównej klasie aplikacji. Ponadto zauważmy, że utworzyliśmy właśnie zbiór tekstur, które nie są zarządzane przez `ContentManager` od MonoGame, dlatego musimy dodać w funkcji `UnloadContent` następujący kod do zwolnienia zasobów:

```
foreach (var texture2D in Trees)
{
	texture2D.Dispose();
}
```

##Efekt paralaksy

Paralaksą nazywamy m.in. wrażenie, że w trakcie ruchu obiekty znajdujące się blisko nas przemieszczają się szybciej od tych znajdujących się daleko. Załóżmy że jedziemy pociągiem i obserwujemy drzewa przez okno - te, które będą blisko, będą przemieszczały się szybciej niż te znajdujące się w większej odległości. Oczywiście w rzeczywistości tak nie jest, ale z racji perspektywy i różnego kąta pod którym obserwujemy obiekty bliskie i dalekie, w naszym mózgu wytwarza się wrażenie na temat różnych prędkość tych obserwowanych drzew. To jest właśnie paralaksa.

Spróbujemy zatem dodać do naszej gry ten efekt poprzez dodanie "drzewek", które będą się przesuwały w tle za postacią oraz również przed postacią. Dodatkowo wykorzystamy do tego celu jedynie kwadraty i kółka jako tekstury i wygenerujemy teksturę drzewa w sposób losowy przy uruchomieniu się gry. Zacznijmy zatem od dodania do `AssetManager` listy do przetrzymywania różnych wariantów drzew, czyli tekstur:

W klasie `GameObjectFactory` dodamy metodę generującą drzewka z utworzonych przed chwilą tekstur. Zgodnie z efektem wizualnym elementy znajdujące się blisko są większe niż obiekty o tych samych rozmiarach, ale znajdujące się dalej od obserwatora. Dlatego dodamy w tej metodzie parametr skali, który zostanie również wykorzystany do zmodyfikowania prędkość przemieszczania się drzew na ekranie. Poza tą różnicą utworzenie obiektu drzewa nie różni się niczym od tworzenia gwiazdek czy przeszkód:

```
public static GameObject CreateTree(Vector2 position, float scale)
{
	var tree = new GameObject();
	tree.Texture = AssetManager.Instance.Trees[Game1.random.Next(0, AssetManager.Instance.Trees.Count)];
	tree.Position = position;
	tree.Radius = 20;
	tree.Scale = scale;
	var p = new Vector2(480 + 80, position.Y);
	var move = BehaviourFactory.CreateMoveBehaviour(new Vector2(-80, position.Y),
		p, (1/scale) * (480 + 80 + 80) / 100);
	move.Finished += (sender, args) =>
	{
		tree.Active = false;
	};
	tree.AddBehaviour(move);
	return tree;
}
```

Następnie musimy w klasie `SingleLine` dodać dwie listy pomocnicze do przechowywania drzewek znajdujących się przed graczem oraz tych będących za graczem:

```
/// <summary>
/// Lista drzewek za postacią
/// </summary>
private List<GameObject> backgroundObjects = new List<GameObject>();
/// <summary>
/// Lista drzewek przed postacią
/// </summary>
private List<GameObject> foregroundObjects = new List<GameObject>();
```

I analogicznie do generowania obiektów użyjemy jeszcze dwóch pól do zliczania czasu, który upłynął od ostatnio wygenerowanego drzewa i czasu jaki musi upłynąć żeby wygenerować kolejne drzewo:

```
/// <summary>
/// Kiedy wygenerowano ostatnie drzewo?
/// </summary>
private float timeFromLastTree;

/// <summary>
/// Po ilu sekundach wygenerować następne drzewo?
/// </summary>
private float timeForGeneratingNextTree = Game1.random.Next(10, 20) / 10.0f;
```

Następnie w funkcji `Update` tej klasy musimy wygenerować nowe drzewo w podobny sposób jak generujemy gwiazdki/przeszkody:

```
timeFromLastTree += (float)gameTime.ElapsedGameTime.TotalSeconds;
if (timeFromLastTree > timeForGeneratingNextTree)
{
	timeFromLastTree = 0;
	timeForGeneratingNextTree = Game1.random.Next(10, 20)/10.0f;
	if (Game1.random.Next(2) == 0)
	{
		var tree = GameObjectFactory.CreateTree(_offset + new Vector2(0, 100), 0.8f);
		backgroundObjects.Add(tree);
	}
	else
	{
		var tree = GameObjectFactory.CreateTree(_offset + new Vector2(0, 100), 1.3f);
		foregroundObjects.Add(tree);
	}
}
```

Jeśli dojdzie do tworzenia drzewa, to zostanie utworzony albo drzewo w dalszym planie o rozmiarze 0.8, albo drzewo na bliższym planie o rozmiarze 1.3. Oczywiście dalej w tej funkcji zapewniamy, że każde drzewo się zaktualizuje:

```
foreach (var gameObject in foregroundObjects)
{
	gameObject.Update(gameTime);
}
foreach (var gameObject in backgroundObjects)
{
	gameObject.Update(gameTime);
}
```

Jak być może zwróciliście uwagę po zakończeniu przesuwania drzewko staje się nieaktywne. Żeby nie dopuścić do problemów z brakiem pamięci musimy jeszcze zapewnić, że obiekty nieaktywne zostaną usunięte w funkcji `Update`:

```
for (int i = foregroundObjects.Count - 1; i >= 0; i--)
{
	var gameObject = foregroundObjects[i];
	if (!gameObject.Active)
		foregroundObjects.RemoveAt(i);
}
for (int i = backgroundObjects.Count - 1; i >= 0; i--)
{
	var gameObject = backgroundObjects[i];
	if (!gameObject.Active)
		backgroundObjects.RemoveAt(i);
}
```

I na koniec już musimy tylko narysować nasze drzewka na ekranie. Pamiętajmy, że lista zawierająca obiekty na dalszym planie ma nie zasłaniać niczego na bliższych planach, z kolei drzewa z pierwszego planu jak najbardziej mają zasłaniać postać gracza, gwiazdki i przeszkody. Dlatego funkcja `Draw` będzie wyglądała teraz w ten sposób:

```
public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
{
	foreach (var gameObject in backgroundObjects)
	{
		gameObject.Draw(gameTime, spriteBatch);
	}
	foreach (var gameObject in obstacles)
	{
		gameObject.Draw(gameTime, spriteBatch);
	}
	Character.Draw(gameTime, spriteBatch);
	foreach (var gameObject in foregroundObjects)
	{
		gameObject.Draw(gameTime, spriteBatch);
	}
}
```

Po uruchomieniu gry naszym oczom powinien się ukazać "piękny" efekt paralaksy i gra oczywiście staje się trochę trudniejsza, bo drzewa z pierwszego planu zasłaniają przeszkody. :)