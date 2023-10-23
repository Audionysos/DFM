using System;
using System.Runtime.Serialization;

namespace audionysos.geom; 
public class Point2 : IPoint2 {
	public double x { get; set; }
	public double y { get; set; }

	public Point2() { }

	public Point2(double x, double y) {
		this.x = x; this.y = y;
	}

	public IPoint2 copy() => new Point2(x, y);

	public IPoint2 create(double x, double y) => new Point2(x, y);

	public static implicit operator Point2((double x, double y) t)
		=> new Point2(t.x, t.y);

	/// <inheritdoc/>
	public override string ToString() {
		return $@"({x}, {y})";
	}
}

public class Rect : IRect<IPoint2> {
	public IPoint2 position { get; }
	public IPoint2 size { get; }

	public Rect(IPoint2 pos, IPoint2 size) {
		position = pos.copy();
		this.size = size.copy();
	}

	public Rect() {
		position = new Point2(double.MinValue, double.MinValue);
		size = new Point2(0, 0);
	}

	public Rect(ILine2 l) {
		position = l.a;
		size = l.b - l.a;
		setSize(size);
	}

	public Rect clear() {
		position.set(double.MinValue, double.MinValue);
		size.set(0, 0);
		return this;
	}

	public Rect grow(IPoint2 p) {
		if (position.isMin()) { position.set(p); return this; }
		if (size.isZero()) { setSize(p - position); return this; }

		//return this;
		var dx = p.x - position.x;
		if (dx < 0) { position.x = dx; size.x += dx; }
		else if (dx > size.x) size.x = dx;

		var dy = p.y - position.y;
		if (dy < 0) { position.y = dy; size.y += dy; }
		else if (dy > size.y) size.y = dy;
		return this;
	}

	private void setSize(IPoint2 s) {
		size.set(s);
		if (s.x < 0) { s.x = -s.x; position.x -= s.x; }
		if (s.y < 0) { s.y = -s.y; position.y -= s.y; }
	}
}
