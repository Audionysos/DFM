// Ignore Spelling: rgb

using audionysos.display;
using audionysos.geom;
using System;
using System.Collections.Generic;

namespace com.audionysos; 

//TODO: Make sure are references are copied
/// <summary>Stores any calls with their arguments on a list.
/// This may be used as temp storage, when no actual surface is provided where the graphics could be drawn.
/// Stored calls can be later invoked on any other graphics object <see cref="transferTo(IGraphics2D)"/>.</summary>
public class CachedGraphics : IGraphics2D {
	public IPoint2 p { get; }
	public double x { get; }
	public double y { get; }

	private List<Action<IGraphics2D>> calls = new List<Action<IGraphics2D>>();

	/// <summary>Invokes all previous calls on given other graphics object.</summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public IGraphics2D transferTo(IGraphics2D other) {
		for (int i = 0; i < calls.Count; i++) {
			calls[i](other);
		}return this;
	}

	public IGraphics2D arcTo(IPoint2 ep, IPoint2 r, double d, int fa = 0, int fs = 0, double e = 0.5) {
		calls.Add(g => g.arcTo(ep, r, d, fa, fs, e));
		return this;
	}

	public IMicroGraphics2D beginFill(uint rgb, double a = 1) {
		calls.Add(g => g.beginFill(rgb, a));
		return this;
	}

	public IMicroGraphics2D beginFill(IFillPiece fill) {
		calls.Add(g => g.beginFill(fill));
		return this;
	}

	public IBasicGraphics2D bezierTo(IPoint2 p1, IPoint2 p2, IPoint2 p3) {
		calls.Add(g => g.bezierTo(p1, p2, p3));
		return this;
	}

	public IBasicGraphics2D bezierTo(IPoint2 p1, IPoint2 p2, IPoint2 p3, IPoint2 p4) {
		calls.Add(g => g.bezierTo(p1, p2, p3));
		return this;
	}

	public IMicroGraphics2D clear() {
		calls.Clear();
		return this;
	}

	public IMicroGraphics2D close() {
		calls.Add(g => g.close());
		return this;
	}

	public IGraphics2D drawCircle(double x = 0, double y = 0, double r = 5) {
		calls.Add(g => g.drawCircle(x,y,r));
		return this;
	}

	public IBasicGraphics2D drawPath(IReadOnlyList<IPoint2> points) {
		calls.Add(g => g.drawPath(points));
		return this;
	}

	public IGraphics2D drawTriangles(IPoint2[] points, IPoint2[] uv, IFillPiece fill) {
		calls.Add(g => g.drawTriangles(points, uv, fill));
		return this;
	}

	public IMicroGraphics2D endFill() {
		calls.Add(g => g.endFill());
		return this;
	}

	public IMicroGraphics2D lineStyle(double w = 0, uint rgb = 0, double a = 1) {
		calls.Add(g => g.lineStyle(w, rgb, a));
		return this;
	}

	public IMicroGraphics2D lineTo(double x, double y) {
		calls.Add(g => g.lineTo(x, y));
		return this;
	}

	public IMicroGraphics2D moveTo(double x, double y) {
		calls.Add(g => g.moveTo(x,y));
		return this;
	}

	public IMicroGraphics2D transform(Transform t) {
		var tc = t.copy();
		calls.Add(g => g.transform(tc));
		return this;
	}

	public IMicroGraphics2D wait() {
		return this;
	}
}
