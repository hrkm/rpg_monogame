#RPG_ver_10 - usuwanie zakończonych zachowań, "wskrzeszenie gracza" jako dodatkowa mechanika gry, ekran końca gry

##Usuwanie zakończonych zachowań

Mamy już całkiem zaawansowany system dodawania zachowań do postaci, jednak brakuje nam możliwości dodawania "czasowych" zachowań, czyli takich, które po pewnym czasie zostaną automatycznie usunięte z danego obiektu gry. Skorzystamy zatem z funkcji `OnFinished` wywoływanego w klasie abstrakcyjnej `Behaviour` i dodamy tam również pole `Completed`, do oznaczenia zachowania jako zakończonego:

```
/// <summary>
/// Zachowanie zakończyło swoje działanie.
/// </summary>
public bool Completed;

/// <summary>
/// Funkcja pomocnicza do rzucenia wydarzenia Finished.
/// </summary>
protected void OnFinished()
{
	Completed = true;
	if (Finished != null)
		Finished(this, new EventArgs());
}
```

Zauważcie, że na razie tylko `InterpolationBehaviour` wykorzystuje tą metodę, a więc zachowania powiązane z cząsteczkami czy skakaniem postaci nie zostaną nigdy ruszone. Musimy teraz jeszcze w funkcji `Update` klasy `GameObject` zapewnić, że zachowania oznaczone jako `Completed` zostaną usunięte z postaci:

```
/// <summary>
/// Funkcja update służy do aktualizacji stanu obiektu.
/// W tym wypadku polega to na zastosowaniu wszystkich
/// zachowań przypiętych do tego obiektu.
/// </summary>
/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
public virtual void Update(GameTime gameTime)
{
	foreach (var updateable in _updateables)
	{
		updateable.Update(gameTime);
	}
	foreach (var behaviour in _behaviours)
	{
		behaviour.Apply(this);
	}
	//usuwamy te zachowania, które już się zakończyły
	for (int i = _behaviours.Count - 1; i >= 0; i--)
	{
		var behaviour = _behaviours[i];
		if (behaviour.Completed)
		{
			_behaviours.RemoveAt(i);
			_updateables.Remove(behaviour);
			if (behaviour is IDrawable)
				_drawables.Remove(behaviour as IDrawable);
		}
	}
}
```

W efekcie po dodaniu `InterpolationBehaviour` do dowolnego obiektu, jeśli to zachowanie zostanie już w pełni zrealizowane, zostanie automatycznie usunięte z obiektu klasy. Jest to istotne z takiego punktu widzenia, że możemy podpiąć dodatkowy kod pod wydarzenie `Finished` zachowania, i gdyby takie zachowanie cały czas było podpięte pod obiekt gry, to jego funkcja `Finished` byłaby za każdym razem uruchamiana co wywołanie funkcji `Update`.

##"Wskrzeszenie gracza" jako dodatkowa mechanika gry

Chcielibyśmy uatrakcyjnić naszą grę przy pomocy prostego zabiegu - jeśli postać gracza zginie (tj. wejdzie w kolizję z czarną gwiazdką), to po zebraniu kolejnych 3 punktów zostaje przywrócona do gry i może dalej kontynuować swoją podróż. Tuż po wskrzeszeniu postać powinna być niezniszczalna, aby nie została od razu unicestwiona przez tą samą przeszkodę.

Dodajmy wpierw w klasie `GameObject` pole:

```
/// <summary>
/// Czy obiekt jest niezniszczalny?
/// </summary>
public bool Indestructible = true;
```

Dodajmy teraz w tej klasie funkcję `Revive`, która przywróci postać do gry z włączoną niezniszczalnością:

```
/// <summary>
/// Wywołanie tej funkcji spowoduje przywrócenie postaci do gry
/// z efektem niezniszczalności 3 sekundowym.
/// </summary>
public void Revive()
{
	Character.Indestructible = true;
	var wait = new InterpolationBehaviour(1, 0, 3, InterpolationBehaviour.InterpolationParameter.None);
	wait.Finished += (sender, args) => { Character.Indestructible = false; };
	Character.AddBehaviour(wait);
	GameOver = false;
}
```

Zauważcie, że wpierw ustawiamy niezniszczalność na obiekcie z postacią, następnie dodajemy "puste" zachowanie interpolacji dla nowo-dodanego parametru `None`, który po prostu nie modyfikuje żadnej właściwości obiektu, ale trwa trzy sekundy. Po zakończeniu tego zachowania zostanie wyłączona niezniszczalność na postaci. Po dodaniu tego zachowania do obiektu postaci dopiero wyłączamy stan końca gry, aby rozgrywka mogła być kontynuowana.

Musimy jeszcze dodać wydarzenie "zebrania gwiazdki" oraz pole do zliczania gwiazdek zebranych w innych liniach po ogłoszeniu stanu `GameOver`.

```
/// <summary>
/// Wydarzenie wywoływane gdy zostanie zebrana gwiazdka.
/// </summary>
public event EventHandler StarCollected;

/// <summary>
/// Liczba gwiazdek zebrana po końcu gry
/// na innych liniach.
/// </summary>
public int StarsAfterGameOver;
```

Weryfikujemy w testowaniu kolizji czy postać gracza jest niezniszczalan. Dodatkowo w chwili zderzenia z czarną gwiazdką dodajemy wyzerowanie licznika `StarsAfterGameOver`, a po zderzeniu ze zwykłą gwiazdką wywołujemy zdarzenie `StarCollected`:

