using System;
using System.Collections.Generic;
using System.Text;
using static System.Math;

namespace audionysos.math; 
public static class NumbersExtensions {

	/// <summary>Return true if this number equals given other number with some tolerance.</summary>
	/// <param name="n"></param>
	/// <param name="o">Other number to compare.</param>
	/// <param name="t">Tolerance - if numbers difference is below this value, method returns true.</param>
	/// <returns></returns>
	public static bool equalsT(this double n, double o, double t = 0.0000001) {
		return Abs(n - o) < t;
	}
}
