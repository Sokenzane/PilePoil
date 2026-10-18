# Contributing to Pile Poil

Thanks for your interest! Pile Poil is a small project: one source file, no dependency. Keeping it that way is part of the goal.

## Contents

- [Report a bug or suggest an idea](#report-a-bug-or-suggest-an-idea)
- [Build](#build)
- [Code guidelines](#code-guidelines)
- [Add a language](#add-a-language)
- [Pull requests](#pull-requests)
- [Release (maintainer)](#release-maintainer)

## Report a bug or suggest an idea

Open an [issue](https://github.com/Sokenzane/PilePoil/issues/new/choose). For a bug, the margins shown in the status bar, your Windows version and your screen setup (count, resolution, scaling) help a lot.

## Build

Double-click `build.bat` (or run `build.bat ci` to skip the final pause). It compiles `src/PilePoil.cs` into `dist/PilePoil.exe` with the C# compiler that ships with .NET Framework 4, so there is nothing to install.

`dist\PilePoil.exe --demo` shows fake windows and screens, and `--lang=en|fr` forces the language.

## Code guidelines

- **C# 5 only.** The Windows built-in compiler (`csc.exe` v4) stops at C# 5. Do not use string interpolation (`$"…"`), `=>` members, `?.`, `nameof`, `out var` or auto-property initializers. `build.bat` will tell you if something slipped through.
- **No dependency, single file.** Everything stays in `src/PilePoil.cs`. Use WinForms, GDI+ and Win32 (P/Invoke) only.
- **Physical pixels for window maths.** Wrap Win32 calls that read or move other windows in `Native.BeginDpi()` / `Native.EndDpi()`.
- **Icons come from [Material Symbols](https://fonts.google.com/icons)** (Outlined, weight 400) to stay consistent: add the file name to `tools/make_icon_paths.py`, run it, and paste the line into `IconData`. Credit is already in `THIRD_PARTY_NOTICES.md`.
- **UI sizes go through `Theme.D()`** so the app scales with Windows' DPI setting.
- **No hard-coded UI text**: every string lives in `Lang` (see below).
- **Settings** go through the `Settings` class (`%APPDATA%\PilePoil\settings.ini`); keep it a small key=value file.
- Code and comments in English, 4-space indentation, CRLF line endings (handled by `.editorconfig` and `.gitattributes`).

## Add a language

1. In `src/PilePoil.cs`, find `static class Lang`.
2. Copy the `French()` method, rename it (e.g. `Spanish()`) and translate every string. Strings you leave out stay in English.
3. Call it from `Load()` for your two-letter language code (e.g. `"es"`).
4. Keep the `{0}`, `{1}`… placeholders and the arrows in `Margins`.
5. The header language button currently switches between EN and FR: make it cycle through the languages in `MainWindow` (`langButton.Click`).
6. Test with `dist\PilePoil.exe --demo --lang=es` and `--demo --compact --lang=es`. Check that nothing is cut off.
7. Add the language to the "Language" section of both READMEs.

## Pull requests

1. Fork, create a branch, make your change.
2. Check that `build.bat` compiles and test on Windows (ideally with two screens for screen-related changes).
3. Add a line to `CHANGELOG.md` under **Unreleased**.
4. Open the pull request and fill in the checklist.

By contributing, you agree that your contribution is licensed under the [GPL-3.0](LICENSE), like the rest of the project.

## Release (maintainer)

1. Update `Program.Version` and the two `AssemblyVersion` / `AssemblyFileVersion` lines in `src/PilePoil.cs`.
2. Move the **Unreleased** notes in `CHANGELOG.md` under the new version.
3. Commit, then tag: `git tag v1.1.0` and `git push --tags`.
4. GitHub Actions checks that the tag matches `Program.Version`, builds `PilePoil.exe` and publishes the release with the CHANGELOG notes and the exe's SHA-256. The README download link always points to the latest release.
