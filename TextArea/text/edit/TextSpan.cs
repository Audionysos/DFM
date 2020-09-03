using System;

namespace com.audionysos.text.edit {
	/// <summary>Represents a sub sequence of characters in a <see cref="Text"/> by stroing boundry indices.</summary>
	public class TextSpan {

		public Text source { get; private set; }
		public int start { get; set; }
		public int end { get; set; }

		public Attributes attributes { get;  }

		#region Derived
		public int length => end - start;

		public string text {
			get {
				if (length == 0) return "";
				var chs = new char[length];
				for (int i = 0; i < chs.Length; i++) {
					chs[i] = source[start + i];
				}
				return new string(chs);
			}
		}

		public string fullOrError {
			get {
				try { return text; }
				catch (Exception e) { return e.Message; }
			}
		}
		#endregion

		public TextSpan(Text text, int start, int end) {
			source = text;
			start = text.clipIndex(start);
			end = text.clipIndex(end);
			if (end < start) {
				this.start = end;
				this.end = start;
			} else {
				this.start = start;
				this.end = end;
			}
		}

		public char this[int i] {
			get => source[start + i];
		}

		public override string ToString() {
			return $@"({start}-{end}): {fullOrError}";
		}
	}
}
