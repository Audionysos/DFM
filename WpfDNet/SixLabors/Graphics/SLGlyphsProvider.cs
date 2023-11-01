using audionysos.geom;
using com.audionysos.text.edit;
using com.audionysos.text.render;
using SixLabors.Fonts;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using F = SixLabors.Fonts;
using X = com.audionysos.text.render;

namespace WpfDNet; 
public class SLGlyphsProvider : GlyphsProvider {

	public X.Glyph get2(char ch, ITextFormat f) {
		if (ch == ' ') return missingGlyph;
		var font = F.SystemFonts.CreateFont(f.font.name, (float)f.size);
		font.TryGetGlyphs(new F.Unicode.CodePoint(ch), out var glyphs);
		var g = glyphs[0];
		var b = new bla();
		TextRenderer.RenderTextTo(b,  ch.ToString(), new TextOptions(font) { 
			
		});
		var rc = g.BoundingBox(GlyphLayoutMode.Horizontal
			,new System.Numerics.Vector2(), 128);
		var width = rc.Width; var height = 0;
		var r = new X.Glyph(ch.ToString(), width, height, b.phs);
		return r;
	}

	public override X.Glyph get(char ch, ITextFormat f) {
		if (ch == ' ') return missingGlyph;
		var font = F.SystemFonts.CreateFont(f.font.name, (float)f.size);

		var phs = SixLabors.ImageSharp.Drawing.TextBuilder
			.GenerateGlyphs(ch.ToString(), new TextOptions(font)
		);
		var paths = new List<Path>();
		foreach (var ph in phs) {
			var fp = ph.Flatten();
			foreach (var sp in fp) {
				var points = sp.Points.ToArray();
				var path = new Path();
				foreach (var p in points)
					path.Add(p.X, p.Y);
				paths.Add(path);
			}
		}

		font.TryGetGlyphs(new F.Unicode.CodePoint(ch), out var glyphs);
		var g = glyphs[0];
		var rc = g.BoundingBox(GlyphLayoutMode.Horizontal
			, new System.Numerics.Vector2(), 128);
		var width = rc.Width; var height = 0;

		var r = new X.Glyph(ch.ToString(), width, height, paths);
		return r;
	}
}


//public override X.Glyph get(char ch, ITextFormat f) {
//	if (ch == ' ') return missingGlyph;
//	var ff = F.SystemFonts.Find(f.font.name);
//	var font = ff.CreateFont((float)f.size);
//	var gl = font.GetGlyph(ch);
//	var gi = gl.Instance;
//	//gi.
//	var min = gi.Height - font.EmSize;
//	var scale = (float)f.size / 11; var x = 0; var y = 0;
//	var sf = (font.Ascender + font.Descender) / gi.ScaleFactor * scale;
//	//var sf = (font.Ascender) / gi.ScaleFactor * scale;
//	var s = new System.Numerics.Vector2(sf, -sf);
//	var off = new System.Numerics.Vector2(0 + x, (gi.Height - min) * s.X + y);

//	var cp = gi.ControlPoints[0] * s + off;
//	var fp = cp;

//	var color = 0u;
//	var width = 0; var height = 0;

//	var phs = new List<X.Path>();
//	var ph = new X.Path();
//	phs.Add(ph);

//	ph.Add(cp.X, cp.Y);
//	var nep = gi.EndPoints[0]; var epi = 0;
//	var m = false;
//	for (int i = 1; i < gi.ControlPoints.Length; i++) {
//		cp = gi.ControlPoints[i] * s + off;
//		if (m) {
//			ph = new X.Path();
//			phs.Add(ph);
//			ph.Add(cp.X, cp.Y);
//			m = false;
//			fp = cp;
//			continue;
//		}
//		ph.Add(cp.X, cp.Y);
//		if (nep == i) {
//			//g.lineTo(fp.X, fp.Y);
//			epi++;
//			if (epi < gi.EndPoints.Length)
//				nep = gi.EndPoints[epi];
//			m = true;
//		}
//	}

//	var r = new X.Glyph(width, height, phs);
//	return r;
//}