"""Converts Material Symbols SVG icons into the path strings used in src/PilePoil.cs.

Pile Poil draws its icons with GDI+ from simple path data (absolute M, L, C and Z
commands on a 24x24 grid), so the exe stays a single file with no icon font.

Usage:
    npm pack @material-symbols/svg-400        # Apache-2.0, by Google
    tar xzf material-symbols-svg-400-*.tgz
    pip install svgelements
    python tools/make_icon_paths.py package/outlined

Then paste the printed lines into the IconData dictionary in src/PilePoil.cs.
"""
import re
import sys

from svgelements import Close, CubicBezier, Line, Move, Path, QuadraticBezier

# Glyph name in PilePoil.cs -> Material Symbols file name
ICONS = {
    "Picker": "colorize",
    "Refresh": "refresh",
    "Center": "filter_center_focus",
    "Screen": "desktop_windows",
    "Check": "check",
    "Error": "error",
    "Info": "info",
    "PinOff": "keep",
    "PinOn": "keep-fill",
    "Compact": "close_fullscreen",
    "Expand": "open_in_full",
    "ChevronDown": "keyboard_arrow_down",
    "Language": "translate",
}


def convert(svg_text):
    d = re.search(r' d="([^"]+)"', svg_text).group(1)
    path = Path(d)
    path.approximate_arcs_with_cubics()

    def pt(x, y):  # viewBox "0 -960 960 960" -> 24x24
        return "%g %g" % (round(x / 40, 3), round((y + 960) / 40, 3))

    out = []
    for s in path.segments():
        if isinstance(s, Move):
            out.append("M" + pt(s.end.x, s.end.y))
        elif isinstance(s, Line):
            out.append("L" + pt(s.end.x, s.end.y))
        elif isinstance(s, CubicBezier):
            out.append("C" + pt(s.control1.x, s.control1.y) + " " + pt(s.control2.x, s.control2.y) + " " + pt(s.end.x, s.end.y))
        elif isinstance(s, QuadraticBezier):
            c1 = s.start + (s.control - s.start) * (2 / 3)
            c2 = s.end + (s.control - s.end) * (2 / 3)
            out.append("C" + pt(c1.x, c1.y) + " " + pt(c2.x, c2.y) + " " + pt(s.end.x, s.end.y))
        elif isinstance(s, Close):
            out.append("Z")
        else:
            raise ValueError("Unexpected segment: %r" % s)
    return "".join(out)


if __name__ == "__main__":
    folder = sys.argv[1] if len(sys.argv) > 1 else "package/outlined"
    for glyph, name in ICONS.items():
        with open("%s/%s.svg" % (folder, name), encoding="utf-8") as f:
            print('            { Glyph.%s, "%s" }, // %s' % (glyph, convert(f.read()), name))
