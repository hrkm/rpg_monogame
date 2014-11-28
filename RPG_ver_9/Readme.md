#RPG_ver_9 - menu gry, zachowania dla parametrów liczbowych

##Menu gry

Skoro w poprzedniej wersji dodaliśmy interfejs ekranu gry, to możemy go teraz implementować na różne sposoby aby uzyskać poszczególne ekrany występujące w grze. Przykładem takiego ekranu będzie menu główne. Stwórzmy wpierw pusty szablon dla menu:

```
public class MenuScreen : IScreen
{
	/// <summary>
	/// Logika gry może być aktualizowana w tej funkcji, np.
	/// sprawdzanie kolizji, odtwarzanie dźwięku, obsługa sterowania.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public void Update(GameTime gameTime)
	{
	}

	/// <summary>
	/// Funkcja rysująca obiekty na ekranie.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	/// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
	public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
	}

	/// <summary>
	/// Funkcja wywoływana jeśli naciśnięto lewy przycisk myszy.
	/// </summary>
	/// <param name="p">Aktualna pozycja kursora myszki.</param>
	public void CheckClick(Vector2 p)
	{
	}

	/// <summary>
	/// Funkcja wywoływana jeśli dotknięto ekranu dotykowego.
	/// </summary>
	/// <param name="p">Lista punktów dotknięcia.</param>
	public void CheckTouchPoints(List<Vector2> p)
	{
	}

	/// <summary>
	/// Funkcja wywoływana jeśl wciśnięto konkretny klawisz.
	/// </summary>
	/// <param name="key">Wciśnięty klawisz na klawiaturze.</param>
	public void ButtonPressed(Keys key)
	{
	}
}
```

Jak widać powyższa klasa posiada pustą implementację dla każdej metody z interfejsu `IScreen`. Dodajmy zatem przycisk, umożliwiający rozpoczęcie rozgrywki. W tym celu skorzystamy ponownie z klasy `GameObject`, aby wykorzystać system kolizji do zweryfikowania, czy przycisk został naciśnięty. Jako grafikę wykorzystamy teksturę z kołem. Dodajemy więc odpowiednią metodę w fabryce obiektów:

```
/// <summary>
/// Utwórz obiekt przycisku. Przycisk posiada teksturę koła.
/// </summary>
/// <param name="position">Pozycja w której umieszczamy przycisk.</param>
/// <returns>Obiekt z przyciskiem.</returns>
public static GameObject CreateButton(Vector2 position)
{
	var button = new GameObject();
	button.Texture = AssetManager.Instance.Circle;
	button.Position = position;
	button.Radius = 50;
	return button;
}
```

A następnie dodajemy ten obiekt do klasy `MenuScreen`:

```
private GameObject newGameButton;
```

I oczywiście wywołujemy metodę `Update` tego obiektu w funkcji `Update` klasy oraz metodę `Draw` obiektu w funkcji `Draw` klasy `MenuScreen`. Dodatkowo w funkcji sprawdzającej kliknięcie dodajemy poniższy fragment kodu sprawdzający czy gracz kliknął w przycisk:

```
var point = new GameObject();
point.Position = p;
if (point.CollidesWith(newGameButton))
{
	Game1.CurrentState = Game1.GameState.Gameplay;
}
```

W klasie `Game1` musimy jeszcze tylko dodać do `enum GameState` wpis `Menu`, a także dodać ekran do słownika pod tym kluczem.

```
public enum GameState
{
	Menu,
	Gameplay
}
```

```
Screens.Add(GameState.Menu, new MenuScreen());
CurrentState = GameState.Menu;
```

I w tej sytuacji po uruchomieniu gry wpierw pojawi się ekran z kółkiem na środku, po kliknięciu którego gracz przejdzie do ekranu rozgrywki i będzie mógł sterować naszymi bohaterami.

##Zachowania dla parametrów liczbowych

Do tej pory dodaliśmy tylko `MoveBehaviour`, które umożliwiało liniową interpolację pozycji obiektu między dwoma punktami. Jednak w mojej grze chciałbym również móc użyć tej interpolacji do zmiany poziomu przezroczystości lub do obracania obiektów. Ponadto w samym przesuwaniu do tej pory modyfikowaliśmy tylko współrzędną X, ale obliczenia dla współrzędnej Y również były wykonywane, mimo że się nie zmieniały. Dlatego zamienimy `MoveBehaviour` na bardziej ogólną klasę `InterpolationBehaviour`:

