using audionysos.display;
using audionysos.geom;
using cnc.geom;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace com.audionysos {

	/// <summary>Provides general interface for creating simple 2D graphics through given base drawer (other graphics object).
	/// The library specifies three main interfaces for the graphics implementation that correspond to levels of capabilities which they may provide.
	/// Those are as follows: <see cref="IMicroGraphics2D"/>, <see cref="IBasicGraphics2D"/> and <see cref="IGraphics2D"/>.
	/// This class is essentially a wrapper for other types implementing those interfaces, while itself implementing the most complex one.
	/// Depending on capabilities of the base drawer, this class uses methods implemented by the base drawer or otherwise uses it's own, generic/standard implementations.
	/// Apart from providing default implementation for more basic implementors this class may be adopted for more specialized scenarios, caching of instruction (see <see cref="CachedGraphics"/>) context switching etc.
	public class Graphics : IGraphics2D, IPoint2 {

		private IMicroGraphics2D m;
		/// <summary>Underlaying graphics on which actual drawing is performed.</summary>
		internal IMicroGraphics2D baseGraphics => m;
		private IBasicGraphics2D b;
		private IGraphics2D g;

		#region Position
		/// <summary>Current position from which next figure will be drawn.</summary>
		public IPoint2 p => this;
		/// <summary>See <see cref="IMicroGraphics2D.x"/>/y. Setter is not doing anything (only for <see cref="IPoint2"/> implementation).</summary>
		public double x { get => m.x; set { } }
		/// <summary>See <see cref="IMicroGraphics2D.x"/>/y. Setter is not doing anything (only for <see cref="IPoint2"/> implementation).</summary>
		public double y { get => m.y; set { } }

		#region Stupid point implementation
		/// <summary>IPoint implementation - returns copy of current draw position.</summary>
		public IPoint2 copy() => new Point2(m.x, m.y);
		/// <summary>IPoint implementation - returns new point.</summary>
		public IPoint2 create(double x, double y) => new Point2(m.x, m.y);
		#endregion

		#endregion

		/// <summary></summary>
		/// <param name="baseDrawer">Graphics object to be wrapped.
		/// This object will not be exposed externally but you may changed it later using <see cref="changeBaseDrawer(IMicroGraphics2D, IMicroGraphics2D)"/> method.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public Graphics(IMicroGraphics2D baseDrawer) {
			m = baseDrawer ?? throw new ArgumentNullException();
			b = baseDrawer as IBasicGraphics2D;
			g = baseDrawer as IGraphics2D;
		}

		/// <summary></summary>
		/// <param name="old">Object that was previous base drawer for this graphics.</param>
		/// <param name="nev">New base drawer to replace old one with.</param>
		/// <exception cref="InvalidOperationException">You must provide old drawer as a proof that you "own" the graphics object.</exception>
		/// <exception cref="ArgumentNullException"></exception>
		public void changeBaseDrawer(IMicroGraphics2D old, [DisallowNull]IMicroGraphics2D nev) {
			if (m != old) throw new InvalidOperationException("Old base drawer is a different instance.");
			m = nev ?? throw new ArgumentNullException();
			b = nev as IBasicGraphics2D;
			g = nev as IGraphics2D;
		}

		public IMicroGraphics2D lineStyle(double w = 0, uint rgb = 0, double a = 1)
			=> m.lineStyle(w, rgb, a);

		#region Fill
		public IMicroGraphics2D beginFill(uint rgb, double a = 1) => m.beginFill(rgb, a);

		public IMicroGraphics2D beginFill(IFillPiece fill) => m.beginFill(fill);

		public IMicroGraphics2D endFill() => m.endFill();
		#endregion

		public IMicroGraphics2D moveTo(double x, double y) => m.moveTo(x, y);

		public IMicroGraphics2D lineTo(double x, double y) => m.lineTo(x, y);

		public IMicroGraphics2D lineTo(IPoint2 p) => m.lineTo(p.x, p.y);

		#region Paths
		public IBasicGraphics2D drawPath(IReadOnlyList<IPoint2> points) {
			if(b != null) return b.drawPath(points);
			for (int i = 0; i < points.Count; i++)
				lineTo(points[i]);
			return this;
		}

		#endregion

		#region Bezier
		/// <summary>Number of straight edges of which a single bezier segment is composed if base drawer does not implement it's own method.</summary>
		private int bezierEdges = 30;

		public IBasicGraphics2D bezierTo(IPoint2 p1, IPoint2 p2, IPoint2 p3) {
			if (b != null) return b.bezierTo(p1, p2, p3);
			bezierTo(new[] { p1, p2, p3 });
			return this;
		}

		public IBasicGraphics2D bezierTo(IPoint2 p1, IPoint2 p2, IPoint2 p3, IPoint2 p4) {
			if (b != null) return b.bezierTo(p1, p2, p3, p4);
			bezierTo(new[] { p1, p2, p3, p4 });
			return this;
		}

		public IBasicGraphics2D bezierTo(params IPoint2[] points) {
			var cpts = Bezier.render(bezierEdges, points);
			drawPath(cpts);
			return this;
		}
		#endregion

		#region Circular

		public IGraphics2D drawCircle(double x = 0, double y = 0, double r = 5) {
			if (g != null) return g.drawCircle(x, y, r);
			var p2 = new Point2(p.x, p.y);
			var p3 = new Point2(p.x, p.y);
			var f = 0.552284749831;
			p.y -= r * f;
			p2.x = x - r * f; p2.y -= r;
			p3.x += r; p3.y -= r;
			bezierTo(p, p2, p3);
			p.y -= 0; p.x = x + r;
			p2.x = x + r * f; p2.y -= 0;
			p3.x += r; p3.y += r;
			bezierTo(p2, p, p3);
			p.x += 0; p.y = y + r * f;
			p2.x += 0; p2.y = y + r;
			p3.x = x; p3.y = y + r;
			bezierTo(p, p2, p3);
			p.x = x - r; p.y += 0;
			p2.x = x - r * f; p2.y += 0;
			p3.x = x - r; p3.y = y;
			bezierTo(p2, p, p3);
			return this;
		}

		public Graphics drawCircle(IPoint2 p, double r = 5) {
			drawCircle(p.x, p.y, r); return this;
		}

		public IGraphics2D arcTo(IPoint2 ep, IPoint2 r, double d, int fa = 0, int fs = 0, double e = 0.5) {
			if (g != null) return g.arcTo(ep, r, d, fa, fs, e);
			var bps = Ellipse.renderR(p, ep, r, d, fa, fs, e);
			drawPath(bps);
			return this;
		}
		#endregion

		public IGraphics2D drawTriangles(IPoint2[] points, IPoint2[] uv, IFillPiece fill) {
			throw new NotImplementedException();
		}

		#region Steering controls
		public IMicroGraphics2D clear() => m.clear();

		public IMicroGraphics2D close() => m.close();

		public IMicroGraphics2D wait() => m.wait();

		public IMicroGraphics2D transform(Transform t) {
			m.transform(t); return this;
		}

		#endregion

	}

}
