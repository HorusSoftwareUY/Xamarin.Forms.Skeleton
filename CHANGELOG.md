# Changelog

Notable changes to this project, from 3.0.0 onwards. Earlier releases are not reconstructed here;
see the note at the end for where to find them.

This project ships two packages from one source tree. `HorusStudio.Maui.Skeleton` is the .NET MAUI
package and is where new work happens. `Xamarin.Forms.Skeleton` is frozen at 2.0.0 and is not
republished; it keeps working for apps that still need it.

## [3.0.0] - Unreleased

> Everything below is on `master` and packaged, but 3.0.0 is not on nuget.org yet. On the day it is
> published, swap **Unreleased** for that date and point the `[3.0.0]` link at the release tag.

The first release since MAUI support landed in 2023. It requires **.NET 8 or newer** — an app on
.NET 6 or 7 stays on 2.0.0, which keeps working.

### Added

- **Shimmer**, a band of light that sweeps across the placeholder and off the other side. ([#49])
- **Aurora**, a wide field of colour that drifts back and forth without leaving the element. ([#49])
- **Tint**, the whole placeholder washing to a colour and back with nothing moving — the quietest of
  the three, for loading states that should stay in the background. ([#49])
- `Direction` and `SweepColors` on `{sk:DefaultAnimation}`, shared by Shimmer and Aurora. Left alone,
  the sweep contrasts with the placeholder colour, so it works on light and dark without
  configuration. ([#49])
- An **agent skill**, shipped in the repository, so an assistant can apply the API correctly without
  the developer having to learn its traps first. ([#46])
- A NuGet publish pipeline using Trusted Publishing, and `docs/ARCHITECTURE.md` describing how the
  two packages share one source tree. ([#43])

### Changed

- **Multi-targets .NET 8, 9 and 10**, so each app gets a binary built against its own MAUI version.
  Packaging moved from a hand-written `.nuspec` into the project file. ([#40])
- **A view holding a single piece of content now fades that content while busy**, as every `Layout`
  already did, and `Skeleton.IsParent` works on it. The test is two conditions: a `View`
  implementing `IContentView`, whose content is itself a `View`. `Border`, `Frame`, `ContentView`,
  `ScrollView`, `RefreshView` and `SwipeView` are the usual ones. A templated control such as
  `RadioButton` qualifies too, but only when its content is a `View` rather than a string. This is
  the one change that can alter how an existing app looks: if content that used to stay visible now
  disappears, set `sk:Skeleton.IsParent="True"` on that container to get the old behaviour back.
  Children previously marked with `Skeleton.Hide` as a workaround keep working; the attribute is
  now redundant.
  ([#44], [#47])
- Animations are **driven from the UI thread**, which is where both frameworks expect to be called
  from. The loop had been running on a threadpool thread and only working by accident. ([#40])
- The README now explains what `IsBusy` and `IsParent` actually do, with worked examples — including
  that `IsBusy` is not inherited and that `IsParent="True"` means "leave my children alone". ([#52])
- The animation documentation now describes the API that exists, rather than properties that were
  never there. ([#48])

### Fixed

- **Rapid flickering on devices with system animations turned off.** `Animate` returned immediately
  and the loop snapped the animated property between its two ends as fast as the UI thread allowed.
  The skeleton now settles into a static placeholder instead. ([#40])
- **A failing animation left a view unable to animate again**, because the flag that guards re-entry
  was never cleared. ([#40])
- **Placeholders that were never removed.** An element with no `BackgroundColor` of its own kept the
  grey after loading finished: the restore wrote back the `null` saved on the way in, and writing
  `null` does not repaint. A `Border` wrapping content was the common case; it now returns to
  transparent. ([#51], closes [#50])
- **A `Label` losing its text permanently.** The same `null` went back to `TextColor`, so a `Label`
  that never declared one stayed transparent for the rest of the session. Its colour is now read off
  the native control, so it hides while loading and comes back afterwards, still following the
  platform's light and dark colours. ([#54], closes [#53])

### Known limitations

- A **`Button` that declares no `TextColor`** of its own is left readable while loading rather than
  hidden. Its native colour is one per state — normal, disabled, pressed — and restoring a single one
  would stop a disabled button looking disabled. Give it a `TextColor` to have its text hidden like
  any other control. Tracked in [#56].
- Restoring a colour that came from a **binding** puts the value back but not the binding, so the
  property stops tracking its source. A dynamic resource is handled and comes back intact; the
  `Opacity` a container restores to its children is not. Tracked in [#45].

### Still

- Zero dependencies.

## Earlier releases

2.0.0 and before are not covered here; this changelog starts with 3.0.0. Their contents are on
[nuget.org](https://www.nuget.org/packages/HorusStudio.Maui.Skeleton) and in the repository history.
2.0.0 added .NET MAUI support alongside the original Xamarin.Forms package.

[3.0.0]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/commits/master
[#40]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/40
[#43]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/43
[#44]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/44
[#45]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/issues/45
[#46]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/46
[#47]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/47
[#48]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/48
[#49]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/49
[#50]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/issues/50
[#51]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/51
[#52]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/52
[#53]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/issues/53
[#54]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/pull/54
[#56]: https://github.com/HorusSoftwareUY/Xamarin.Forms.Skeleton/issues/56
