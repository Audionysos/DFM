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
}
