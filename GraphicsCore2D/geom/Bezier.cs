using audionysos.math;
using System;
using P = audionysos.geom.IPoint2;
using PE = audionysos.geom.IPoint2Extensions;

namespace cnc.geom; 
/// <summary>Provides some methods to work on bezier curves.</summary>
public static class Bezier {

	/// <summary>Returns point on Bezier curve of any degree between fist and last point in given set.</summary>
	/// <param name="t">Position of point on curve. 0 will returns fist and 1 last point of given set.</param>
	/// <param name="points">List of curve points including endpoints and control points.</param>
	public static P interpolate(double t = 0.5, params P[] points) {
		var ps = (P[])points.Clone(); var c = ps.Length;
		while (c-- > 1) 
			for (var i = 1; i <= c; i++)
				ps[i - 1] = PE.interpolate(ps[i - 1], ps[i], t);
		return ps.Length > 0 ? ps[0] : null;
	}

	/// <summary>Returns list of points forming straights segments along given Bezier curve of any degree.
	/// This method produces interpolation points of evenly distributed steps which not considers curvature i.e. approximation error is varying across the curve.</summary>
	/// <param name="ns">Number of segments.</param>
	/// <param name="ps">List of curve points including endpoints and control points.</param>
	/// <returns>Points forming path of straight segments.</returns>
	public static P[] render(int ns, P[] ps) {
		if (ns <= 0) return new P[0];
		var tps = 1d / ns;
		var o = new P[ns + 1];
		for (var i = 0; i <= ns; i++)
			o[i] = interpolate(i * tps, ps);
		return o;
	}

	/// <summary>Finds point on a curve segment that has <paramref name="x"/> component of specified value.
	/// This only work for flat curves (where <see cref="x"/> lies between endpoints and only single point exist) - used for normalized value weight smoothing.</summary>
	/// <param name="x"></param>
	/// <param name="md"></param>
	/// <param name="points"></param>
	/// <returns></returns>
	public static P findPointForX(double x, double md = 0.0001, params P[] points) {
		var s = 0d; var e = 1d;
		var p1 = interpolate(s, points);
		var p2 = interpolate(e, points);
		if (p1.x < x && p2.x < x
			|| p1.x > x && p2.x > x) return null;
		p1.x.equalsT(x, md); var c = 0;
		P mp; do {
			var t = s + (e - s) * .5;
			mp = interpolate(t, points);
			if (mp.x < x) s = t; 
			else e = t;
		} while (!mp.x.equalsT(x, md) && c++ < 1000);
		return mp;
	}

}
