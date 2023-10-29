using audionysos.math;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization.Metadata;

namespace com.audionysos.text.edit; 
/// <summary>Represents a continuous sub sequence of characters in a <see cref="Text"/> by storing boundary indexes.</summary>
public class TextSpan {
	public event Action<TextSpan> CHANGED;

	/// <summary>Source text this span is part of.</summary>
	public Text source { get; private set; }
	/// <summary>Position of first character of this span in <see cref="source"/> text.</summary>
	public int start { get; set; }
	/// <summary>First position after last character of this span.</summary>
	public int end { get; set; }

	public Attributes attributes { get; } = new Attributes();

	public MutatingBehavior mutating;

	#region Derived
	public int length => end - start;

	/// <summary>Creates substring from <see cref="source"/> between <see cref="start"/> and <see cref="end"/>.
	/// Note that this main throw is the span is out of range.</summary>
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
		text.CHANGED += onTextChanged;
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

	//public void grow(int a) {
	//	var h = a / 2;
	//}

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
				start = e.at;
				end = start + d.right.length;
			}else if (d.onlyLeft) {
				var rc = length - d.left.length;
				end -= rc;
			}else if (d.splitted) {
				var rc = length - d.left.length - d.right.length;
				end -= rc;
			}else {
				Debugger.Break();
				Debug.Assert(false);
			}

		}

	}

	protected virtual void mutate(TextChangedEvent e, RangeOperationResult<int> d) {

	}


	public char this[int i] {
		get => source[start + i];
	}

	/// <inheritdoc/>
	public override string ToString() {
		return $@"({start}-{end}): {fullOrError}";
	}
}

public enum MutatingBehavior {
	/// <summary></summary>
	DEFUALT,
	/// <summary>Don't mutate - span's properties will stay untouched regardless of parent text change.</summary>
	STATIC,
	CUSTOM

}