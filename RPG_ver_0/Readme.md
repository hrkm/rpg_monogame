#RPG_ver_0 - tworzenie projektu, rysowanie tekstur, przesuwanie obiektów, wypisywanie tekstu, obsługa klawiatury

##Tworzenie projektu
1. Tworzymy nowy projekt wybierając New Project -> Windows -> Console Application.

	W ustawieniach projektu możemy opcjonalnie zmienić na Windows Application, żeby nie pojawiało się okienko konsoli.

2. Dodajemy przy pomocy NuGet bibliotekę MonoGame.
	
	Jeśli po dodanie brakuje w References powiązania do MonoGameFramework, należy ręcznie dodać odpowiednią *.dll'kę z folderu "packages".
	
3. Tworzymy nowy folder Content.

##Kompilowanie grafiki/dźwięku/czcionek do plików \*.xnb

1. Uruchamiamy Pipeline.exe.

2. Tworzymy nowy projekt w tym narzędziu.

3. Dodajemy wszystkie pliki z grafikami, czcionki, pliki dźwiękowe etc.

4. Klikamy "Build".
	
##Rysowanie na ekranie tekstur
	
1. Dodajemy skompilowane pliki *.xnb do folderu Content.

	Najlepiej dodać jako "linki".

2. Ustawiamy właściwości tych plików jako "Content" i "Copy Always".
	
3. Dodajemy zmienną `Texture2D` do przechowywania tekstury "square.xnb".

```private Texture2D square;```

4. W funkcji `LoadContent` uzupełniamy linijkę:

```square = Content.Load<Texture2D>("square");```
	
5. W funkcji `Draw` uzupełniamy linijki:
	
```
spriteBatch.Begin();
spriteBatch.Draw(square, new Vector2(0,0), Color.White);
spriteBatch.End();
```
	
Zauważyliście że kwadrat jest przesunięty trochę w prawo? Naprawmy to w funkcji `Draw`:
	
```
spriteBatch.Draw(square, position, null, Color.White, 0, new Vector2(50, 50), 1, SpriteEffects.None, 0);
```

Ta wersja metody Draw dodatkowo umożliwia podanie obrotu grafiki, punktu względem którego wykonywane są operacje obrotu/przesunięcia/skalowania, wartości przeskalowania obiektu. Pozostałe parametry są nam obecnie niepotrzebne.
	
##Modyfikacja ustawień grafiki (np. rozdzielczość, orientacja ekranu)
	
1. W funkcji `Initialize` ustawiamy parametry ekranu
	
```
graphics.PreferredBackBufferWidth = 480;
graphics.PreferredBackBufferHeight = 800;
graphics.SupportedOrientations = DisplayOrientation.Portrait;
```

##Przesuwanie obiektów
	
1. Dodajemy zmienną do przetrzymywania pozycji prostokąta:
	
```
private Vector2 position = new Vector2(240, 400);
```

2. Modyfikujemy tą zmienną w funkcji `Update`:
	
```
if (position.Y < 700)
	position.Y += (float)gameTime.ElapsedGameTime.TotalSeconds*100;
```
	
Zauważmy, że powyższy zapis oznacza przesuwanie współrzędnej X wektora `position` o 100 pikseli na sekundę. W tym celu wykorzystywana jest zmienna `gameTime.ElapsedGameTime.TotalSeconds` podająca liczbę sekund, która upłynęła od ostatniego wywołania funkcji `Update`.

##Obsługa klawiatury

Dodajmy interakcję użytkownika z klawiatury - trzymanie wciśniętej spacji powinno spowodować "podskok" prostokąta, a dokładniej jego powolne przesuwanie się ku górze.

1. Usuwamy zatem logikę dodaną w poprzednim rozdziale i wklejamy poniższy fragment kodu:
	
```
var state = Keyboard.GetState();
if (state.IsKeyDown(Keys.Space))
	position.Y -= (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
if (position.Y < 400)
	position.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 50;
```
		

##Wypisywanie tekstu

	Aby dodać tekst wpierw musimy dodać plik z zasobem tekstu do folderu Content (jeśli nie zrobiliśmy tego wcześniej).

1. Wczytujemy `SpriteFont`:
	
```
font = Content.Load<SpriteFont>("font");
```
	
2. W funkcji `Draw` dodajemy:
	
```
spriteBatch.DrawString(font, "Ahoj przygodo!", new Vector2(6,6), Color.White);
```
	