using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.audionysos.text.utils; 

/// <summary>Extension methods for string objects.</summary>
public static class StringExtensions {

	/// <summary>Shorthand for <see cref="string.IsNullOrEmpty(string)"/></summary>
	public static bool isEmpty(this string s) => string.IsNullOrEmpty(s); 

}
