#RPG_ver_4 - wydzielenie obecnej rozgrywki do osobnej klasy, dodanie losowości

##Wydzielenie obecnej rozgrywki do osobnej klasy

Do tej pory gra polegała na tym, że skaczemy pojedynczym kwadratem unikając czarnych gwiazdek i zbierając gwiazdki żółte. Docelowo chcielibyśmy mieć trzy takie linie. Najprościej będzie to osiągnąć poprzez wydzielenie większości elementów z obecnej implementacji klasy `Game1` do osobnej klasy.

```
class SingleLine : IUpdateable, IDrawable
{
	/// <summary>
	/// Postać sterowana przez gracza.
	/// </summary>
	private GameObject character;
	/// <summary>
	/// Skok podczepiony pod postać gracza.
	/// </summary>
	private JumpBehaviour jump;

	/// <summary>
	/// Czy jest już koniec gry?
	/// </summary>
	public bool GameOver;
	/// <summary>
	/// Aktualny wynik uzyskany dla tego fragmentu gry.
	/// </summary>
	public int Score;

	/// <summary>
	/// Lista przeszkód i gwiazdek.
	/// </summary>
	private List<GameObject> obstacles = new List<GameObject>();

	/// <summary>
	/// Kiedy wygenerowano ostatni obiekt?
	/// </summary>
	private float timeFromLastObject;

	/// <summary>
	/// Wektor przesunięcia obiektu SingleLine na ekranie głównym gry.
	/// </summary>
	private Vector2 _offset;

	/// <summary>
	/// Tworzymy nowy obiekt klasy SingleLine podając jego pozycję na ekranie.
	/// Wszystkie pozycje dla umieszczanych obiektów muszą być przesunięte o ten
	/// wektor. Dodatkowo ustawiamy kolor postaci na unikatowy dla danej linii.
	/// </summary>
	/// <param name="offset">Pozycja na ekranie.</param>
	/// <param name="color">Kolor postaci sterowanej przez gracza.</param>
	public SingleLine(Vector2 offset, Color color)
	{
		_offset = offset;

		character = GameObjectFactory.CreateCharacter(_offset + new Vector2(240, 120));
		character.Color = color;
		character.Rotation = 45*(float) Math.PI / 180;

		jump = BehaviourFactory.CreateJumpBehaviour(character, 100, 100);
		character.AddBehaviour(jump);
	}

	/// <summary>
	/// Wywołanie instrukcji skoku na zachowaniu skoku.
	/// </summary>
	public void Jump()
	{
		jump.Jump();
	}

	/// <summary>
	/// Jeśli jest koniec gry, to nic nie rób.
	/// W przeciwnym wypadku zaktualizuj wszystkie obiekty i
	/// sprawdź kolizje.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public void Update(GameTime gameTime)
	{
		if (GameOver)
			return;

		timeFromLastObject += (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (timeFromLastObject > 3)
		{
			timeFromLastObject = 0;
			if (Game1.random.Next(2) == 0)
			{
				var star = GameObjectFactory.CreateStar(_offset + new Vector2(0, 0));
				star.AddBehaviour(new EmitParticlesBehaviour());
				obstacles.Add(star);
			}
			else
			{
				var obstacle = GameObjectFactory.CreateObstacle(_offset + new Vector2(0, 170));
				obstacles.Add(obstacle);
			}
		}

		character.Update(gameTime);

		foreach (var gameObject in obstacles)
		{
			gameObject.Update(gameTime);
		}

		for (int i = obstacles.Count - 1; i >= 0; i--)
		{
			var obstacle = obstacles[i];
			if (!obstacle.Active)
				obstacles.RemoveAt(i);
			else if (character.CollidesWith(obstacle))
			{
				//TODO: poniższy kod prowadzi do bad smells, w kolejnej wersji należy go zrefaktoryzować!
				if (obstacle.Color == Color.Black)
					GameOver = true;
				else
				{
					obstacles.RemoveAt(i);
					Score += 1;
				}
			}
		}
	}

	/// <summary>
	/// Funkcja rysująca wszystkie elementy składowe na ekranie.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	/// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
	public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		foreach (var gameObject in obstacles)
		{
			gameObject.Draw(gameTime, spriteBatch);
		}
		character.Draw(gameTime, spriteBatch);
	}
}
```

W powyższej implementacji lekko zostały również zmodyfikowane pozycje poszczególnych elementów oraz postać gracza została obrócona o 45 stopni.

Pozostawiamy natomiast obsługę klawiatury i dodajemy listę obiektów klasy `SingleLine` w klasie `Game1`, żeby można było dodać kilka obiektów i wyświetlić na ekranie.

```
private List<SingleLine> lines;
```

Następnie korzystamy z pętli `foreach` w celu aktualizacji i rysowania tych obiektów odpowiednio w funkcjach `Update` oraz `Draw`:

```
foreach (var singleLine in lines)
{
	singleLine.Update(gameTime);
}
```

```
foreach (var singleLine in lines)
{
	singleLine.Draw(gameTime, spriteBatch);   
}
```

Na koniec dodajemy jeszcze dwie pomocnicze metody w klasie `Game1` do sprawdzenia czy wszystkie linie są w stanie końca gry oraz do zsumowania łącznego wyniku z każdej z linii.

```
/// <summary>
/// Jeśli na co najmniej jednej linii trwa jeszcze
/// rozgrywka, zwróć fałsz. Przeciwnie zwróć prawdę.
/// </summary>
public bool GameOver
{
	get
	{
		foreach (var singleLine in lines)
		{
			if (!singleLine.GameOver)
				return false;
		}
		return true;
	}
}

/// <summary>
/// Jaki jest łączny wynik punktowy na wszystkich liniach?
/// </summary>
public int Score
{
	get
	{
		int total = 0;
		foreach (var singleLine in lines)
		{
			total += singleLine.Score;
		}
		return total;
	}
}
```

W efekcie uzyskujemy tą samą rozgrywkę co w poprzedniej wersji, ale zwielokrotnioną trzykrotnie.

##Dodanie losowości

Możemy teraz zróżnicować poszczególne fragmenty gry dodając elementy losowości. Na początek spowodujmy, aby elementy generowały się co losową wartość z przedziału 1-3 sekund. W klasie `SingleLine` dodajemy pole:

```
/// <summary>
/// Po ilu sekundach wygenerować następny obiekt?
/// </summary>
private float timeForGeneratingNextObject;
```

A następnie w konstruktorze oraz w pętli `Update` po spełnieniu warunku generacji obiektu ustawiamy wartość tego pola w sposób losowy:

```
timeForGeneratingNextObject = Game1.random.Next(10, 30)/10.0f;
```

Zawuważcie, że korzystam ze statycznego pola `random` z klasy `Game1` - gdybym wytworzył obiekt klasy `Random` osobno w klasie `SingleLine`, to istniałoby ryzyko, że obiekty te byłyby takie same i w efekcie każda linia generowałaby taki sam tor przeszkód. Tego byśmy nie chcieli. :)

W samym warunku sprawdzającym czy wygenerować obiekt oczywiście porównujemy wartość z nową zmienną:

```
if (timeFromLastObject > timeForGeneratingNextObject)
```