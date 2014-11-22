#RPG_ver_3 - nowe interfejsy, system cząsteczek

Uwaga: projekt RPG_ver_3 korzysta z folderu "packages" projektu RPG_ver_0 w celu zaoszczędzenia miejsca. W związku z tym zaleca się ściągnięcie całego repozytorium lub dodanie MonoGame przy pomocy NuGeta.

##Nowe interfejsy

Zauważyliście pewnie, że wiele obiektów posiada metody `Update` oraz `Draw`? To przykład kiedy warto zainteresować się utworzeniem odpowiednich interfejsów dla tych dwóch typów, ponieważ później łatwiej będzie nam korzystać z listy obiektów implementujących wspólny interfejs, ale niekoniecznie będących pochodnymi tej samej klasy.

```
public interface IUpdateable
{
	void Update(GameTime gameTime);
}
```

```
public interface IDrawable
{
	void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
```

Wszystkie obiekty, które posiadają metodę `Update` możemy automatycznie oznaczyć jako implementujące interfejs `IUpdateable`. Analogicznie, jeśli mamy metodę `Draw` to klasa implementuje interfejs `IDrawable`.

Mając do dyspozycji te dwa interfejsy możemy dodać do klasy `GameObject` dwie listy obiektów implementujących odpowiednio interfejs `IUpdateable` oraz `IDrawable`, co umożliwi nam podpięcie różnych obiektów pod nasz obiekt z gry, w tym również system cząsteczkowy zdefiniowany w kolejnym punkcie:

```
/// <summary>
/// Lista zawierająca wszystkie podczepione elementy IUpdateable.
/// </summary>
private List<IUpdateable> _updateables = new List<IUpdateable>(); 

/// <summary>
/// Lista zawierająca wszystkie podczepione elementy IDrawable.
/// </summary>
private List<IDrawable> _drawables = new List<IDrawable>(); 
```

Oczywiście uaktualniamy funkcję `Update` w klasie `GameObject` dodając na początku:

```
foreach (var updateable in _updateables)
{
	updateable.Update(gameTime);
}
```

oraz metodę `Draw` poprzez dodanie na jej początku:

```
foreach (var drawable in _drawables)
{
	drawable.Draw(gameTime, spriteBatch);
}
```

##System cząsteczek

System cząsteczek to nic innego jak obiekt, który emituje z siebie inne obiekty, czyli cząsteczki. Cząsteczką może być pojedynczy punkt, piksel czy linia, ale równie dobrze możemy zastosować inny obiekt z gry i nim "rzucić" po kątach. Cechą charakterystyczną każdej cząsteczki jest to, że po jakimś czasie ulega dezaktywacji i znika z ekranu. Dzięki temu uzyskujemy efekt bardzo prostej animacji i gra nabiera więcej dynamiki.

Na początku zaimplementujmy więc klasę odpowiedzialną za emitowanie obiektów klasy `GameObject`.

```
public class ParticleSystem : GameObject
{
	/// <summary>
	/// Lista do przechowywania aktywnych cząstek.
	/// </summary>
	private readonly List<GameObject> _particles = new List<GameObject>();
	/// <summary>
	/// Lista do przetrzymywania pozostałej długości życia każdej z cząstek.
	/// </summary>
	private readonly List<float> _lifespans = new List<float>(); 

	/// <summary>
	/// Dodanie wskazanej cząstki do systemu cząsteczek.
	/// </summary>
	/// <param name="particle">Dodawana cząsteczka.</param>
	/// <param name="lifespan">Długość życia wyrażona w sekundach.</param>
	public void AddParticle(GameObject particle, float lifespan)
	{
		_particles.Add(particle);
		_lifespans.Add(lifespan);
	}

	/// <summary>
	/// Aktualizacja systemu cząsteczek. Skrócenie życia każdej z cząstek
	/// i sprawdzenie czy powinna już zniknąć czy jeszcze zostać.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public override void Update(GameTime gameTime)
	{
		var elapsedSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
		for (int i = _particles.Count - 1; i >= 0; i--)
		{
			var particle = _particles[i];
			_lifespans[i] -= elapsedSeconds;
			if (_lifespans[i] < 0)
			{
				_particles.RemoveAt(i);
				_lifespans.RemoveAt(i);
			}
			else
			{
				particle.Update(gameTime);   
			}
		}
	}

	/// <summary>
	/// Rysowanie każdej cząsteczki dodanej do systemu na ekranie.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	/// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
	public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		foreach (var particle in _particles)
		{
			particle.Draw(gameTime, spriteBatch);
		}
	}
}
```

W celu podpięcia systemu cząsteczek pod istniejący obiekt w grze zastosujemy pewną sztuczkę. Zdefiniujmy zachowanie `EmitParticlesBehaviour`. Jeśli zrobiliście poprzedni punkt zgodnie z założeniami, to `IBehaviour` powinno dziedziczyć po `IUpdateable`, ale nie po `IDrawable`. Przed chwilą zdefiniowana klasa `ParticleSystem` dziedziczy po obydwu interfejsach. Stąd nasze nowe zachowanie również będzie dziedziczyło po tych dwóch interfejsach:

