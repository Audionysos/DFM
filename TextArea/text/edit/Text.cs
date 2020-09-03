using com.audionysos.text.utils;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace com.audionysos.text.edit {

	public class Text : IReadOnlyList<char> {
		private string chars;

		public int Count { get; }
		public char this[int index] => chars[index];
		private List<TextSpan> _lines = new List<TextSpan>();
		public IReadOnlyList<TextSpan> lines => _lines;

		public Text(string text = null) {
			chars = text ?? "";
			splitLines(chars);
		}

		private void splitLines(string text) {
			char p, c = default; //current and previous characters;
			var ls = 0; //line start index
			for (int i = 0; i < text.Length; i++) {
				p = c; c = text[i]; var e = -1;
				if (c == '\n') e = i + 1;
				else if (p == '\r') e = i;
				if (e < 0) continue;
				_lines.Add(new TextSpan(this, ls, e));
				ls = e;
			}
			_lines.Add(new TextSpan(this, ls, text.Length));
		}

		public int clipIndex(int i) {
			Debug.Assert(chars != null);
			//if (chars.Length == 0) return 0;
			if (i < 0) return 0;
			if (i > chars.Length) return chars.Length;
			return i;
		}

		/// <summary>Returns character index at given column-line position.</summary>
		public int getIndex(Int2 pos) {
			return _lines[pos.y].start + pos.x;
		}

		public IEnumerator<char> GetEnumerator() => chars.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => chars.GetEnumerator();

		public static implicit operator Text(string t) => new Text(t);
	}

	public class Line : TextSpan {

		public Line(Text text, int start, int end)
			: base(text, start, end) {

		}
	}
}
