using System;
using System.Collections.Generic;
using System.Text;

namespace audioysos.geom {
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

		public static IPoint2 intersection(ILine2 l1, ILine2 l2) {

			return null;
		}

	}

	public static class ILine2Extensions {
		

	}

}
