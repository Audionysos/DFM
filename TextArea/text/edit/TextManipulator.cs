﻿using audionysos.math;
using com.audionysos.text.render;
using com.audionysos.text.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace com.audionysos.text.edit; 

/// <summary>Provides methods for manipulating <see cref="Text"/> and associated data.</summary>
public class TextManipulator {
	/// <summary>Text that is been manipulated.</summary>
	public Text text { get; private set; }
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
	/// <summary>Given access to information associated with each character of <see cref="text"/>.</summary>
	public IReadOnlyList<CharInfo> infos { get; private set; }
	public TextDisplayContext context { get; }
	private TextDisplayContext ctx => context;

	#region Initialization

	#region Constructors
	public TextManipulator(render.TextDisplayContext context) {
		text = new Text();
		init();
		this.context = context;
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
		createInfos();
	}

	public void insert(char c) {
		//text[0].
	}

	private void createInfos() {
		var lst = new List<CharInfo>(text.Count);
		foreach (var c in text) {
			var i = new CharInfo(c, text.span);
			lst.Add(i);
		}
		infos = lst;
	}
	#endregion

	#region Navigation/Positioning
	public ColumnLine clipPosition(ColumnLine p) {
		var y = p.y; var x = p.x;
		y = y.clip(0, text.lines.Count-1);
		if (ctx.renderer.lines.Count == 0) return (0, 0);
		var rl = ctx.renderer.lines[y];
		x = x.clip(0, rl.columns);
		return (x, y);
	}

	public ColumnLine clipLine(ColumnLine p) {
		var y = p.y;
		y = y.clip(0, text.lines.Count - 1);
		if (ctx.renderer.lines.Count == 0) return (0, p.x);
		return (p.x, y);
	}

	public CharLine toCharLine(ColumnLine p) {
		var y = p.y; var x = p.x;
		y = y.clip(0, text.lines.Count - 1);
		var rl = ctx.renderer.lines[y];
		x = rl.glyphAt(x, out _);
		return (x, y);
	}

	public ColumnLine getPosition(CharLine p) {
		var y = p.y; var x = p.x;
		y = y.clip(0, text.lines.Count - 1);
		var rl = ctx.renderer.lines[y];
		x = rl.columnAt(x);
		return (x, y);
	}
	#endregion

	/// <summary>Returns information for <see cref="text"/>'s character at given index.</summary>
	public CharInfo getCharInfo(int index) {
		return infos[index];
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
