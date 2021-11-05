using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;
using V = audioysos.geom.IPoint2;

namespace audioysos.geom {

	/// <summary>Provides copied dirty method for producing an ellipse, hopefully I did't broke it.</summary>
	public class Ellipse {

		/// <summary>Debug variable</summary>
		public static double dv1 = 0;

		/// <summary>Debug variable</summary>
		public static double dv2 = 0;

		/// <summary>Debug variable</summary>
		public static double dv3 = 0;

		/// <summary>Renders ellipse. Based on SVG standard.</summary>
		/// <param name="sp">Ellipse start point.</param>
		/// <param name="ep">Ellipse end point.</param>
		/// <param name="r">Radii for x and y.</param>
		/// <param name="d">Rotation of the ellipse in degrees</param>
		/// <param name="fa">Large arc flag.</param>
		/// <param name="fs">Sweep flag.</param>
		/// <param name="e">Rounding error.</param>
		/// <returns>List of points forming a path along the ellipse.</returns>
		public static List<V> renderR(V sp, V ep, V r, double d, int fa = 0, int fs = 0, double e = 0.5) {
			var rv = new List<V>();
			e = 0.01;
			d = d / 180 * PI;
			var rm = new Matrix(2, //ellipse rotation matrix
				Cos(d), -Sin(d),
				Sin(d), Cos(d));


			var angs = arcTo(sp.x, sp.y, r.x, r.y, d * 180 / PI, fa > 0, fs > 0, ep.x, ep.y, null);
			//var angsd = (angs.se / PI * 180, angs.ee / PI * 180);
			r = angs.r;
			//angs.cp.y *= -1;

			var ca = 0d;
			var aadd = dv1 * PI / 180;
			var a = angs.se + aadd;
			var ta = angs.ee;
			var ea = a + ta;

			var apex = 2 * Acos((e - r.x) / -r.x); //angle per edge for x radius
			var apey = 2 * Acos((e - r.y) / -r.y); //angle per edge for y radius
			var ad = apey - apex; //angle per edge difference

			var dir = Sign(ta);
			//if (fs > 0) dir *= -1;
			//if (ta > 0) ta *= -1;
			//if (a > ta && ad > 0 || 
			//	a < ta && ad < 0) dir = -1;
			for (; (dir < 0 && ca > ta) || (dir > 0 && ca < ta);) {
				var v = sp.create( //local vertex
					r.x * Cos(a),
					r.y * Sin(a)
				);
				rv.Add((rm * v).add(angs.cp));
				var lar = (Cos(a * 2) + 1) / 2 * ad;
				var g = dir * (apey - lar);
				a += g;
				ca += g;
			}
			var lv = sp.create( //local vertex
				r.x * Cos(ea),
				r.y * Sin(ea)
			);
			rv.Add((rm * lv).add(angs.cp));
			return rv;
		}

