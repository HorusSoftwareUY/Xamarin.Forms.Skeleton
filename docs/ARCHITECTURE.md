# Architecture

Notes for anyone working on this repository. For how to *use* the library, see the [README](../README.md).

## Two packages, one set of sources

The repository ships two NuGet packages from the same C# files.

| Package | Project | Target | Status |
| --- | --- | --- | --- |
| `HorusStudio.Maui.Skeleton` | `Maui.Skeleton/` | `net8.0;net9.0;net10.0` | active |
| `Xamarin.Forms.Skeleton` | `Xamarin.Forms.Skeleton/` | `netstandard2.0` | frozen |

Every `.cs` file physically lives in `Xamarin.Forms.Skeleton/`. `Maui.Skeleton.csproj` owns no sources of
its own; it pulls the same files in by glob:

```xml
<Compile Include="..\Xamarin.Forms.Skeleton\**\*.cs"
         Exclude="..\Xamarin.Forms.Skeleton\bin\**\*.cs;..\Xamarin.Forms.Skeleton\obj\**\*.cs"
         LinkBase="Shared" />
```

The two frameworks are reconciled with `#if NET6_0_OR_GREATER`, which is true for every `net8.0` and
later target and false for `netstandard2.0`. It switches the namespace (`Maui.Skeleton` versus
`Xamarin.Forms.Skeleton`), `Colors.Transparent` versus `Color.Transparent`, and the main-thread helper.

**Xamarin.Forms is frozen.** It went out of support in May 2024 and its package will not be
republished. Do not spend effort on that side.

### Where to put a new file

A file added under `Xamarin.Forms.Skeleton/` is compiled into **both** projects, so it has to build
against netstandard2.0 and Xamarin.Forms 4.4 as well, and it would ship in a future Xamarin package.

**New MAUI-only code belongs directly in `Maui.Skeleton/`.** It is picked up by the SDK's default
globbing, needs no `#if`, and never reaches the frozen project. Only touch the shared folder when a
change genuinely has to apply to both.

## Public API

Everything is attached properties on `View`, in the static `Skeleton` class.

| Property | Type | Effect |
| --- | --- | --- |
| `IsBusy` | bool | The only trigger. Its `propertyChanged` runs all the logic. |
| `IsParent` | bool | Marks a container so its children are left alone. |
| `Hide` | bool | Hides the control (`IsVisible = false`) instead of drawing a placeholder. |
| `BackgroundColor` | Color | Placeholder colour while loading. |
| `Animation` | BaseAnimation | Animation to run while loading. `null` means none. |

Internal attached properties (`Animating`, `CancelAnimation`, `OriginalBackgroundColor`,
`OriginalTextColor`, `UseDynamicTextColor`, `UseDynamicBackground`) hold the state needed to restore
the view, and live on the view rather than on the animation instance. That is why a single
`BaseAnimation` instance can safely be shared by several views.

### A worked example

Trimmed from `SkeletonSample/Pages/Fade.xaml`, the item template of the list:

```xml
<StackLayout Orientation="Horizontal" Spacing="20">

    <Frame WidthRequest="70" HeightRequest="70" CornerRadius="20"
           IsClippedToBounds="True"
           sk:Skeleton.IsParent="True"
           sk:Skeleton.IsBusy="{Binding IsBusy}"
           sk:Skeleton.BackgroundColor="{StaticResource GrayColor}">
        <Image Source="{Binding Image}" Aspect="AspectFill" />
    </Frame>

    <StackLayout Spacing="4">
        <Frame CornerRadius="5" Padding="0" BackgroundColor="Transparent"
               HorizontalOptions="Start"
               sk:Skeleton.IsBusy="{Binding IsBusy}"
               sk:Skeleton.BackgroundColor="{StaticResource GrayColor}">
            <Label Text="{Binding Title}" FontSize="20"
                   sk:Skeleton.IsBusy="{Binding IsBusy}"
                   sk:Skeleton.Hide="True" />
        </Frame>
    </StackLayout>

</StackLayout>
```

Read it carefully, because not everything in it is doing what it looks like it is doing:

- **The grey blocks come from `BackgroundColor` on the frames.** That is the actual placeholder.
- **`Hide="True"` on the label** is what removes the text. Without it you would see the real title
  on top of the grey.
