using audionysos.geom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsCore2DTest;
[TestClass]
public  class LineTests {

	[TestMethod]
	public void slopes() {
		var l = new Line2((0,0), (1,1));
		Assert.AreEqual(1, l.slope());

		l = new Line2((0, 0), (1, 2));
		Assert.AreEqual(2, l.slope());

		l = new Line2((0, 0), (1, -2));
		Assert.AreEqual(-2, l.slope());

		l = new Line2((0, 0), (1, 0));
		Assert.AreEqual(0, l.slope());

		l = new Line2((0, 0), (0, 1));
		Assert.AreEqual(double.PositiveInfinity, l.slope());

		l = new Line2((0, 0), (0, -1));
		Assert.AreEqual(double.NegativeInfinity, l.slope());

		//this is unspecified, however it should not throw
		l = new Line2((0, 0), (0, 0));
	}
	//TODO: Line touches intersection tests

	#region Parallels
	[TestMethod]
	public void intersectParallel_Ys() {
		var a = new Line2((0, 0), (0, 10));
		var b = new Line2((10, 0), (10, 10));
		var ip = Line2.intersection(a, b);
		Assert.IsNull(ip);
	}

	[TestMethod]
	public void intersectParallel_Xs() {
		var a = new Line2((0, 0), (10, 0));
		var b = new Line2((0, 10), (10, 10));
		var ip = Line2.intersection(a, b);
		Assert.IsNull(ip);
	}

	[TestMethod]
	public void intersectParallel_Diagonal_BottomRight() {
		var a = new Line2((0, 0), (10, 10));
		var b = new Line2((0, 10), (10, 20));
		var ip = Line2.intersection(a, b);
		Assert.IsNull(ip);
	}

	[TestMethod]
	public void intersectParallel_Diagonal_TopRight() {
		var a = new Line2((0, 0), (10, -10));
		var b = new Line2((0, -10), (10, -20));
		var ip = Line2.intersection(a, b);
		Assert.IsNull(ip);
	}

	[TestMethod]
	public void intersectParallel_Diagonal_TopLeft() {
		var a = new Line2((0, 0), (-10, -10));
		var b = new Line2((0, -10), (-10, -20));
		var ip = Line2.intersection(a, b);
		Assert.IsNull(ip);
	}

	[TestMethod]
	public void intersectParallel_Diagonal_BottomLeft() {
		var a = new Line2((0, 0), (-10, 10));
		var b = new Line2((0, 10), (-10, 20));
		var ip = Line2.intersection(a, b);
		Assert.IsNull(ip);
	}
	#endregion

	#region Perpendicular crossings
	[TestMethod]
	public void perpendicularCrossings() {
		var dirs = new[] {
			perpendicularCrossingA(),
			perpendicularCrossingB(),
			perpendicularCrossingC(),
			perpendicularCrossingD(),
		};
		dirs = withOffsets(dirs, quadrantOffsets);
		for (int i = 0; i < dirs.Length; i++) {
			var d = dirs[i];
			if (testConfig(d)) {
				Debugger.Break();
				Assert.Fail();
			}
		}
	}

	private CrossingConfig perpendicularCrossingA() {
		var cc = new CrossingConfig {
			a = ((-5, 0), (5, 0)),
			b = ((0, -5), (0, 5)),
			expected = (0, 0)
		};
		return cc;
	}

	private CrossingConfig perpendicularCrossingB() {
		var cc = new CrossingConfig {
			a = ((5, 0), (-5, 0)),
			b = ((0, -5), (0, 5)),
			expected = (0, 0)
		};
		return cc;
	}

	private CrossingConfig perpendicularCrossingC() {
		var cc = new CrossingConfig {
			a = ((5, 0), (-5, 0)),
			b = ((0, 5), (0, -5)),
			expected = (0, 0)
		};
		return cc;
	}

	private CrossingConfig perpendicularCrossingD() {
		var cc = new CrossingConfig {
			a = ((-5, 0), (5, 0)),
			b = ((0, 5), (0, -5)),
			expected = (0, 0)
		};
		return cc;
	}
	#endregion

	#region Diagonal crossings
	[TestMethod]
	public void diagonalCrossings() {
		var dirs = new[] {
			diagonalCrossingA(),
			diagonalCrossingB(),
			diagonalCrossingC(),
			diagonalCrossingD(),
		};
		dirs = withOffsets(dirs, quadrantOffsets);
		for (int i = 0; i < dirs.Length; i++) {
			var d = dirs[i];
			if (testConfig(d)) {
				Debugger.Break();
				Assert.Fail();
			}
		}
	}

	private CrossingConfig diagonalCrossingA() {
		var cc = new CrossingConfig {
			a = ((-5, -5), (5, 5)),
			b = ((-5, 5), (5, -5)),
			expected = (0, 0)
		};
		return cc;
	}

	private CrossingConfig diagonalCrossingB() {
		var cc = new CrossingConfig {
			a = ((5, 5), (-5, -5)),
			b = ((-5, 5), (5, -5)),
			expected = (0, 0)
		};
		return cc;
	}

	private CrossingConfig diagonalCrossingC() {
		var cc = new CrossingConfig {
			a = ((5, 5), (-5, -5)),
			b = ((5, -5), (-5, 5)),
			expected = (0, 0)
		};
		return cc;
	}

	private CrossingConfig diagonalCrossingD() {
		var cc = new CrossingConfig {
			a = ((-5, -5), (5, 5)),
			b = ((5, -5), (-5, 5)),
			expected = (0, 0)
		};
		return cc;
	}
	#endregion

	/// <summary>Assuming base configs are centered in range of -5 to 5 on both axles, this will create new configs for each quadrants.</summary>
	private static (double, double)[] quadrantOffsets = {
		(0, 0), //center
		(15, 15), //bottom right
		(-10, 15), //bottom left
		(-10, -10), //top left
		(15, -10), // top right
	};

	private CrossingConfig[] withOffsets(CrossingConfig[] basic, (double x, double y)[] offsets) {
		var r = new CrossingConfig[basic.Length * offsets.Length];
		for (int i = 0; i < basic.Length; i++) {
			var b = basic[i];
			for (int j = 0; j < offsets.Length; j++) {
				var o = offsets[j];
				r[offsets.Length * i + j] = b.move(o.x, o.y);
			}
		}return r;
	}

	private bool testConfig(CrossingConfig cc) {
		var ip = Line2.intersection(cc.a, cc.b);
		if (softAssertPoint(cc.expected, ip))
			return true;
		ip = Line2.intersection(cc.b, cc.a);
		if(softAssertPoint(cc.expected, ip))
			return true;
		return false;
	}

	private bool softAssertPoint((double x, double y) p, IPoint2 ip) {
		if (ip == null) return true;
		if(p.x != ip.x) return true;
		if(p.y != ip.y) return true;
		return false;
	}

	private void assertPoint((double x, double y) p, IPoint2 ip) {
		Assert.AreEqual(p.x, ip.x);
		Assert.AreEqual(p.y, ip.y);
	}

	public record CrossingConfig {
		public Line2 a; public Line2 b;
		public (double x, double y) expected;

		public CrossingConfig move(double x, double y) {
			return new CrossingConfig() {
				a = a.copy().move(x, y),
				b = b.copy().move(x, y),
				expected = (expected.x + x, expected.y + y)
			};
		}

		public CrossingConfig swapOrder() {
			return new CrossingConfig() {
				a = b,
				b = a,
				expected = expected
			};
		}
	}
}
