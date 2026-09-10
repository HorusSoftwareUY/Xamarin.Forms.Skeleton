---
name: skeleton
description: Add skeleton loading placeholders to .NET MAUI views using the HorusStudio.Maui.Skeleton package. Use this whenever someone wants a loading state, skeleton screen, shimmer effect, content placeholder, or "grey boxes while data loads" in a MAUI app, and whenever they mention Skeleton.IsBusy, Skeleton.IsParent, or the HorusStudio.Maui.Skeleton package by name. Also use it when someone asks to improve perceived loading time on a XAML page, or is replacing an ActivityIndicator or spinner with something better — the API has several traps that produce placeholders which look correct but leak the real content through, and this skill explains how to avoid them.
---

# Skeleton loading for .NET MAUI

`HorusStudio.Maui.Skeleton` turns controls a developer already has into grey placeholders while data
loads, instead of hiding the page behind a spinner. There are no new controls to adopt: you attach
properties to the existing XAML.

Work directly on the view the developer names. Do not introduce a base class, a wrapper control, or a
new view model — the point of this library is that neither is needed.

## Setup

The package is `HorusStudio.Maui.Skeleton`, version 3.0.0 or later, which requires .NET 8 or newer.
Apps on .NET 6 or 7 have to stay on 2.0.0.

Every XAML file that uses it needs this namespace:

```xml
xmlns:sk="clr-namespace:Maui.Skeleton;assembly=Maui.Skeleton"
```

The assembly is `Maui.Skeleton` even though the package is `HorusStudio.Maui.Skeleton`. Do not write
`clr-namespace:Xamarin.Forms.Skeleton` — that is the legacy Xamarin.Forms package and the type does
not exist in the MAUI assembly.

## The five properties

| Property | Type | What it does |
| --- | --- | --- |
| `Skeleton.IsBusy` | bool | The switch. Everything else only takes effect while this is true. |
| `Skeleton.BackgroundColor` | Color | The placeholder colour to paint while busy. |
| `Skeleton.Hide` | bool | Hide the element entirely while busy, instead of painting it. |
| `Skeleton.IsParent` | bool | On a container, stop it from fading out its own content. |
| `Skeleton.Animation` | BaseAnimation | Optional motion while busy. |

**`IsBusy` is not inherited.** Every element that should react needs its own
`sk:Skeleton.IsBusy="{Binding ...}"`. Setting it once on the page or on an outer layout does nothing
for the elements inside. This is the single most common mistake — a page where only the outer
container carries the binding renders no placeholders at all.

Bind `IsBusy` to the loading property the view model already exposes (`IsLoading`, `IsBusy`,
`IsRefreshing`, whatever it is called). Only add a new property if there genuinely is none, and say so.

## Deciding what each element gets

Go element by element through the view. The right treatment depends on what the element is.

**A container** — `Grid`, `StackLayout`, `VerticalStackLayout`, `FlexLayout`, `Border`, `Frame`,
`ContentView`, `ScrollView`. One property decides how it behaves, and the question it answers is whether the
container is *one shape* or *several shapes*:

```xml
<!-- one grey block: an avatar, an icon, a thumbnail -->
<Border sk:Skeleton.IsBusy="{Binding IsLoading}"
        sk:Skeleton.BackgroundColor="{StaticResource Gray200}">
    <Image Source="{Binding Avatar}" />
</Border>

<!-- several shapes: each child paints its own placeholder -->
<VerticalStackLayout sk:Skeleton.IsParent="True"
                     sk:Skeleton.IsBusy="{Binding IsLoading}">
    <Label ... />
    <Label ... />
</VerticalStackLayout>
```

Without `IsParent` the container fades its content out, so only its `BackgroundColor` remains — one
solid block. With `IsParent="True"` the content is left alone and every child has to declare its own
treatment. The container still paints its colour and runs its animation either way; `IsParent` only
governs whether it touches what is inside.

A container without `IsParent` **and** without `BackgroundColor` leaves an empty hole: the content
fades but nothing is painted in its place. If you leave `IsParent` off, give it a colour.

**A `Label` or a `Button`** paints its own placeholder — the library makes the text transparent, so
the background colour becomes the grey bar:

```xml
sk:Skeleton.IsBusy="{Binding IsLoading}"
sk:Skeleton.BackgroundColor="{StaticResource Gray200}"
```

Give it a `WidthRequest` if the placeholder would otherwise collapse to nothing. A label bound to a
string that is empty while loading has no width, so there is nothing to paint.

**Anything that should simply disappear** rather than turn grey — a chevron, a badge, a decorative
icon — takes `Hide` instead:

```xml
sk:Skeleton.IsBusy="{Binding IsLoading}"
sk:Skeleton.Hide="True"
```

`Hide` sets `IsVisible = false`, so the element also stops taking up space and the layout closes up
around it. Reach for it when the gap is better than a placeholder; inside a container that already
fades its content, it is redundant.

## Animations

Attach one per element with the markup extension:

```xml
sk:Skeleton.Animation="{sk:DefaultAnimation Source=Fade, Interval=600, Parameter=0.3}"
```

`Source` is the extension's content property, so `{sk:DefaultAnimation Fade}` is equally valid when
you want the defaults.

| Source | What moves | `Parameter` means | Default |
| --- | --- | --- | --- |
| `Fade` | opacity | opacity to fade to | 0.6 |
| `Beat` | scale | scale to grow to | 1.03 |
| `VerticalShake` | position | offset in units, up and down | 15 |
| `HorizontalShake` | position | offset in units, left and right | 10 |
| `None` | nothing | — | — |

`Interval` is the duration in milliseconds of **each half** of a cycle, so a `Fade` at 600 takes
1200 ms to go down and back. It defaults to 500.

