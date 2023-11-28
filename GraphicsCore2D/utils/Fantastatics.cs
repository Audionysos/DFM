// Ignore Spelling: Fantastatics, hh, ss, ffff

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace audionysos.utils;
public static class Fantastatics {

	/// <summary>Return current time in ss:ffff format.
	/// For use when logging mouse events or similar.</summary>
	public static string RapidTimeStamp
		=> DateTime.Now.ToString("ss:ffff");

	/// <summary>Compares this object to first objects in the tuples list and returns second object from a tuple where <see cref="object.Equals(object?)"/> returned true.</summary>
	/// <typeparam name="R">Type of returned object.</typeparam>
	/// <typeparam name="I">Type of input/compared object.</typeparam>
	/// <param name="input">Compared object.</param>
	/// <param name="ms">List of case-result tuples.</param>
	/// <returns></returns>
	public static R? @switch<R, I>(this I? input, params (I c, R r)[] ms) {
		foreach (var m in ms)
			if (Equals(input, m.c))
				return m.r;
		return default;
	}

	/// <summary>Executes given function one time per each listed object.</summary>
	/// <typeparam name="T">Type of objects passed to the function.</typeparam>
	/// <typeparam name="R">Type of object returned by the function.</typeparam>
	/// <param name="f">Function to execute.</param>
	/// <param name="ps">List of to be passed as the function parameters.</param>
	/// <returns>List of function calls results.</returns>
	public static void each<T>(Action<T> f, params T[] ps) {
		foreach (var p in ps) f(p);
	}

	/// <summary>Executes given function one time per each listed object and returns all returned values as an array.</summary>
	/// <typeparam name="T">Type of objects passed to the function.</typeparam>
	/// <typeparam name="R">Type of object returned by the function.</typeparam>
	/// <param name="f">Function to execute.</param>
	/// <param name="ps">List of to be passed as the function parameters.</param>
	/// <returns>List of function calls results.</returns>
	public static R[] each<T, R>(Func<T, R> f, params T[] ps) {
		var r = new R[ps.Length];
		for (var i = 0; i < ps.Length; i++)
			r[i] = f(ps[i]);
		return r;
	}
}
