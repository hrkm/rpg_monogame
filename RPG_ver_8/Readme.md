#RPG_ver_8 - interfejs ekranu gry, ekran głównej rozgrywki, główna klasa aplikacji bez elementów rozgrywki

##Interfejs ekranu gry

Chcemy zmodyfikować naszą grę w taki sposób, aby można było w dość łatwy sposób tworzyć nowe ekrany, takie jak ekran z menu, ekran z rozgrywką, ekran najlepszych wyników etc. Moglibyśmy oczywiście wszystko trzymać w jednej klasie i przełączać to co jest wyświetlane na ekranie w zależności od tego co robi gracz, ale doprowadzi to do dość skomplikowanego utrzymania kodu w przyszłości i wprowadzanie modyfikacji będzie bardzo uciążliwe. Dlatego zdefiniujmy interfejs, który opisuje wszystkie cechy ekranu:

```
public interface IScreen
{
	/// <summary>
	/// Funkcja update służy do aktualizacji stanu obiektu.
	/// W tym wypadku polega to na zastosowaniu wszystkich
	/// zachowań przypiętych do tego obiektu.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	void Update(GameTime gameTime);
	
	/// <summary>
	/// Funkcja rysująca obiekt na ekranie zgodnie z jego stanem.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	/// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
	void Draw(GameTime gameTime, SpriteBatch spriteBatch);
	
	/// <summary>
	/// Funkcja wywoływana jeśli naciśnięto lewy przycisk myszy.
	/// </summary>
	/// <param name="p">Aktualna pozycja kursora myszki.</param>
	void CheckClick(Vector2 p);
	
	/// <summary>
	/// Funkcja wywoływana jeśli dotknięto ekranu dotykowego.
	/// </summary>
	/// <param name="p">Lista punktów dotknięcia.</param>
	void CheckTouchPoints(List<Vector2> p);
	
	/// <summary>
	/// Funkcja wywoływana jeśl wciśnięto konkretny klawisz.
	/// </summary>
	/// <param name="key">Wciśnięty klawisz na klawiaturze.</param>
	void ButtonPressed(Keys key);
}
```

Funkcje `Update` i `Draw` powinniście już powoli kojarzyć, bo przewijają się właściwie w każdym elemencie gry. :)

Pozostałe trzy funkcje służą do obsługi sterowania myszką, ekranem dotykowym i klawiaturą, zgodnie z kolejnością w jakiej zostały podane. Zauważcie że myszka może wskazywać w danej chwili tylko jeden punkt, ale na ekranie dotykowym z obsługą multi-touch możemy mieć kilka punktów styku, dlatego przekazujemy w metodzie `CheckTouchPoints` listę punktów. Oczywiście funkcja obsługi klawiatury powinna przekazywać w argumencie wciśnięty klawisz.

##Ekran głównej rozgrywki

Mając już interfejs ekranu możemy go zaimplementować. W tym celu dostosujemy tak naprawdę naszą obecną klasę `Game1` i przemianujemy ją na `GameScreen`. Oczywiście dodamy informacje, że klasa ta implementuje interfejs `IScreen`, a zatem musi posiadać implementację każdej metody z tego interfejsu. Usuwamy za to wszystkie elementy nie związane z tym ekranem - tu mam na myśli inicjalizację ustawień graficznych, obsługę sterowania czy wczytanie tekstur i dźwięków - te elementy zostaną wczytane w nowej wersji klasy `Game1`.

Następnie, ponieważ nie mamy już obsługi sterowania w funkcji `Update` klasy `GameScreen`, wygląda ona znacznie prościej:

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
		//elementy aktualizowane w stanie końca gry
	}
}
```

Funkcja `Draw` będzie wywoływana w ramach bloku `Begin`/`End`, a więc możemy pominąć ten fragment. Nowa funkcja rysująca wygląda następująco:

```
public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
{
	foreach (var singleLine in lines)
	{
		singleLine.Draw(gameTime, spriteBatch);   
	}

	var m = AssetManager.Instance.Font.MeasureString(Score.ToString());
	spriteBatch.DrawString(AssetManager.Instance.Font, Score.ToString(), new Vector2(240 - m.X/2, 6), Color.White);
	if (GameOver)
	{
		m = AssetManager.Instance.Font.MeasureString("Game Over");
		spriteBatch.DrawString(AssetManager.Instance.Font, "Game Over", new Vector2(240 - m.X/2, 300), Color.White);
	}
}
```

Jak widać tylko rysujemy obiekty związane z grą w tej funkcji, nie stosujemy `ResolutionIndependentRenderer` ani nie rysujemy kursora myszki - to znajdzie się w głównej klasie aplikacji modyfikowanej w kolejnej sekcji.

Fragmenty obsługujące sterowanie dotychczas znajdujące się w funkcji `Update`, trafią teraz do odpowiednich metod z interfejsu `IScreen`:

```
public void CheckClick(Vector2 p)
{
	if (!GameOver)
	{
		var mouse = new GameObject();
		mouse.Position = p;
		foreach (var singleLine in lines)
		{
			if (mouse.CollidesWith(singleLine.Character))
			{
				singleLine.Jump();
			}
		}
	}
	else
	{
		//obsługa myszy/dotyku przy game over
	}
}

