# Xamarin.Forms.Skeleton

<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/master/icon.png" width="128">

The new loading approach for cool apps in Xamarin Forms

## Setup
* Available on NuGet: [Plugin.MaterialDesignControls](https://www.nuget.org/packages/Xamarin.Forms.Skeleton/) [![NuGet](https://img.shields.io/nuget/v/Xamarin.Forms.Skeleton.svg?label=NuGet)](https://www.nuget.org/packages/Xamarin.Forms.Skeleton/)
* Install into your Core project.

**Platform Support**

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 8+|
|Xamarin.Android|API 16+|
|Xamarin.Forms|>= 4.0.0.425677|

## API Usage

You must add this namespace to your xaml files:

```XML
xmlns:extension="clr-namespace:Xamarin.Forms.Skeleton;assembly=Xamarin.Forms.Skeleton"
```

Add the following properties to generate a load animation:

```XML
<ListView RowHeight="109"
	SeparatorVisibility="None"
	SelectionMode="None"
	VerticalOptions="FillAndExpand"
	ItemTemplate="{StaticResource ItemTemplate}"
	ItemsSource="{Binding Items}"
	extension:Skeleton.IsParent="True"
	extension:Skeleton.IsBusy="{Binding IsBusy}"
	extension:Skeleton.Animation="Fade" />
```

Add the following properties to set a load background color:

```XML
<Label Text="{Binding Title}"
	TextColor="#c92726"
	FontSize="Large"
	extension:Skeleton.IsBusy="{Binding IsBusy}"
	extension:Skeleton.BackgroundColor="#c6c6c5" />
```

### Properties

#### IsParent (Boolean)
- Indicates if it is a parent control that has skeleton children controls.
- The default value is false.

#### IsBusy (Boolean)
- Indicates if the control is in busy state.
- The default value is false.

#### BackgroundColor (Color)
- Background color of the control in busy state.
- The default value is the xamarin forms default color.

#### Animation (AnimationTypes)
- Control animation in busy state.
- Possible values: None, Fade and Beat.
- The default value is None.

#### AnimationInterval (Int)
- Animation interval in busy state.
- The value is in milliseconds.
- The default value is 500.

#### AnimationParameter (Double?)
- Animation parameter used to modify the animation.
- The default value is null.

## Sample

<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/master/screenshots/iOS.gif" width="300">

## Demo
https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/tree/master/SkeletonExample

## Contributions
Contributions are welcome! If you find a bug want a feature added please report it.

If you want to contribute code please file an issue, create a branch, and file a pull request.

## License 
MIT License - see LICENSE.txt
