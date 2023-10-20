using C = com.audionysos.text.parsing.LanguageConstruct;
using static com.audionysos.text.parsing.BasicMatchers;

namespace com.audionysos.text.parsing.patterns {
	public class BasicPatterns {

		/// <summary>Mathes floting point and int numbers in decimal system.</summary>
		public class Number : C {
			public Number() {
				OPT("+").or("-").OPT<Whitespace>().OPT<UnDigits>()
					.OPT(".").REQ<UnDigits>().OPT<NExponent>();
			}
		}

		/// <summary>Patter for digits separated with uderscores like 123__456_789.</summary>
		public class UnDigits : C {
			public UnDigits() {
				REQ<Digits>().OPT(c => c.REQ("_").REQ<Digits>()).takeAll();
			}
		}

		/// <summary>Pattter for number expennt (e-6)</summary>
		public class NExponent : C {
			public NExponent() {
				REQ("e").or("E").OPT("-").or("+").REQ<UnDigits>();
			}
		}

	}
}
