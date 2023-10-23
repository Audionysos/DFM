using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace audionysos.geom; 
public interface ILine2 {
	public IPoint2 a { get; }
	public IPoint2 b { get; }

	public ILine2 create(IPoint2 a, IPoint2 b);
}

public class Line2 : ILine2 {
	public IPoint2 a { get; }
	public IPoint2 b { get; }

	public Line2(IPoint2 a, IPoint2 b) {
		this.a = a;
		this.b = b;
	}

	public Line2((double x, double y) a, (double x, double y) b) {
		this.a = new Point2(a.x, a.y);
		this.b = new Point2(b.x, b.y);
	}

	//TODO: intersection method
	/// <summary>Presumobly this works</summary>
	/// <param name="l1"></param>
	/// <param name="l2"></param>
	/// <returns></returns>
	public static IPoint2 intersection(ILine2 l1, ILine2 l2) {
		if (   l1.a.x < l2.a.x && l1.a.x < l2.b.x
			&& l1.b.x < l2.b.x && l1.b.x < l2.a.x
			&& l1.a.y < l2.a.y && l1.a.y < l2.b.y
			&& l1.b.y < l2.b.y && l1.b.y < l2.a.y
		) return null;
		var (c1, c2, s1, s2, parallel) = orderLines(l1, l2);
		if (parallel) return null;

		double xMove;
		//c1.a.x should be 0 here
		double s; //combined slope
		double x = 0; double y = 0; //output
		//moving both lines so that first one starts on x=0
		xMove = -c1.a.x;
		c1.moveX(xMove); c2.moveX(xMove);
		//changing length of second line so it starts on x=0
		c2.a.y += -c2.a.x * s2;
		c2.a.x = 0;
		y = c2.a.y;

		if (double.IsInfinity(s1)) s1 = 0; //Y-Axis aligned
		if (double.IsInfinity(s2)) s2 = 0;
		s = Math.Abs(s1) + Math.Abs(s2);

		//sliding on second line
		if (s != 0) { //will be 0 if lines are perpendicular in which case start point is actual intersection
			var dy = Math.Abs(c2.a.y - c1.a.y);
			var dx = dy / s;
			x = c2.a.x + dx;
			y = c2.a.y + dx * s2;
		}
		x -= xMove; 
		var ip = l1.a.create(x, y);
		//check if result is in the actual lines bounds (at this point "ip" may be far outside on infinite lines, the input lines are part of).
		if(!new Rect(c1.moveX(-xMove)).isInside(ip)) return null;
		if(!new Rect(c2.moveX(-xMove)).isInside(ip)) return null;
		return ip;
	}

	/// <summary>Returns copies of given lines with their slopes.
	/// Output lines are rearranged so that their start points will always have x <= endpoints x.
	/// If second line is paraller to Y-Axis, the order of output lines is swapped.
	/// Additionally the method returns true as the last output if given lines are paraller to each other.
	/// </summary>
	private static (ILine2 o1, ILine2 o2, double s1, double s2, bool parallel) orderLines(ILine2 l1, ILine2 l2) {
		var o1 = l1.a.x > l1.b.x ? l1.swapPoints() : l1.copy();
		var o2 = l2.a.x > l2.b.x ? l2.swapPoints() : l2.copy();
		var s1 = o1.slope();
		var s2 = o2.slope();
		var p = (s1 == s2) || (double.IsInfinity(s1) && double.IsInfinity(s2));
		if (!double.IsInfinity(s2)) return (o1, o2, s1, s2, p);
		else return (o2, o1, s2, s1, p);
	}

	public static implicit operator Line2(
		((double x, double y) a,
		 (double x, double y) b
		) t
	) => new Line2(t.a, t.b);

	public override string ToString() {
		return $"[{a}->{b}]";
	}

	public ILine2 create(IPoint2 a, IPoint2 b)
		=> new Line2(a, b);
}

public static class ILine2Extensions {

	/// <summary>Returns amount of 'y' value change per change of 1 in 'x' coordinate.</summary>
	/// <param name="l"></param>
	/// <returns></returns>
	public static double slope(this ILine2 l) {
		var xd = l.b.x - l.a.x;
		var yd = l.b.y - l.a.y;
		if (xd == 0) return double.PositiveInfinity * Math.Sign(yd);
		//if (yd == 0) return 0;
		return yd / xd;
	}

	/// <summary>Makes deep copy of the line.</summary>
	public static T copy<T>(this T l) where T : ILine2
		=> (T)l.create(l.a.copy(), l.b.copy());

	/// <summary>Makes deep copy of the line with it's endpoints swapped.</summary>
	public static T swapPoints<T>(this T l) where T : ILine2
		=> (T)l.create(l.b.copy(), l.a.copy());

	/// <summary>Moves and returns this line.</summary>
	public static T moveX<T>(this T l, double x) where T : ILine2{
		l.a.x += x; l.b.x += x;
		return l;
	}

	/// <summary>Moves and returns this line.</summary>
	public static T move<T>(this T l, double x, double y) where T : ILine2 {
		l.a.x += x; l.b.x += x;
		l.a.y += y; l.b.y += y;
		return l;
	}

}