```
if (obstacle.Color == Color.Black)
{
	if (!Character.Indestructible)
	{
		GameOver = true;
		StarsAfterGameOver = 0;
		AssetManager.Instance.Hit.Play();
	}
}
else
{
	obstacles.RemoveAt(i);
	Score += 1;
	if (StarCollected != null)
		StarCollected(this, null);
	AssetManager.Instance.PickUp.Play();
}
```

Ostatnią już zmianą będzie modyfikacja w funkcji `NewGame` w klasie ekranu gry `GameScreen`. Musimy tam dodać obsługę dodanego przed chwilą wydarzenia, która z jednej strony będzie zwiększała liczbę gwiazdek zdobytych po uzyskaniu stanu końca gry na danej linii, kiedy zostaną one zebrane na innych liniach, a z drugiej strony umożliwi wywołanie funkcji `Revive` jeśli udało nam się od momentu końca gry danej linii zebrać 3 gwiazdki. W związku z tym po utworzeniu obiektów `SingleLine` możemy do nich dodać obsługę wydarzenia `StarCollected` w następujący sposób:

```
foreach (var singleLine in lines)
{
	singleLine.StarCollected += (sender, args) =>
	{
		foreach (var line in lines)
		{
			if (line.GameOver)
			{
				line.StarsAfterGameOver++;
				if (line.StarsAfterGameOver >= 3)
					line.Revive();
			}
		}
	};
}
```

Dodatkowo w klasie `SingleLine` napiszemy na ekranie informacje ile gwiazdek potrzeba aby wskrzesić postać.

```
if (GameOver)
{
	var text = "zbierz " + (3 - StarsAfterGameOver) + " gwiazdki";
	var m = AssetManager.Instance.Font.MeasureString(text);
	spriteBatch.DrawString(AssetManager.Instance.Font, text, _offset + new Vector2(Game1.Width/2 - m.X/2), Color.White);
}
```

##Ekran końca gry

Dodajmy ekran końca gry. Powinien on wyświetlić uzyskany przez gracza wynik i umożliwić ponowną rozgrywkę. Przejście do tego ekranu powinno odbyć się automatycznie po tym jak wszystkie trzy linie znajdą się w stanie końca gry.

Dodajemy zatem klasę `EndGameScreen`, który jest obecnie bardzo podobny do klasy `MenuScreen`, posiada jedynie dodatkowo informacje na temat liczby zdobytych punktów:

```
public class EndGameScreen : IScreen
{
	/// <summary>
	/// Przycisk do rozpoczęcia nowej gry.
	/// </summary>
	private GameObject newGameButton;

	/// <summary>
	/// Wynik uzyskany w ostatniej grze.
	/// </summary>
	public int Score;

	public EndGameScreen()
	{
		newGameButton = GameObjectFactory.CreateButton(new Vector2(Game1.Width/2, Game1.Height/2));
	}

	/// <summary>
	/// Logika gry może być aktualizowana w tej funkcji, np.
	/// sprawdzanie kolizji, odtwarzanie dźwięku, obsługa sterowania.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public void Update(GameTime gameTime)
	{
		newGameButton.Update(gameTime);
	}

	/// <summary>
	/// Funkcja rysująca obiekty na ekranie.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	/// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
	public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		var m = AssetManager.Instance.Font.MeasureString("Zagraj ponownie");
		spriteBatch.DrawString(AssetManager.Instance.Font, "Zagraj ponownie", new Vector2(240 - m.X / 2, 300), Color.White);
		var pointsText = "Wynik to " + Score + " punkty!";
		m = AssetManager.Instance.Font.MeasureString(pointsText);
		spriteBatch.DrawString(AssetManager.Instance.Font, pointsText, new Vector2(240 - m.X / 2, 150), Color.White);
		newGameButton.Draw(gameTime, spriteBatch);

		newGameButton.Draw(gameTime, spriteBatch);
	}

	/// <summary>
	/// Funkcja wywoływana jeśli naciśnięto lewy przycisk myszy.
	/// </summary>
	/// <param name="p">Aktualna pozycja kursora myszki.</param>
	public void CheckClick(Vector2 p)
	{
		var point = new GameObject();
		point.Position = p;
		if (point.CollidesWith(newGameButton))
		{
			Game1.CurrentState = Game1.GameState.Gameplay;
		}
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

Dodajemy nowy stan gry:

```
public enum GameState
{
	Menu,
	Gameplay,
	EndGame
}
```

```
Screens.Add(GameState.EndGame, new EndGameScreen());
```

I na koniec w `GameScreen` w chwili końca rozgrywki po prostu ustawiamy liczbę zdobytych punktów na nowym ekranie i zmieniamy stan gry na `EndGame`, co może być sprawdzane w funkcji `Update`:

```
public void Update(GameTime gameTime)
{
	if (!GameOver)
	{
		foreach (var singleLine in lines)
		{
			singleLine.Update(gameTime);
		}
	}
	else
	{
		(Game1.Screens[Game1.GameState.EndGame] as EndGameScreen).Score = Score;
		NewGame(); //restart gry, bo jak wrócimy do tego ekranu to chcemy grać od nowa
		Game1.CurrentState = Game1.GameState.EndGame;
	}
}
```

Oczywiście musimy usunąć wywołania funkcji `NewGame`, które do tej pory były w funkcjach obsługujących klawiaturę oraz ekran dotykowy, bo gracz będzie "przechodził" przez ekran końca gry w celu rozpoczęcia nowej rozgrywki.