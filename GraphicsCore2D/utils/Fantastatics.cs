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

	/// <summary>Compares this object to objects in the list and returns other object from the tuple where the objects were equal.</summary>
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
}
