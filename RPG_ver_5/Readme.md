#RPG_ver_5 - obsługa myszy, obsługa ekranów dotykowych, rysowanie z przezroczystością, odtwarzanie dźwięków i muzyki

Uwaga: projekt RPG_ver_5 korzysta z folderu "packages" projektu RPG_ver_0 w celu zaoszczędzenia miejsca. W związku z tym zaleca się ściągnięcie całego repozytorium lub dodanie MonoGame przy pomocy NuGeta.

##Obsługa myszy

Dodajemy do projektu plik "mouse.xnb", który przedstawia kursor myszki i dodajemy odpowiedni wpis do klasy `AssetManager` do załadowania tej tekstury do pamięci.

Następnie... tworzymy `GameObject`, który będzie reprezentował kursor myszki! :)

W tym celu możemy dodać następującą metodę w naszej fabryce abstrakcyjnej:

```
/// <summary>
/// Utwórz obiekt kursora myszy. Postać posiada teksturę Mouse.
/// </summary>
/// <param name="position">Pozycja w której umieszczamy postać.</param>
/// <returns>Obiekt z postacią.</returns>
public static GameObject CreateMouseCursor(Vector2 position)
{
	var mouse = new GameObject();
	mouse.Texture = AssetManager.Instance.Mouse;
	mouse.Position = position;
	mouse.Radius = 20;
	return mouse;
}
```

Następnie musimy tylko w głównej klasie aplikacji `Game1` zainicjalizować obiekt przy pomocy tej metody:

```
private GameObject mouse;
```

```
mouse = GameObjectFactory.CreateMouseCursor(new Vector2(0, 0));
```

A także ustawiać jego pozycję przy każdym wywołaniu metody `Update` na pozycję kursora:

```
var mouseState = Mouse.GetState();
mouse.Position = new Vector2(mouseState.X, mouseState.Y);
mouse.Update(gameTime);
```

Oczywiście w funkcji `Draw` należy jeszcze narysować nasz obiekt.

```
mouse.Draw(gameTime, spriteBatch);
```

I mamy jeżdżący kursor myszki na ekranie.

Dlaczego wykorzystaliśmy do jego reprezentacji obiekt klasy `GameObject`? Z prostego powodu: dzięki temu mamy automatycznie zaimplementowany test czy kursor myszki znajduje się nad innym obiektem gry - metoda `CollidesWith` przyjmuje jako argument inny obiekt. W związku z tym przy obsłudze kliknięcia wystarczy wywołać tą metodę względem wszystkich obiektów, które gracz powinien móc kliknąć i jeśli wystąpiła kolizja, to kursor myszki wskazuje na dany obiekt! :)

Zmodyfikujmy teraz nieco grę, aby dany kwadrat (już właściwie romb) skakał tylko wtedy gdy użytkownik kliknie na nim lewym przyciskiem myszy lub naciśnie klawisze Q, W lub E dla odpowiednio górnego, środkowego i dolnego bohatera gry.

W tym celu zmienimy modyfikator dostępu pola `character` w klasie `SingleLine` na public, a funkcję `Update` w głównej klasie aplikacji dla sytuacji gdy gra jeszcze trwa (```!GameOver```) zmodyfikujemy w następujący sposób:

```
var state = Keyboard.GetState();
if (state.IsKeyDown(Keys.Q))
{
	lines[0].Jump();
}
if (state.IsKeyDown(Keys.W))
{
	lines[1].Jump();
}
if (state.IsKeyDown(Keys.E))
{
	lines[2].Jump();
}

if (mouseState.LeftButton == ButtonState.Pressed)
{
	foreach (var singleLine in lines)
	{
		if (mouse.CollidesWith(singleLine.Character))
		{
			singleLine.Jump();
		}
	}
}
```

##Obsługa ekranów dotykowych

