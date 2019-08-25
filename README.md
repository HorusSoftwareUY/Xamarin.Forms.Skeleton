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

### Properties

#### PropertyName
PropertyDescription

## Sample

**XAML**

```XML
<material:MaterialButton Text="Save" Icon="save.png" Command="{Binding TapCommand}" CommandParameter="Saved" />
```

**Result**

<img src="https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/blob/master/screenshots/button.jpg" width="300">

## Demo
https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/tree/master/SkeletonExample

## Contributions
Contributions are welcome! If you find a bug want a feature added please report it.

If you want to contribute code please file an issue, create a branch, and file a pull request.

## License 
MIT License - see LICENSE.txt
