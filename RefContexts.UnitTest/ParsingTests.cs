using com.audionysos.text.parsing;
using com.audionysos.text.parsing.expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static com.audionysos.text.parsing.BasicMatchers;
using static com.audionysos.text.parsing.patterns.BasicPatterns;

namespace RefContexts.UnitTest {
	[TestClass]
	public class ParsingTests {

		[TestMethod]
		public void NumberConstructBasicTest() {
			var sp = new SyntaxParser("f-.1456e-35 ", new Number());
			sp.produece();
		}

		[TestMethod]
		public void LanguageConstructBasicTest() {
			var sp = new SyntaxParser("fed.oi34_45ter = -.1456e-35 ", new Number());
			sp.produece();
		}

		[TestMethod]
		public void ExpressionTest() {
			checkMatches(
			//	 |    .    |    .    |    .    |    .    |
			//	 01234567891123456789212345678931234567894
				"object.getMethod<Type>().value = 4 + 9;",
				("object", (0, 5)),
				("getMethod", (7, 15)),
				("<Type>", (16, 21)),
				("Type", (17, 20)),
				("()", (22, 23)),
				("value", (25, 29)),
				("=", (31, 31)),
				("4", (33, 33)),
				("+", (35, 35)),
				("9", (37, 37)),
				("object", (0, 5))
			);
		}

		#region Ranges
		[TestMethod]
		public void RangeMatcher_SingleMaches() {
			checkSingleRange("<>", "<>", (0, 1));
			checkSingleRange("0123<567>9AB", "<567>", (4, 8));
			checkSingleRange("<123>56789AB", "<123>", (0, 4));
			checkSingleRange("012345<789A>", "<789A>", (6, 11));
		}

		[TestMethod]
		public void RangeMatcher_MultipleRowMaches() {
			checkMultipleRanges(
				"<><>",
				("<>", (0, 1)),
				("<>", (2, 3)),
				("<><>", (0, 3))
			);
			checkMultipleRanges(
				"<1>3<567>9<B>", 
				("<1>",   (0, 2)),
				("<567>", (4, 8)),
				("<B>",   (10, 12))
			);
		}

		[TestMethod]
		public void RangeMatcher_MultipleMixedMaches() {
			checkMultipleRanges(
				"<123<567>9AB>",
				("<123<567>", (0, 8)),
				("<123<567>9AB>", (0, 12)),
				("<567>", (4, 8)),
				("<567>9AB>", (4, 12))
			);
		}

		private void checkSingleRange(string source, string result, (int s, int e) position) {
			var p = new SCSExpressionParser(source);
			var rm = from m in p.validMaches
				 where m.matcher.GetType() == typeof(Range)
						&& m.value == result
						&& m.pos == position
				 select m;
			Assert.IsTrue(rm.Count() > 0);
		}

		private void checkMultipleRanges(string source, params (string result, (int s, int e) position)[] results) {
			var p = new SCSExpressionParser(source);
			var rm = from m in p.validMaches
					 where m.matcher.GetType() == typeof(Range)
					 select m;
			foreach (var r in results) {
				Assert.IsTrue(
					rm.FirstOrDefault(
						m => m.value == r.result
							&& m.pos == r.position) != null
				, $@"Didn't found range ""{r.result}"" in ""{source}"" soruce.");
			}
		}
		#endregion

		private void checkMatches(string source, params (string result, (int s, int e) position)[] results) {
			var p = new SCSExpressionParser(source);
			foreach (var r in results) {
				Assert.IsTrue(
					p.validMaches.FirstOrDefault(
						m => m.value == r.result
							&& m.pos == r.position) != null
				, $@"Didn't found range ""{r.result}"" in ""{source}"" soruce.");
			}
		}

	}
}
  