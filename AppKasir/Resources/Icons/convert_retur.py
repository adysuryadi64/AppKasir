import cairosvg
import os

svg_dir = os.path.join(os.path.dirname(__file__), "svg")
out_dir = os.path.dirname(__file__)

files = ["retur-beli", "retur-jual"]
sizes = [20, 24]

for name in files:
    svg_path = os.path.join(svg_dir, f"{name}.svg")
    for size in sizes:
        out_path = os.path.join(out_dir, f"{name}_{size}.png")
        cairosvg.svg2png(url=svg_path, write_to=out_path, output_width=size, output_height=size)
        print(f"Saved: {out_path}")

print("Done.")