```
public class EmitParticlesBehaviour : IBehaviour, IDrawable
{
	/// <summary>
	/// Częstotliwość generowania cząsteczek.
	/// </summary>
	public float ParticleSpawnDelay = 0.2f;

	/// <summary>
	/// Liczba wygenerowanych cząsteczek co określony okres.
	/// </summary>
	public float ParticlesPerSpawn = 7;

	private readonly ParticleSystem _particleSystem = new ParticleSystem();

	/// <summary>
	/// Zmienna pomocnicza do liczenia ile czasu upłynęło od ostatniej generacji cząsteczek.
	/// </summary>
	private float timeFromLastParticle;

	/// <summary>
	/// Aktualizacja systemu cząsteczek, plus sprawdzenie
	/// czy nie trzeba dodać nowych cząsteczek.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	public void Update(GameTime gameTime)
	{
		_particleSystem.Update(gameTime);

		timeFromLastParticle += (float) gameTime.ElapsedGameTime.TotalSeconds;
		if (timeFromLastParticle > ParticleSpawnDelay)
		{
			timeFromLastParticle = 0;
			for (int i = 0; i < ParticlesPerSpawn; i++)
			{
				var particle = GameObjectFactory.CreateParticle(_particleSystem.Position);
				_particleSystem.AddParticle(particle, 0.5f);
			}
		}
	}

	/// <summary>
	/// Zastosowanie danego zachowania do obiektu.
	/// W tym wypadku przepisujemy pozycję obiektu do systemu cząsteczek.
	/// </summary>
	/// <param name="gameObject">Obiekt na którym działamy.</param>
	public void Apply(GameObject gameObject)
	{
		_particleSystem.Position = gameObject.Position;
	}

	/// <summary>
	/// Rysowanie systemu cząsteczek na ekranie.
	/// </summary>
	/// <param name="gameTime">Dostarcza informacje na temat czasu.</param>
	/// <param name="spriteBatch">Umożliwia rysowanie na ekranie.</param>
	public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		_particleSystem.Draw(gameTime, spriteBatch);
	}
}
```

Teraz musimy jeszcze ukryć kolekcję zachowań w klasie `GameObject` zmieniając jej modyfikator dostępu na `private`, a następnie dodać nową metodą umożliwiająco dodawanie zachowań do obiektu:

```
/// <summary>
/// Dodaje zachowanie do obiektu gry.
/// </summary>
/// <param name="behaviour">Zachowanie do dodania.</param>
public void AddBehaviour(IBehaviour behaviour)
{
	_behaviours.Add(behaviour);
	_updateables.Add(behaviour);
	
	if (behaviour is IDrawable)
		_drawables.Add(behaviour as IDrawable);
}
```

I jak widać w tej funkcji przy dodawaniu nowego obiektu `IBehaviour` dodajemy go do listy zachowań, do listy obiektów implementujących interfejs `IUpdateable` (bo każde zachowanie musi implementować ten interfejs) i dodatkowo sprawdzmy, czy przypadkiem przekazane zachowanie nie implementuje również interfejsu `IDrawable`. Przed chwilą zdefiniowaliśmy zachowanie implementujące ten interfejs, więc dodanie go do dowolnego obiektu gry spowoduje automatyczne dodanie tego zachowania również do listy obiektów rysowanych na planszy z poziomu tego obiektu. Tworzymy w ten sposób prostą hierarchię obiektów, trochę podobnie do wzorca kompozytu. Oczywiście jeśli obiekt klasy `GameObject` do którego podpinamy to zachowanie zostanie usunięty, to również cząsteczki ulegną zniszczeniu.

Dla porządku podaje również implementację metody `CreateParticle`, która została dodana do `GameObjectFactory`:

```
/// <summary>
/// Utwórz obiekt cząsteczki. Cząsteczka posiada teksturę Star,
/// żółty kolor oraz jest bardzo mała. Dodatkowo jej pozycja jest
/// losowo lekko przesuwana względem podanej pozycji w argumencie.
/// </summary>
/// <param name="position">Pozycja w której umieszczamy cząsteczkę.</param>
/// <returns>Obiekt z cząsteczką</returns>
public static GameObject CreateParticle(Vector2 position)
{
	var particle = new GameObject();
	particle.Texture = AssetManager.Instance.Star;
	particle.Color = Color.Yellow;
	particle.Position = position;
	particle.Position.Y += r.Next(-20, 20);
	particle.Position.X += r.Next(-20, 20);
	particle.Scale = r.Next(20, 30) / 100.0f;
	particle.Rotation = r.Next(360) * 180 / (float)Math.PI;
	return particle;
}
```