public void CheckTouchPoints(List<Vector2> p)
{
	if (GameOver)
	{
		if (p.Count >= 2)
			NewGame();
	}
}

public void ButtonPressed(Keys key)
{
	if (!GameOver)
	{
		if (key == Keys.Q)
		{
			lines[0].Jump();
		}
		if (key == Keys.W)
		{
			lines[1].Jump();
		}
		if (key == Keys.E)
		{
			lines[2].Jump();
		}
	}
	else
	{
		if (key == Keys.Space)
		{
			NewGame();
		}
	}
}
```

Prawda, że wygląda to zdecydowanie bardziej przejrzyście niż gdy było umieszczone wszystko w jednej metodzie? :)

##Główna klasa aplikacji bez elementów rozgrywki

Skoro zmieniliśmy dotychczasową klasę `Game1` w `GameScreen`, to wypada utworzyć nową wersję tej klasy, która umożliwi wykorzystanie nowego interfejsu. Pola, których pozbyliśmy się tworząc klasę `GameScreen`, takie jak wysokość i szerokość ekranu czy klasa odpowiedzialna za renderowanie niezależnie od rozdzielczości ekranu, trafią oczywiście do klasy `Game1`. Dodatkowo dodamy następujące pola i typ enum:

```
/// <summary>
/// Aktualny stan gry, w którym jesteśmy.
/// </summary>
public static GameState CurrentState { get; set; }

/// <summary>
/// Aktualnie wyświetlany ekran gry.
/// </summary>
private IScreen CurrentScreen
{
	get { return Screens[CurrentState]; }
}

/// <summary>
/// Lista zawierająca wszystkie ekrany gry.
/// </summary>
public static Dictionary<GameState, IScreen> Screens = new Dictionary<GameState, IScreen>();

/// <summary>
/// Dostępne stany gry, w których możemy się znaleźć.
/// </summary>
public enum GameState
{
	Gameplay
}
```

Klasa `Game1` oczywiście dziedziczy po klasie `Game`, jak to miało miejsce dotychczas. Musimy więc mieć takie metody jak `LoadContent` czy `Initialize`, a które tak naprawdę był już zaimplementowane w poprzedniej wersji tej klasy. Dodatkowo, po wczytaniu tekstur utwórzmy wszystkie ekrany, z których będziemy korzystać - obecnie jest to tylko ekran dla stanu `Gameplay`:

```
//dla każdego stanu gry dodaj odpowiedni ekran
Screens.Add(GameState.Gameplay, new GameScreen());
CurrentState = GameState.Gameplay;
```

Funkcja `Update` musi zaktualizować aktywny ekran trzymany w polu `CurrentScreen` oraz obsłużyć sterowanie, którego obsługa zostanie przekazana przez odpowiednie metody interfejsu również do aktualnego ekranu:

```
protected override void Update(GameTime gameTime)
{
	CheckInput(gameTime);
	CurrentScreen.Update(gameTime);

	base.Update(gameTime);
}
```

Zanim pokażę funkcję `CheckInput`, zajmijmy się rysowaniem na ekran. Główna klasa aplikacji odpowiedzialna jest za "odpalenie" rysowania niezależnie od rozdzielczości, narysowania aktualnego ekranu, a na koniec narysowania kursora myszki. Stąd wygląda ta funkcja w ten sposób:

```
protected override void Draw(GameTime gameTime)
{
	renderer.BeginDraw();
	spriteBatch.Begin(SpriteSortMode.Deferred, null, null,
					 null, null, null, renderer.GetTransformationMatrix());

	if (CurrentScreen != null)
		CurrentScreen.Draw(gameTime, spriteBatch);

#if !WINDOWS_PHONE && !ANDROID
	mouse.Draw(gameTime, spriteBatch);
#endif

	spriteBatch.End();

	base.Draw(gameTime);
}
```

Powyższy kod jest bardzo czytelny i nie posiada żadnych elementów specyficznych dla rozgrywki - po prostu gwarantuje narysowanie ekranu aktualnie wskazywanego przez `CurrentScreen`, i jeśli jesteśmy na platformie PC to pojawi się również kursor. Rysowanie ekranu odbywa się już pomiędzy blokiem funkcji `Begin`/`End` obiektu `spriteBatch`, dlatego w implementacji `GameScreen` nie było wywołań tych metod.

Na koniec funkcja `CheckInput`, która przekazuje informacje na temat wciśniętych klawiszy czy dotkniętych punktów na ekranie do obiektu implementującego interfejs `IScreen`:

```
protected void CheckInput(GameTime gameTime)
{
	if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
		Keyboard.GetState().IsKeyDown(Keys.Escape))
	{
		Exit();
	}

	//obsługa ekranu dotykowego
	List<Vector2> points = new List<Vector2>();
	TouchCollection touchCollection = TouchPanel.GetState();
	foreach (TouchLocation tl in touchCollection)
	{
		if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
		{
			Vector2 p = renderer.ScaleMouseToScreenCoordinates(tl.Position);
			points.Add(p);
		}
		//jeśli dopiero został wciśnięty, to potraktuj to również jak kliknięcie myszką
		if (tl.State == TouchLocationState.Pressed)
		{
			Vector2 p = renderer.ScaleMouseToScreenCoordinates(tl.Position);
			CurrentScreen.CheckClick(p);
		}
	}
	CurrentScreen.CheckTouchPoints(points);

