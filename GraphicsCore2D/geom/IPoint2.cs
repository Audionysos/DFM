namespace audionysos.geom;

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using P = audionysos.geom.IPoint2;


public interface IPoint2  {
	double x { get; set; }
	double y { get; set; }

	P copy();

	/// <summary>Produce new instance with given coordinates.</summary>
	public P create(double x, double y);

	public static P operator -(P a, P b) => a.copy().sub(b);
	public static P operator +(P a, P b) => a.copy().add(b);
	public static P operator +(P a, (double x, double y)t)
		=> a.copy().add(t.x, t.y);

	public string ToRawString() {
		return $@"{x.ToString(CultureInfo.InvariantCulture)}, {y.ToString(CultureInfo.InvariantCulture)}";
	}
}

public static class IPoint2Extensions {

	public static P interpolate(P v1, P v2, double d) {
		return v2.copy().sub(v1).mul(d).add(v1);
	}

	public static bool equal(this P p, P o) {
		if (p == null && o == null) return true;
		if (p == null || o == null) return false;
		return p.x == o.x && p.y == o.y;
	}

	public static T copy<T>(this T p) where T : P
		=> (T)p.copy();

	/// <summary>Adds coordinates of some other point to this point and returns this point.</summary>
	/// <param name="a"></param>
	/// <param name="b">Other point which coordinates to add.</param>
	/// <returns></returns>
	public static P add(this P a, P b) {
		a.x += b.x; a.y += b.y; return a;
	}

	public static T add<T>(this T a, double x, double y) where T : P {
		a.x += x; a.y += y; return a;
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

	/// <summary>Sets coordinates and returns this point.</summary>
	public static P set(this P a, P o) {
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
	T position { get; }
	T size { get; }

	double right => position.x + size.x;
	double bottom => position.y + size.y;

	
}

public static class IRectExtensions {
	/// <summary>Returns true if given point lays inside or on the edge of the rectangle.</summary>
	public static bool isInside<T>(this IRect<T> r, T p) where T : P {
		if (p.x < r.position.x) return false;
		if (p.y < r.position.y) return false;
		if (p.x > r.right) return false;
		if (p.y > r.bottom) return false;
		return true;
	}
}