Analogicznie do klasy obsługującej klawiaturę oraz klasy obsługującej mysz, jest również klasa do obsługi ekranów dotykowych i tego co one generują - czyli punktów dotknięcia. Poniżej kod do sprawdzenia czy gracz dotyka palcem na ekranie w miejscu gdzie znajduje się jeden z rombów i jeśli tak, to wykonujemy skok, podobnie jak ma to miejsce przy kliknięciu lewym przyciskiem myszy. Wykorzystamy zatem tą samą pętlę w środku tej metody, ale skorzystamy z klasy `TouchCollection`, posiadajacej informacje na temat wszystkich "paluchów na ekranie".

```
TouchCollection touchCollection = TouchPanel.GetState();
foreach (TouchLocation tl in touchCollection)
{
	if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
	{
		var touchLocation = new GameObject();
		touchLocation.Position = tl.Position;
		foreach (var singleLine in lines)
		{
			if (touchLocation.CollidesWith(singleLine.Character))
			{
				singleLine.Jump();
			}
		}
	}
}
```

Standardowo pobieramy stan metodą `GetState`, a następnie sprawdzamy po kolei czy dany punkt dotknięcia ekranu jest obecnie w stanie `TouchLocationState.Pressed`, czyli właśnie został dotknięty, lub w stanie `TouchLocationState.Moved`, czyli po dotknięciu ekranu palec przesunął się, ale nadal dotyka ekranu. Jeśli tak jest, to znaczy że dany palec powinien działać jak kliknięcie myszki - pobieramy jego pozycję, zamieniamy na obiekt klasy `GameObject` w celu przetestowania kolizji z obiektami `character` w każdym wystąpieniu klasy `SingleLine` i jeśli ta kolizja występuje to wykonujemy skok daną postacią.

O ile nie mamy monitora z ekranem dotykowym przy komputerze, to obecnie nie jesteśmy w stanie przetestować działania tego kodu, ale przy eksporcie projektu na platformy mobilne będzie to już możliwe.

Dodatkowo dodajmy możliwość zrestartowania gry jeśli doszło do jej końca poprzez dotknięcie ekranu dwoma palcami.

```
int count = 0;
TouchCollection touchCollection = TouchPanel.GetState();
foreach (TouchLocation tl in touchCollection)
{
	if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
	{
		count++;
	}
}

if (count >= 2)
	NewGame();
```

##Rysowanie z przezroczystością

Aby rysować obiekty z przezroczystością należy po prostu przemnożyć kolor podawany jako argument funkcji `Draw` przez wartość przezroczystości. Dodajmy zatem do klasy `GameObject` pole:

```
/// <summary>
/// Współczynnik przezroczystości obiektu.
/// </summary>
public float Alpha = 1.0f;
```

A w samej funkcji `Draw` tej klasy zaktualizujmy wywołanie `spriteBatch.Draw` następująco:

```
spriteBatch.Draw(Texture, Position, null, Color * Alpha, Rotation, Origin, Scale, SpriteEffects.None, 0);
```

Możemy teraz dodać do naszych obiektów postaci emiter cząsteczek, który będzie dublował oryginalny obiekt, ale rysował z wartością przezroczystości 0.5. Cząsteczki te będą przesuwane w lewo ekranu nadając dynamiki grze.

Wpierw zmodyfikujmy jego kod w taki sposób, aby można było ustawić "wzorzec" cząsteczki, który następnie będzie powielany i modyfikowany w sposób losowy. Przy okazji dodajem również pole opisujące czas życia cząsteczki.

```
/// <summary>
/// Wzorzec cząsteczki.
/// </summary>
public GameObject ParticleTemplate;

/// <summary>
/// Czas życia pojedynczej cząsteczki.
/// </summary>
public float Lifespan = 0.5f;
```

A w samej funkcji `Update` zamiast dotychczasowego korzystania z fabryki obiektów umieszczamy poniższy kod generujący losową, nieco mniejszą od obiektu oryginalnego cząsteczkę:

```
var particle = ParticleTemplate.Clone();
particle.Position = _particleSystem.Position;
particle.Position.Y += Game1.random.Next(-20, 20);
particle.Position.X += Game1.random.Next(-20, 20);
particle.Scale = Game1.random.Next(20, 30) / 100.0f;
//kąt w radianach = kąt w stopniach * PI / 180
particle.Rotation = Game1.random.Next(360) * (float)Math.PI / 180;
_particleSystem.AddParticle(particle, 0.5f);
```

