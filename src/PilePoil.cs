// Pile Poil - Center any window, pixel-perfect, on the screen of your choice.
// Copyright (C) 2026 Sokenzane
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Single-file WinForms app in C# 5, built with the csc.exe shipped with Windows (build.bat).
// Options: --lang=en|fr, --compact, --demo (fake windows and screens, settings not saved).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

[assembly: AssemblyTitle("Pile Poil")]
[assembly: AssemblyDescription("Center any window, pixel-perfect, on the screen of your choice")]
[assembly: AssemblyProduct("Pile Poil")]
[assembly: AssemblyCopyright("Copyright (C) 2026 Sokenzane - GPL-3.0")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace PilePoil
{
    static class Program
    {
        public const string Version = "1.0.0";
        public static bool Demo;

        [DllImport("user32.dll")] static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main(string[] args)
        {
            string lang = null;
            bool forceCompact = false;
            foreach (string a in args)
            {
                if (a == "--demo") Demo = true;
                if (a == "--compact") forceCompact = true;
                if (a.StartsWith("--lang=")) lang = a.Substring(7);
            }
            Settings.Load();
            if (lang != null) Settings.Language = lang; // --lang wins over the saved choice
            if (forceCompact) Settings.Compact = true;
            Lang.Load(Settings.EffectiveLanguage);

            try { SetProcessDPIAware(); } catch (Exception) { }
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero)) Theme.Scale = g.DpiX / 96f;
            // Readable message instead of the .NET crash dialog
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += delegate (object s, System.Threading.ThreadExceptionEventArgs e)
            {
                MessageBox.Show(Lang.UnexpectedError + "\n\n" + e.Exception.GetType().Name + " : " + e.Exception.Message,
                    "Pile Poil " + Version, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }

    // ------------------------------------------------------------------------
    //  UI strings (to add a language, see CONTRIBUTING.md)
    // ------------------------------------------------------------------------
    static class Lang
    {
        public static string Code;

        public static string Tagline, OpenWindows, ScreenSection, AreaSection;
        public static string Picker, Refresh, Identify, Center;
        public static string WorkArea, FullScreen, CurrentScreen, ScreenName, Primary, Minimized, Maximized;
        public static string ChooseWindow, TipPin, TipCompact, TipFull, TipLanguage, TipPicker;
        public static string Ready, ReadyCompact, Found, NoSelection, Hover, NoTarget, Selected;
        public static string WindowGone, IsMaximized, AccessDenied, MoveFailed, Margins;
        public static string ScreenRefused, TooTall, NotExact, Centered, CenteredOdd, UnexpectedError;

        public static void Load(string code)
        {
            English();
            if ((code ?? "").ToLowerInvariant().StartsWith("fr")) French();
        }

        static void English()
        {
            Code = "en";
            Tagline = "Center your windows, pixel-perfect";
            OpenWindows = "Open windows";
            ScreenSection = "Screen";
            AreaSection = "Area";
            Picker = "Picker";
            Refresh = "Refresh";
            Identify = "Identify";
            Center = "Center";
            WorkArea = "Work area";
            FullScreen = "Full screen";
            CurrentScreen = "Window's screen";
            ScreenName = "Screen {0}";
            Primary = "primary";
            Minimized = "minimized";
            Maximized = "maximized";
            ChooseWindow = "Choose a window…";
            TipPin = "Always on top";
            TipCompact = "Compact mode";
            TipFull = "Full mode";
            TipLanguage = "Passer en français";
            TipPicker = "Picker: hold, then drop on a window to center it";

            Ready = "Ready. Pick a window from the list, or hold the picker and drop it on the window to center.";
            ReadyCompact = "Drop the picker on a window to center it in one go, or choose it from the list.";
            Found = "{0} window(s) and {1} screen(s) found.";
            NoSelection = "Pick a window first, from the list or with the picker.";
            Hover = "Hover over the window to center, then release…";
            NoTarget = "No window targeted.";
            Selected = "Selected: {0} · {1}. Click Center.";

            WindowGone = "This window no longer exists. Click Refresh.";
            IsMaximized = "This window is maximized: it already fills the screen. Restore it first.";
            AccessDenied = "Access denied: this app runs as administrator. Restart Pile Poil as administrator.";
            MoveFailed = "Could not move the window (Windows error {0}).";
            Margins = "margins ← {0} | {1} → px, ↑ {2} | {3} ↓ px";
            ScreenRefused = "The app refused to change screens and moved back.";
            TooTall = "Centered horizontally on {0}. Too tall for the screen: aligned to the top to keep the title bar visible. {1}";
            NotExact = "The app adjusted its own position, so the result is not exact. {0}";
            Centered = "Pixel-perfectly centered on {0} · {1}";
            CenteredOdd = "Centered on {0}, pixel-perfect (1 px off: odd size) · {1}";
            UnexpectedError = "Unexpected error. Pile Poil keeps running; if it happens again, please report it on github.com/Sokenzane/PilePoil/issues with the details below.";
        }

        static void French()
        {
            Code = "fr";
            Tagline = "Centre tes fenêtres au pixel près";
            OpenWindows = "Fenêtres ouvertes";
            ScreenSection = "Écran";
            AreaSection = "Zone";
            Picker = "Pipette";
            Refresh = "Actualiser";
            Identify = "Identifier";
            Center = "Centrer";
            WorkArea = "Zone utile";
            FullScreen = "Écran entier";
            CurrentScreen = "Écran de la fenêtre";
            ScreenName = "Écran {0}";
            Primary = "principal";
            Minimized = "réduite";
            Maximized = "maximisée";
            ChooseWindow = "Choisis une fenêtre…";
            TipPin = "Toujours au premier plan";
            TipCompact = "Mode compact";
            TipFull = "Mode complet";
            TipLanguage = "Switch to English";
            TipPicker = "Pipette : maintiens, puis lâche sur une fenêtre pour la centrer";

            Ready = "Prêt. Choisis une fenêtre, ou maintiens la pipette et lâche-la sur la fenêtre à centrer.";
            ReadyCompact = "Lâche la pipette sur une fenêtre pour la centrer d'un coup, ou choisis-la dans la liste.";
            Found = "{0} fenêtre(s) et {1} écran(s) trouvés.";
            NoSelection = "Choisis d'abord une fenêtre dans la liste, ou avec la pipette.";
            Hover = "Survole la fenêtre à centrer, puis relâche le clic…";
            NoTarget = "Aucune fenêtre visée.";
            Selected = "Fenêtre choisie : {0} · {1}. Clique sur Centrer.";

            WindowGone = "Cette fenêtre n'existe plus. Clique sur Actualiser.";
            IsMaximized = "Cette fenêtre est maximisée : elle remplit déjà l'écran. Restaure-la d'abord.";
            AccessDenied = "Accès refusé : cette application tourne en administrateur. Relance Pile Poil en administrateur.";
            MoveFailed = "Impossible de déplacer la fenêtre (erreur Windows {0}).";
            Margins = "marges ← {0} | {1} → px, ↑ {2} | {3} ↓ px";
            ScreenRefused = "L'application a refusé de changer d'écran, elle est revenue à sa place.";
            TooTall = "Centrée horizontalement sur {0}. Trop haute pour l'écran : collée en haut pour garder la barre de titre. {1}";
            NotExact = "L'application a corrigé sa position elle-même, le centrage n'est pas exact. {0}";
            Centered = "Centrée au pixel près sur {0} · {1}";
            CenteredOdd = "Centrée au pixel près sur {0} (1 px d'écart : taille impaire) · {1}";
            UnexpectedError = "Erreur inattendue. Pile Poil continue de fonctionner ; si ça se reproduit, signale-le sur github.com/Sokenzane/PilePoil/issues avec le détail ci-dessous.";
        }
    }

    // ------------------------------------------------------------------------
    //  Settings, saved as key=value lines in %APPDATA%\PilePoil\settings.ini
    // ------------------------------------------------------------------------
    static class Settings
    {
        public static string Language = "system"; // "system", "en" or "fr"
        public static bool Compact;
        public static bool AlwaysOnTop = true;
        public static bool FullScreenArea;

        static string FilePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                    "PilePoil", "settings.ini");
            }
        }

        public static void Load()
        {
            if (Program.Demo) return;
            try
            {
                if (!File.Exists(FilePath)) return;
                foreach (string line in File.ReadAllLines(FilePath))
                {
                    int eq = line.IndexOf('=');
                    if (eq < 0) continue;
                    string key = line.Substring(0, eq).Trim(), value = line.Substring(eq + 1).Trim();
                    if (key == "language") Language = value;
                    else if (key == "compact") Compact = value == "1";
                    else if (key == "alwaysOnTop") AlwaysOnTop = value == "1";
                    else if (key == "fullScreenArea") FullScreenArea = value == "1";
                }
            }
            catch (Exception) { } // unreadable: keep the defaults
        }

        public static void Save()
        {
            if (Program.Demo) return;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllLines(FilePath, new string[] {
                    "language=" + Language,
                    "compact=" + (Compact ? "1" : "0"),
                    "alwaysOnTop=" + (AlwaysOnTop ? "1" : "0"),
                    "fullScreenArea=" + (FullScreenArea ? "1" : "0") });
            }
            catch (Exception) { } // read-only profile: not saved
        }

        public static string EffectiveLanguage
        {
            get { return Language == "system" ? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName : Language; }
        }
    }

    // ------------------------------------------------------------------------
    //  Material 3 dark theme (baseline purple palette)
    // ------------------------------------------------------------------------
    static class Theme
    {
        public static float Scale = 1f;

        public static readonly Color Surface = Hex("#141218");
        public static readonly Color Container = Hex("#211F26");
        public static readonly Color ContainerHigh = Hex("#2B2930");
        public static readonly Color ContainerHighest = Hex("#36343B");
        public static readonly Color OnSurface = Hex("#E6E0E9");
        public static readonly Color OnSurfaceVariant = Hex("#CAC4D0");
        public static readonly Color Outline = Hex("#938F99");
        public static readonly Color OutlineVariant = Hex("#49454F");
        public static readonly Color Primary = Hex("#D0BCFF");
        public static readonly Color OnPrimary = Hex("#381E72");
        public static readonly Color PrimaryContainer = Hex("#4F378B");
        public static readonly Color OnPrimaryContainer = Hex("#EADDFF");
        public static readonly Color SecondaryContainer = Hex("#4A4458");
        public static readonly Color OnSecondaryContainer = Hex("#E8DEF8");
        public static readonly Color Error = Hex("#F2B8B5");
        public static readonly Color Success = Hex("#A6D9A0");

        public static Color Hex(string h) { return ColorTranslator.FromHtml(h); }

        // 96-dpi pixels to screen pixels
        public static int D(float v) { return (int)Math.Round(v * Scale); }

        static readonly Dictionary<string, Font> fonts = new Dictionary<string, Font>();
        public static Font Font(float px, bool bold)
        {
            string key = px + (bold ? "b" : "r");
            Font f;
            if (!fonts.TryGetValue(key, out f))
            {
                f = new Font(bold ? "Segoe UI Semibold" : "Segoe UI", px * Scale, FontStyle.Regular, GraphicsUnit.Pixel);
                fonts[key] = f;
            }
            return f;
        }

        // Material state layer: hover 8 %, pressed 12 %
        public static Color State(Color c, bool hover, bool pressed)
        {
            return Color.FromArgb(pressed ? 31 : hover ? 20 : 0, c);
        }
    }

    public enum Glyph { Picker, Refresh, Center, Screen, Check, Error, Info, PinOff, PinOn, Compact, Expand, ChevronDown, Language }

    // ------------------------------------------------------------------------
    //  Drawing helpers: shapes, text, icons
    // ------------------------------------------------------------------------
    static class Draw
    {
        const TextFormatFlags Base = TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;

        public static void Quality(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        public static GraphicsPath Rounded(RectangleF r, float radius)
        {
            GraphicsPath p = new GraphicsPath();
            float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            if (d <= 0) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static void Fill(Graphics g, RectangleF r, float radius, Color c)
        {
            if (c.A == 0) return;
            using (GraphicsPath p = Rounded(r, radius))
            using (SolidBrush b = new SolidBrush(c))
                g.FillPath(b, p);
        }

        public static void Stroke(Graphics g, RectangleF r, float radius, Color c, float width)
        {
            using (GraphicsPath p = Rounded(r, radius))
            using (Pen pen = new Pen(c, width))
                g.DrawPath(pen, p);
        }

        // Width of a label, measured with the exact flags used to draw it
        public static int Measure(string t, Font f)
        {
            return TextRenderer.MeasureText(t, f, new Size(int.MaxValue, int.MaxValue), Base).Width + 1;
        }

        // Short label (buttons, chips...): never truncated
        public static void Label(Graphics g, string t, Font f, int x, int height, Color c)
        {
            TextRenderer.DrawText(g, t, f, new Rectangle(x, 0, Measure(t, f) + Theme.D(4), height), c,
                Base | TextFormatFlags.VerticalCenter | TextFormatFlags.NoClipping);
        }

        // Text in a box: shortened with "…" if it does not fit
        public static void Text(Graphics g, string t, Font f, Rectangle r, Color c, TextFormatFlags extra)
        {
            TextRenderer.DrawText(g, t, f, r, c, Base | TextFormatFlags.EndEllipsis | extra);
        }

        // Material Symbols by Google (Apache-2.0, see THIRD_PARTY_NOTICES.md),
        // as 24x24 M/L/C/Z paths generated by tools/make_icon_paths.py
        static readonly Dictionary<Glyph, string> IconData = new Dictionary<Glyph, string>
        {
            { Glyph.Picker, "M3 21L3 16.8L12.425 7.375L10.625 5.575L11.65 4.55L13.95 6.85L17.5 3.3C17.583 3.217 17.675 3.15 17.775 3.1C17.875 3.05 17.975 3.025 18.075 3.025C18.175 3.025 18.275 3.05 18.375 3.1C18.475 3.15 18.575 3.217 18.675 3.3L20.7 5.325C20.783 5.425 20.85 5.525 20.9 5.625C20.95 5.725 20.975 5.825 20.975 5.925C20.975 6.025 20.95 6.125 20.9 6.225C20.85 6.325 20.783 6.417 20.7 6.5L17.15 10.05L19.45 12.35L18.425 13.375L16.625 11.575L7.2 21L3 21ZM4.5 19.5L6.675 19.5L15.55 10.625L13.375 8.45L4.5 17.325L4.5 19.5ZM16.075 9L19.175 5.9L18.1 4.825L15 7.925L16.075 9ZM16.075 9L15 7.925L16.075 9Z" }, // colorize
            { Glyph.Refresh, "M12 20C9.783 20 7.896 19.221 6.338 17.663C4.779 16.104 4 14.217 4 12C4 9.783 4.779 7.896 6.338 6.338C7.896 4.779 9.783 4 12 4C13.417 4 14.658 4.287 15.725 4.862C16.792 5.438 17.717 6.225 18.5 7.225L18.5 4L20 4L20 10.35L13.65 10.35L13.65 8.85L17.85 8.85C17.217 7.85 16.408 7.042 15.425 6.425C14.442 5.808 13.3 5.5 12 5.5C10.183 5.5 8.646 6.129 7.388 7.388C6.129 8.646 5.5 10.183 5.5 12C5.5 13.817 6.129 15.354 7.388 16.613C8.646 17.871 10.183 18.5 12 18.5C13.383 18.5 14.65 18.104 15.8 17.312C16.95 16.521 17.75 15.475 18.2 14.175L19.75 14.175C19.267 15.925 18.308 17.333 16.875 18.4C15.442 19.467 13.817 20 12 20Z" }, // refresh
            { Glyph.Center, "M10.137 13.863C9.646 13.371 9.4 12.75 9.4 12C9.4 11.25 9.646 10.629 10.137 10.137C10.629 9.646 11.25 9.4 12 9.4C12.75 9.4 13.371 9.646 13.863 10.137C14.354 10.629 14.6 11.25 14.6 12C14.6 12.75 14.354 13.371 13.863 13.863C13.371 14.354 12.75 14.6 12 14.6C11.25 14.6 10.629 14.354 10.137 13.863ZM12.775 12.775C12.992 12.558 13.1 12.3 13.1 12C13.1 11.7 12.992 11.442 12.775 11.225C12.558 11.008 12.3 10.9 12 10.9C11.7 10.9 11.442 11.008 11.225 11.225C11.008 11.442 10.9 11.7 10.9 12C10.9 12.3 11.008 12.558 11.225 12.775C11.442 12.992 11.7 13.1 12 13.1C12.3 13.1 12.558 12.992 12.775 12.775ZM4.5 21C4.1 21 3.75 20.85 3.45 20.55C3.15 20.25 3 19.9 3 19.5L3 15.2L4.5 15.2L4.5 19.5L8.8 19.5L8.8 21L4.5 21ZM15.2 21L15.2 19.5L19.5 19.5L19.5 15.2L21 15.2L21 19.5C21 19.9 20.85 20.25 20.55 20.55C20.25 20.85 19.9 21 19.5 21L15.2 21ZM3 8.8L3 4.5C3 4.1 3.15 3.75 3.45 3.45C3.75 3.15 4.1 3 4.5 3L8.8 3L8.8 4.5L4.5 4.5L4.5 8.8L3 8.8ZM19.5 8.8L19.5 4.5L15.2 4.5L15.2 3L19.5 3C19.9 3 20.25 3.15 20.55 3.45C20.85 3.75 21 4.1 21 4.5L21 8.8L19.5 8.8Z" }, // filter_center_focus
            { Glyph.Screen, "M8.35 21L8.35 19.5L10.5 19.5L10.5 17L3.5 17C3.1 17 2.75 16.85 2.45 16.55C2.15 16.25 2 15.9 2 15.5L2 4.5C2 4.1 2.15 3.75 2.45 3.45C2.75 3.15 3.1 3 3.5 3L20.5 3C20.9 3 21.25 3.15 21.55 3.45C21.85 3.75 22 4.1 22 4.5L22 15.5C22 15.9 21.85 16.25 21.55 16.55C21.25 16.85 20.9 17 20.5 17L13.5 17L13.5 19.5L15.65 19.5L15.65 21L8.35 21ZM3.5 15.5L20.5 15.5L20.5 4.5L3.5 4.5L3.5 15.5ZM3.5 15.5L3.5 4.5L3.5 15.5Z" }, // desktop_windows
            { Glyph.Check, "M9.45 17.85L3.85 12.25L4.925 11.175L9.45 15.7L19.05 6.1L20.125 7.175L9.45 17.85Z" }, // check
            { Glyph.Error, "M12.588 16.763C12.746 16.605 12.825 16.409 12.825 16.175C12.825 15.942 12.746 15.746 12.588 15.588C12.43 15.429 12.234 15.35 12 15.35C11.767 15.35 11.571 15.429 11.412 15.587C11.254 15.745 11.175 15.941 11.175 16.175C11.175 16.408 11.254 16.604 11.412 16.762C11.57 16.921 11.766 17 12 17C12.233 17 12.429 16.921 12.588 16.763ZM11.325 13.175L12.825 13.175L12.825 6.85L11.325 6.85L11.325 13.175ZM12.007 22C10.628 22 9.332 21.738 8.119 21.212C6.906 20.688 5.846 19.971 4.938 19.062C4.029 18.154 3.312 17.093 2.788 15.879C2.263 14.665 2 13.368 2 11.988C2 10.607 2.263 9.31 2.788 8.096C3.312 6.882 4.029 5.825 4.938 4.925C5.846 4.025 6.907 3.312 8.121 2.788C9.335 2.263 10.632 2 12.012 2C13.393 2 14.69 2.263 15.904 2.788C17.118 3.312 18.175 4.025 19.075 4.925C19.975 5.825 20.688 6.883 21.212 8.1C21.738 9.317 22 10.614 22 11.993C22 13.372 21.738 14.668 21.212 15.881C20.688 17.094 19.975 18.153 19.075 19.058C18.175 19.963 17.117 20.68 15.9 21.208C14.683 21.736 13.386 22 12.007 22ZM12.012 20.5C14.371 20.5 16.375 19.671 18.025 18.012C19.675 16.354 20.5 14.346 20.5 11.988C20.5 9.629 19.677 7.625 18.03 5.975C16.383 4.325 14.373 3.5 12 3.5C9.65 3.5 7.646 4.323 5.987 5.97C4.329 7.617 3.5 9.627 3.5 12C3.5 14.35 4.329 16.354 5.987 18.012C7.646 19.671 9.654 20.5 12.012 20.5ZM12 12Z" }, // error
            { Glyph.Info, "M11.325 17L12.825 17L12.825 11L11.325 11L11.325 17ZM12.588 8.92C12.746 8.767 12.825 8.577 12.825 8.35C12.825 8.109 12.746 7.907 12.588 7.744C12.43 7.582 12.234 7.5 12 7.5C11.767 7.5 11.571 7.582 11.412 7.744C11.254 7.907 11.175 8.109 11.175 8.35C11.175 8.577 11.254 8.767 11.412 8.92C11.57 9.073 11.766 9.15 12 9.15C12.233 9.15 12.429 9.073 12.588 8.92ZM12.007 22C10.628 22 9.332 21.738 8.119 21.212C6.906 20.688 5.846 19.971 4.938 19.062C4.029 18.154 3.312 17.093 2.788 15.879C2.263 14.665 2 13.368 2 11.988C2 10.607 2.263 9.31 2.788 8.096C3.312 6.882 4.029 5.825 4.938 4.925C5.846 4.025 6.907 3.312 8.121 2.788C9.335 2.263 10.632 2 12.012 2C13.393 2 14.69 2.263 15.904 2.788C17.118 3.312 18.175 4.025 19.075 4.925C19.975 5.825 20.688 6.883 21.212 8.1C21.738 9.317 22 10.614 22 11.993C22 13.372 21.738 14.668 21.212 15.881C20.688 17.094 19.975 18.153 19.075 19.058C18.175 19.963 17.117 20.68 15.9 21.208C14.683 21.736 13.386 22 12.007 22ZM12.012 20.5C14.371 20.5 16.375 19.671 18.025 18.012C19.675 16.354 20.5 14.346 20.5 11.988C20.5 9.629 19.677 7.625 18.03 5.975C16.383 4.325 14.373 3.5 12 3.5C9.65 3.5 7.646 4.323 5.987 5.97C4.329 7.617 3.5 9.627 3.5 12C3.5 14.35 4.329 16.354 5.987 18.012C7.646 19.671 9.654 20.5 12.012 20.5ZM12 12Z" }, // info
            { Glyph.PinOff, "M15.85 12.8L18 14.725L18 16.225L12.75 16.225L12.75 22.25L12 23L11.25 22.25L11.25 16.225L6 16.225L6 14.725L8 12.8L8 4.5L6.75 4.5L6.75 3L17.1 3L17.1 4.5L15.85 4.5L15.85 12.8ZM8.025 14.725L15.825 14.725L14.35 13.35L14.35 4.5L9.5 4.5L9.5 13.35L8.025 14.725ZM11.925 14.725Z" }, // keep
            { Glyph.PinOn, "M15.85 12.8L18 14.725L18 16.225L12.75 16.225L12.75 22.25L12 23L11.25 22.25L11.25 16.225L6 16.225L6 14.725L8 12.8L8 4.475L6.75 4.475L6.75 2.975L17.1 2.975L17.1 4.475L15.85 4.475L15.85 12.8Z" }, // keep-fill
            { Glyph.Compact, "M3.05 22L2 20.95L9.45 13.5L4 13.5L4 12L12 12L12 20L10.5 20L10.5 14.55L3.05 22ZM12 12L12 4L13.5 4L13.5 9.45L20.95 2L22 3.05L14.55 10.5L20 10.5L20 12L12 12Z" }, // close_fullscreen
            { Glyph.Expand, "M3 21L3 13.5L4.5 13.5L4.5 18.45L18.45 4.5L13.5 4.5L13.5 3L21 3L21 10.5L19.5 10.5L19.5 5.55L5.55 19.5L10.5 19.5L10.5 21L3 21Z" }, // open_in_full
            { Glyph.ChevronDown, "M12 15.4L6 9.4L7.075 8.325L12 13.25L16.925 8.325L18 9.4L12 15.4Z" }, // keyboard_arrow_down
            { Glyph.Language, "M11.875 22L16.5 10L18.475 10L23.1 22L21.475 22L20.35 19.075L14.6 19.075L13.475 22L11.875 22ZM4.05 18.975L3 17.925L8.025 12.9C7.175 12.017 6.463 11.121 5.888 10.213C5.312 9.304 4.875 8.4 4.575 7.5L6.2 7.5C6.467 8.217 6.829 8.925 7.287 9.625C7.746 10.325 8.35 11.058 9.1 11.825C9.867 11.025 10.575 10.046 11.225 8.887C11.875 7.729 12.342 6.6 12.625 5.5L1 5.5L1 4L8.25 4L8.25 2L9.75 2L9.75 4L17 4L17 5.5L14.175 5.5C13.892 6.8 13.379 8.129 12.637 9.488C11.896 10.846 11.067 11.992 10.15 12.925L12.7 15.525L12.1 17.1L9.075 13.975L4.05 18.975ZM15.125 17.7L19.825 17.7L17.475 11.5L15.125 17.7Z" }, // translate
        };

        static readonly Dictionary<Glyph, GraphicsPath> iconPaths = new Dictionary<Glyph, GraphicsPath>();

        // Parser for the M/L/C/Z subset used above
        static GraphicsPath ParseIcon(string data)
        {
            GraphicsPath path = new GraphicsPath(FillMode.Winding);
            string[] tokens = System.Text.RegularExpressions.Regex.Split(data.Replace("M", " M ").Replace("L", " L ").Replace("C", " C ").Replace("Z", " Z "), @"[\s,]+");
            List<float> n = new List<float>();
            char cmd = ' ';
            PointF cur = PointF.Empty, start = PointF.Empty;
            foreach (string t in tokens)
            {
                if (t.Length == 0) continue;
                if (char.IsLetter(t[0]))
                {
                    cmd = t[0];
                    n.Clear();
                    if (cmd == 'M') path.StartFigure();
                    if (cmd == 'Z') { path.CloseFigure(); cur = start; }
                    continue;
                }
                n.Add(float.Parse(t, CultureInfo.InvariantCulture));
                if (cmd == 'M' && n.Count == 2) { cur = start = new PointF(n[0], n[1]); n.Clear(); cmd = 'L'; }
                else if (cmd == 'L' && n.Count == 2) { PointF p = new PointF(n[0], n[1]); path.AddLine(cur, p); cur = p; n.Clear(); }
                else if (cmd == 'C' && n.Count == 6)
                {
                    PointF p = new PointF(n[4], n[5]);
                    path.AddBezier(cur, new PointF(n[0], n[1]), new PointF(n[2], n[3]), p);
                    cur = p;
                    n.Clear();
                }
            }
            return path;
        }

        public static void Icon(Graphics g, Glyph glyph, RectangleF r, Color c)
        {
            GraphicsPath p;
            if (!iconPaths.TryGetValue(glyph, out p))
            {
                p = ParseIcon(IconData[glyph]);
                iconPaths[glyph] = p;
            }
            GraphicsState state = g.Save();
            g.TranslateTransform(r.X, r.Y);
            g.ScaleTransform(r.Width / 24f, r.Height / 24f);
            using (SolidBrush b = new SolidBrush(c)) g.FillPath(b, p);
            g.Restore(state);
        }
    }

    // ------------------------------------------------------------------------
    //  Data
    // ------------------------------------------------------------------------
    public class WindowInfo
    {
        public IntPtr Handle;
        public string Process;
        public string Title;
        public int Width;
        public int Height;
        public string Screen;
        public bool Maximized;
        public bool Minimized;
        public Bitmap Icon;    // app icon, or null

        public WindowInfo WithIcon(Bitmap icon) { Icon = icon; return this; }
    }

    public class ScreenInfo
    {
        public IntPtr Handle;
        public int Number;     // Windows display number (Screen 1, Screen 2...)
        public string Name;
        public int Width;
        public int Height;
        public int ScalePercent;
        public bool Primary;
    }

    // ------------------------------------------------------------------------
    //  Win32: windows, screens, DPI, moving
    // ------------------------------------------------------------------------
    public static class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT { public int Left, Top, Right, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int X, Y; }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        delegate bool EnumWindowsProc(IntPtr h, IntPtr l);
        delegate bool MonitorEnumProc(IntPtr m, IntPtr dc, ref RECT r, IntPtr data);

        [DllImport("user32.dll")] static extern bool EnumWindows(EnumWindowsProc cb, IntPtr l);
        [DllImport("user32.dll")] static extern bool EnumDisplayMonitors(IntPtr dc, IntPtr clip, MonitorEnumProc cb, IntPtr data);
        [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr h);
        [DllImport("user32.dll")] static extern bool IsIconic(IntPtr h);
        [DllImport("user32.dll")] static extern bool IsZoomed(IntPtr h);
        [DllImport("user32.dll")] static extern bool IsWindow(IntPtr h);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
        [DllImport("user32.dll")] static extern int GetWindowTextLength(IntPtr h);
        [DllImport("user32.dll")] static extern IntPtr GetWindow(IntPtr h, uint cmd);
        [DllImport("user32.dll")] static extern int GetWindowLong(IntPtr h, int index);
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
        [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr h, out RECT r);
        [DllImport("dwmapi.dll")] static extern int DwmGetWindowAttribute(IntPtr h, int attr, out RECT r, int size);
        [DllImport("dwmapi.dll", EntryPoint = "DwmGetWindowAttribute")] static extern int DwmGetWindowAttributeInt(IntPtr h, int attr, out int v, int size);
        [DllImport("user32.dll")] static extern IntPtr MonitorFromWindow(IntPtr h, uint flags);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern bool GetMonitorInfo(IntPtr m, ref MONITORINFOEX mi);
        [DllImport("shcore.dll")] static extern int GetDpiForMonitor(IntPtr m, int type, out uint dpiX, out uint dpiY);
        [DllImport("user32.dll", SetLastError = true)] static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr h, int cmd);
        [DllImport("user32.dll")] static extern bool GetCursorPos(out POINT p);
        [DllImport("user32.dll")] static extern IntPtr SetThreadDpiAwarenessContext(IntPtr ctx);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(IntPtr h, uint msg, IntPtr w, IntPtr l, uint flags, uint timeout, out IntPtr result);
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")] static extern IntPtr GetClassLongPtr64(IntPtr h, int index);
        [DllImport("user32.dll", EntryPoint = "GetClassLong")] static extern uint GetClassLong32(IntPtr h, int index);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOOLWINDOW = 0x80;
        const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
        const int DWMWA_CLOAKED = 14;
        const uint GW_OWNER = 4;
        const uint MONITOR_DEFAULTTONEAREST = 2;
        public const uint SWP_NOSIZE = 0x0001, SWP_NOZORDER = 0x0004, SWP_NOACTIVATE = 0x0010, SWP_SHOWWINDOW = 0x0040;
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        // Window maths run in physical pixels (per-monitor DPI), otherwise Windows
        // scales coordinates when screens use different scaling.
        public static IntPtr BeginDpi()
        {
            try { return SetThreadDpiAwarenessContext(new IntPtr(-4)); } // PER_MONITOR_AWARE_V2
            catch (EntryPointNotFoundException) { return IntPtr.Zero; }
        }

        public static void EndDpi(IntPtr previous)
        {
            if (previous == IntPtr.Zero) return;
            try { SetThreadDpiAwarenessContext(previous); } catch (EntryPointNotFoundException) { }
        }

        // Visible bounds: GetWindowRect includes ~7 px invisible resize borders.
        public static RECT Visible(IntPtr h)
        {
            RECT r;
            if (DwmGetWindowAttribute(h, DWMWA_EXTENDED_FRAME_BOUNDS, out r, Marshal.SizeOf(typeof(RECT))) != 0)
                GetWindowRect(h, out r);
            return r;
        }

        static MONITORINFOEX MonitorInfo(IntPtr monitor)
        {
            MONITORINFOEX mi = new MONITORINFOEX();
            mi.cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            GetMonitorInfo(monitor, ref mi);
            return mi;
        }

        static int DisplayNumber(MONITORINFOEX mi)
        {
            string digits = "";
            foreach (char c in mi.szDevice ?? "") if (char.IsDigit(c)) digits += c;
            int n;
            return int.TryParse(digits, out n) ? n : 0;
        }

        static string ScreenName(MONITORINFOEX mi)
        {
            return string.Format(Lang.ScreenName, DisplayNumber(mi));
        }

        // Connected screens, sorted by Windows display number
        public static List<ScreenInfo> Screens()
        {
            List<ScreenInfo> list = new List<ScreenInfo>();
            IntPtr previous = BeginDpi();
            try
            {
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, delegate (IntPtr m, IntPtr dc, ref RECT r, IntPtr data)
                {
                    MONITORINFOEX mi = MonitorInfo(m);
                    ScreenInfo s = new ScreenInfo();
                    s.Handle = m;
                    s.Number = DisplayNumber(mi);
                    s.Name = ScreenName(mi);
                    s.Width = mi.rcMonitor.Right - mi.rcMonitor.Left;
                    s.Height = mi.rcMonitor.Bottom - mi.rcMonitor.Top;
                    s.Primary = (mi.dwFlags & 1) != 0;
                    uint dx, dy;
                    try { s.ScalePercent = GetDpiForMonitor(m, 0, out dx, out dy) == 0 ? (int)Math.Round(dx * 100 / 96.0) : 100; }
                    catch (Exception) { s.ScalePercent = 100; }
                    list.Add(s);
                    return true;
                }, IntPtr.Zero);
            }
            finally { EndDpi(previous); }
            list.Sort(delegate (ScreenInfo a, ScreenInfo b) { return a.Number.CompareTo(b.Number); });
            return list;
        }

        // Shows each screen's number in big for 2 seconds
        public static void IdentifyScreens()
        {
            List<ScreenBadge> badges = new List<ScreenBadge>();
            IntPtr previous = BeginDpi();
            try
            {
                foreach (ScreenInfo s in Screens())
                {
                    MONITORINFOEX mi = MonitorInfo(s.Handle);
                    ScreenBadge b = new ScreenBadge(s.Number.ToString(), s.Width + "×" + s.Height);
                    IntPtr created = b.Handle;
                    int size = 260;
                    SetWindowPos(b.Handle, HWND_TOPMOST, mi.rcMonitor.Left + (s.Width - size) / 2,
                        mi.rcMonitor.Top + (s.Height - size) / 2, size, size, SWP_NOACTIVATE | SWP_SHOWWINDOW);
                    badges.Add(b);
                }
            }
            finally { EndDpi(previous); }
            Timer t = new Timer();
            t.Interval = 2000;
            t.Tick += delegate { t.Stop(); foreach (ScreenBadge b in badges) b.Dispose(); t.Dispose(); };
            t.Start();
        }

        static bool IsAppWindow(IntPtr h, int excludedPid)
        {
            if (!IsWindowVisible(h)) return false;
            if (GetWindowTextLength(h) == 0) return false;
            if (GetWindow(h, GW_OWNER) != IntPtr.Zero) return false;
            if ((GetWindowLong(h, GWL_EXSTYLE) & WS_EX_TOOLWINDOW) != 0) return false;
            int cloaked;
            if (DwmGetWindowAttributeInt(h, DWMWA_CLOAKED, out cloaked, 4) == 0 && cloaked != 0) return false;
            uint pid;
            GetWindowThreadProcessId(h, out pid);
            return (int)pid != excludedPid;
        }

        // Top-level app windows, front to back
        static List<IntPtr> Handles(int excludedPid)
        {
            List<IntPtr> list = new List<IntPtr>();
            EnumWindows(delegate (IntPtr h, IntPtr l)
            {
                if (IsAppWindow(h, excludedPid)) list.Add(h);
                return true;
            }, IntPtr.Zero);
            return list;
        }

        static string Title(IntPtr h)
        {
            StringBuilder sb = new StringBuilder(GetWindowTextLength(h) + 1);
            GetWindowText(h, sb, sb.Capacity);
            return sb.ToString();
        }

        public static WindowInfo Describe(IntPtr h)
        {
            IntPtr previous = BeginDpi();
            try
            {
                WindowInfo w = new WindowInfo();
                w.Handle = h;
                w.Title = Title(h);
                uint pid;
                GetWindowThreadProcessId(h, out pid);
                try { w.Process = System.Diagnostics.Process.GetProcessById((int)pid).ProcessName; }
                catch (Exception) { w.Process = "?"; }
                RECT v = Visible(h);
                w.Width = v.Right - v.Left;
                w.Height = v.Bottom - v.Top;
                w.Screen = ScreenName(MonitorInfo(MonitorFromWindow(h, MONITOR_DEFAULTTONEAREST)));
                w.Maximized = IsZoomed(h);
                w.Minimized = IsIconic(h);
                return w;
            }
            finally { EndDpi(previous); }
        }

        public static List<WindowInfo> Windows(int excludedPid)
        {
            List<WindowInfo> list = new List<WindowInfo>();
            foreach (IntPtr h in Handles(excludedPid))
            {
                WindowInfo w = Describe(h);
                if (w.Title == "Program Manager") continue; // the desktop
                list.Add(w);
            }
            return list;
        }

        // Picker: the front-most window under the mouse cursor
        public static IntPtr WindowUnderCursor(int excludedPid)
        {
            IntPtr previous = BeginDpi();
            try
            {
                POINT p;
                GetCursorPos(out p);
                foreach (IntPtr h in Handles(excludedPid))
                {
                    if (IsIconic(h)) continue;
                    RECT r = Visible(h);
                    if (p.X >= r.Left && p.X < r.Right && p.Y >= r.Top && p.Y < r.Bottom)
                        return Title(h) == "Program Manager" ? IntPtr.Zero : h;
                }
                return IntPtr.Zero;
            }
            finally { EndDpi(previous); }
        }

        public static Bitmap WindowIcon(IntPtr h)
        {
            IntPtr icon = IntPtr.Zero, r;
            foreach (int type in new int[] { 1, 2, 0 }) // ICON_BIG, ICON_SMALL2, ICON_SMALL
            {
                if (SendMessageTimeout(h, 0x7F /* WM_GETICON */, new IntPtr(type), IntPtr.Zero, 0x2 /* ABORTIFHUNG */, 60, out r) != IntPtr.Zero
                    && r != IntPtr.Zero)
                {
                    icon = r;
                    break;
                }
            }
            if (icon == IntPtr.Zero) icon = ClassLong(h, -14); // GCLP_HICON
            if (icon == IntPtr.Zero) icon = ClassLong(h, -34); // GCLP_HICONSM
            if (icon != IntPtr.Zero)
            {
                try { using (Icon ic = System.Drawing.Icon.FromHandle(icon)) return ic.ToBitmap(); }
                catch (Exception) { }
            }
            try
            {
                uint pid;
                GetWindowThreadProcessId(h, out pid);
                string exe = System.Diagnostics.Process.GetProcessById((int)pid).MainModule.FileName;
                using (Icon ic = System.Drawing.Icon.ExtractAssociatedIcon(exe)) return ic.ToBitmap();
            }
            catch (Exception) { return null; }
        }

        static IntPtr ClassLong(IntPtr h, int index)
        {
            return IntPtr.Size == 8 ? GetClassLongPtr64(h, index) : new IntPtr(GetClassLong32(h, index));
        }

        // Centers a window on a screen (IntPtr.Zero = its current screen), without resizing it.
        public static string CenterWindow(IntPtr h, bool fullScreen, IntPtr targetMonitor, out bool success)
        {
            success = false;
            if (!IsWindow(h)) return Lang.WindowGone;
            IntPtr previous = BeginDpi();
            try
            {
                if (IsZoomed(h)) return Lang.IsMaximized;
                if (IsIconic(h))
                {
                    ShowWindow(h, 9); // SW_RESTORE
                    System.Threading.Thread.Sleep(200);
                }

                IntPtr monitor = targetMonitor == IntPtr.Zero ? MonitorFromWindow(h, MONITOR_DEFAULTTONEAREST) : targetMonitor;
                bool changesScreen = MonitorFromWindow(h, MONITOR_DEFAULTTONEAREST) != monitor;
                MONITORINFOEX mi = MonitorInfo(monitor);
                RECT area = fullScreen ? mi.rcMonitor : mi.rcWork;
                int aw = area.Right - area.Left, ah = area.Bottom - area.Top;
                bool tooTall = false;

                // Up to 3 passes: apps often resize themselves on a screen with another scaling.
                for (int pass = 0; pass < 3; pass++)
                {
                    RECT w;
                    GetWindowRect(h, out w);
                    RECT v = Visible(h);
                    int vx = area.Left + (aw - (v.Right - v.Left)) / 2;
                    int vy = area.Top + (ah - (v.Bottom - v.Top)) / 2;
                    tooTall = vy < area.Top;
                    if (tooTall) vy = area.Top; // keep the title bar reachable
                    if (v.Left == vx && v.Top == vy) break;

                    if (!SetWindowPos(h, IntPtr.Zero, w.Left + (vx - v.Left), w.Top + (vy - v.Top), 0, 0,
                                      SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE))
                    {
                        int err = Marshal.GetLastWin32Error();
                        return err == 5 ? Lang.AccessDenied : string.Format(Lang.MoveFailed, err);
                    }
                    System.Threading.Thread.Sleep(changesScreen ? 250 : 80);
                }

                // Measure again: the margins shown are the real result
                RECT n = Visible(h);
                int left = n.Left - area.Left, right = area.Right - n.Right;
                int top = n.Top - area.Top, bottom = area.Bottom - n.Bottom;
                string margins = string.Format(Lang.Margins, left, right, top, bottom);
                string screen = string.Format("{0} ({1}×{2})", ScreenName(mi), aw, ah);

                if (MonitorFromWindow(h, MONITOR_DEFAULTTONEAREST) != monitor) return Lang.ScreenRefused;
                success = true;
                if (tooTall) return string.Format(Lang.TooTall, screen, margins);
                if (Math.Abs(left - right) > 1 || Math.Abs(top - bottom) > 1)
                {
                    success = false;
                    return string.Format(Lang.NotExact, margins);
                }
                return string.Format(left == right && top == bottom ? Lang.Centered : Lang.CenteredOdd, screen, margins);
            }
            finally { EndDpi(previous); }
        }
    }

    // Frame around the targeted window while dragging the picker.
    // Click-through, never takes focus.
    public class Highlight : Form
    {
        [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr h, int cmd);

        Highlight()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Lime;
            TransparencyKey = Color.Lime;
        }

        public static Highlight Create()
        {
            // Per-monitor DPI aware, to be placed in physical pixels
            IntPtr previous = Native.BeginDpi();
            try
            {
                Highlight h = new Highlight();
                IntPtr created = h.Handle;
                return h;
            }
            finally { Native.EndDpi(previous); }
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20 | 0x80 | 0x08000000; // TRANSPARENT | TOOLWINDOW | NOACTIVATE
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float w = 6;
            using (Pen p = new Pen(Theme.Primary, w))
                e.Graphics.DrawRectangle(p, w / 2, w / 2, ClientSize.Width - w, ClientSize.Height - w);
        }

        public void Cover(IntPtr target)
        {
            IntPtr previous = Native.BeginDpi();
            try
            {
                Native.RECT r = Native.Visible(target);
                SetWindowPos(Handle, Native.HWND_TOPMOST, r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top,
                    Native.SWP_NOACTIVATE | Native.SWP_SHOWWINDOW);
                Invalidate();
            }
            finally { Native.EndDpi(previous); }
        }

        public void HideFrame() { ShowWindow(Handle, 0); }
    }

    // Big number shown in the middle of a screen by "Identify"
    public class ScreenBadge : Form
    {
        readonly string number, detail;

        public ScreenBadge(string number, string detail)
        {
            this.number = number;
            this.detail = detail;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Theme.PrimaryContainer;
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20 | 0x80 | 0x08000000;
                return cp;
            }
        }

        // Rounded corners cut with a region (no transparency key: no colored fringe)
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            using (GraphicsPath p = Draw.Rounded(new RectangleF(0, 0, Width, Height), Width * 0.16f))
                Region = new Region(p);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Draw.Quality(g);
            int h = ClientSize.Height;
            using (Font f = new Font("Segoe UI Semibold", h * 0.36f, GraphicsUnit.Pixel))
                Draw.Text(g, number, f, new Rectangle(0, 0, ClientSize.Width, h * 3 / 4), Theme.OnPrimaryContainer,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            using (Font f = new Font("Segoe UI", h * 0.08f, GraphicsUnit.Pixel))
                Draw.Text(g, detail, f, new Rectangle(0, h / 2, ClientSize.Width, h / 2), Theme.Primary,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    // ------------------------------------------------------------------------
    //  Material 3 components
    // ------------------------------------------------------------------------

    // Base: owner-drawn, flicker-free control with hover / pressed states
    public class MaterialControl : Control
    {
        protected bool hover, pressed;

        public MaterialControl()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Surface;
            ForeColor = Theme.OnSurface;
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { pressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { pressed = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnTextChanged(EventArgs e) { Invalidate(); base.OnTextChanged(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

        protected Graphics Begin(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            Draw.Quality(g);
            return g;
        }
    }

    public enum ButtonStyle { Filled, Tonal, Text }

    // Material 3 button with an icon: filled, tonal or text
    public class MaterialButton : MaterialControl
    {
        public ButtonStyle Style;
        public Glyph Glyph;
        public bool Active;   // "in progress" look (picker being dragged)
        public bool IconOnly; // round button with just the icon (compact mode picker)

        public MaterialButton(string text, ButtonStyle style, Glyph glyph)
        {
            Text = text;
            Style = style;
            Glyph = glyph;
            Height = Theme.D(40);
        }

        Font LabelFont { get { return Theme.Font(Height > Theme.D(44) ? 16 : 14, true); } }

        public int PreferredWidth()
        {
            if (IconOnly) return Height;
            int pad = Style == ButtonStyle.Text ? 12 : 24;
            return Draw.Measure(Text, LabelFont) + Theme.D(pad * 2 - 8 + 18 + 8);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = Begin(e);
            RectangleF r = new RectangleF(0.5f, 0.5f, Width - 1, Height - 1);
            float radius = r.Height / 2;
            Color fill, ink;
            if (Style == ButtonStyle.Filled || (Style == ButtonStyle.Tonal && Active)) { fill = Theme.Primary; ink = Theme.OnPrimary; }
            else if (Style == ButtonStyle.Tonal) { fill = Theme.SecondaryContainer; ink = Theme.OnSecondaryContainer; }
            else { fill = Color.Transparent; ink = Theme.Primary; }

            if (!Enabled)
            {
                fill = Style == ButtonStyle.Filled || Style == ButtonStyle.Tonal ? Color.FromArgb(31, Theme.OnSurface) : Color.Transparent;
                ink = Color.FromArgb(97, Theme.OnSurface);
            }

            Draw.Fill(g, r, radius, fill);
            if (Enabled) Draw.Fill(g, r, radius, Theme.State(ink, hover, pressed));

            if (IconOnly)
            {
                float s = Theme.D(24);
                Draw.Icon(g, Glyph, new RectangleF((Width - s) / 2f, (Height - s) / 2f, s, s), ink);
                return;
            }
            Font f = LabelFont;
            int tw = Draw.Measure(Text, f);
            int iw = Theme.D(18), gap = Theme.D(8);
            int x = (Width - (iw + gap + tw)) / 2;
            Draw.Icon(g, Glyph, new RectangleF(x, (Height - iw) / 2f, iw, iw), ink);
            Draw.Label(g, Text, f, x + iw + gap, Height, ink);
        }
    }

    // Material 3 filter chip (screen choice)
    public class Chip : MaterialControl
    {
        bool selected;
        public IntPtr Monitor;
        public string Detail = "";

        public bool Selected { get { return selected; } set { selected = value; Invalidate(); } }

        public Chip(string text, IntPtr monitor)
        {
            Text = text;
            Monitor = monitor;
            Height = Theme.D(32);
        }

        public int PreferredWidth()
        {
            int w = Draw.Measure(Text, Theme.Font(14, true)) + Theme.D(16 + 16);
            if (Detail.Length > 0) w += Draw.Measure(Detail, Theme.Font(13, false)) + Theme.D(6);
            if (selected) w += Theme.D(18);
            return w;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = Begin(e);
            RectangleF r = new RectangleF(0.5f, 0.5f, Width - 1, Height - 1);
            float radius = Theme.D(8);
            Color ink = selected ? Theme.OnSecondaryContainer : Theme.OnSurfaceVariant;
            if (selected) Draw.Fill(g, r, radius, Theme.SecondaryContainer);
            else Draw.Stroke(g, r, radius, Theme.OutlineVariant, 1);
            Draw.Fill(g, r, radius, Theme.State(ink, hover, pressed));

            int x = Theme.D(selected ? 8 : 16);
            if (selected)
            {
                int iw = Theme.D(18);
                Draw.Icon(g, Glyph.Check, new RectangleF(x, (Height - iw) / 2f, iw, iw), ink);
                x += iw + Theme.D(8);
            }
            Font f = Theme.Font(14, true);
            Draw.Label(g, Text, f, x, Height, ink);
            if (Detail.Length > 0)
                Draw.Label(g, Detail, Theme.Font(13, false), x + Draw.Measure(Text, f) + Theme.D(6), Height,
                    selected ? Theme.OnSecondaryContainer : Theme.Outline);
        }
    }

    // Material 3 segmented button (single choice)
    public class SegmentedButton : MaterialControl
    {
        string[] options;
        int selected, hovered = -1;
        public event EventHandler Changed;

        public int Selected
        {
            get { return selected; }
            set { selected = value; Invalidate(); if (Changed != null) Changed(this, EventArgs.Empty); }
        }

        public SegmentedButton(params string[] options)
        {
            this.options = options;
            Height = Theme.D(40);
        }

        public int PreferredWidth()
        {
            int widest = 0;
            foreach (string o in options) widest = Math.Max(widest, Draw.Measure(o, Theme.Font(14, true)));
            return options.Length * (widest + Theme.D(18 + 8 + 24 * 2));
        }

        public void SetOptions(params string[] newOptions)
        {
            options = newOptions;
            Invalidate();
        }

        int SegmentAt(int x) { return Math.Max(0, Math.Min(options.Length - 1, x * options.Length / Math.Max(1, Width))); }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int s = SegmentAt(e.X);
            if (s != hovered) { hovered = s; Invalidate(); }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e) { hovered = -1; base.OnMouseLeave(e); }
        protected override void OnMouseClick(MouseEventArgs e) { Selected = SegmentAt(e.X); base.OnMouseClick(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = Begin(e);
            RectangleF r = new RectangleF(0.5f, 0.5f, Width - 1, Height - 1);
            float sw = r.Width / options.Length;
            using (GraphicsPath shape = Draw.Rounded(r, r.Height / 2))
            {
                g.SetClip(shape);
                for (int i = 0; i < options.Length; i++)
                {
                    RectangleF s = new RectangleF(r.X + i * sw, r.Y, sw, r.Height);
                    bool sel = i == selected;
                    Color ink = sel ? Theme.OnSecondaryContainer : Theme.OnSurface;
                    if (sel) using (SolidBrush b = new SolidBrush(Theme.SecondaryContainer)) g.FillRectangle(b, s);
                    if (i == hovered) using (SolidBrush b = new SolidBrush(Theme.State(ink, true, pressed))) g.FillRectangle(b, s);

                    Font f = Theme.Font(14, true);
                    int tw = Draw.Measure(options[i], f);
                    int iw = sel ? Theme.D(18) : 0, gap = sel ? Theme.D(8) : 0;
                    int x = (int)(s.X + (s.Width - iw - gap - tw) / 2);
                    if (sel) Draw.Icon(g, Glyph.Check, new RectangleF(x, (Height - iw) / 2f, iw, iw), ink);
                    Draw.Label(g, options[i], f, x + iw + gap, Height, ink);
                }
                g.ResetClip();
                using (Pen p = new Pen(Theme.Outline, 1))
                {
                    g.DrawPath(p, shape);
                    for (int i = 1; i < options.Length; i++) g.DrawLine(p, r.X + i * sw, r.Y, r.X + i * sw, r.Bottom);
                }
            }
        }
    }

    // Card: rounded surface that hosts the window list
    public class Card : Panel
    {
        public Card()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Theme.Surface;
            Padding = new Padding(Theme.D(8));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            Draw.Quality(g);
            Draw.Fill(g, new RectangleF(0, 0, Width - 1, Height - 1), Theme.D(16), Theme.Container);
        }
    }

    // Two-line Material list: app icon, window title, details
    public class WindowList : ListBox
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)] static extern int SetWindowTheme(IntPtr h, string app, string idList);

        int hovered = -1;

        public WindowList()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = Theme.D(60);
            BorderStyle = BorderStyle.None;
            IntegralHeight = false;
            BackColor = Theme.Container;
            ForeColor = Theme.OnSurface;
        }

        // Dark scrollbar (Windows 10 1809+)
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try { SetWindowTheme(Handle, "DarkMode_Explorer", null); } catch (Exception) { }
        }

        public WindowInfo Current { get { return SelectedItem as WindowInfo; } }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int i = IndexFromPoint(e.Location);
            if (i != hovered)
            {
                int old = hovered;
                hovered = i;
                if (old >= 0 && old < Items.Count) Invalidate(GetItemRectangle(old));
                if (i >= 0) Invalidate(GetItemRectangle(i));
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (hovered >= 0 && hovered < Items.Count) Invalidate(GetItemRectangle(hovered));
            hovered = -1;
            base.OnMouseLeave(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count) return;
            WindowInfo w = Items[e.Index] as WindowInfo;
            if (w == null) return;

            // Drawn off-screen first to avoid flicker
            Rectangle b = e.Bounds;
            using (Bitmap bmp = new Bitmap(Math.Max(1, b.Width), Math.Max(1, b.Height)))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(BackColor);
                    Draw.Quality(g);
                    RectangleF row = new RectangleF(0, Theme.D(2), b.Width, b.Height - Theme.D(4));
                    bool sel = (e.State & DrawItemState.Selected) != 0;
                    if (sel) Draw.Fill(g, row, Theme.D(12), Theme.SecondaryContainer);
                    else if (e.Index == hovered) Draw.Fill(g, row, Theme.D(12), Color.FromArgb(20, Theme.OnSurface));

                    int s = Theme.D(36);
                    RectangleF ic = new RectangleF(Theme.D(12), (b.Height - s) / 2f, s, s);
                    if (w.Icon != null)
                        g.DrawImage(w.Icon, RectangleF.Inflate(ic, -Theme.D(2), -Theme.D(2)));
                    else
                    {
                        Draw.Fill(g, ic, s / 2f, Theme.PrimaryContainer);
                        string initial = string.IsNullOrEmpty(w.Process) ? "?" : w.Process.Substring(0, 1).ToUpper();
                        Draw.Text(g, initial, Theme.Font(16, true), Rectangle.Round(ic), Theme.OnPrimaryContainer,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }

                    int x = (int)ic.Right + Theme.D(16);
                    int width = b.Width - x - Theme.D(12);
                    bool dim = w.Maximized || w.Minimized;
                    string state = w.Minimized ? Lang.Minimized : w.Maximized ? Lang.Maximized : w.Width + " × " + w.Height;
                    Draw.Text(g, w.Title, Theme.Font(15, false), new Rectangle(x, Theme.D(9), width, Theme.D(22)),
                        dim ? Theme.Outline : (sel ? Theme.OnSecondaryContainer : Theme.OnSurface), TextFormatFlags.Left);
                    Draw.Text(g, w.Process + "  ·  " + state + "  ·  " + w.Screen, Theme.Font(13, false),
                        new Rectangle(x, Theme.D(32), width, Theme.D(20)),
                        sel ? Theme.OnSecondaryContainer : Theme.OnSurfaceVariant, TextFormatFlags.Left);
                }
                e.Graphics.DrawImageUnscaled(bmp, b.Location);
            }
        }
    }

    public enum StatusKind { Info, Success, Error, Accent }

    // Status bar at the bottom (Material "snackbar" style)
    public class StatusBar : MaterialControl
    {
        StatusKind kind = StatusKind.Info;

        public StatusBar() { Cursor = Cursors.Default; }

        public void SetMessage(string text, StatusKind kind)
        {
            this.kind = kind;
            Text = text;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = Begin(e);
            Draw.Fill(g, new RectangleF(0, 0, Width - 1, Height - 1), Theme.D(12), Theme.ContainerHigh);
            Color c = kind == StatusKind.Success ? Theme.Success : kind == StatusKind.Error ? Theme.Error :
                      kind == StatusKind.Accent ? Theme.Primary : Theme.OnSurfaceVariant;
            Glyph icon = kind == StatusKind.Success ? Glyph.Check : kind == StatusKind.Error ? Glyph.Error :
                         kind == StatusKind.Accent ? Glyph.Picker : Glyph.Info;
            int iw = Theme.D(20);
            Draw.Icon(g, icon, new RectangleF(Theme.D(16), (Height - iw) / 2f, iw, iw), c);
            int x = Theme.D(16) + iw + Theme.D(12);
            // Smaller text in compact mode
            Font f = Theme.Font(Width < Theme.D(480) ? 13 : 14, false);
            Draw.Text(g, Text, f, new Rectangle(x, Theme.D(4), Width - x - Theme.D(16), Height - Theme.D(8)),
                kind == StatusKind.Info ? Theme.OnSurfaceVariant : Theme.OnSurface,
                TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }
    }

    // Material 3 standard icon button (40 px circle, 24 px icon).
    // A toggle icon button is filled, and shows GlyphOn, when it is on.
    public class IconButton : MaterialControl
    {
        public Glyph Glyph, GlyphOn;
        public bool Toggle;
        bool on;

        public bool On { get { return on; } set { on = value; Invalidate(); } }

        public IconButton(Glyph glyph, bool toggle)
        {
            Glyph = GlyphOn = glyph;
            Toggle = toggle;
            Size = new Size(Theme.D(40), Theme.D(40));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = Begin(e);
            RectangleF r = new RectangleF(0.5f, 0.5f, Width - 1, Height - 1);
            bool filled = Toggle && on;
            Color ink = filled ? Theme.OnSecondaryContainer : Theme.OnSurfaceVariant;
            if (filled) Draw.Fill(g, r, r.Width / 2, Theme.SecondaryContainer);
            Draw.Fill(g, r, r.Width / 2, Theme.State(ink, hover, pressed));
            float s = Theme.D(24);
            Draw.Icon(g, filled ? GlyphOn : Glyph, new RectangleF((Width - s) / 2f, (Height - s) / 2f, s, s), ink);
        }
    }

    // Outlined field showing the chosen window, with a drop-down arrow (compact mode)
    public class WindowField : MaterialControl
    {
        WindowInfo window;
        public string Placeholder = "";

        public WindowInfo Window { get { return window; } set { window = value; Invalidate(); } }

        public WindowField() { Height = Theme.D(56); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = Begin(e);
            RectangleF r = new RectangleF(0.5f, 0.5f, Width - 1, Height - 1);
            float radius = Theme.D(8);
            Draw.Stroke(g, r, radius, hover ? Theme.OnSurface : Theme.Outline, 1);
            Draw.Fill(g, r, radius, Theme.State(Theme.OnSurface, false, pressed));

            int x = Theme.D(12);
            if (window == null)
            {
                Draw.Text(g, Placeholder, Theme.Font(15, false), new Rectangle(Theme.D(16), 0, Width - Theme.D(64), Height),
                    Theme.OnSurfaceVariant, TextFormatFlags.VerticalCenter);
            }
            else
            {
                int s = Theme.D(32);
                RectangleF ic = new RectangleF(x, (Height - s) / 2f, s, s);
                if (window.Icon != null) g.DrawImage(window.Icon, ic);
                else
                {
                    Draw.Fill(g, ic, s / 2f, Theme.PrimaryContainer);
                    string initial = string.IsNullOrEmpty(window.Process) ? "?" : window.Process.Substring(0, 1).ToUpper();
                    Draw.Text(g, initial, Theme.Font(14, true), Rectangle.Round(ic), Theme.OnPrimaryContainer,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                int tx = x + s + Theme.D(12), tw = Width - tx - Theme.D(48);
                Draw.Text(g, window.Title, Theme.Font(15, false), new Rectangle(tx, Theme.D(8), tw, Theme.D(22)), Theme.OnSurface, TextFormatFlags.Left);
                string state = window.Minimized ? Lang.Minimized : window.Maximized ? Lang.Maximized : window.Width + " × " + window.Height;
                Draw.Text(g, window.Process + "  ·  " + state + "  ·  " + window.Screen, Theme.Font(13, false),
                    new Rectangle(tx, Theme.D(30), tw, Theme.D(18)), Theme.OnSurfaceVariant, TextFormatFlags.Left);
            }
            float c = Theme.D(24);
            Draw.Icon(g, Glyph.ChevronDown, new RectangleF(Width - Theme.D(16) - c, (Height - c) / 2f, c, c), Theme.OnSurfaceVariant);
        }
    }

    // Dark look for drop-down menus and tooltips
    public class DarkRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(Theme.Container)) e.Graphics.FillRectangle(b, e.AffectedBounds);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            using (Pen p = new Pen(Theme.OutlineVariant))
                e.Graphics.DrawRectangle(p, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
        }

        public static ToolTip CreateToolTip()
        {
            ToolTip t = new ToolTip();
            t.OwnerDraw = true;
            t.InitialDelay = 400;
            t.Popup += delegate (object s, PopupEventArgs e)
            {
                Size sz = TextRenderer.MeasureText(t.GetToolTip(e.AssociatedControl), Theme.Font(13, false));
                e.ToolTipSize = new Size(sz.Width + Theme.D(16), sz.Height + Theme.D(10));
            };
            t.Draw += delegate (object s, DrawToolTipEventArgs e)
            {
                using (SolidBrush b = new SolidBrush(Theme.ContainerHighest)) e.Graphics.FillRectangle(b, e.Bounds);
                TextRenderer.DrawText(e.Graphics, e.ToolTipText, Theme.Font(13, false), e.Bounds, Theme.OnSurface,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            };
            return t;
        }
    }

    // ------------------------------------------------------------------------
    //  Main window: full layout, or compact (drop-down + picker + Center;
    //  dropping the picker on a window centers it right away)
    // ------------------------------------------------------------------------
    public class MainWindow : Form
    {
        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int value, int size);

        readonly int myPid = System.Diagnostics.Process.GetCurrentProcess().Id;
        readonly Dictionary<IntPtr, Bitmap> iconCache = new Dictionary<IntPtr, Bitmap>();
        readonly List<Chip> chips = new List<Chip>();
        readonly ToolTip tips = DarkRenderer.CreateToolTip();

        Bitmap logo;
        IconButton modeButton, langButton, pinButton;
        MaterialButton picker, refresh, identify, center;
        Card card;
        WindowList list;
        WindowField field;
        SegmentedButton area;
        StatusBar status;
        Highlight highlight;

        bool compact, dragging;
        IntPtr target = IntPtr.Zero;
        IntPtr chosenMonitor = IntPtr.Zero;
        int yWindows, yScreen, yArea;

        public MainWindow()
        {
            Text = "Pile Poil";
            BackColor = Theme.Surface;
            ForeColor = Theme.OnSurface;
            Font = Theme.Font(14, false);
            DoubleBuffered = true;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = Settings.AlwaysOnTop;

            // Window icon: the exe's icon. Header logo: embedded logo.png resource
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch (Exception) { }
            Stream res = Assembly.GetExecutingAssembly().GetManifestResourceStream("logo.png");
            if (res != null) logo = new Bitmap(res);

            // Header, left to right: language, always on top, compact / full mode
            modeButton = new IconButton(Glyph.Compact, false);
            modeButton.Click += delegate { SetCompact(!compact); };

            langButton = new IconButton(Glyph.Language, false);
            langButton.Click += delegate { SetLanguage(Lang.Code == "fr" ? "en" : "fr"); };

            pinButton = new IconButton(Glyph.PinOff, true);
            pinButton.GlyphOn = Glyph.PinOn;
            pinButton.On = Settings.AlwaysOnTop;
            pinButton.Click += delegate
            {
                pinButton.On = !pinButton.On;
                TopMost = Settings.AlwaysOnTop = pinButton.On;
                Settings.Save();
            };

            picker = new MaterialButton("", ButtonStyle.Tonal, Glyph.Picker);
            picker.Cursor = Cursors.Cross;
            picker.MouseDown += PickerDown;
            picker.MouseMove += PickerMove;
            picker.MouseUp += PickerUp;

            refresh = new MaterialButton("", ButtonStyle.Text, Glyph.Refresh);
            refresh.Click += delegate { RefreshAll(); };

            identify = new MaterialButton("", ButtonStyle.Text, Glyph.Screen);
            identify.Click += delegate { if (!Program.Demo) Native.IdentifyScreens(); };

            center = new MaterialButton("", ButtonStyle.Filled, Glyph.Center);
            center.Click += delegate { CenterSelected(); };

            // Full mode: list in a card. Compact mode: drop-down field
            card = new Card();
            list = new WindowList();
            list.Dock = DockStyle.Fill;
            list.DoubleClick += delegate { CenterSelected(); };
            card.Controls.Add(list);

            field = new WindowField();
            field.Click += delegate { ShowWindowMenu(); };

            area = new SegmentedButton("", "");
            area.Selected = Settings.FullScreenArea ? 1 : 0;
            area.Changed += delegate { Settings.FullScreenArea = area.Selected == 1; Settings.Save(); };

            status = new StatusBar();

            Controls.AddRange(new Control[] { modeButton, langButton, pinButton, picker, refresh, identify, center, card, field, area, status });

            if (!Program.Demo) highlight = Highlight.Create();
            ApplyTexts();
            RefreshScreens();
            RefreshWindows(Program.Demo ? new IntPtr(1) : IntPtr.Zero);
            SetCompact(Settings.Compact);
            if (Program.Demo) Cursor.Position = Point.Empty; // no hover effect on screenshots
        }

        // Dark title bar (Windows 10 20H1+; attribute 19 on older builds)
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int yes = 1;
            try
            {
                if (DwmSetWindowAttribute(Handle, 20, ref yes, 4) != 0)
                    DwmSetWindowAttribute(Handle, 19, ref yes, 4);
            }
            catch (Exception) { }
        }

        // ---------- Modes and language ----------

        void SetCompact(bool on)
        {
            // Carry the chosen window over to the other layout
            WindowInfo current = SelectedWindow();
            compact = on;
            Settings.Compact = on;
            Settings.Save();

            if (on) field.Window = current;
            else RefreshWindows(current != null ? current.Handle : IntPtr.Zero);

            SuspendLayout();
            foreach (Control c in new Control[] { refresh, identify, card, area }) c.Visible = !on;
            foreach (Chip c in chips) c.Visible = !on;
            field.Visible = on;

            Point topLeft = Location;
            MinimumSize = Size.Empty;
            MaximumSize = Size.Empty;
            if (on)
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                MaximizeBox = false;
                ClientSize = new Size(Theme.D(440), Theme.D(280));
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                MaximizeBox = true;
                ClientSize = new Size(Theme.D(720), Theme.D(760));
                MinimumSize = new Size(Width - ClientSize.Width + Theme.D(640), Height - ClientSize.Height + Theme.D(640));
            }
            if (IsHandleCreated) Location = topLeft; // grow / shrink from the top-left corner
            ResumeLayout();

            modeButton.Glyph = modeButton.GlyphOn = on ? Glyph.Expand : Glyph.Compact;
            modeButton.Invalidate();
            tips.SetToolTip(modeButton, on ? Lang.TipFull : Lang.TipCompact);
            picker.IconOnly = on;
            tips.SetToolTip(picker, on ? Lang.TipPicker : null);
            LayoutControls();
            status.SetMessage(on ? Lang.ReadyCompact : Lang.Ready, StatusKind.Info);
        }

        void SetLanguage(string code)
        {
            Settings.Language = code;
            Settings.Save();
            Lang.Load(code);
            ApplyTexts();
            RefreshScreens();
            RefreshWindows(list.Current != null ? list.Current.Handle : IntPtr.Zero);
            if (field.Window != null) field.Window = Program.Demo ? field.Window : Native.Describe(field.Window.Handle).WithIcon(field.Window.Icon);
            status.SetMessage(compact ? Lang.ReadyCompact : Lang.Ready, StatusKind.Info);
            LayoutControls();
        }

        void ApplyTexts()
        {
            picker.Text = Lang.Picker;
            refresh.Text = Lang.Refresh;
            identify.Text = Lang.Identify;
            center.Text = Lang.Center;
            area.SetOptions(Lang.WorkArea, Lang.FullScreen);
            field.Placeholder = Lang.ChooseWindow;
            field.Invalidate();
            list.Invalidate();
            tips.SetToolTip(langButton, Lang.TipLanguage);
            tips.SetToolTip(pinButton, Lang.TipPin);
            tips.SetToolTip(modeButton, compact ? Lang.TipFull : Lang.TipCompact);
            tips.SetToolTip(picker, compact ? Lang.TipPicker : null);
            Invalidate();
        }

        // ---------- Layout ----------

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutControls();
        }

        // Both modes, top to bottom: header, choose a window, options (full only), Center, status
        void LayoutControls()
        {
            if (status == null) return;
            int W = ClientSize.Width, H = ClientSize.Height;
            int m = Theme.D(compact ? 16 : 24), gap = Theme.D(8), b = Theme.D(40);

            // Header buttons, right-aligned, centered on the logo
            int yHeader = compact ? Theme.D(12) : Theme.D(24);
            modeButton.SetBounds(W - m - b + Theme.D(4), yHeader, b, b);
            pinButton.SetBounds(modeButton.Left - b, yHeader, b, b);
            langButton.SetBounds(pinButton.Left - b, yHeader, b, b);

            if (compact)
            {
                int y = Theme.D(64), s = Theme.D(56);
                picker.SetBounds(W - m - s, y, s, s);
                field.SetBounds(m, y, picker.Left - gap - m, s);
                center.SetBounds(m, field.Bottom + Theme.D(12), W - 2 * m, Theme.D(48));
                status.SetBounds(m, center.Bottom + Theme.D(12), W - 2 * m, H - center.Bottom - Theme.D(12) - m);
                Invalidate();
                return;
            }

            // Bottom-up: status, then the action row (area on the left, Center on the right)
            status.SetBounds(m, H - m - Theme.D(56), W - 2 * m, Theme.D(56));
            int yRow = status.Top - Theme.D(16) - Theme.D(48);
            int cw = Math.Max(Theme.D(160), center.PreferredWidth());
            center.SetBounds(W - m - cw, yRow, cw, Theme.D(48));
            area.SetBounds(m, yRow + Theme.D(4), Math.Min(area.PreferredWidth(), center.Left - m - Theme.D(16)), Theme.D(40));
            yArea = yRow - Theme.D(28);

            // Screen chips, wrapped on several lines if needed
            int x = m, lines = 1;
            List<Point> spots = new List<Point>();
            foreach (Chip c in chips)
            {
                int w = c.PreferredWidth();
                if (x > m && x + w > W - m) { x = m; lines++; }
                spots.Add(new Point(x, lines - 1));
                x += w + gap;
            }
            int chipsHeight = lines * Theme.D(32) + (lines - 1) * gap;
            int yChips = yArea - Theme.D(16) - chipsHeight;
            for (int i = 0; i < chips.Count; i++)
                chips[i].SetBounds(spots[i].X, yChips + spots[i].Y * Theme.D(40), chips[i].PreferredWidth(), Theme.D(32));

            // Section rows: title on the left, actions on the right
            yScreen = yChips - Theme.D(4) - b;
            identify.SetBounds(W - m - identify.PreferredWidth(), yScreen, identify.PreferredWidth(), b);

            yWindows = Theme.D(88);
            picker.SetBounds(W - m - picker.PreferredWidth(), yWindows, picker.PreferredWidth(), b);
            refresh.SetBounds(picker.Left - gap - refresh.PreferredWidth(), yWindows, refresh.PreferredWidth(), b);
            int yCard = yWindows + b + gap;
            card.SetBounds(m, yCard, W - 2 * m, Math.Max(Theme.D(80), yScreen - Theme.D(12) - yCard));
            Invalidate();
        }

        // Header and section titles
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Draw.Quality(g);
            int m = Theme.D(compact ? 16 : 24);
            int room = langButton.Left - Theme.D(16);
            if (compact)
            {
                int s = Theme.D(32);
                if (logo != null) g.DrawImage(logo, m, Theme.D(16), s, s);
                int tx = m + s + Theme.D(12);
                Draw.Text(g, "Pile Poil", Theme.Font(20, true), new Rectangle(tx, Theme.D(16), room - tx, s),
                    Theme.OnSurface, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                return;
            }
            int t = Theme.D(48);
            if (logo != null) g.DrawImage(logo, m, Theme.D(20), t, t);
            int x = m + t + Theme.D(16);
            Draw.Text(g, "Pile Poil", Theme.Font(24, true), new Rectangle(x, Theme.D(18), room - x, Theme.D(32)), Theme.OnSurface, TextFormatFlags.Left);
            Draw.Text(g, Lang.Tagline, Theme.Font(14, false), new Rectangle(x, Theme.D(50), room - x, Theme.D(20)), Theme.OnSurfaceVariant, TextFormatFlags.Left);

            Section(g, Lang.OpenWindows + "  ·  " + list.Items.Count, yWindows, Theme.D(40));
            Section(g, Lang.ScreenSection, yScreen, Theme.D(40));
            Section(g, Lang.AreaSection, yArea, Theme.D(20));
        }

        void Section(Graphics g, string title, int y, int height)
        {
            Draw.Text(g, title, Theme.Font(14, true), new Rectangle(Theme.D(28), y, Theme.D(400), height), Theme.Primary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        // ---------- Data ----------

        WindowInfo SelectedWindow() { return compact ? field.Window : list.Current; }

        List<WindowInfo> LoadWindows()
        {
            List<WindowInfo> windows = Program.Demo ? DemoWindows() : Native.Windows(myPid);
            if (!Program.Demo)
                foreach (WindowInfo w in windows) w.Icon = CachedIcon(w.Handle);
            return windows;
        }

        Bitmap CachedIcon(IntPtr h)
        {
            Bitmap icon;
            if (!iconCache.TryGetValue(h, out icon))
            {
                icon = Native.WindowIcon(h);
                iconCache[h] = icon;
            }
            return icon;
        }

        void RefreshAll()
        {
            RefreshScreens();
            RefreshWindows(list.Current != null ? list.Current.Handle : IntPtr.Zero);
            status.SetMessage(string.Format(Lang.Found, list.Items.Count, chips.Count - 1), StatusKind.Info);
        }

        void RefreshWindows(IntPtr select)
        {
            list.BeginUpdate();
            list.Items.Clear();
            foreach (WindowInfo w in LoadWindows())
            {
                int i = list.Items.Add(w);
                if (w.Handle == select) list.SelectedIndex = i;
            }
            list.EndUpdate();
            Invalidate(new Rectangle(0, yWindows, ClientSize.Width, Theme.D(40)));
        }

        void RefreshScreens()
        {
            foreach (Chip c in chips) { Controls.Remove(c); c.Dispose(); }
            chips.Clear();

            List<ScreenInfo> screens = Program.Demo ? DemoScreens() : Native.Screens();
            bool stillThere = false;
            foreach (ScreenInfo s in screens) if (s.Handle == chosenMonitor) stillThere = true;
            if (!stillThere) chosenMonitor = IntPtr.Zero;

            AddChip(new Chip(Lang.CurrentScreen, IntPtr.Zero));
            foreach (ScreenInfo s in screens)
            {
                Chip c = new Chip(s.Name, s.Handle);
                c.Detail = s.Width + "×" + s.Height + (s.ScalePercent != 100 ? " · " + s.ScalePercent + " %" : "") +
                           (s.Primary ? " · " + Lang.Primary : "");
                AddChip(c);
            }
            LayoutControls();
        }

        void AddChip(Chip c)
        {
            c.Selected = c.Monitor == chosenMonitor;
            c.Visible = !compact;
            c.Click += delegate
            {
                chosenMonitor = c.Monitor;
                foreach (Chip other in chips) other.Selected = other == c;
                LayoutControls(); // the selected chip gets wider (check mark)
            };
            chips.Add(c);
            Controls.Add(c);
        }

        // Compact mode window list. Built once and reused: disposing it while it closes crashes.
        ToolStripDropDown windowMenu;
        WindowList menuList;
        ToolStripControlHost menuHost;

        void ShowWindowMenu()
        {
            if (windowMenu == null)
            {
                menuList = new WindowList();
                menuHost = new ToolStripControlHost(menuList);
                menuHost.Margin = Padding.Empty;
                menuHost.Padding = Padding.Empty;
                menuHost.AutoSize = false;

                windowMenu = new ToolStripDropDown();
                windowMenu.Renderer = new DarkRenderer();
                windowMenu.Padding = new Padding(Theme.D(4));
                windowMenu.Items.Add(menuHost);

                menuList.MouseClick += delegate
                {
                    WindowInfo w = menuList.Current;
                    if (w == null) return;
                    field.Window = w;
                    windowMenu.Close();
                    status.SetMessage(string.Format(Lang.Selected, w.Process, w.Title), StatusKind.Info);
                };
            }

            menuList.BeginUpdate();
            menuList.Items.Clear();
            foreach (WindowInfo w in LoadWindows()) menuList.Items.Add(w);
            menuList.EndUpdate();
            int rows = Math.Max(1, Math.Min(5, menuList.Items.Count));
            menuList.Size = new Size(field.Width - Theme.D(8), rows * menuList.ItemHeight);
            menuHost.Size = menuList.Size;
            windowMenu.Show(field, new Point(0, field.Height + Theme.D(4)));
        }

        // ---------- Actions ----------

        void CenterSelected()
        {
            WindowInfo w = SelectedWindow();
            if (w == null)
            {
                status.SetMessage(Lang.NoSelection, StatusKind.Error);
                return;
            }
            if (Program.Demo) return;
            bool ok;
            string message = Native.CenterWindow(w.Handle, Settings.FullScreenArea, compact ? IntPtr.Zero : chosenMonitor, out ok);
            status.SetMessage(w.Process + " : " + message, ok ? StatusKind.Success : StatusKind.Error);
            if (compact) field.Window = Native.Describe(w.Handle).WithIcon(w.Icon);
            else RefreshWindows(w.Handle);
        }

        // Picker: the pressed button captures the mouse; follow the window under it
        void PickerDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || Program.Demo) return;
            dragging = true;
            target = IntPtr.Zero;
            picker.Active = true;
            picker.Invalidate();
            status.SetMessage(Lang.Hover, StatusKind.Accent);
        }

        void PickerMove(object sender, MouseEventArgs e)
        {
            if (!dragging) return;
            IntPtr h = Native.WindowUnderCursor(myPid);
            if (h == target) return;
            target = h;
            if (h == IntPtr.Zero)
            {
                highlight.HideFrame();
                status.SetMessage(Lang.Hover, StatusKind.Accent);
            }
            else
            {
                highlight.Cover(h);
                WindowInfo w = Native.Describe(h);
                status.SetMessage(w.Process + "  ·  " + w.Title + "  ·  " + w.Width + " × " + w.Height + "  ·  " + w.Screen, StatusKind.Accent);
            }
        }

        void PickerUp(object sender, MouseEventArgs e)
        {
            if (!dragging) return;
            dragging = false;
            highlight.HideFrame();
            picker.Active = false;
            picker.Invalidate();
            if (target == IntPtr.Zero)
            {
                status.SetMessage(Lang.NoTarget, StatusKind.Error);
                return;
            }
            if (compact)
            {
                // Compact mode: center right away
                field.Window = Native.Describe(target).WithIcon(CachedIcon(target));
                CenterSelected();
                return;
            }
            RefreshWindows(target);
            WindowInfo w = list.Current;
            if (w != null) status.SetMessage(string.Format(Lang.Selected, w.Process, w.Title), StatusKind.Info);
            center.Focus();
        }

        // Enter = center · F5 = refresh · Esc = quit
        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            if (key == Keys.Enter) { CenterSelected(); return true; }
            if (key == Keys.F5) { RefreshAll(); return true; }
            if (key == Keys.Escape) { Close(); return true; }
            return base.ProcessCmdKey(ref msg, key);
        }

        // Coming back to Pile Poil refreshes the list (full mode)
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!dragging && !compact) RefreshWindows(list.Current != null ? list.Current.Handle : IntPtr.Zero);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (highlight != null) highlight.Dispose();
            if (windowMenu != null) windowMenu.Dispose();
            base.OnFormClosed(e);
        }

        // ---------- Demo data (--demo) ----------

        static List<WindowInfo> DemoWindows()
        {
            string s1 = string.Format(Lang.ScreenName, 1), s2 = string.Format(Lang.ScreenName, 2);
            object[,] d = {
                { "Ankama Launcher", "Ankama Launcher", 1280, 760, s1, false },
                { "Code", "launchers.bat - Scripts - Visual Studio Code", 1600, 900, s2, false },
                { "brave", "GitHub - Sokenzane/PilePoil - Brave", 1920, 1040, s2, true },
                { "Discord", "#general - Discord", 1180, 720, s2, false },
                { "explorer", "Projects", 960, 620, s1, false },
                { "Spotify", "Spotify Premium", 1100, 700, s1, false } };
            List<WindowInfo> l = new List<WindowInfo>();
            for (int i = 0; i < d.GetLength(0); i++)
            {
                WindowInfo w = new WindowInfo();
                w.Handle = new IntPtr(i + 1);
                w.Process = (string)d[i, 0];
                w.Title = (string)d[i, 1];
                w.Width = (int)d[i, 2];
                w.Height = (int)d[i, 3];
                w.Screen = (string)d[i, 4];
                w.Maximized = (bool)d[i, 5];
                l.Add(w);
            }
            return l;
        }

        static List<ScreenInfo> DemoScreens()
        {
            List<ScreenInfo> l = new List<ScreenInfo>();
            for (int i = 1; i <= 2; i++)
            {
                ScreenInfo s = new ScreenInfo();
                s.Handle = new IntPtr(100 + i);
                s.Number = i;
                s.Name = string.Format(Lang.ScreenName, i);
                s.Width = 1920;
                s.Height = 1080;
                s.ScalePercent = 100;
                s.Primary = i == 1;
                l.Add(s);
            }
            return l;
        }
    }
}
