using com.audionysos.text.utils;
using System;

namespace com.audionysos.text.edit; 

/// <summary>Represents a master caret that can hold additional child carets.
/// Any modification of master caret will be also relatively applied to it's children.</summary>
public class TextCarets : TextCaret {

	public TextCarets(TextManipulator man) : base(man) {

	}

	public static TextCarets operator ++(TextCarets c)
		{ c.move(1); return c;}

	public static TextCarets operator --(TextCarets c)
		{ c.move(-1); return c; }

}

public class TextCaret {
	/// <summary>Second argument is different between current and previous positions.</summary>
	public event Action<TextCaret, int> CHANGED;
	private Text text { get; set; }
	public TextManipulator man { get; }

	private int pc;
	private int _c;
	/// <summary>Absolute index of character within the text before which the caret is placed.</summary>
	public int ch {
		get => _c;
		set =>  pos = man.getPosition(value);
	}

	/// <summary>Index of the character in current line before which the caret is placed.</summary>
	public int lCh { get; private set; }

	private ColumnLine _pos = new ColumnLine();
	/// <summary>Column-line coordinates of current caret position.</summary>
	public ColumnLine pos {
		get => _pos;
		set {
			_pos.set(value);
		}
	}
	public ColumnLine actualPos => man.clipPosition(pos);

	private bool correcting;
	private void onPositionChanged(Int2 p, Int2 ch) {
		if (correcting) return;
		correcting = true;
		pos = man.clipLine(pos);
		correcting = false;
		var chl = man.getPosition(pos);
		lCh = chl.x;
		_c = text.getIndex(chl);

		var d = _c - pc;
		pc = _c;
		CHANGED?.Invoke(this, d);
	}

	public static TextCaret operator ++(TextCaret c)
		=> c.move(1);
	public static TextCaret operator --(TextCaret c)
		=> c.move(-1);

	public TextCaret move(int v) {
		_c += v;
		if(ch > text.Count) _c = text.Count;
		if (ch < 0) _c = 0;
		var p = text.getPos(ch);
		var np = man.getPosition(p);
		pos = np;
		return this;
	}

	public TextCaret(TextManipulator man) {
		this.man = man;
		text = man.text;
		_pos.CHANGED += onPositionChanged;
	}


}

/// <summary>Represents position withing a text view/editor.
/// Note that <see cref="ColumnLine"/> and <see cref="CharLine"/> are different classes because a character may occupy different number of columns.</summary>
public class ColumnLine : Int2 {
	public ColumnLine(int x = 0, int y = 0) : base(x, y) {}

	/// <summary></summary>
	new public ColumnLine copy() => new ColumnLine(x, y);

	public static implicit operator ColumnLine((int x, int y) t)
		=> new ColumnLine(t.x, t.y);
}

/// <summary>Represents position within a text.
/// Note that <see cref="ColumnLine"/> and <see cref="CharLine"/> are different classes because a character may occupy different number of columns.</summary>
public class CharLine : Int2 {
	public CharLine(int x = 0, int y = 0) : base(x, y) { }

	/// <summary></summary>
	new public CharLine copy() => new CharLine(x, y);

	public static implicit operator CharLine((int x, int y) t)
		=> new CharLine(t.x, t.y);
}
