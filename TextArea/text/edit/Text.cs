using audionysos.math;
using com.audionysos.text.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace com.audionysos.text.edit; 

/// <summary>Represents raw text data as set of logically connected chains (like lines) or characters.
/// This does not include any other no-data information such as related to text rendering, or formatting.</summary>
public class Text : IReadOnlyList<char> {
	public event Action<TextChangedEvent> CHANGED;

	private StringBuilder chars;

	/// <summary>Number of buffered characters.</summary>
	public int Count => chars.Length;
	/// <summary>Returns character at given index.</summary>
	public char this[int index] => chars[index];

	private List<TextLine> _lines = new List<TextLine>();
	/// <summary>List of all lines associated with the text.</summary>
	public IReadOnlyList<TextLine> lines => _lines;

	/// <summary>Span containing whole text.</summary>
	public TextSpan span { get; }

	/// <summary></summary>
	/// <param name="text">Source string from which the text will be produced.</param>
	public Text(string text = null) {
		chars = new StringBuilder(text ?? "");
		splitLines(text ?? "", _lines);
		span = new TextSpan(this, 0, chars.Length-1);

	}

	public void insert(string x, int p) {
		chars.Insert(p, x);
		if (CHANGED == null) return;
		var e = new TextChangedEvent(this, TextChangeType.ADDED, p, x);
		CHANGED(e);
	}

	public void remove(int p, int count) {
		var x = "";
		for (int i = p; i < p + count; i++)
			x += chars[i];
		chars.Remove(p, count);
		if (CHANGED == null) return;
		var e = new TextChangedEvent(this, TextChangeType.REMOVED, p, x);
		CHANGED(e);
	}

	/// <summary>Produces <see cref="lines"/> out of source text.</summary>
	/// <param name="text"></param>
	/// <param name="lines">List to end of which the lines spans will be add. If not specified, new list will be created.</param>
	private void splitLines(string text, IList<TextLine> lines = null) {
		lines ??= new List<TextLine>();
		char p, c = default; //current and previous characters;
		var ls = 0; //line start index
		for (int i = 0; i < text.Length; i++) {
			p = c; c = text[i]; var e = -1;
			if (c == '\n') e = i + 1;
			else if (p == '\r') e = i;
			if (e < 0) continue;
			lines.Add(new TextLine(this, ls, e));
			ls = e;
		}
		lines.Add(new TextLine(this, ls, text.Length));
	}

	#region Navigation
	/// <summary>Returns given character index clipped to the range of this text.</summary>
	public int clipIndex(int i) {
		Debug.Assert(chars != null);
		//if (chars.Length == 0) return 0;
		if (i < 0) return 0;
		if (i > chars.Length) return chars.Length;
		return i;
	}

	/// <summary>Returns new, proper position where any excess of x position is carried to next lines.</summary>
	public CharLine clipPosition(CharLine p) {
		var y = p.y; var x = p.x;
		if (y < 0) y = 0;
		if(y >= _lines.Count) y = _lines.Count -1;
		var l = _lines[y]; 
		while (x >= l.length && y < _lines.Count -1) {
			x -= l.length;
			l = _lines[++y];
		}
		while (x < 0 && y > 0) {
			l = _lines[--y];
			x += l.length;
		}
		if (x < 0) x = 0;
		if (x > l.length) x = l.length;
		return (x, y);
	}

	/// <summary>Returns character-line index at given absolute character index.</summary>
	public CharLine getPos(int ch) {
		for (int i = 0; i < lines.Count; i++) {
			var l = lines[i];
			if (ch < l.end) return (ch - l.start,i);
		}return (lines[^1].end ,lines.Count - 1);
	}

	/// <summary>Returns absolute character index at given character-line position.</summary>
	public int getIndex(CharLine pos) {
		return _lines[pos.y].start + pos.x;
	}

	/// <inheritdoc/>
	public IEnumerator<char> GetEnumerator() {
		for (int i = 0; i < chars.Length; i++) {
			yield return chars[i];
		}
		yield break;
	}
	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	#region Conversions
	/// <summary>Converts string into text.</summary>
	public static implicit operator Text(string t) => new Text(t);

	/// <summary>False if null.</summary>
	public static implicit operator bool(Text t) => t != null;

	/// <inheritdoc/>
	public override string ToString() {
		return chars.ToString();
	}
	#endregion

}

public class TextChangedEvent {
	public Text text { get; }
	public TextChangeType type { get; }
	public int at { get; }
	public int end => at + content.Length;
	public int size => content.Length;
	public Range<int> range { get; }
	/// <summary>Text that was add or removed.</summary>
	public string content { get; }

	
	public TextChangedEvent(Text text, TextChangeType type, int at, [DisallowNull]string content) {
		this.text = text;
		this.type = type;
		this.at = at;
		this.content = content;
		range = at.to(end);
	}

}

public enum TextChangeType {
	ADDED,
	REMOVED,
}

/// <summary>Represents a line of <see cref="Text"/>.</summary>
public class TextLine : TextSpan {

	public TextLine(Text text, int start, int end)
		: base(text, start, end) {

	}
}
