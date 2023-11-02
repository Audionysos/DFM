using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
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

	public static N clip<N>(this N n, N min, N max) where N : INumber<N> {
		if (n < min) return min;
		if (n > max) return max;
		return n;
	}

	/// <summary>Creates range from this up to given number.</summary>
	public static Range<N> to<N>(this N start, N otherEnd) where N : INumber<N>
		=> new(start, otherEnd);

	public static RangeEnumerator<N> every<N>(this Range<N> r, N step)
		where N : INumber<N>, IBinaryInteger<N> => new(r, step);
	

	public static RangeEnumerator<N> all<N>(this Range<N> r)
		where N : INumber<N>, IBinaryInteger<N> => new(r, N.One);
}

public static class NumbersExtensions2 {

	public static FloatRangeEnumerator<N> every<N>(this Range<N> r, N step)
		where N : IBinaryFloatingPointIeee754<N> => new(r, step);

	public static FloatRangeEnumerator<N> all<N>(this Range<N> r)
		where N : IBinaryFloatingPointIeee754<N> => new(r, N.Zero);

	public static void all<N>(this Range<N> r, Action<N> a)
		where N : IBinaryFloatingPointIeee754<N> {
			foreach (var n in r.all()) a(n); }

	public static FloatRangeEnumerator<N> allRev<N>(this Range<N> r)
		where N : IBinaryFloatingPointIeee754<N> => new(r, N.NegativeZero);

	public static FloatRangeEnumerator<N> spread<N>(this Range<N> r, int count)
		where N : IBinaryFloatingPointIeee754<N>
		=> new(r, r.length / N.CreateChecked(count-1));

}