```
public class InterpolationBehaviour : Behaviour
{
	public enum InterpolationParameter
	{
		PositionX,
		PositionY,
		Alpha,
		Scale,
		Rotation
	}

	public InterpolationParameter Parameter;

	/// <summary>
	/// Aktualna wartość.
	/// </summary>
	public float CurrentValue;
	/// <summary>
	/// Pozycja docelowa.
	/// </summary>
	public float TargetValue;
	/// <summary>
	/// Prędkość zmiany wartości liczbowej wyrażona w jednostkach na sekundę.
	/// </summary>
	protected float dv;

	/// <summary>
	/// Konstruktor zachowania interpolacji liniowej.
	/// </summary>
	/// <param name="targetValue">Pozycja docelowa.</param>
	/// <param name="startingValue">Pozycja początkowa.</param>
	/// <param name="duration">Liczba sekund przeznaczona na wykonanie przesunięcia.</param>
	/// <param name="parameter">Interpolowany parametr liczbowy.</param>
	public InterpolationBehaviour(float targetValue, float startingValue, float duration, InterpolationParameter parameter)
	{
		Parameter = parameter;
		TargetValue = targetValue;
		CurrentValue = startingValue;
		//ustawienie czasu wykonania na zero to natychmiastowe przypisanie wartości docelowej
		if (duration == 0)
		{
			CurrentValue = targetValue;
			dv = 0;
		}
		else
		{
			dv = (TargetValue - CurrentValue)/duration;
		}
	}

	/// <summary>
	/// Aktualizacja stanu zachowania poprzez aktualizację obecnej wartości.
	/// Jeśli wartość docelowa została już osiągnięta/przekroczona,
	/// to poniższa funkcja zagwarantuje, że zostanie ona ustawiona dokładnie
	/// na wskazaną wartość docelową.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public override void Update(GameTime gameTime)
	{
		CurrentValue += dv*(float) gameTime.ElapsedGameTime.TotalSeconds;
		if (dv > 0)
		{
			if (CurrentValue > TargetValue) CurrentValue = TargetValue;
		}
		else
		{
			if (CurrentValue < TargetValue) CurrentValue = TargetValue;
		}
		if (CurrentValue == TargetValue && CurrentValue == TargetValue)
		{
			OnFinished();
		}
	}

	/// <summary>
	/// Zastosowanie zachowania do wskazanego obiektu gry.
	/// Odbywa się to poprzez przepisanie wartości z zachowania do odpowiedniego
	/// pola obiektu docelowego.
	/// </summary>
	/// <param name="gameObject">Obiekt, który ma być zmodyfikowany.</param>
	public override void Apply(GameObject gameObject)
	{
		switch (Parameter)
		{
			case InterpolationParameter.Alpha:
				gameObject.Alpha = CurrentValue;
				break;
			case InterpolationParameter.Rotation:
				gameObject.Rotation = CurrentValue;
				break;
			case InterpolationParameter.PositionX:
				gameObject.Position.X = CurrentValue;
				break;
			case InterpolationParameter.PositionY:
				gameObject.Position.Y = CurrentValue;
				break;
			case InterpolationParameter.Scale:
				gameObject.Scale = CurrentValue;
				break;
		}
		base.Apply(gameObject);
	}
}
```

I w tej sytuacji oczywiście musimy zaktualizować metodę w fabryce zachowań oraz jej wywołania w taki sposób, aby uwzględniały dodatkowo informacje o tym które pole ma być aktualizowane.

```
public static InterpolationBehaviour CreateInterpolationBehaviour(float targetValue, float startingValue, float duration, InterpolationBehaviour.InterpolationParameter parameter)
{
	var interpolation = new InterpolationBehaviour(targetValue, startingValue, duration, parameter);
	return interpolation;
}
```

```
var move = BehaviourFactory.CreateInterpolationBehaviour(-80, 480 + 80, (1/scale) * (480 + 80 + 80) / 100, InterpolationBehaviour.InterpolationParameter.PositionX);
```

Mając tak zdefiniowane nowe zachowanie dodajmy do systemu cząsteczek dodatkowe zachowanie interpolujące współczynnik przezroczystości:

```
var alpha = new InterpolationBehaviour(0, 1, Lifespan, InterpolationBehaviour.InterpolationParameter.Alpha);
particle.AddBehaviour(alpha);
```

Dzięki temu cząsteczka przez cały swój okres życia będzie powoli zanikać, co da przyjemniejszy dla gracza efekt niż nagłe, 