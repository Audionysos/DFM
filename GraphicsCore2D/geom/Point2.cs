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

public class Rect : IRect<Point2> {
	public Point2 postion { get; }
	public Point2 size { get; }

	public Rect(Point2 pos, Point2 size) {
		postion = pos.copy() as Point2;
		size = size.copy() as Point2;
	}

	public Rect() {
		postion = new Point2(double.MinValue, double.MinValue);
		size = new Point2(0, 0);
	}

	public Rect clear() {
		postion.set(double.MinValue, double.MinValue);
		size.set(0, 0);
		return this;
	}

	public Rect grow(IPoint2 p) {
		if (size.isZero()) { postion.set(p); return this; }
		if (postion.isMin()) { postion.set(p); return this; }

		var dx = p.x - postion.x;
		if (dx < 0) p.x = dx;
		else if (dx > size.x) size.x = dx;

		var dy = p.y - postion.y;
		if (dy < 0) p.y = dy;
		else if (dy > size.y) size.y = dy;
		return this;
	}
}
