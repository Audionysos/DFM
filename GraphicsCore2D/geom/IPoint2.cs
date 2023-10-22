namespace audionysos.geom; 

using P = audionysos.geom.IPoint2;

public interface IPoint2 {
	double x { get; set; }
	double y { get; set; }

	P copy();

	/// <summary>Produce new instance with given coordinates.</summary>
	public P create(double x, double y);

	public static P operator -(P a, P b) => a.copy().sub(b);
	public static P operator +(P a, P b) => a.copy().add(b);
}

public static class IPoint2Extensions {

	public static P interpolate(P v1, P v2, double d) {
		return v2.copy().sub(v1).mul(d).add(v1);
	}

	/// <summary>Adds coordinates of some other point to this point and returns this point.</summary>
	/// <param name="a"></param>
	/// <param name="b">Other point which coordinates to add.</param>
	/// <returns></returns>
	public static P add(this P a, P b) {
		a.x += b.x; a.y += b.y; return a;
	}

	public static P sub(this P a, P b) {
		a.x -= b.x; a.y -= b.y; return a;
	}

	public static P mul(this P a, double n) {
		a.x *= n; a.y *= n; return a;
	}

	public static P set(this P a, double x, double y) {
		a.x = x; a.y = y; return a;
	}

	public static P set(this P a, IPoint2 o) {
		a.x = o.x; a.y = o.y; return a;
	}

	public static bool isMin(this P p) {
		return p.x == double.MinValue && p.y == double.MinValue;
	}

	public static bool isZero(this P a) {
		return a.x == 0 && a.y == 0;
	}
}

public interface IRect<T> where T : P {
	T postion { get; }
	T size { get; }
}
