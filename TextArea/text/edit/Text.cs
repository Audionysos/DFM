﻿using com.audionysos.text.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace com.audionysos.text.edit; 

/// <summary>Represents raw text data as set of logically connected chains (like lines) or characters.
/// This does not include any other no-data information such as related to text rendering, or formatting.</summary>
public class Text : IReadOnlyList<char> {
	private string chars;

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
		chars = text ?? "";
		splitLines(chars, _lines);
		span = new TextSpan(this, 0, chars.Length-1);
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

	/// <summary>Returns given character index clipped to the range of this text.</summary>
	public int clipIndex(int i) {
		Debug.Assert(chars != null);
		//if (chars.Length == 0) return 0;
		if (i < 0) return 0;
		if (i > chars.Length) return chars.Length;
		return i;
	}

	/// <summary>Returns new, proper position where any excess of x position is carried to next lines.</summary>
	public Int2 clipPosition(Int2 p) {
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

	/// <summary>Returns character index at given character-line position.</summary>
	public int getIndex(CharLine pos) {
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

	/// <summary>Returns character-line index at given absolute character index.</summary>
	public (int ch, int ln) getPos(int ch) {
		for (int i = 0; i < lines.Count; i++) {
			var l = lines[i];
			if (ch < l.end) return (ch - l.start,i);
		}return (lines[^1].end ,lines.Count - 1);
	}
}

/// <summary>Represents a line of <see cref="Text"/>.</summary>
public class TextLine : TextSpan {

	public TextLine(Text text, int start, int end)
		: base(text, start, end) {

	}
}