Keep animations subtle. `Beat` at 1.03 and `Fade` at 0.6 are the shipped defaults for a reason —
a loading placeholder that pulses hard is more distracting than a spinner. Prefer `Fade`, and reach
for the shakes only when the developer asks for something livelier.

`{sk:DefaultAnimation None}` resolves to no animation at all, which is a reasonable default for large
surfaces like a header image where motion is more noticeable than helpful.

Animations are per element, and a group of placeholders animating in unison reads better than each
one drifting on its own timing. Use the same `Source` and `Interval` across a view unless there is a
reason not to.

## A complete example

A list row, before:

```xml
<Grid ColumnDefinitions="64,*,Auto" ColumnSpacing="16" Padding="16">
    <Border Grid.Column="0" WidthRequest="64" HeightRequest="64" StrokeShape="RoundRectangle 12">
        <Image Source="{Binding Avatar}" Aspect="AspectFill" />
    </Border>
    <VerticalStackLayout Grid.Column="1" Spacing="6" VerticalOptions="Center">
        <Label Text="{Binding Name}" FontSize="17" FontAttributes="Bold" />
        <Label Text="{Binding Role}" FontSize="14" TextColor="Gray" />
    </VerticalStackLayout>
    <Image Grid.Column="2" Source="chevron.png" WidthRequest="20" VerticalOptions="Center" />
</Grid>
```

After:

```xml
<Grid ColumnDefinitions="64,*,Auto" ColumnSpacing="16" Padding="16"
      sk:Skeleton.IsParent="True"
      sk:Skeleton.IsBusy="{Binding IsLoading}">

    <Border Grid.Column="0" WidthRequest="64" HeightRequest="64" StrokeShape="RoundRectangle 12"
            sk:Skeleton.IsBusy="{Binding IsLoading}"
            sk:Skeleton.BackgroundColor="{StaticResource Gray200}"
            sk:Skeleton.Animation="{sk:DefaultAnimation Source=Fade, Interval=600, Parameter=0.4}">
        <Image Source="{Binding Avatar}" Aspect="AspectFill" />
    </Border>

    <VerticalStackLayout Grid.Column="1" Spacing="6" VerticalOptions="Center"
                         sk:Skeleton.IsParent="True"
                         sk:Skeleton.IsBusy="{Binding IsLoading}">
        <Label Text="{Binding Name}" FontSize="17" FontAttributes="Bold" WidthRequest="160"
               sk:Skeleton.IsBusy="{Binding IsLoading}"
               sk:Skeleton.BackgroundColor="{StaticResource Gray200}"
               sk:Skeleton.Animation="{sk:DefaultAnimation Source=Fade, Interval=600, Parameter=0.4}" />
        <Label Text="{Binding Role}" FontSize="14" TextColor="Gray" WidthRequest="100"
               sk:Skeleton.IsBusy="{Binding IsLoading}"
               sk:Skeleton.BackgroundColor="{StaticResource Gray200}"
               sk:Skeleton.Animation="{sk:DefaultAnimation Source=Fade, Interval=600, Parameter=0.4}" />
    </VerticalStackLayout>

    <Image Grid.Column="2" Source="chevron.png" WidthRequest="20" VerticalOptions="Center"
           sk:Skeleton.IsBusy="{Binding IsLoading}"
           sk:Skeleton.Hide="True" />
</Grid>
```

Note what did not change: no element was replaced, no layout was restructured, and the bindings to
real data stayed exactly as they were. The avatar needs nothing of its own — the `Border` around it
has no `IsParent`, so it fades its content and leaves a grey square. The chevron uses `Hide` because
an empty gap suits it better than a grey block. The two labels gained a `WidthRequest` so their
placeholders have a shape while the text is empty.

## Custom animations

Inherit `BaseAnimation` and implement one cycle plus how to settle back:

```csharp
using Maui.Skeleton.Animations;

public sealed class PulseAnimation : BaseAnimation
{
    protected override async Task<bool> Animate(BindableObject bindable)
    {
        if (bindable is not View view) return false;
        await view.FadeTo(0.4, Interval);
        await view.FadeTo(1, Interval);
        return true;
    }

    protected override async Task StopAnimation(BindableObject bindable)
    {
        if (bindable is View view) await view.FadeTo(1, 1);
    }
}
```

Keep a cycle meaningfully longer than one frame. The library stops the loop when `Animate` returns in
under 16 ms, because that means the platform is not animating at all — users can turn animations off
system-wide, and without that guard the loop would spin and flicker. A cycle that finishes in a couple
of milliseconds will be treated as "not animating" and stopped.

Attach it by binding a property that returns an instance:

```xml
sk:Skeleton.Animation="{Binding MyAnimation}"
```

## Things that look right but are not

- **Binding `IsBusy` only on the outer container.** It does not propagate. Every element needs it.
- **Marking every child with `Hide` inside a container that has no `IsParent`.** The container
  already fades its content; the extra attributes are noise.
- **A `Label` placeholder with no width.** While loading the text is usually empty, so there is
  nothing to paint. Add a `WidthRequest`.
- **`clr-namespace:Xamarin.Forms.Skeleton`.** Wrong package for MAUI.
- **Adding a new `IsLoading` property when the view model already has one.** Wire up what exists.
- **Wrapping everything in new `Border` elements to get grey blocks.** `Label` and `Button` paint
  their own; only reach for a container when the element cannot paint itself, like an `Image`.

## After editing

Say which elements were changed and what each one does while loading, and point out anything that
needs the developer's judgement — a colour that should come from their theme, a placeholder width
that was guessed, or a view model property that had to be added.
