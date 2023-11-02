using audionysos.math;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;

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

	public void move(int a) {
		start += a; end += a;
		CHANGED?.Invoke(this);
	}

	public void moveTo(int newStart) {
		int a = newStart - start;
		start += a; end += a;
		CHANGED?.Invoke(this);
	}

	public void setTo(int start, int end) {
		s = start; e = end;
		CHANGED?.Invoke(this);
	}

	public void expandTo(int pos) {
		if(pos < start) s = pos;
		else if(pos > end) e = pos;
		else return;
		CHANGED?.Invoke(this);
	}

	public void grow(int s, int e) {
		start -= s; end += e;
		CHANGED?.Invoke(this);
	}

	private void onTextChanged(TextChangedEvent e) {
		if (mutating == MutatingBehavior.STATIC)
			return;
		var r = start.to(end);
        if (r.endsBefore(e.range)) return; //not affected
		if (r.startsAfter(e.range)) { // only moved
			if (e.type == TextChangeType.ADDED)
				move(e.size);
			else if (e.type == TextChangeType.REMOVED)
				move(-e.size);
			else throw new Exception(@$"Unsupported text change event ""{e.type}""");
			return;
		}
		var d = r.subtract(e.range);
		if(e.type == TextChangeType.ADDED) {
			if (d.onlyRight || d.empty) move(e.size); //was pushed
			else {
				if (mutating == MutatingBehavior.CUSTOM)
					mutate(e, d);
				else {
					if (d.splitted) grow(0, e.size);
					else if (d.onlyLeft) grow(0, e.size);
				}
			}
		}else if(e.type == TextChangeType.REMOVED) {
			if (d.onlyRight) {
				//var rc = length - d.right.length;
				s = e.at;
				this.e = start + d.right.length;
			}else if (d.onlyLeft) {
				var rc = length - d.left.length;
				this.e -= rc;
			}else if (d.splitted) {
				var rc = length - d.left.length - d.right.length;
				this.end -= rc;
			}else {
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

public enum MutatingBehavior {
	/// <summary></summary>
	DEFUALT,
	/// <summary>Don't mutate - span's properties will stay untouched regardless of parent text change.</summary>
	STATIC,
	CUSTOM

}