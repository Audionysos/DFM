using com.audionysos.text.parsing.expression;
using com.audionysos.text.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.audionysos.text.parsing; 

public class BasicMatchers {

	/// <summary>Match any identifier name. Can only contain letters and digits and "_". Cannot start with digit</summary>
	public class Name : SequenceMatcher {
		protected override bool isCharOk(char c) {
			if (size == 0 && charIsNoneOF(LETTERS) && c != '_') return false;
			if (size > 0 && charIsNoneOF(LETTERS) && charIsNoneOF(DIGITS)) return false;
			return true;
		}
	}

	/// <summary>Matches sequence of digits in a row. Any no-digit character will break the sequence.</summary>
	public class Digits : SequenceMatcher {
		protected override bool isCharOk(char c) => charIsAnyOF(DIGITS);
	}

	/// <summary>Matches all spaces or tabs in a row.</summary>
	public class Whitespace : SequenceMatcher {
		protected override bool isCharOk(char c) => charIsAnyOF(WHITES); 
	}

	public class Sequence : SequenceMatcher {
		public string sequence { get; private set; }

		/// <summary>Matches speficic sequence of characters (case sensitive).</summary>
		public Sequence(string sequence) {
			if (sequence.isEmpty()) throw new ArgumentException("Empty or null string cannot represent a sequence", nameof(sequence));
			this.sequence = sequence;
			signature = sequence;
		}
		protected override bool isCharOk(char c) 
			=> size < sequence.Length && c == sequence[size];
		protected override bool validate() => size == sequence.Length;
	}

	public class Range : SequenceMatcher {
		private (int s, int e) i = (0, 0);
		private char[] open;
		private char[] close;

		/// <summary>Inclusively matches ranges between <paramref name="open"/> and <paramref name="close"/> sequences.</summary>
		public Range(string open, string close) {
			double d = -.1456e-35;
			multiOut = true;
			allowOverlap = true;
			signature = $@"""{open}"" to ""{close}""";
			this.open = open.ToArray();
			this.close = close.ToArray();
		}

		protected override bool isCharOk(char c) {
			if (size == 0 && c != open[0]) return false;
			if (i.s < open.Length) {
				if (c != open[i.s++]) return false;
			} else if (i.e == 0) {
				if (c == close[0]) { i.e++; return true; }
			} else if (i.e == close.Length || c != close[i.e++]) return false;
			return true;
		}

		protected override bool validate()
			=> i.s == open.Length && i.e == close.Length;

		protected override void resetToInitialState() => i = (0, 0);
		protected override void resetForNextOutput() => i.e = 0;
	}

	/// <summary>Maches any single character of given list.</summary>
	public class Character : SequenceMatcher {
		private char[] chars;

		/// <summary>Maches any single character of given list.</summary>
		public Character(params char[] chars) {
			this.chars = chars;
			signature = new string(chars);
		}

		protected override bool isCharOk(char c) {
			return size == 0 && charIsAnyOF(chars);
		}
	}
}
