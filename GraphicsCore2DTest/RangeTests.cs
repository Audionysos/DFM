using audionysos.math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsCore2DTest;
[TestClass]
public class RangeTests {

	[TestMethod]
	public void initSwap() {
		var r = (5).to(-5);
		Assert.AreEqual((-5).to(5), r);
	}

	[TestMethod]
	public void expanding() {
		var r1 = 0.to(10);
		var r2 = 20.to(30);
		var r = r1.expand(r2);
		Assert.AreEqual((0, 30), r);
	}

	[TestMethod]
	public void gap() {
		var r1 = 0.to(10);
		var r2 = 20.to(30);
		var r = r1.gap(r2);
		Assert.AreEqual((10, 20), r);
	}

	[TestMethod]
	public void emptyGap() {
		var r1 = 0.to(15);
		var r2 = 15.to(30);
		var r = r1.gap(r2);
		Assert.AreEqual(Range<int>.empty, r);
	}

	[TestMethod]
	public void overlap() {
		var r1 = 0.to(20);
		var r2 = 10.to(30);
		var r = r1.overlap(r2);
		Assert.AreEqual((10, 20), r);
	}

	[TestMethod]
	public void emptyOverlap() {
		var r1 = 0.to(10);
		var r2 = 20.to(30);
		var r = r1.overlap(r2);
		Assert.AreEqual(Range<int>.empty, r);
	}

	[TestMethod]
	public void subtract() {
		var r1 = 0.to(20);
		var r2 = 10.to(30);
		var r = r1.subtract(r2);
		Assert.AreEqual(
			((0, 10) , Range<int>.empty)
			, r);

		r1 = 0.to(20);
		r2 = (-10).to(10);
		r = r1.subtract(r2);
		Assert.AreEqual(
			(Range<int>.empty, (10, 20) )
			, r);

		r1 = 0.to(20);
		r2 = (5).to(15);
		r = r1.subtract(r2);
		Assert.AreEqual(
			((0, 5), (15, 20))
			, r);

		r1 = 0.to(20);
		r2 = (30).to(40);
		r = r1.subtract(r2);
		Assert.AreEqual(
			((0, 20), (0, 20))
			, r);

		r1 = 0.to(20);
		r2 = (0).to(40);
		r = r1.subtract(r2);
		Assert.AreEqual(
			( Range<int>.empty, Range<int>.empty )
			, r);

		var ui = 5u;
		var ue = 10u;
		var ur = ui.to(ue);
	}


}
