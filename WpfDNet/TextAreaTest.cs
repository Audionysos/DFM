using com.audionysos.text.edit;
using com.audionysos.text.render;
using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using F = SixLabors.Fonts;
using SixLabors.Fonts;
using X = com.audionysos.text.render;
using audioysos.display;
using com.audionysos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using W = System.Windows;
using WM = System.Windows.Media;
using WMI = System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Linq;

namespace WpfDNet {
	public class TextAreaTest {
		public SLDiplaySurface surface;

		public TextAreaTest() {
			surface = new SLDiplaySurface();
			TextDisplayContext.defaulGlyphsProvider = new SLGlyphsProvider();
			var ta = new TextAreaView();
			surface.Add(ta.view);
			ta.text =
				"Chrząszcz brzmi w trzcinie w trzebżeszynie" +
				"\nJaba daba doom";
			surface.transferBitmap();
		}

	}

	public class SLDiplaySurface : DisplaySurface {
		(int x, int y) size = (500, 500);
		Image<Bgra32> image;
		public WMI.WriteableBitmap wpfBitmap { get; private set; }

		public SLDiplaySurface() {
			image = new Image<Bgra32>(size.x, size.y);

			wpfBitmap = new WMI.WriteableBitmap(size.x, size.y,
				96, 96,
				WM.PixelFormats.Bgra32,
				null);
			drawBackground();
		}

		private void drawBackground() {
			var p = new Star(100f, 100f, 5, 20f, 50f);
			image.Mutate(x => 
				x.Fill(SixLabors.ImageSharp.Color.LightGray));
		}

		public void transferBitmap() {
			var img = image.GetPixelMemoryGroup();
			var mg = img.ToArray()[0];
			var PixelData = MemoryMarshal.AsBytes(mg.Span).ToArray();

			wpfBitmap.Lock();
			wpfBitmap.WritePixels(new W.Int32Rect(0, 0, size.x, size.y),
				PixelData,
				size.x * 4, //stride (bytes per row);
				0, 0);
			wpfBitmap.Unlock();
		}

		public override Graphics createGraphics() {
			var sg = new SharpGraphics(image);
			var g = new com.audionysos.Graphics(sg);
			return g;
		}
	}

	public class SLGlyphsProvider : GlyphsProvider {

		public override X.Glyph get(char ch, ITextFormat f) {
			if (ch == ' ') return missingGlyph;
			var ff = F.SystemFonts.Find(f.font.name);
			var font = ff.CreateFont((float)f.size);
			var gl = font.GetGlyph(ch);
			var gi = gl.Instance;
			//gi.
			var min = gi.Height - font.EmSize;
			var scale = (float)f.size/11; var x = 0; var y = 0;
			var sf = (font.Ascender + font.Descender) / gi.ScaleFactor * scale;
			//var sf = (font.Ascender) / gi.ScaleFactor * scale;
			var s = new System.Numerics.Vector2(sf, -sf);
			var off = new System.Numerics.Vector2(0 + x, (gi.Height - min) * s.X + y);

			var cp = gi.ControlPoints[0] * s + off;
			var fp = cp;

			var color = 0u;
			var width = 0; var height = 0;

			var phs = new List<X.Path>();
			var ph = new X.Path();
			phs.Add(ph);

			ph.Add(cp.X, cp.Y);
			var nep = gi.EndPoints[0]; var epi = 0;
			var m = false;
			for (int i = 1; i < gi.ControlPoints.Length; i++) {
				cp = gi.ControlPoints[i] * s + off;
				if (m) {
					ph = new X.Path();
					phs.Add(ph);
					ph.Add(cp.X, cp.Y);
					m = false;
					fp = cp;
					continue;
				}
				ph.Add(cp.X, cp.Y);
				if (nep == i) {
					//g.lineTo(fp.X, fp.Y);
					epi++;
					if (epi < gi.EndPoints.Length)
						nep = gi.EndPoints[epi];
					m = true;
				}
			}

			var r = new X.Glyph(width, height, phs);
			return r;
		}
	}

}