#if !WINDOWS_PHONE && !ANDROID
	//obsługa myszki (poza platformami mobilnymi)
	previous = current;
	current = Mouse.GetState();
	mouse.Position = renderer.ScaleMouseToScreenCoordinates(new Vector2(current.X, current.Y));
	mouse.Update(gameTime);
	if (previous.Position != current.Position) mouse.Active = true;
	if (current.LeftButton == ButtonState.Pressed)
	{
		Vector2 p = renderer.ScaleMouseToScreenCoordinates(new Vector2(current.X, current.Y));
		if (previous.LeftButton == ButtonState.Released)
		{
			CurrentScreen.CheckClick(p);
		}
		//przytrzymanie wciśniętego przycisku myszki jest traktowane jak palec na ekranie dotykowym
		CurrentScreen.CheckTouchPoints(new List<Vector2>(1) { p });
	}
#endif

	//obsługa klawiatury
	var keyboard = Keyboard.GetState();
	foreach (var pressedKey in keyboard.GetPressedKeys())
	{
		CurrentScreen.ButtonPressed(pressedKey);
	}
}
```

Pierwszy warunek sprawdza, czy gracz wcisnął przycisk wstecz lub klawisz Escape aby zakończyć działanie gry.

Następnie weryfikowana jest obsługa ekranu dotykowego. Ponieważ mogą być ekrany typu multi-touch i istotna może być informacja na temat liczby jednocześnie dotkniętych punktów (wykorzystujemy to np. w przypadku stanu końca gry - dotknięcie ekranu dwoma palcami powoduje restart), dlatego jeśli dany punkt znajduje się w stanie `Pressed` lub `Moved` to trafia na listę punktów, która zostaje później przekazana do funkcji `CheckTouchPoints`. Z drugiej strony jeśli w danej chwili punkt został dopiero co dotknięty i jest w stanie `Pressed`, to możemy go zinterpretować jako kliknięcie lewym przyciskiem myszy, dlatego jest wywołana metoda `CheckClick`. W obu tych przypadkach zwróćcie uwagę, że punkty zebrane przez klasę `Game1` są konwertowane przez `IndependentResolutionRenderer` na punkty logiczne rodzielczości użytej do renderowania gry.

Kolejny blok odczytuje informacje o stanie myszki. W tym bloku sprawdzamy czy lewy przycisk myszy został właśnie wciśnięty (jeśli w poprzednim stanie nie był wciśnięty, a w tym jest wciśnięty, to znaczy że nastąpił "klik"), z drugiej strony niezależnie od poprzedniego stanu jeśli przycisk jest nadal przytrzymywany, to traktujemy to jako przesuwanie palca po ekranie dotykowym. Może to ułatwić np. implementację funkcjonalności drag&drop.

Na koniec krótki blok sprawdzający stan klawiatury i wywołujący metodę `ButtonPressed` dla każdego wciśniętego obecnie klawisza. Tutaj nie porównujemy poprzedniego stanu, żeby wychwycić czy to nowe naciśnięcie danego klawisza czy być może przytrzymanie już wciśniętego klawisza - założenie jest takie, że jeśli klawisz jest przytrzymywany, to akcja z nim powiązana może być wykonywana w sposób ciągły (np. przesuwanie postaci w jakimś kierunku).

Wprowadziliśmy dosyć sporo zmian i chociaż gra nie uzyskała nowych funkcjonalności z punktu widzenia gracza, to w kolejnych odsłonach w bardzo łatwy sposób będziemy mogli dodać np. menu główne gry. Dlatego mam nadzieję, że docenicie zaletę tej dekompozycji na klasy. :)