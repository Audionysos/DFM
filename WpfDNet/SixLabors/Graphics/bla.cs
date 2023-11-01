using F = SixLabors.Fonts;
using SixLabors.Fonts;
using System.Numerics;
using X = audionysos.geom;
using System.Collections.Generic;
using System;

namespace WpfDNet; 
public class bla : F.IGlyphRenderer {
	public List<X.Path> phs = new ();
	private X.Path ph = null;
	private Vector2 cp;

	public TextDecorations EnabledDecorations() {
		return TextDecorations.None;
	}

	public void SetDecoration(TextDecorations textDecorations, Vector2 start, Vector2 end, float thickness)
		{}

	public void BeginText(in FontRectangle bounds) {}

	public bool BeginGlyph(in FontRectangle bounds, in GlyphRendererParameters parameters)
		=> true;

	public void BeginFigure() {
		//ph = new X.Path ();
		cp = new Vector2();
	}

	private float s = 1;
	public void LineTo(Vector2 p) {
		cp = p * s;
		ph.Add(cp.X, cp.Y);
	}

	public void MoveTo(Vector2 point) {
		if(ph != null) phs.Add(ph);
		ph = new X.Path();
		cp = point * s;
		ph.Add(cp.X, cp.Y);
	}

	public void QuadraticBezierTo(Vector2 c, Vector2 e) {
		var g = 10f;
		for (int i = 1; i <= g; i++) {
			var t = i / g;
			var m1 = mid(cp, c, t);
			var m2 = mid(c, e, t);
			cp = mid(m1, m2, t);
			cp *= s;
			ph.Add(cp.X, cp.Y);
		}
	}

	private Vector2 mid(Vector2 s, Vector2 e, float t)
		=> s + (e - s) * t;

	public void CubicBezierTo(Vector2 c, Vector2 c2, Vector2 e) {
		var g = 10f;
		for (int i = 0; i <= g; i++) {
			var t = i / g;
			var m1 = mid(cp, c, t);
			var m2 = mid(c, c2, t);
			var m3 = mid(c2, e, t);
			var m21 = mid(m1 , m2, t);
			var m22 = mid(m2 , m3, t);
			cp = mid(m21, m22, t);
			cp *= s;
			ph.Add(cp.X, cp.Y);
		}
	}

	public void EndFigure() {
		//var sp = ph[0];
		//ph.Add(sp);
		phs.Add(ph);
	}

	public void EndGlyph() {}

	public void EndText() {}


}