		//https://github.com/BigBadaboom/androidsvg/blob/master/androidsvg/src/main/java/com/caverock/androidsvg/SVGAndroidRenderer.java
		private static (double se, double ee, Point2 r, Point2 cp) arcTo(double lastX, double lastY, double rx, double ry, double angle, bool largeArcFlag, bool sweepFlag, double x, double y, string pather) {
			//if (lastX == x && lastY == y) {
			//	// If the endpoints (x, y) and (x0, y0) are identical, then this
			//	// is equivalent to omitting the elliptical arc segment entirely.
			//	// (behaviour specified by the spec)
			//	return;
			//}

			//// Handle degenerate case (behaviour specified by the spec)
			//if (rx == 0 || ry == 0) {
			//	pather.lineTo(x, y);
			//	return;
			//}

			// Sign of the radii is ignored (behaviour specified by the spec)
			rx = Abs(rx);
			ry = Abs(ry);

			// Convert angle from degrees to radians
			var angleRad = (angle % 360.0) * PI / 180;
			var cosAngle = Cos(angleRad);
			var sinAngle = Sin(angleRad);

			// We simplify the calculations by transforming the arc so that the origin is at the
			// midpoint calculated above followed by a rotation to line up the coordinate axes
			// with the axes of the ellipse.

			// Compute the midpoint of the line between the current and the end point
			var dx2 = (lastX - x) / 2.0;
			var dy2 = (lastY - y) / 2.0;

			// Step 1 : Compute (x1', y1')
			// x1,y1 is the midpoint vector rotated to take the arc's angle out of consideration
			var x1 = (cosAngle * dx2 + sinAngle * dy2);
			var y1 = (-sinAngle * dx2 + cosAngle * dy2);

			var rx_sq = rx * rx;
			var ry_sq = ry * ry;
			var x1_sq = x1 * x1;
			var y1_sq = y1 * y1;

			// Check that radii are large enough.
			// If they are not, the spec says to scale them up so they are.
			// This is to compensate for potential rounding errors/differences between SVG implementations.
			var radiiCheck = x1_sq / rx_sq + y1_sq / ry_sq;
			if (radiiCheck > 0.99999) {
				var radiiScale = Sqrt(radiiCheck) * 1.00001;
				rx = (float)(radiiScale * rx);
				ry = (float)(radiiScale * ry);
				rx_sq = rx * rx;
				ry_sq = ry * ry;
			}

			// Step 2 : Compute (cx1, cy1) - the transformed centre point
			double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
			var sq = ((rx_sq * ry_sq) - (rx_sq * y1_sq) - (ry_sq * x1_sq)) / ((rx_sq * y1_sq) + (ry_sq * x1_sq));
			sq = (sq < 0) ? 0 : sq;
			var coef = (sign * Sqrt(sq));
			var cx1 = coef * ((rx * y1) / ry);
			var cy1 = coef * -((ry * x1) / rx);

			// Step 3 : Compute (cx, cy) from (cx1, cy1)
			var sx2 = (lastX + x) / 2.0;
			var sy2 = (lastY + y) / 2.0;
			var cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
			var cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);

			// Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
			var ux = (x1 - cx1) / rx;
			var uy = (y1 - cy1) / ry;
			var vx = (-x1 - cx1) / rx;
			var vy = (-y1 - cy1) / ry;
			double p, n;

			// Angle betwen two vectors is +/- acos( u.v / len(u) * len(v))
			// Where '.' is the dot product. And +/- is calculated from the sign of the cross product (u x v)

			var TWO_PI = Math.PI * 2.0;

			// Compute the start angle
			// The angle between (ux,uy) and the 0deg angle (1,0)
			n = Sqrt((ux * ux) + (uy * uy));  // len(u) * len(1,0) == len(u)
			p = ux;                                // u.v == (ux,uy).(1,0) == (1 * ux) + (0 * uy) == ux
			sign = (uy < 0) ? -1.0 : 1.0;          // u x v == (1 * uy - ux * 0) == uy
			var angleStart = sign * Acos(p / n);  // No need for checkedArcCos() here. (p >= n) should always be true.

			// Compute the angle extent
			n = Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
			p = ux * vx + uy * vy;
			sign = (ux * vy - uy * vx < 0) ? -1.0f : 1.0f;
			var angleExtent = sign * checkedArcCos(p / n);

			//// Catch angleExtents of 0, which will cause problems later in arcToBeziers
			//if (angleExtent == 0f) {
			//	pather.lineTo(x, y);
			//	return;
			//}

			if (!sweepFlag && angleExtent > 0) {
				angleExtent -= TWO_PI;
			} else if (sweepFlag && angleExtent < 0) {
				angleExtent += TWO_PI;
			}
			angleExtent %= TWO_PI;
			angleStart %= TWO_PI;

			return (angleStart, angleExtent, (rx, ry), (cx, cy));

			//// Many elliptical arc implementations including the Java2D and Android ones, only
			//// support arcs that are axis aligned.  Therefore we need to substitute the arc
			//// with bezier curves.  The following method call will generate the beziers for
			//// a unit circle that covers the arc angles we want.
			//float[] bezierPoints = arcToBeziers(angleStart, angleExtent);

