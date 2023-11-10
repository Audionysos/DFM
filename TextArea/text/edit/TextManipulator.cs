using audionysos.math;
using com.audionysos.text.render;
using com.audionysos.text.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace com.audionysos.text.edit; 

/// <summary>Provides methods for manipulating <see cref="Text"/> and associated data.</summary>
public class TextManipulator {
	private Text _t;
	/// <summary>Text that is been manipulated.</summary>
	public Text text {
		get => _t;
		private set {
			_t = value;
			_t.CHANGED += onTextChanged;
		}
	}

	/// <summary>Sores information about currently selected text.</summary>
	public TextSelection selection { get; private set; }
	private TextCarets _carets;
	/// <summary>Contains information about current caret(s) placement.</summary>
	public TextCarets carets {
		get => _carets;
		set {
			if(value == null) throw new ArgumentNullException(nameof(value));
			if (value.man != this) throw new ArgumentException("Cannot set carets with a different manipulator.");
			_carets = value;
		}
	}
	/// <summary>Stores information about all distinguishable text regions.</summary>
	public TextSpans regions { get; private set; }
	public List<CharInfo> _infos;
	/// <summary>Given access to information associated with each character of <see cref="text"/>.</summary>
	public IReadOnlyList<CharInfo> infos => _infos;
	public TextDisplayContext context { get; }
	private TextDisplayContext ctx => context;

	#region Initialization

	#region Constructors
	public TextManipulator(render.TextDisplayContext context) {
		text = new Text();
		this.context = context;
		init();
	}

	public TextManipulator(string text, render.TextDisplayContext context) {
		this.text = text;
		this.context = context;
		init();
	}

	public TextManipulator(string text) {
		this.text = text;
		init();
	}
	#endregion

	private void init() {
		selection = new TextSelection(text);
		carets = new TextCarets(this);
		carets.CHANGED += onCaretsChanged;
		createInfos();
	}


	private void createInfos() {
		var lst = new List<CharInfo>(text.Count);
		foreach (var c in text) {
			var i = new CharInfo(c, text.span);
			lst.Add(i);
		}
		_infos = lst;
	}
	#endregion

	#region Editing
	public void insert(char c) {
		if (c == '\b') { remove(); return; }
		if (c == '\r') c = '\n';
		isInserting = true;
		text.insert(c.ToString(), carets.ch);
		carets++;
		isInserting = false;
	}

	public void insert(string t) {
		//return;
		isInserting = true;
		text.insert(t, carets.ch);
		carets.move(t.Length);
		isInserting = false;
	}

	public void remove(int c = 0) {
		if(c == 0) {
			if (selection.length > 0) {
				removeSelection();
				return;
			} else c = 1; //TODO: \r\n should be removed together be default
		}
		if (carets.ch == 0) return;
		carets.move(-c); 
		text.remove(carets.ch, c);
	}

	private void removeSelection() {
		//var l = selection.length;
		text.remove(selection.start, selection.length);
		carets.ch = selection.end = selection.start;
		//carets.move(-l); //this causes problem if caret is on start and it doesn't actually move
	}
	#endregion

	private void onTextChanged(TextChangedEvent e) {
		if(e.type == TextChangeType.ADDED) {
			var nfs = createInfos(e.content);
			_infos.InsertRange(e.at, nfs);
		}else if(e.type == TextChangeType.REMOVED) {
			_infos.RemoveRange(e.at, e.size);
		}
		context.renderer?.render();
	}

	private bool isInserting;
	/// <summary>Movement of the caret will mark <see cref="selection"/> if set to true.</summary>
	public bool isSelecting { get; set; }
	private void onCaretsChanged(TextCaret caret, int change) {
		var pp = carets.ch - change;
		if (isSelecting && !isInserting) {
			if (pp == selection.start) {
				selection.start = carets.ch;
			} else if (pp == selection.end) {
				selection.end = carets.ch;
			} else Debug.Assert(false);
		} else {
			if(selection.length == 0) {
				selection.setTo(carets.ch, carets.ch);
				return;
			}
			if (change > 0) {
				selection.start = selection.end;
			} else if(change < 0) {
				selection.end = selection.start;
			} else {
				selection.setTo(carets.ch, carets.ch);
			}
			caret.ch = selection.start;
		}
	}

