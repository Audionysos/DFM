using audionysos.math;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Channels;

namespace com.audionysos.text.edit; 
/// <summary>Represents a continuous sub sequence of characters in a <see cref="Text"/> by storing boundary indexes.</summary>
public class TextSpan {
	public event Action<TextSpan> CHANGED;

	/// <summary>Source text this span is part of.</summary>
	public Text source { get; private set; }
	private int s;
	/// <summary>Position of first character of this span in <see cref="source"/> text.</summary>
	public int start { get => s; set {
			if (value == s) return;
			if (value > end) e = value;
			else s = value;
			CHANGED?.Invoke(this);
		}
	}
	private int e;
	/// <summary>First position after last character of this span.</summary>
	public int end { get => e; set {
			if (value == e) return;
			if (value < start)
				s = value;
			else e = value;
			CHANGED?.Invoke(this);
		}
	}

	public Attributes attributes { get; } = new Attributes();

	public MutatingBehavior mutating;

	#region Derived
	public int length => e - s;
	/// <summary>Index of last character that is included in the span.</summary>
	public int last => e - 1;

	/// <summary>Creates substring from <see cref="source"/> between <see cref="start"/> and <see cref="end"/>.
	/// Note that this may throw if the span is out of range.</summary>
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

	/// <summary>Returns <see cref="text"/> or message of the exception if the attempt failed.</summary>
	public string fullOrError {
		get {
			try { return text; }
			catch (Exception e) { return e.Message; }
		}
	}
	#endregion

	public TextSpan([DisallowNull]Text text, int start, int end) {
		source = text;
		order(text.clipIndex(start), text.clipIndex(end));
		text.CHANGED += onTextChanged;
	}

	public void dispose() {
		if (!source) return;
		source.CHANGED -= onTextChanged;
	}

	public TextSpan(int start, int end) {
		order(start, end);
	}

	private void order(int start, int end) {
		if (end < start) {
			s = end;
			e = start;
		} else {
			s = start;
			e = end;
		}
	}

	public TextSpan setTo(int start, int end) {
		s = start; e = end;
		CHANGED?.Invoke(this);
		return this;
	}

	public void expandTo(int pos) {
		if(pos < start) s = pos;
		else if(pos > end) e = pos;
		else return;
		CHANGED?.Invoke(this);
	}

	public void grow(int s, int e) {
		this.s -= s; this.e += e;
		CHANGED?.Invoke(this);
	}

	private void onTextChanged(TextChangedEvent e) {
		//if (this is TextLine) Debugger.Break();
		if (mutating == MutatingBehavior.STATIC)
			return;
		var re = mutating == MutatingBehavior.DEFUALT ? 1 : 0;
		var r = start.to(end-re);
        if (r.endsBefore(e.range)) return; //not affected
		if (r.startsAfter(e.range)) { // only moved
			if (e.type == TextChangeType.ADDED)
				this.move(e.size);
			else if (e.type == TextChangeType.REMOVED)
				this.move(-e.size);
			else throw new Exception(@$"Unsupported text change event ""{e.type}""");
			return;
		}
		var d = r.subtract(e.range);
		if(e.type == TextChangeType.ADDED) {
			if (d.onlyRight || d.empty) this.move(e.size); //was pushed
			else {
				if (mutating == MutatingBehavior.CUSTOM)
					mutate(e, d);
				else {
					if (d.splitted) grow(0, e.size);
					else if (d.onlyLeft) grow(0, e.size);
					else grow(0, e.size);
				}
			}
		}else if(e.type == TextChangeType.REMOVED) {
			if (d.onlyRight) {
				//var rc = length - d.right.length;
				s = e.at;
				this.e = start + d.right.length;
				if (mutating == MutatingBehavior.DEFUALT_EXPAND_FORWARD)
					this.e--;
			}else if (d.onlyLeft) {
				var rc = length - d.left.length;
				this.e -= rc;
			}else if (d.splitted) {
				var rc = length - d.left.length - d.right.length;
				this.e -= rc;
				if (mutating == MutatingBehavior.DEFUALT_EXPAND_FORWARD)
					this.e--;
			}else if (d.single == r) {
				this.e = s;
			} else {
				s = this.e = e.at;
				//May happen when span is empty, don't care for now
				//Debugger.Break();
				//Debug.Assert(false);
			}
			CHANGED?.Invoke(this);

		}

	}

	protected virtual void mutate(TextChangedEvent e, RangeOperationResult<int> d) {

	}

	/// <summary>Returns true if character is in range of this span.</summary>
	internal bool contains(int ch) 
		=> ch >= start && ch < end;

	/// <summary>Tries to read character at given local position.</summary>
	public bool tryAt(int i, out char c) {
		c = default;
		var p = start + i;
		if (p >= source.Count || p < 0)
			return false;
		c = source[p];
		return true;
	}

	public char this[int i] {
		get => source[start + i];
	}

	public static implicit operator TextSpan(Range<int> r)
		=> new TextSpan(r.start, r.end);

	public static implicit operator TextSpan((int start, int end) r)
		=> new TextSpan(r.start, r.end);

	/// <inheritdoc/>
	public override string ToString() {
		return source ? $@"({start}-{end}): {fullOrError}"
					  : $@"({start}-{end}) [free TextSpan]";
	}

}

public static class TextSpanExtensions {
	public static T moveTo<T>(this T s, int newStart) where T : TextSpan {
		int a = newStart - s.start;
		s.setTo(s.start + a, s.end + a);
		return s;
	}

	public static T move<T>(this T s, int a) where T : TextSpan
		=> (T)s.setTo(s.start + a, s.end + a);

	/// <summary>Returns local span index of character that satisfies given predicate.</summary>
	public static int last(this TextSpan s, Predicate<char> p) {
		for (int i = s.length-1; i >= 0; i--)
			if (s.tryAt(i, out var ch) && p(ch))
				return i;
		return -1;
	}

	/// <summary>Returns local span index of last character before tailing new line break ("\n" or "\r\n").
	/// I line break is not at the end of the span, span.length -1 is returned.</summary>
	public static int lastBeforeNewLine(this TextSpan s) {
		if (s.length == 0) return -1;
		var i = s.length-1;
		var c = s[i];
		if (c == '\r') return i - 1;
		else if (c == '\n') {
			if (s.length == 1) return -1;
			if (s[i - 1] == '\r') return i - 2;
			return i - 1;
		} else return i;
	}
}

public enum MutatingBehavior {
	/// <summary></summary>
	DEFUALT,
	DEFUALT_EXPAND_FORWARD,
	/// <summary>Don't mutate - span's properties will stay untouched regardless of parent text change.</summary>
	STATIC,
	CUSTOM

}