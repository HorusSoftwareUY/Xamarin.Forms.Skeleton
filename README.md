# Skeleton for Xamarin and MAUI apps
<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/icon.png" width="128">

The Skeleton control is a popular approach to loading content in mobile apps that provides one or more visual placeholders while content is being loaded. This technique is particularly useful for improving user experience, as it reduces perceived load times and provides a more engaging experience.

<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/screenshots/Skeleton_general.gif" width="300">

## Setup

### .NET MAUI

* Available on NuGet: [HorusStudio.Maui.Skeleton](https://www.nuget.org/packages/HorusStudio.Maui.Skeleton/) [![NuGet](https://img.shields.io/nuget/v/HorusStudio.Maui.Skeleton.svg?label=NuGet)](https://www.nuget.org/packages/HorusStudio.Maui.Skeleton/)

|Platform|Version|
| ------------------- | :------------------: |
|.NET|8, 9 and 10|
|Android, iOS, macOS, Windows|all versions supported by .NET MAUI|

Version 3.0.0 and later require .NET 8 or newer. Apps on .NET 6 or 7 should stay on 2.0.0, which keeps working.

### Xamarin.Forms (legacy)

* Available on NuGet: [Xamarin.Forms.Skeleton](https://www.nuget.org/packages/Xamarin.Forms.Skeleton/) [![NuGet](https://img.shields.io/nuget/v/Xamarin.Forms.Skeleton.svg?label=NuGet)](https://www.nuget.org/packages/Xamarin.Forms.Skeleton/)

**Xamarin.Forms reached end of support in May 2024.** This package is frozen at 2.0.0 and will not
receive further updates. New work happens on the .NET MAUI package above.

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 8+|
|Xamarin.Android|API 16+|
|Xamarin.Forms|>= 4.0.0.425677|

## Usage

You must add this namespace to your xaml files.

For .NET MAUI:

```XML
xmlns:sk="clr-namespace:Maui.Skeleton;assembly=Maui.Skeleton"
```

For Xamarin.Forms:

```XML
xmlns:sk="clr-namespace:Xamarin.Forms.Skeleton;assembly=Xamarin.Forms.Skeleton"
```

Add the following properties to generate a loading animation:

```XML
<ListView RowHeight="90"
	SeparatorVisibility="None"
	SelectionMode="None"
	VerticalOptions="FillAndExpand"
	BackgroundColor="Transparent"
	ItemTemplate="{StaticResource ItemTemplate}"
	ItemsSource="{Binding Items}"
	sk:Skeleton.IsParent="True"
	sk:Skeleton.IsBusy="{Binding IsBusy}"
	sk:Skeleton.Animation="{sk:DefaultAnimation Fade}" />
```

Add the following properties to set a loading animation with a specific background color:

```XML
<Frame BackgroundColor="Transparent"
       HasShadow="False"
       CornerRadius="5"
       Padding="0"
       HorizontalOptions="Start"
       sk:Skeleton.IsBusy="{Binding IsBusy}"
       sk:Skeleton.BackgroundColor="#c6c6c5">
			<Label Text="{Binding Title}"
					TextColor="#000000"
					FontSize="20"
					FontAttributes="Bold"
					HorizontalOptions="Start"/>
</Frame>
```

### Properties

#### IsBusy (Boolean)
- Indicates if the control is busy in a loading state.
- The default value is false.

**`IsBusy` is not inherited.** Every element that should react needs its own
`sk:Skeleton.IsBusy="{Binding ...}"`. Setting it once on an outer layout does nothing for the
elements inside it.

#### IsParent (Boolean)
- Controls whether a container covers its own content or lets each child draw itself.
- The default value is false.

**The name reads backwards from what it does.** `IsParent="True"` does not mean "handle my
children" — it means **"leave my children alone, they handle themselves"**.

| | What happens |
| --- | --- |
| `IsParent="False"` (default) | The container hides its content and paints its own `BackgroundColor`. You get **one solid shape**. |
| `IsParent="True"` | The content is left untouched. **Every child that should show a placeholder needs its own `IsBusy`.** |

Either way the container still paints its own colour and runs its own animation. `IsParent` only
governs whether it touches what is inside.

**One solid shape** — no `IsParent`, and the container needs a colour:

```XML
<Border sk:Skeleton.IsBusy="{Binding IsBusy}"
        sk:Skeleton.BackgroundColor="#c6c6c5">
    <Label Text="{Binding Title}" />
</Border>
```

**Several shapes** — `IsParent="True"`, and each child declares itself:

```XML
<VerticalStackLayout sk:Skeleton.IsParent="True"
                     sk:Skeleton.IsBusy="{Binding IsBusy}">

    <Border sk:Skeleton.IsBusy="{Binding IsBusy}"
            sk:Skeleton.BackgroundColor="#c6c6c5">
        <Label Text="{Binding Title}" />
    </Border>

    <Border sk:Skeleton.IsBusy="{Binding IsBusy}"
            sk:Skeleton.BackgroundColor="#c6c6c5">
        <Label Text="{Binding Subtitle}" />
    </Border>

</VerticalStackLayout>
```

**What changed in 3.0.0.** Until 2.0.0 only `Grid` and `StackLayout` hid their content. `Border`,
`Frame`, `ContentView` and `ScrollView` painted the placeholder and left their content showing on
top of it, which is why samples and apps used `Skeleton.Hide="True"` on each child to get out of the
way. They now behave like every other container, so that workaround is no longer needed — it still
works, it is just redundant.

If content that used to stay visible now disappears, that container is the one deciding it: set
`sk:Skeleton.IsParent="True"` on it to get the old behaviour back.

**Three mistakes this prevents**, none of which reports an error:

- `IsParent="True"` with a child that declares nothing does **nothing at all** to that child. If it
  holds real content, the content shows straight through the loading state. If you drew your own
  placeholder shape there, it stays on screen after loading finishes, on top of the real content.
- No `IsParent` and no `BackgroundColor` leaves an empty hole: the content is hidden and nothing is
  painted in its place.
- `IsBusy` on an outer layout alone renders no placeholders at all, because it is not inherited.

#### BackgroundColor (Color)
- Control background color when is busy.
- The default value is the xamarin forms default color.

#### Hide (Boolean)
- Indicates if the control is hide when is busy.
- The default value is false.

#### Animation (BaseAnimation)
- Control animation when is busy.
- Possible values: None, Fade, Beat, HorizontalShake, VerticalShake, Shimmer, Aurora, Tint and custom animation inheriting from BaseAnimation.
- The default value is null, which means no animation runs.

### Animation settings

The interval and the parameter are **not** attached properties. They are set on the
`DefaultAnimation` markup extension, alongside `Source`:

```XML
sk:Skeleton.Animation="{sk:DefaultAnimation Source=Fade, Interval=600, Parameter=0.3}"
```

`Source` is the extension's content property, so `{sk:DefaultAnimation Fade}` is shorthand for
`{sk:DefaultAnimation Source=Fade}`.

| Setting | Meaning | Default |
| --- | --- | --- |
| `Source` | Which built-in animation to use. `None` resolves to no animation at all. | `None` |
| `Interval` | Duration in milliseconds of **each half** of a cycle, so a `Fade` at 600 takes 1.2s per pulse. | 500 |
| `Parameter` | What the animation interpolates towards. See the table below. | per animation |

| Source | What moves | `Parameter` means | Default |
| --- | --- | --- | --- |
| `Fade` | opacity | opacity to fade to | 0.6 |
| `Beat` | scale | scale to grow to | 1.03 |
| `VerticalShake` | position | offset in units, up and down | 15 |
| `HorizontalShake` | position | offset in units, left and right | 10 |
| `Shimmer` | a band of light across the placeholder | not used, see below | — |
| `Aurora` | a wide field of colour drifting back and forth | not used, see below | — |
| `Tint` | the whole placeholder washing to a colour and back | not used, see below | — |

#### Shimmer, Aurora and Tint

These repaint the placeholder rather than animating a property of the view, so they behave a little
differently from the other four. `Shimmer` sends a band of light across and off the other side;
`Aurora` pans a much wider field of colour back and forth, so colour is always on screen; `Tint`
washes the whole placeholder to a colour and back, with nothing moving at all.

- It must be attached to the element that shows the placeholder colour. It does not carry down to
  children the way `Fade` and `Beat` do.
- **It does not work on `Frame`.** `Frame` is deprecated in MAUI and its renderer does not repaint
  when the background is replaced, so the band never moves. Use `Border`.
- `Interval` is the whole movement, not half a cycle: one pass for `Shimmer`, out and back for
  `Aurora` and `Tint`. 1600 is a good value for any of them.
- They ignore `Parameter`, and take `Direction` and `SweepColors` instead.

```XML
<Border StrokeShape="RoundRectangle 5"
        StrokeThickness="0"
        sk:Skeleton.IsBusy="{Binding IsBusy}"
        sk:Skeleton.BackgroundColor="#c6c6c5"
        sk:Skeleton.Animation="{sk:DefaultAnimation Source=Shimmer, Interval='1600', Direction='Diagonal'}" />
```

| Setting | Values | Default |
| --- | --- | --- |
| `Direction` | `Horizontal`, `Vertical`, `Diagonal`, `DiagonalReverse`. Not used by `Tint`. | `Horizontal` |
| `SweepColors` | two or three `#AARRGGBB` colours, comma separated and quoted | follows the placeholder |

Left alone, the band contrasts with the placeholder automatically: light over a dark placeholder,
dark over a light one. A placeholder bound with `AppThemeBinding` therefore shimmers correctly in
both themes with no extra work.

## Animations

### Skeleton Animation

<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/screenshots/Skeleton_details.gif" width="300">

### Beat Animation

<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/screenshots/Beat_details.gif" width="300">

### Fade Animation

<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/screenshots/Fade_details.gif" width="300">

### Vertical Shake Animation

<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/screenshots/Vertical_details.gif" width="300">

### Horizontal Shake Animation

<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/screenshots/Horizontal_details.gif" width="300">

### Custom Animation

<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/screenshots/Custom_details.gif" width="300">


## Demo

### MAUI
https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/tree/master/SkeletonSample

### Xamarin.Forms:
https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/tree/master/SkeletonExample
 
## Changelog

What changed in each release, including the behaviour change in 3.0.0 and how to opt out of it, is
in [CHANGELOG.md](CHANGELOG.md).

## Developed by
<a href="http://horus.com.uy" ><img src="https://horus.com.uy/img/logo_horus.png" width="128"></a>

## Contributions
Contributions are welcome! If you find a bug want a feature added please report it.

If you want to contribute code please file an issue, create a branch, and file a pull request.

## License 
MIT License - see LICENSE.txt
