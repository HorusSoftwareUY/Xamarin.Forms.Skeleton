# Xamarin.Forms.Skeleton 
<img src="https://raw.githubusercontent.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/develop/icon.png" width="128">

The latest trend for loading approaches in Xamarin Forms apps. Skeleton can be easily implemented on each view contained in your Xaml.

## Sample
<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/develop/screenshots/Skeleton_general.gif" width="300">

### Skeleton Animation

<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/develop/screenshots/Skeleton_details.gif" width="300">

### Beat Animation

<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/develop/screenshots/Beat_details.gif" width="300">

### Fade Animation

<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/develop/screenshots/Fade_details.gif" width="300">

### Vertical Shake Animation

<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/develop/screenshots/Vertical_details.gif" width="300">

### Horizontal Shake Animation

<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/develop/screenshots/Horizontal_details.gif" width="300">

### Custom Animation

<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/develop/screenshots/Custom_details.gif" width="300">

## Setup
* Available on NuGet: [Xamarin.Forms.Skeleton](https://www.nuget.org/packages/Xamarin.Forms.Skeleton/) [![NuGet](https://img.shields.io/nuget/v/Xamarin.Forms.Skeleton.svg?label=NuGet)](https://www.nuget.org/packages/Xamarin.Forms.Skeleton/)
* Install into your Core and Client projects.

**Platform Support**

|Platform|Version|
| ------------------- | :------------------: |
|Xamarin.iOS|iOS 8+|
|Xamarin.Android|API 16+|
|Xamarin.Forms|>= 4.0.0.425677|

## API Usage

You must add this namespace to your xaml files:

```XML
xmlns:sk="clr-namespace:Xamarin.Forms.Skeleton;assembly=Xamarin.Forms.Skeleton"
```

Add the following properties to generate a loading animation:

```XML
<ListView RowHeight="109"
	SeparatorVisibility="None"
	SelectionMode="None"
	VerticalOptions="FillAndExpand"
	ItemTemplate="{StaticResource ItemTemplate}"
	ItemsSource="{Binding Items}"
	sk:Skeleton.IsParent="True"
	sk:Skeleton.IsBusy="{Binding IsBusy}"
	sk:Skeleton.Animation="{sk:DefaultAnimation Fade}" />
```

Add the following properties to set a loading animation with a specific background color:

```XML
<Label Text="{Binding Title}"
	TextColor="#c92726"
	FontSize="Large"
	sk:Skeleton.IsBusy="{Binding IsBusy}"
	sk:Skeleton.BackgroundColor="#c6c6c5" />
```

### Properties

#### IsParent (Boolean)
- Indicates if it is a parent control which has skeleton children controls.
- The default value is false.

#### IsBusy (Boolean)
- Indicates if the control is busy in a loading state.
- The default value is false.

#### BackgroundColor (Color)
- Control background color when is busy.
- The default value is the xamarin forms default color.

#### Hide (Boolean)
- Indicates if the control is hide when is busy.
- The default value is false.

#### Animation (BaseAnimation)
- Control animation when is busy.
- Possible values: None, Fade, Beat, HorizontalShake, VerticalShake and custom animation inheriting from BaseAnimation.
- The default value is None.

#### AnimationInterval (Int)
- Animation interval when is busy.
- Value in milliseconds.
- The default value is 500.

#### AnimationParameter (Double?)
- Animation parameter to modify the animation.
- The default value is null.

## Demo
https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/tree/master/SkeletonExample

## Roadmap
- Automatic ItemsSource population (fake data) for skeleton preview in: ListView, CollectionView, Repeater, BindableLayout, etc.
 
## Developed by
<a href="http://horus.com.uy" ><img src="https://horus.com.uy/img/logo_horus.png" width="128"></a>

## Contributions
Contributions are welcome! If you find a bug want a feature added please report it.

If you want to contribute code please file an issue, create a branch, and file a pull request.

## License 
MIT License - see LICENSE.txt
