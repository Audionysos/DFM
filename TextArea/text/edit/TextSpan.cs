using System;

namespace com.audionysos.text.edit; 
/// <summary>Represents a continous sub sequence of characters in a <see cref="Text"/> by stroing boundry indices.</summary>
public class TextSpan {

	/// <summary>Source text this span is part of.</summary>
	public Text source { get; private set; }
	/// <summary>Position of first character of this span in <see cref="source"/> text.</summary>
	public int start { get; set; }
	/// <summary>Position of last character of this span.</summary>
	public int end { get; set; }

	public Attributes attributes { get; } = new Attributes();

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

	/// <inheritdoc/>
	public override string ToString() {
		return $@"({start}-{end}): {fullOrError}";
	}
}