- **`IsParent="True"` on the frame is decorative.** `Frame` is not a `Layout` in MAUI, so the branch
  it controls never runs. See the open items at the end.
- **The avatar is blank because the view model says so.** The image has no `Hide`; the loading items
  in `SkeletonViewModel` simply carry no `Image`, so the binding resolves to null. Supplying empty
  placeholder data is a legitimate technique, but it is the view model doing the work here, not
  this library.

Where `IsParent` genuinely matters is on a real layout. Given a `Grid` or a `StackLayout` with
`IsBusy` set and no `IsParent`, every child is pushed to `Opacity = 0` and the whole block becomes a
single blank area. Adding `IsParent="True"` leaves the children alone so each one can render its own
placeholder, which is what the sample wants everywhere.

The animation is attached per element:

```xml
sk:Skeleton.Animation="{sk:DefaultAnimation Source=Fade, Interval=600, Parameter=0.3}"
```

`Interval` is the duration in milliseconds of each half of a cycle and `Parameter` is what the
animation interpolates towards: opacity for `Fade`, scale for `Beat`, offset in units for the two
shakes. `{sk:DefaultAnimation None}` resolves to `null` and no animation runs at all.

The six pages under `SkeletonSample/Pages/` are the same layout with a different animation on each,
and `SkeletonSample/MyCustomAnimation.cs` shows a consumer-defined one.

### What happens when IsBusy flips

`HandleIsBusyChanged` in `Skeleton.cs`:

- `Hide` set: toggle `IsVisible` and stop there.
- Otherwise, on the way in: a `Layout` that is not `IsParent` gets `Opacity = 0` on its children; a
  `Label` or `Button` has its `TextColor` saved and set to transparent; the background colour is
  swapped; the animation starts.
- On the way out, the same steps in reverse.

### Theming

`ViewExtensions` walks the `Style` setters (and their `BasedOn` chain) looking for a `DynamicResource`.
When the original colour came from one, restoring re-applies `SetDynamicResource` instead of writing
back a literal value, so a theme switch during loading is not baked in.

## The animation engine

`BaseAnimation` is the extension point. Subclasses implement two methods:

```csharp
protected abstract Task<bool> Animate(BindableObject bindable);   // one cycle
protected abstract Task StopAnimation(BindableObject bindable);   // settle back to rest
```

`Run` loops `Animate` until `CancelAnimation` is set. Built-ins: `BeatAnimation` (`ScaleTo`),
`FadeAnimation` (`FadeTo`), `VerticalShakeAnimation` and `HorizontalShakeAnimation` (`TranslateTo`).

Three things about this loop are not obvious, and all three were bugs at some point:

**It must run on the UI thread.** `Start` and `Stop` go through `RunOnMainThread`. The loop used to be
launched with `Task.Run` onto a threadpool thread, which is not where either framework's animation
manager expects to be called from, and it also read and wrote bindable properties off the UI thread.

**It must not spin when the platform is not animating.** Users can turn animations off system wide
(Android developer options, OEM battery savers). Every `ScaleTo`/`FadeTo`/`TranslateTo` then completes
immediately, and an unguarded loop snaps the animated property between its two ends as fast as the UI
thread allows — which is what flickering looks like. Measured on a moto g54 with animations off,
before and after the guard:

| | Iterations | Interval |
| --- | --- | --- |
| Unguarded, on a threadpool thread | ~47 per view | 30 ms |
| Unguarded, on the UI thread | 37,846 | 0 ms |
| Guarded | 6 (5 bail-outs) | — |

The guard stops the loop when `Animate` returns in under one frame (16 ms) and leaves a static
placeholder, which is also what a user who turned animations off is asking for. Note the two fixes are
not independent: moving to the UI thread on its own made the spin *worse*, and is only safe because of
the guard.

**Time it with `Stopwatch`, not `DateTime`.** The wall clock is not monotonic; a backwards NTP jump
would make a real animation look instantaneous and trip the guard.

The loop also runs inside `try/finally`. If `Animate` throws and the `Animating` flag is left set,
`Skeleton.RunAnimation` skips that view forever after, so it would never animate again.

### Adding an animation

