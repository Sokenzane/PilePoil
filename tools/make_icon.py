"""Draws the Pile Poil icon and writes assets/pilepoil.ico + assets/logo.png.

Usage:  python tools/make_icon.py      (needs Pillow: pip install pillow)
"""
import os
from PIL import Image, ImageDraw

ASSETS = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "assets")
S = 1024  # drawn big, then downscaled for each size


def hexc(h):
    return tuple(int(h[i:i + 2], 16) for i in (1, 3, 5))


def lerp(a, b, t):
    return tuple(int(a[i] + (b[i] - a[i]) * t) for i in range(3))


def render():
    img = Image.new("RGBA", (S, S), (0, 0, 0, 0))

    # Background: diagonal purple gradient (Material 3 primary), rounded square
    grad = Image.new("RGBA", (S, S))
    a, b = hexc("#8B6FD6"), hexc("#4F378B")
    px = grad.load()
    for y in range(S):
        for x in range(S):
            px[x, y] = lerp(a, b, (x + y) / (2 * S - 2)) + (255,)
    mask = Image.new("L", (S, S), 0)
    ImageDraw.Draw(mask).rounded_rectangle([0, 0, S - 1, S - 1], radius=int(S * 0.23), fill=255)
    img.paste(grad, (0, 0), mask)
    d = ImageDraw.Draw(img)

    # Centered window with a title bar
    ww, wh = int(S * 0.44), int(S * 0.34)
    x0, y0 = (S - ww) // 2, (S - wh) // 2
    r = int(S * 0.05)
    d.rounded_rectangle([x0, y0, x0 + ww, y0 + wh], radius=r, fill=hexc("#F6EDFF"))
    tb = int(wh * 0.26)
    d.rounded_rectangle([x0, y0, x0 + ww, y0 + tb + r], radius=r, fill=hexc("#D0BCFF"))
    d.rectangle([x0, y0 + tb, x0 + ww, y0 + tb + r], fill=hexc("#F6EDFF"))

    # Target dot
    c = S // 2
    pr = int(S * 0.035)
    cy = y0 + tb + (wh - tb) // 2
    d.ellipse([c - pr, cy - pr, c + pr, cy + pr], fill=hexc("#6750A4"))

    # Crosshair: 4 strokes pointing at the window
    w = int(S * 0.055)
    col = hexc("#EADDFF")
    m, gap = int(S * 0.11), int(S * 0.045)

    def stroke(xa, ya, xb, yb):
        d.line([xa, ya, xb, yb], fill=col, width=w)
        for (x, y) in ((xa, ya), (xb, yb)):
            d.ellipse([x - w // 2, y - w // 2, x + w // 2, y + w // 2], fill=col)

    stroke(m, c, x0 - gap, c)
    stroke(S - m, c, x0 + ww + gap, c)
    stroke(c, m, c, y0 - gap)
    stroke(c, S - m, c, y0 + wh + gap)
    return img


if __name__ == "__main__":
    big = render()
    sizes = [16, 20, 24, 32, 40, 48, 64, 128, 256]
    imgs = [big.resize((t, t), Image.LANCZOS) for t in sizes]
    imgs[-1].save(os.path.join(ASSETS, "pilepoil.ico"), sizes=[(t, t) for t in sizes], append_images=imgs[:-1])
    big.resize((128, 128), Image.LANCZOS).save(os.path.join(ASSETS, "logo.png"))
    print("assets/pilepoil.ico and assets/logo.png written")
