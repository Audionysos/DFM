using audioysos.display;
using audioysos.geom;
using System;
using System.Collections.Generic;

namespace com.audionysos {
	public class CachedGraphics : IGraphics2D {
		public IPoint2 p { get; }
		public double x { get; }
		public double y { get; }

		private List<Action<IGraphics2D>> calls = new List<Action<IGraphics2D>>();

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

		public IGraphics2D drawTraingles(IPoint2[] points, IPoint2[] uv, IFillPiece fill) {
			calls.Add(g => g.drawTraingles(points, uv, fill));
			return this;
		}

		public IMicroGraphics2D endFill() {
			calls.Add(g => g.endFill());
			return this;
		}

		public IMicroGraphics2D lineSyle(double w = 0, uint rgb = 0, double a = 1) {
			calls.Add(g => g.lineSyle(w, rgb, a));
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
			calls.Add(g => g.transform(t));
			return this;
		}

		public IMicroGraphics2D wait() {
			return this;
		}
	}

}