	#region Navigation/Positioning
	public ColumnLine clipPosition(ColumnLine p) {
		var y = p.y; var x = p.x;
		y = y.clip(0, text.lines.Count-1);
		if (ctx.renderer.lines.Count == 0) return (0, 0);
		var rl = ctx.renderer.lines[y];
		x = x.clip(0, rl.columns);
		return (x, y);
	}

	/// <summary>Clips <see cref="Int2.y"/> value to current bounds of the <see cref="text"/>.</summary>
	public ColumnLine clipLine(ColumnLine p) {
		var y = p.y;
		y = y.clip(0, text.lines.Count - 1);
		if (ctx.renderer.lines.Count == 0) return (0, p.x);
		return (p.x, y);
	}

	/// <summary>Returns character-line position in the <see cref="text"/>.</summary>
	public CharLine getPosition(ColumnLine p) {
		var y = p.y; var x = p.x;
		y = y.clip(0, text.lines.Count - 1);
		var rl = ctx.renderer.lines[y];
		x = rl.glyphAt(x, out _);
		return (x, y);
	}

	/// <summary>Returns absolute character index in the <see cref="text"/> at given position.</summary>
	public int getCharacter(ColumnLine p)
		=> text.getIndex(getPosition(p));

	public ColumnLine getPosition(CharLine p) {
		var y = p.y; var x = p.x;
		y = y.clip(0, text.lines.Count - 1);
		var rl = ctx.renderer.lines[y];
		x = rl.columnAt(x);
		return (x, y);
	}

	/// <summary>Returns column-line position at given absolute character index in the <see cref="text"/>.</summary>
	public ColumnLine getPosition(int ch) 
		=> getPosition(text.getPos(ch));
	#endregion

	/// <summary>Returns information for <see cref="text"/>'s character at given index.</summary>
	public CharInfo getCharInfo(int index) {
		return infos[index];
	}

	private CharInfo[] createInfos(string content) {
		var a  = new CharInfo[content.Length];
		for (int i = 0; i < content.Length; i++) {
			a[i] = new CharInfo(content[i], text.span);
		}return a;
	}
}

public class CharInfo {
	/// <summary>Stores information about all spans of text the character is associated with.</summary>
	public List<TextSpan> spans { get; private set; } = new List<TextSpan>(1);
	/// <summary>Specifies a format in which the character is displayed.</summary>
	public ITextFormat format;
	public object rect;
	public char character;

	public CharInfo(char c, TextSpan span) {
		character = c;
		spans.Add(span);
	}
}

#region Attributes
/// <summary>Sores dynamic composition of attributes.</summary>
public class Attributes {
	public static readonly NoAttributes none = NoAttributes.instance;

	public T get<T>() { return default; }

	internal void Add<T>(T a) {
		throw new NotImplementedException();
	}
}

public class NoAttributes : Attributes {
	public static readonly NoAttributes instance = new NoAttributes();
	private NoAttributes() { }
}

public class TextFormat : ITextFormat, ITextFormatProvider {
	/// <summary>Returns this.</summary>
	public ITextFormat textFormat => this;
	/// <inheritdoc/>
	public double size { get; set; } = 11;
	public IFill foreground { get; set; } = (Color)0x000000FF;
	public IFill background { get; set; } = (Color)0xFFFFFFFF;
	public ITextFont font { get; set; } = new NamedFont("Consolas");
	//public ITextFont font { get; set; } = new NamedFont("Cascadia Code");
}

/// <summary>Interface for objects providing <see cref="ITextFormat"/>.</summary>
public interface ITextFormatProvider {
	ITextFormat textFormat { get; }
}

/// <summary>Specifies format that could be applied specifically to text, texts spans, or a single characters.</summary>
public interface ITextFormat {
	ITextFont font { get; set; }
	/// <summary>Font size.</summary>
	double size { get; set; }
	/// <summary>A fill that is applied to text.</summary>
	IFill foreground { get; set; }
	/// <summary>Fill that is applied to what is behind the text.</summary>
	IFill background { get; set; }
}

public class NamedFont : ITextFont {
	public string name { get; }
	public NamedFont(string name) {
		this.name = name;
	}
	/// <inheritdoc/>
	public override string ToString() {
		return name;
	}
}

public interface ITextFont {
	public string name { get; }
} 


#endregion