1. Create the class in `Maui.Skeleton/`, inheriting `BaseAnimation`.
2. Implement `Animate` (one cycle) and `StopAnimation` (settle immediately).
3. Keep a cycle meaningfully longer than 16 ms or the guard will treat it as "not animating".
4. To expose it through the markup extension, add it to the `AnimationTypes` enum and to the switch
   in `DefaultAnimationExtension`. `Source` is that extension's content property, so both forms work:
   `{sk:DefaultAnimation Fade}` and `{sk:DefaultAnimation Source=Fade, Interval=600, Parameter=0.3}`.
   Both files are shared, so an addition there is compiled into the Xamarin project too.
5. Add a page to `SkeletonSample/` to exercise it.

Consumers can also subclass `BaseAnimation` in their own app; `SkeletonSample/MyCustomAnimation.cs`
shows how.

## Building

`Maui.Skeleton` takes `Microsoft.Maui.Controls` as a plain `PackageReference` and does **not** set
`UseMaui`. `UseMaui=true` on a non-platform TFM stopped resolving MAUI's reference assemblies in
current SDKs — that is what made the old `net6.0` project impossible to build. This works because the
library has no platform-specific code at all: no `Platforms/` folder, no `#if ANDROID`, no handlers.

Two consequences worth knowing:

- **No MAUI workload is needed** to build or pack the library, which is why CI runs on Linux.
- **If native code is ever added**, this stops working and the project has to move to platform TFMs
  (`net10.0-android;net10.0-ios;...`) with `UseMaui` back on.

Each `Microsoft.Maui.Controls` reference is pinned to the **baseline** of its band (8.0.3 / 9.0.0 /
10.0.0). A `PackageReference` version is a floor, so pinning a later patch would force every consumer
onto that patch or newer.

Do not build the solution as a whole: it still contains the legacy Xamarin sample projects, which
current tooling cannot restore. Build `Maui.Skeleton/Maui.Skeleton.csproj` directly.

## Publishing

Package metadata lives in `Maui.Skeleton.csproj`; there is no `.nuspec` for the MAUI package.
`SuppressDependenciesWhenPacking` keeps the package declaring zero dependencies, matching what has
been published historically.

`PackageId` must never change. Download counts on nuget.org are tracked per id, and a rename starts
from zero.

`.github/workflows/publish-nuget.yml` builds, packs, and optionally publishes:

| Event | Result |
| --- | --- |
| Pull request | build and pack only |
| Push to `master` | publishes only when the `AUTO_PUBLISH` repository variable is `true` |
| Manual run | publishes only when the input box is ticked |

The package is uploaded as a build artifact on every run, so a dry run still lets you download and
inspect exactly what would have been published.

Authentication uses **NuGet Trusted Publishing**: no long-lived API key is stored. `NuGet/login`
exchanges a GitHub OIDC token for a key valid for one hour, which is why that step sits immediately
before the push. The policy on nuget.org is bound to the repository *and the workflow file name* —
renaming `publish-nuget.yml` breaks publishing. The only secret involved is `NUGET_USER`, the
nuget.org profile name.

Bumping the floor TFM is a breaking change for consumers below it and needs a major version, even
though the public API is untouched. That is why dropping `net6.0` produced 3.0.0.

## Open items

- **The README documents two properties that do not exist** — `AnimationInterval` and
  `AnimationParameter`. See [#42](https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/issues/42).
- **The Xamarin.Forms side of `RunOnMainThread` has never been executed.** It compiles, but the legacy
  sample cannot be built with current tooling, so `Device.BeginInvokeOnMainThread` is unverified.
- **Windows has never been built or run.** There is no platform-specific code, so it should work, but
  it is untested.
- **`Xamarin.Forms.Skeleton.nuspec` copies the assembly to `lib/netstandard1.0/`** while the project
  targets `netstandard2.0`. Harmless in practice and already published that way; only matters if that
  package is ever republished, which is not planned.
- **The shared sources produce nullable warnings** under the MAUI build, which has `Nullable` enabled
  while the code is not annotated.
- **`IsBusy` does not blank the content of `Border`, `Frame` or `ContentView`**, unlike on a `Layout`
  and unlike on Xamarin.Forms, because none of them derive from `Layout` in MAUI. Parts of the sample
  are misleading for this reason. See
  [#41](https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/issues/41).
- **The samples use `Frame` everywhere**, which is deprecated in MAUI in favour of `Border`.
