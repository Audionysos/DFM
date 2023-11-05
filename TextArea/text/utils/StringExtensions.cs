using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.audionysos.text.utils; 

/// <summary>Extension methods for string objects.</summary>
public static class StringExtensions {

	/// <summary>Shorthand for <see cref="string.IsNullOrEmpty(string)"/></summary>
	public static bool isEmpty(this string s) => string.IsNullOrEmpty(s);

	#region String literals 

	public static string escaped(this string s)
		=> s == null ? "" : FormatLiteral(s);

	//On of answers from https://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal
	public static string ToLiteral(string input) {
		var literal = new StringBuilder(input.Length + 2);
		literal.Append("\"");
		foreach (var c in input) {
			switch (c) {
				case '\'': literal.Append(@"\'"); break;
				case '\"': literal.Append("\\\""); break;
				case '\\': literal.Append(@"\\"); break;
				case '\0': literal.Append(@"\0"); break;
				case '\a': literal.Append(@"\a"); break;
				case '\b': literal.Append(@"\b"); break;
				case '\f': literal.Append(@"\f"); break;
				case '\n': literal.Append(@"\n"); break;
				case '\r': literal.Append(@"\r"); break;
				case '\t': literal.Append(@"\t"); break;
				case '\v': literal.Append(@"\v"); break;
				default:
					if (Char.GetUnicodeCategory(c) != UnicodeCategory.Control) {
						literal.Append(c);
					} else {
						literal.Append(@"\u");
						literal.Append(((ushort)c).ToString("x4"));
					}
					break;
			}
		}
		literal.Append("\"");
		return literal.ToString();
	}

	#region Roslyn original
	public static string FormatLiteral(string value) {
		if (value == null) {
			throw new ArgumentNullException(nameof(value));
		}

		//const char quote = '"';

		//(delete) - StringBuilder was taken from a pool here 
		var builder = new StringBuilder();

		//var useQuotes = false;// options.IncludesOption(ObjectDisplayOptions.UseQuotes);
		var escapeNonPrintable = true;// options.IncludesOption(ObjectDisplayOptions.EscapeNonPrintableCharacters);

		//var isVerbatim = useQuotes && !escapeNonPrintable && ContainsNewLine(value);

		//if (useQuotes) {
		//	if (isVerbatim) {
		//		builder.Append('@');
		//	}
		//	builder.Append(quote);
		//}

		for (int i = 0; i < value.Length; i++) {
			char c = value[i];
			if (escapeNonPrintable && CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.Surrogate) {
				var category = CharUnicodeInfo.GetUnicodeCategory(value, i);
				if (category == UnicodeCategory.Surrogate) {
					// an unpaired surrogate
					builder.Append("\\u" + ((int)c).ToString("x4"));
				} else if (NeedsEscaping(category)) {
					// a surrogate pair that needs to be escaped
					var unicode = char.ConvertToUtf32(value, i);
					builder.Append("\\U" + unicode.ToString("x8"));
					i++; // skip the already-encoded second surrogate of the pair
				} else {
					// copy a printable surrogate pair directly
					builder.Append(c);
					builder.Append(value[++i]);
				}
			} else if (escapeNonPrintable && TryReplaceChar(c, out var replaceWith)) {
				builder.Append(replaceWith);
			}
			//else if (useQuotes && c == quote) {
			//	if (isVerbatim) {
			//		builder.Append(quote);
			//		builder.Append(quote);
			//	} else {
			//		builder.Append('\\');
			//		builder.Append(quote);
			//	}
			//} 
			else {
				builder.Append(c);
			}
		}

		//if (useQuotes) {
		//	builder.Append(quote);
		//}

		return builder.ToString();
	}

	private static bool NeedsEscaping(UnicodeCategory category) {
		switch (category) {
			case UnicodeCategory.Control:
			case UnicodeCategory.OtherNotAssigned:
			case UnicodeCategory.ParagraphSeparator:
			case UnicodeCategory.LineSeparator:
			case UnicodeCategory.Surrogate:
				return true;
			default:
				return false;
		}
	}

	private static bool TryReplaceChar(char c, out string replaceWith) {
		replaceWith = null;
		switch (c) {
			case '\\':
				replaceWith = "\\\\";
				break;
			case '\0':
				replaceWith = "\\0";
				break;
			case '\a':
				replaceWith = "\\a";
				break;
			case '\b':
				replaceWith = "\\b";
				break;
			case '\f':
				replaceWith = "\\f";
				break;
			case '\n':
				replaceWith = "\\n";
				break;
			case '\r':
				replaceWith = "\\r";
				break;
			case '\t':
				replaceWith = "\\t";
				break;
			case '\v':
				replaceWith = "\\v";
				break;
		}

		if (replaceWith != null) {
			return true;
		}

		if (NeedsEscaping(CharUnicodeInfo.GetUnicodeCategory(c))) {
			replaceWith = "\\u" + ((int)c).ToString("x4");
			return true;
		}

		return false;
	}
	#endregion

	#endregion

}
