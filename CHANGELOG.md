# Changelog

All notable changes to Pile Poil are listed here.
Format: [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), versions follow [Semantic Versioning](https://semver.org/).

## Contents

- [Unreleased](#unreleased)
- [1.0.0](#100)

## Unreleased

## 1.0.0

First public release.

### Added

- Pixel-perfect centering of any window, using its visible bounds (invisible Windows 10/11 borders ignored).
- Window list with app icons, and a drag-and-drop picker with a highlight frame.
- Screen choice: the window's current screen or any other, with an *Identify* button.
- Area choice: work area (without the taskbar) or full screen.
- Per-monitor DPI handling, with re-centering when an app resizes itself on another screen.
- Real margins measured and shown after each move.
- Compact mode: window drop-down, picker and Center only; dropping the picker centers the window right away.
- English / French language button in the header.
- Settings remembered between launches (`%APPDATA%\PilePoil\settings.ini`): language, compact mode, always on top, area.
- Material 3 dark interface with Material Symbols icons, dark title bar and scrollbar.
- English and French; until you choose, the Windows display language is used (`--lang=en|fr` to force).
- `--demo` mode for screenshots, `--compact` to start in compact mode.
- Single-file build with the C# compiler built into Windows (`build.bat`), GitHub Actions build and release.