			//// Calculate a transformation matrix that will move and scale these bezier points to the correct location.
			//Matrix m = new Matrix();
			//m.postScale(rx, ry);
			//m.postRotate(angle);
			//m.postTranslate((float)cx, (float)cy);
			//m.mapPoints(bezierPoints);

			//// The last point in the bezier set should match exactly the last coord pair in the arc (ie: x,y). But
			//// considering all the mathematical manipulation we have been doing, it is bound to be off by a tiny
			//// fraction. Experiments show that it can be up to around 0.00002.  So why don't we just set it to
			//// exactly what it ought to be.
			//bezierPoints[bezierPoints.length - 2] = x;
			//bezierPoints[bezierPoints.length - 1] = y;

			//// Final step is to add the bezier curves to the path
			//for (int i = 0; i < bezierPoints.length; i += 6) {
			//	pather.cubicTo(bezierPoints[i], bezierPoints[i + 1], bezierPoints[i + 2], bezierPoints[i + 3], bezierPoints[i + 4], bezierPoints[i + 5]);
			//}
		}

		// Check input to Math.acos() in case rounding or other errors result in a val < -1 or > +1.
		// For example, see the possible KitKat JIT error described in issue #62.
		private static double checkedArcCos(double val) {
			return (val < -1.0) ? PI : (val > 1.0) ? 0 : Acos(val);
		}


		private class Matrix {
			private readonly double[] _d;
			/// <summary>Width - number of columns.</summary>
			public int w;
			/// <summary>Height - number of rows.</summary>
			public int h;

			public Matrix(int c = 0, params double[] values) {
				_d = values;
				w = c;
				var h = _d.Length / (double)c;
				if (h % 1 > 0) throw new ArgumentException("Matrix values passed in constructor are not divisible by specified columns number.");
				this.h = (int)h;
			}

			public Matrix(int c, int r) {
				w = c;
				h = r;
				_d = new double[w * h];
			}

			public static Matrix operator *(Matrix m1, Matrix m2) {
				//var sr = m2.ToString();
				var om = new Matrix(m2.w, m2.h);
				for (var c = 0; c < m2.w; c++) {
					for (var r = 0; r < m1.h; r++) {
						var v = 0d;
						for (var i = 0; i < m1.w; i++) {
							var a = m1[i, r]; var b = m2[c, i];
							v += a * b;
						}
						om[c, r] = v;
					}
				}
				//var s = om.ToString();
				//var s0 = om[0, 0];
				//var s1 = om[1, 0];
				//var s2 = om[0, 1];
				//var s3 = om[1, 1];
				return om;
			}

			//public static V operator *(Matrix m, V v) {
			//	return m * (Matrix)v;
			//}

			public static V operator *(Matrix m, V p) {
				var p2 = p as Point2;
				return (Point2)(m * (Matrix)p2);
			}

			//public static implicit operator Matrix(V v)
			//	=> new Matrix(1, v.x, v.y, v.z);

			public static explicit operator Matrix(Point2 p)
				=> new Matrix(1, p.x, p.y);

			//public static implicit operator V(Matrix m) {
			//	if (m.h >= 4)
			//		return new V(
			//			m[0, 0],
			//			m[0, 1],
			//			m[0, 2],
			//			m[0, 3]
			//		);
			//	var v = new V();
			//	for (var i = 0; i < m.h; i++) v[i] = m[0, i];
			//	return v;
			//}

			public static implicit operator Point2(Matrix m) {
				return new Point2(m[0, 0], m[0,1]);
			}

			public double this[int c, int r] {
				get => _d[r * w + c];
				set => _d[r * w + c] = value;
			}

			private StringBuilder sb;
			public string sr;
			public override string ToString() {
				sb ??= new StringBuilder();
				sb.Clear();
				sb.Append("[ ");
				for (var y = 0; y < h; y++) {
					for (var x = 0; x < w; x++) {
						var sv = this[x, y].ToString(); //.ics("0.##");
						sb.Append(sv);
						if (x < w - 1) sb.Append("	");
					}
					if (y == h - 1) sb.Append(" ]");
					else sb.AppendLine().Append("  ");
				}
				sr = sb.ToString();
				return sr;
			}
		}


	}
}
