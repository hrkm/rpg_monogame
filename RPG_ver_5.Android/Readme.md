#RPG_ver_5.Android - tworzenie wersji dla platformy Android

##Tworzenie wersji dla platformy Android

Wpierw należy utworzyć nowy projekt. Wybieramy New -> Project -> Android, a z listy wzorzec "Blank App (Android)". Ten wzorzec pochodzi z [Xamarin.Android](http://xamarin.com/platform), bez tego nie jesteśmy w stanie utworzyć aplikacji na Androida.

Dodajemy do projektu MonoGame Binaries przy pomocy NuGeta.

	Z jakiegoś powodu referencja do projektu nie jest dodawana, mimo że biblioteka jest importowana w projekcie. Dlatego należy dodać ją ręcznie wybierając z "packages" folder "MonoAndroid".

Dodajemy wszystkie pliki kodu źródłowego z projektu RPG_ver_5 - tutaj możemy je po prostu podlinkować, żeby nie kopiować ich niepotrzebnie. Podejście to ma tą zaletę, że jeśli coś zmodyfikujemy później w wersji dla platformy PC, to zmiana ta będzie automatycznie widoczna w projekcie dla WP8. (oczywiście jeśli dodaliśmy jakieś nowe klasy/pliki, to trzeba je dodać ręcznie, ale o tym już się dowiemy przy próbie kompilacji jeśli o czymś zapomnieliśmy)

Dodajemy folder "Content" i linkujemy tam pliki z zasobami. Istotna różnica w przypadku dźwięków - nie dodajemy plików .xnb tylko oryginalne pliki w postaci "surowej", czyli .wav oraz .mp3!

Dodanie MonoGame do projektu automatycznie utworzyło `GameActivity`, które niestety musimy nieco zmodyfikować. Działająca wersja powinna wyglądać tak:

```
[Activity(Label = "RPG - MonoGame Demo"
	, MainLauncher = true
	, Icon = "@drawable/icon"
	, AlwaysRetainTaskState = true
	, LaunchMode = LaunchMode.SingleInstance
	, ScreenOrientation = ScreenOrientation.Portrait
	, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
{
	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		Game1.Activity = this;
		var g = new Game1();
		SetContentView(g.Window);
		g.Run();
	}
}
```

I nie trzeba nic więcej dodawać aby gra zadziałała. Możemy więc po prostu kliknąć debuguj i cieszyć się z działającej gry. :)

##Dodatkowe zmiany w projekcie RPG_ver_5

Na platformie mobilnej nie ma kursora myszy, więc w funkcji `Draw` dodajemy dyrektywę preprocesora, która wytnie tą instrukcję jeśli kompilujemy wersję dla tej platformy:

```
#if !ANDROID
	mouse.Draw(gameTime, spriteBatch);
#endif
```

Analogicznie w klasie `AssetManager` zamiast umieszczać kodu wczytującego muzykę w komentarzu możemy napisać:

```
#if ANDROID
	BackgroundMusic = content.Load<Song>("Rolemusic_-_pl4y1ng");
#endif
```

I w samej aplikacji możemy odtworzyć tą muzykę jeśli działamy na platformie Windows Phone:

```
#if ANDROID
	MediaPlayer.Play(AssetManager.Instance.BackgroundMusic);
#endif
```

Dodatkowo w celu obsługi klawisza "Back" dodajemy w metodzie `Update`:

```
if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
	Exit();
```

Oczywiście warunki w dyrektywach preprocesora można łączyć:

```
#if WINDOWS_PHONE && ANDROID
	...
#endif
```