Brakuje nam jeszcze implementacji metody `Clone` w klasie `GameObject`, która wykonuje płytką kopię danego obiektu gry, czyli tylko przepisuje podstawowe pola powiązane z tym obiektem, bez dodatkowych zachowań.

```
/// <summary>
/// Metoda wykonująca płytką kopię obiektu gry, bez przenoszenia zachowań.
/// </summary>
/// <returns>Płytka kopia obiektu gry.</returns>
public GameObject Clone()
{
	var o = new GameObject();
	o.Alpha = Alpha;
	o.Active = Active;
	o.Radius = Radius;
	o.Rotation = Rotation;
	o.Scale = Scale;
	o.Position = Position;
	o.Color = Color;
	o.Texture = Texture;
	return o;
}
```

I wówczas po utworzeniu postaci możemy dodać poniższy kod w celu uzyskania "ogona" za postacią:

```
var emitParticles = new EmitParticlesBehaviour();
emitParticles.Lifespan = 2;
emitParticles.ParticlesPerSpawn = 3;
emitParticles.AddRandomBehaviour = true;
emitParticles.ParticleTemplate = Character.Clone();
emitParticles.ParticleTemplate.Alpha = 0.5f;
Character.AddBehaviour(emitParticles);
```

W samej klasie `EmitParticlesBehaviour` dodaliśmy jeszcze pole typu bool `AddRandomBehaviour`, które jeśli jest ustawione na prawdę, to spowoduje dodanie do każdej cząsteczki zachowania `RandomBehaviour`, którego implementacja znajduje się poniżej:

```
public class RandomBehaviour : IBehaviour
{
	private float _x = 480 + 50;

	public RandomBehaviour(float x)
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
	}

	/// <summary>
	/// Zastosowanie danego zachowania do obiektu.
	/// </summary>
	/// <param name="gameObject">Obiekt na którym działamy.</param>
	public void Apply(GameObject gameObject)
	{
		gameObject.Position.X = _x;
		gameObject.Rotation += 0.1f;
	}
}
```

##Odtwarzanie dźwięków i muzyki

Dodajmy wpierw do folderu `Content` pliki .xnb ze skompilowanymi dźwiękami oraz plik .wma ze ścieżką dźwiękową.

	UWAGA: wersja MonoGame dla projektów desktop nie wspiera obiektów `Song` w formacie innym niż WAV, ale wersje dla platform mobilnych już wspierają. Dlatego na razie dodamy kod w postaci zakomentowanej dla odtwarzania muzyki w tle.
	
Teraz w klasie `JumpBehaviour` możemy odtworzyć dźwięk skoku w następujący sposób:

```
AssetManager.Instance.Jump.Play();
```

	Jeśli po uruchomieniu uzyskamy komunikat o braku "openal32.dll", to musimy zainstalować tą [bibliotekę](http://www.openal.org/). Na platformach mobilnych nie ma tego problemu.
	
Dodajemy jeszcze w odpowiednich miejscach naszej aplikacji odtwarzanie dźwięku na kolizję z gwiazdką i przeszkodą, czyli w metodzie `Update` klasy `SingleLine`, gdzie sprawdzamy czy doszło do kolizji.

Zaraz po wczytaniu wszystkich zasobów graficznych i dźwiękowych możemy od razu przystąpić do odtworzenia ścieżki dźwiękowej. Do tego użyjemy poniższego kodu:

```
MediaPlayer.Play(AssetManager.Instance.BackgroundMusic);
```

##Notka odnośnie pochodzenia dźwięków i ścieżki dźwiekowej

Efekty dźwiękowe zostały wygenerowane przy pomocy narzędzia [BFXR](http://www.bfxr.net/).

Ścieżka dźwiękowa została stworzona przez [Rolemusic](http://freemusicarchive.org/music/Rolemusic/~/mixdown) i jest rozpowszechniana na licencji [CC by Attribution 4.0](http://creativecommons.org/licenses/by/4.0/).