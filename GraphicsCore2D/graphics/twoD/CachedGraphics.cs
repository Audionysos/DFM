using audioysos.geom;
using System.Collections.Generic;

namespace com.audionysos {
	public class CachedGraphics : IGraphics2D {
		public IPoint2 p { get; }
		public double x { get; }
		public double y { get; }

		public IGraphics2D arcTo(IPoint2 ep, IPoint2 r, double d, int fa = 0, int fs = 0, double e = 0.5) {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D beginFill(uint rgb, double a = 1) {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D beginFill(IFillPiece fill) {
			throw new System.NotImplementedException();
		}

		public IBasicGraphics2D bezierTo(IPoint2 p1, IPoint2 p2, IPoint2 p3) {
			throw new System.NotImplementedException();
		}

		public IBasicGraphics2D bezierTo(IPoint2 p1, IPoint2 p2, IPoint2 p3, IPoint2 p4) {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D clear() {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D close() {
			throw new System.NotImplementedException();
		}

		public IGraphics2D drawCircle(double x = 0, double y = 0, double r = 5) {
			throw new System.NotImplementedException();
		}

		public IBasicGraphics2D drawPath(IReadOnlyList<IPoint2> points) {
			throw new System.NotImplementedException();
		}

		public IGraphics2D drawTraingles(IPoint2[] points, IPoint2[] uv, IFillPiece fill) {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D endFill() {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D lineSyle(double w = 0, uint rgb = 0, double a = 1) {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D lineTo(double x, double y) {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D moveTo(double x, double y) {
			throw new System.NotImplementedException();
		}

		public IMicroGraphics2D wait() {
			throw new System.NotImplementedException();
		}
	}

}
