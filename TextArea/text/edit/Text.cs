using com.audionysos.text.utils;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace com.audionysos.text.edit; 

/// <summary>Represents raw text data as set of logically connected chains (like lines) or characters.
/// This does not inculde any other no-data information such as related to text rendering, or formatting.</summary>
public class Text : IReadOnlyList<char> {
	private string chars;

	/// <summary>Number of buffered characters.</summary>
	public int Count => chars.Length;
	/// <summary>Returns character at given index.</summary>
	public char this[int index] => chars[index];

	private List<TextSpan> _lines = new List<TextSpan>();
	/// <summary>List of all lines associated with the text.</summary>
	public IReadOnlyList<TextSpan> lines => _lines;

	/// <summary>Span containing whole text.</summary>
	public TextSpan span { get; }

	/// <summary></summary>
	/// <param name="text">Source string from which the text will be produces.</param>
	public Text(string text = null) {
		chars = text ?? "";
		splitLines(chars, _lines);
		span = new TextSpan(this, 0, chars.Length-1);
	}

	/// <summary>Produces <see cref="lines"/> out of source text.</summary>
	/// <param name="text"></param>
	/// <param name="lines">List to end of which the lines spans will be add. If not specified, new list will be created.</param>
	private void splitLines(string text, IList<TextSpan> lines = null) {
		lines ??= new List<TextSpan>();
		char p, c = default; //current and previous characters;
		var ls = 0; //line start index
		for (int i = 0; i < text.Length; i++) {
			p = c; c = text[i]; var e = -1;
			if (c == '\n') e = i + 1;
			else if (p == '\r') e = i;
			if (e < 0) continue;
			lines.Add(new TextSpan(this, ls, e));
			ls = e;
		}
		lines.Add(new TextSpan(this, ls, text.Length));
	}

	/// <summary>Returns given character index clipped to the range of this text.</summary>
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

	/// <inheritdoc/>
	public IEnumerator<char> GetEnumerator() => chars.GetEnumerator();
	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => chars.GetEnumerator();

	/// <summary>Converts string into text.</summary>
	public static implicit operator Text(string t) => new Text(t);

	/// <inheritdoc/>
	public override string ToString() {
		return chars;
	}
}

/// <summary>Represents a line of <see cref="Text"/>.</summary>
public class TextLine : TextSpan {

	public TextLine(Text text, int start, int end)
		: base(text, start, end) {

	}
}
