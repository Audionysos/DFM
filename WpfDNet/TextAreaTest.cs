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

namespace WpfDNet {
	public class TextAreaTest {

		public TextAreaTest() {
			TextDisplayContext.defaulGlyphsProvider = new SLGlyphsProvider();
			var ta = new TextAreaView();
			ta.text = "Chrząszcz brzmi w trzcinie w trzebżeszynie";
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
			var scale = 1; var x = 0; var y = 0;
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

	//public class GlyphPathsProvider : IGlyphPathsProvider {
	//	public Path[] getPath(Glyph g) {
	//		g.
	//	}
	//}
}
