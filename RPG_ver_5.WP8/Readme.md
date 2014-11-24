#RPG_ver_5.WP8 - tworzenie wersji dla platformy Windows Phone 8

##Tworzenie wersji dla platformy Windows Phone 8

Wpierw należy utworzyć nowy projekt. Wybieramy New -> Project -> Windows Store -> Windows Phone Apps, a z listy wzorzec "Blank App (Windows Phone Silverlight)". To jest typ aplikacji kompatybilny z systemem Windows Phone 8.0 oraz 8.1, ale to nie jest najnowsza wersja API (figurujące pod nazwą Universal App). Korzystamy z tego wzorca, bo nie ma jeszcze paczek NuGeta dla tych nowych typów - działać będzie dokładnie tak samo. ;)

Dodajemy do projektu MonoGame Binaries przy pomocy NuGeta.

	Z jakiegoś powodu referencja do projektu nie jest dodawana, mimo że biblioteka jest importowana w projekcie. Dlatego należy dodać ją ręcznie wybierając dowolny podfolder z "packages" dla "wp8", tj. albo "ARM" albo "x86" - na etapie kompilacji VS sam podepnie odpowiednią wersję.

Dodajemy wszystkie pliki kodu źródłowego z projektu RPG_ver_5 - tutaj możemy je po prostu podlinkować, żeby nie kopiować ich niepotrzebnie. Podejście to ma tą zaletę, że jeśli coś zmodyfikujemy później w wersji dla platformy PC, to zmiana ta będzie automatycznie widoczna w projekcie dla WP8. (oczywiście jeśli dodaliśmy jakieś nowe klasy/pliki, to trzeba je dodać ręcznie, ale o tym już się dowiemy przy próbie kompilacji jeśli o czymś zapomnieliśmy)

Dodajemy folder "Content" i linkujemy tam pliki z zasobami.

Modyfikujemy zawartość pliku "MainPage.xaml", zasadniczo usuwając wszystkie elementy i dodając `DrawingSurfaceBackgroundGrid` oraz w nim `MediaElement`:

```
<phone:PhoneApplicationPage
    x:Class="RPG_ver_5.WP8.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:phone="clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone"
    xmlns:shell="clr-namespace:Microsoft.Phone.Shell;assembly=Microsoft.Phone"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    FontFamily="{StaticResource PhoneFontFamilyNormal}"
    FontSize="{StaticResource PhoneFontSizeNormal}"
    Foreground="{StaticResource PhoneForegroundBrush}"
    SupportedOrientations="Portrait" Orientation="Portrait"
    shell:SystemTray.IsVisible="False">

    <DrawingSurfaceBackgroundGrid x:Name="XnaSurface" Background="Transparent">
        <MediaElement></MediaElement>
    </DrawingSurfaceBackgroundGrid>

</phone:PhoneApplicationPage>
```

Istotna jest nazwa "XnaSurface", bowiem tej będzie szukał MonoGame w celu osadzenia naszej gry.

Zawartość plikue "MainPage.xaml.cs" powinna wyglądać następująco:

```
public partial class MainPage : PhoneApplicationPage
{
	private Game1 _game;

	public MainPage()
	{
		InitializeComponent();

		_game = XamlGame<Game1>.Create("", this);
	}
}
```

I nie trzeba nic więcej dodawać aby gra zadziałała. Możemy więc po prostu kliknąć debuguj i cieszyć się z działającej gry. :)

##Dodatkowe zmiany w projekcie RPG_ver_5

Na platformie mobilnej nie ma kursora myszy, więc w funkcji `Draw` dodajemy dyrektywę preprocesora, która wytnie tą instrukcję jeśli kompilujemy wersję dla tej platformy:

```
#if !WINDOWS_PHONE
	mouse.Draw(gameTime, spriteBatch);
#endif
```

Analogicznie w klasie `AssetManager` zamiast umieszczać kodu wczytującego muzykę w komentarzu możemy napisać:

```
#if WINDOWS_PHONE
	BackgroundMusic = content.Load<Song>("Rolemusic_-_pl4y1ng");
#endif
```

I w samej aplikacji możemy odtworzyć tą muzykę jeśli działamy na platformie Windows Phone:

```
#if WINDOWS_PHONE
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