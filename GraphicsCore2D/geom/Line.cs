using System;
using System.Collections.Generic;
using System.Text;

namespace audionysos.geom {
	public interface ILine2 {
		public IPoint2 a { get; }
		public IPoint2 b { get; }

	}

	public class Line2 : ILine2 {
		public IPoint2 a { get; }
		public IPoint2 b { get; }

		public Line2(IPoint2 a, IPoint2 b) {
			this.a = a;
			this.b = b;
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
			var s1 = l1.slope(); var s2 = l2.slope();
			if (s1 == s2) return null;
			var s = s1 + s2;
			var dy = (l2.a.x - l1.a.x) - l1.a.y;
			var x = l1.a.x  + (dy / s);
			var y = l1.a.y + (l1.a.x - x) * s1;
			var ip = l1.a.create(x, y);
			return ip;
		}

	}

	public static class ILine2Extensions {

		public static double slope(this ILine2 l) {
			var xd = l.b.x - l.a.x;
			var yd = l.b.y - l.a.y;
			if (yd == 0) return 0;
			return xd / yd;
		}
	}

}
