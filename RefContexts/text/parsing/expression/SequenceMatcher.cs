using System;
using System.Linq;
using static com.audionysos.text.parsing.expression.SCSExpressionParser;

namespace com.audionysos.text.parsing.expression {
	public abstract class SequenceMatcher : Pattern {
		#region Char sets
		/// <summary>All lower and uppercase letters.</summary>
		public static char[] LETTERS { get; private set; }
			= Enumerable.Range('a', 26).Select(x => (char)x).Concat(
				Enumerable.Range('A', 26).Select(x => (char)x)).ToArray();

		/// <summary>List of all digits characters from 0 to 9.</summary>
		public static char[] DIGITS { get; private set; } = "0123456789".ToArray();
		/// <summary>List of white space characters (space, tab).</summary>
		public static char[] WHITES { get; private set; } = "  \t".ToArray();

		#endregion

		#region Properites
		private int si; private int ei;
		public override int startIndex => si;
		public override int endIndex => ei;

		/// <summary>Current parsing context.</summary>
		public ParsingContext pc { get; private set; }
		/// <summary>Currently processed char.</summary>
		private char cc;
		/// <summary>Current size of matched sequence.
		/// This number is automatically increased by base class when a char is approved.</summary>
		public int size { get; private set; } = 0;
		/// <summary>Tells if sequence is valid.</summary>
		public bool isValid { get; private set; }

		public bool equivalent(Pattern m)
			=> GetType() == m.GetType() && m.signature == signature;  

		/// <summary>Tells that execution of matches has ended and it was validated. <see cref="take(char)"/> method can no longer be called until <see cref="reset"/> method is invoked.</summary>
		public bool wasValidated { get; private set; }
		/// <summary>Creates string from current result.</summary>
		public string strin => startIndex < 0 || endIndex < 0 ? null :
			pc.source?.Substring(startIndex, size);

		/// <summary>Thell that instances of valid matches can overlap.
		/// For example when finding a word in "AB" string, if this flag is set to true. Whole parsing will return "AB" and  "B". If flag is set to false only "AB" is matched.
		/// By default, this flag is set to false.</summary>
		public bool allowOverlap = false;

		/// <summary>Tell that matcher can output multiple valid matches starting from the same position.
		/// This efectively forces parser to process the matcher to the end of source string, starting from each character.
		/// This flag can be used for example to find all possible combination of ranges (seqence paris like "(...)" "{...}") in complex strings like "(...(...)...)" wher 4 combinations are possible.
		/// This however is inefficient and sould bo only used for trivial cases.
		/// Before new match testing is started a virtal <see cref="resetForNextOutput"/> method is called.</summary>
		public bool multiOut { get; protected set; }
		#endregion

		public SequenceMatcher() {
			//result = new char[maxSize];
		}

		#region For descendants
		protected abstract bool isCharOk(char c);
		/// <summary>Return true if sequence is valid. By default true is returned if size is greater than 0.</summary>
		protected virtual bool validate() => size > 0;
		/// <summary>Tells to reset additional state variables when matcher should be reused for new search.
		/// Default method is empty.</summary>
		protected virtual void resetToInitialState() { }
		/// <summary>This method isc called when descentant class have set <see cref="multiOut"/> flag to true and some match have just returned.
		/// Resent variable needed to find new match.</summary>
		protected virtual void resetForNextOutput() { }
		#endregion

		#region Helper methods
		protected bool charIsAnyOF(char[] chars) {
			foreach (var rc in chars) if (cc == rc) return true; return false;
		}

		protected bool charIsNoneOF(char[] chars) {
			foreach (var rc in chars) if (cc == rc) return false; return true;
		}
		#endregion

		/// <summary>Tell if this char was accepted as part of the sequence.
		/// When this methods return false, you check <see cref="isValid"/> to tell if match occured.</summary>
		/// <param name="c">Char to take or discard.</param>
		public bool take(ParsingContext c) {
			if (wasValidated) throw new InvalidOperationException($@"Once a matcher is validated {nameof(take)} method cannot be called unitl the {nameof(reset)} method is called.");
			pc = c;  cc = c.c;
			if (c.i == c.source.Length) return finalize();
			if (isCharOk(cc)) {
				if (size++ == 0) si = c.i;
				//result[size++] = cc;
				return true;
			}
			return finalize();
		}

		/// <summary>If <see cref="size"/> is greated than 0, validates a match and sets <see cref="startIndex"/> to given value.
		/// Always retruns false.</summary>
		/// <param name="i">End index to be set.</param>
		private bool finalize() {
			if (size == 0) return false;
			isValid = validate();
			wasValidated = true;
			ei = pc.i - 1;
			return false;
		}

		/// <summary>Resets the matcher so that search could start from beginning.</summary>
		public SequenceMatcher reset() {
			wasValidated = false;
			isValid = false;
			if (multiOut && pc.i < pc.source.Length) {
				resetForNextOutput();
				return this;
			}
			si = ei = -1;
			size = 0;
			//result = new char[result.Length];
			resetToInitialState();
			return this;
		}

		public override string ToString() {
			return $@"""{strin}"" {GetType().Name}({signature})";
		}

		/// <summary>Creates new instance of parameterles matcher (Just so that editor can show matcher description when howering over a type).</summary>
		/// <param name="t"></param>
		public static implicit operator SequenceMatcher(Type t) {
			if (!typeof(SequenceMatcher).IsAssignableFrom(t)) throw new Exception($@"""{t}"" is not a {nameof(SequenceMatcher)}.");
			return Activator.CreateInstance(t) as SequenceMatcher;
		}
	}
}
