using audionysos.geom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace GraphicsCore2DTest;
[TestClass]
public  class LineTests {

	#region Various
	[TestMethod]
	public void CaseX() {
		var il = new Line2((-51.08984375, -54.6875), (253.6, 1.5999999999999996));
		var tl = new Line2((8.771484375, 7.9765625), (7.61279296875, 7.3876953125));
		var ip = Line2.intersection(il, tl);
		Assert.IsNull(ip);
	}

	[TestMethod]
	public void CaseX2() => testConfigs(caseX2);

	static CrossingConfig caseX2 = new() {
		a = ((100, 100), (158.4, 200)),
		b = ((150, 200), (150, 150)),
		expected = (150, 185.61643835616437), //not really buy expecting something
	};

	[TestMethod]
	public void uneven() {
		var configs = new[] {
			unevenA(),
			unevenB(),
		};
		testConfigs(configs);
	}

	public CrossingConfig unevenA() {
		return new CrossingConfig {
			a = ((-10, -10), (5, 5)),
			b = ((2, 3),(4, 10)),
		};
	}

	public CrossingConfig unevenB() {
		return new CrossingConfig {
			a = ((-51.08984375, -54.6875), (253.6, 1.5999999999999996)),
			b = ((8.771484375, 7.9765625), (7.61279296875, 7.3876953125)),
		};
	}

	public CrossingConfig unevenC() {
		return new CrossingConfig {
			a = ((2, 2), (4, -2)),
			b = ((4, 4), (10, 0)),
		};
	}
	#endregion

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
	public void parallels() {
		testConfigs(offsets: quadrantOffsets,
			intersectParallel_Xs(),
			intersectParallel_Ys,
			intersectParallel_Diagonal_Right(),
			intersectParallel_Diagonal_Left()
		);
	}

	//public CrossingConfig intersectParallel_Ys() {
	static CrossingConfig intersectParallel_Ys => new () {
			a = new Line2((-5, -5), (-5, 5)),
			b = new Line2((5, -5), (5, 5)),
	};

	public CrossingConfig intersectParallel_Xs() {
		return new CrossingConfig {
			a = ((-5, -5), (5, -5)),
			b = ((-5, 5), (5, 5)),
		};
	}

	public CrossingConfig intersectParallel_Diagonal_Right() {
		return new CrossingConfig {
			a = ((-5, 0), (0, 5)),
			b = ((0, -5), (5, 0)),
		};
	}

	public CrossingConfig intersectParallel_Diagonal_Left() {
		return new CrossingConfig {
			a = ((0, -5), (-5, 0)),
			b = ((5, 0), (0, 5)),
		};
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
		testConfigs(quadrantOffsets, dirs);
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

	#region Common
	private void testConfigs(params CrossingConfig[] configs)
		=> testConfigs((IReadOnlyList<CrossingConfig>)configs);
	private void testConfigs((double x, double y)[] offsets, params CrossingConfig[] configs)
		=> testConfigs(withOffsets(configs, offsets));
	private void testConfigs(IReadOnlyList<CrossingConfig> configs) {
		ILine2 a; ILine2 b; IPoint2 expected; IPoint2 actutal; //for debugger "Locals" appearance
		for (int i = 0; i < configs.Count; i++) {
			var d = configs[i];
			a = d.a; 
			b = d.b;
			expected = d.expected;
			if (testConfig(d, out actutal)) {
				Debugger.Break();
				testConfig(d);

				Assert.Fail();
			}
		}
	}

	[DebuggerNonUserCode]
	private bool testConfig(CrossingConfig cc)
		=> testConfig(cc, out _);
	[DebuggerNonUserCode]
	private bool testConfig(CrossingConfig cc, out IPoint2 result) {
		result = Line2.intersection(cc.a, cc.b);
		if (softAssertPoint(cc.expected, result))
			return true;
		result = Line2.intersection(cc.b, cc.a);
		if(softAssertPoint(cc.expected, result))
			return true;
		return false;
	}

	/// <summary>Assuming base configs are centered in range of -5 to 5 on both axles, this will create new configs for each quadrant.</summary>
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
		}
		return r;
	}

	[DebuggerNonUserCode]
	private bool softAssertPoint(IPoint2 p, IPoint2 ip) {
		if (p == null && ip == null) return false;
		if (p == null || ip == null) return true;
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
		required public Line2 a;
		required public Line2 b;
		public Point2 expected;

		public CrossingConfig move(double x, double y) {
			return new CrossingConfig() {
				a = a.copy().move(x, y),
				b = b.copy().move(x, y),
				expected = expected?.copy().add(x, y)
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
	#endregion
}
