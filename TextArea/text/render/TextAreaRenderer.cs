using audionysos.display;
using audionysos.geom;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using audionysos.math;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace com.audionysos.text.render; 

public class TextAreaRenderer {
	TextDisplayContext ctx;
	private Text tracked;

	public TextAreaRenderer(TextDisplayContext ctx) {
		this.ctx = ctx;
		ctx.view.view.addChild(caret.view);
	}

	private List<RenderedGlyph> rendered = new List<RenderedGlyph>();

	//private Graphics g;
	//public void render2() {
	//	rendered.Clear();
	//	var man = ctx.manipulator;
	//	var grc = new GlyphRenderingContext();
	//	ctx.format.size = 12; 
	//	grc.gfx = ctx.gfx;
	//	grc.gfx.clear();
	//	var dw = 10d; //default width
	//	var w = 0d; //char width
	//	var linesSpacing = 4; var tabSize = 4;
	//	var lns = man.text.lines;
	//	for (int i = 0; i < lns.Count; i++) {
	//		var l = lns[i];
	//		for (int j = 0; j < l.length; j++) {
	//			var chi = man.getCharInfo(l.start + j);
	//			//if (chi.character == '\n') continue;
	//			//if (chi.character == '\t') {
	//			//	grc.position.x += dw * tabSize;
	//			//	continue;
	//			//}
	//			var rg = renderCharacter(chi, grc);
	//			w = (rg == null) ? dw : rg.size.x;
	//			if(rg != null) rendered.Add(rg);
	//			grc.position.x += w;
	//		}
	//		grc.position.x = 0;
	//		grc.position.y += ctx.format.size + linesSpacing;
	//	}

	//	ctx.gfx.close();
	//	renderCarets();
	//}

	private List<TextLineLayout> _lines = new();
	public IReadOnlyList<TextLineLayout> lines => _lines;

	public void render() {
		trackText();

		foreach (var r in rendered) {
			ctx.container.removeChild(r.data as Shape);
		}

		rendered.Clear();
		_lines.Clear();
		var man = ctx.manipulator;
		var grc = new GlyphRenderingContext();
		ctx.format.size = 12;
		//grc.gfx = ctx.gfx;
		//grc.gfx.clear();
		//var linesSpacing = 4;
		grc.position.y = linesSpacing;

		var txt = man.text;
		var lc = txt.lines.Count;
		for (int i = 0; i < lc; i++) {
			var tl = txt.lines[i];
			var l = new TextLineLayout(tl, man);
			_lines.Add(l);

			for (int j = 0; j < tl.length; j++) {
				var chi = man.getCharInfo(tl.start + j);
				var rg = renderCharacter(chi, grc);
				var v = rg.data as Shape;
				v.transform.pos = grc.position;
				rendered.Add(rg);
				l.Add(rg);
				grc.position.x += charWidth * rg.template.columnWidth;
				//grc.position.x += rg.size.x;
				ctx.container.addChild(v);
			}

			grc.position.x = 0;
			grc.position.y += ctx.format.size + linesSpacing;
		}
	}

	private void trackText() {
		if (tracked == ctx.manipulator.text) return;
		//if (tracked != null) tracked.CHANGED -= onTextChanged;
		tracked = ctx.manipulator.text;
		//tracked.CHANGED += onTextChanged;
	}

	private void onTextChanged(TextChangedEvent @event) {
		render();
	}

	private RenderedGlyph renderCharacter(CharInfo chi, GlyphRenderingContext grc) {
		if (chi.character == ' ') return spaceGlyph(grc);
		if (chi.character == '\t') return tabGlyph(grc);
		if (chi.character == '\n') return newLineGlyph(grc);
		var r = ctx.glyphsRenderer;
		var fp = chi.spans[0].attributes.get<ITextFormatProvider>();
		var tf = fp?.textFormat ?? ctx.format;
		//Maybe it would be better to do per/span rendering. Need to think about that...
		//g.beginFill(tf.foreground);
		var gf = ctx.glyphs.get(chi.character, tf);
		grc.g = gf;
		grc.format = tf;
		return r.render(grc) ?? missingGlyph(grc, chi.character);
	}

	/// <summary>Returns column-line position for given local coordinates.
	/// Note this may return position out of character range for example negative values.</summary>
	public ColumnLine getPosition(IPoint2 p, bool clip = false) {
		p = p.copy();
		p.y -= linesSpacing;
		p.y /= ctx.format.size + linesSpacing;
		p.x /= charWidth;
		//var cl = new ColumnLine((int)Math.Round(p.x), (int)Math.Round(p.y));
		var cl = new ColumnLine((int)p.x, (int)p.y);
		if (clip) {
			cl.y = cl.y.clip(0, lines.Count - 1);
			var l = lines[cl.y];
			cl.x = cl.x.clip(0, l.columns-l.lineEndSize.clip(1)); //TODO: The line end size is stupid and will break in future
		}

		if(cl.y < lines.Count) { //corrects column position for wider chars like \t
			var l = lines[cl.y];
			var c = l.glyphAt(cl.x, out var g);
			if (g == null) return cl;
			cl.x = l.columnAt(c);
		}
		return cl;
	}

	#region Special characters
	private RenderedGlyph missingGlyph(GlyphRenderingContext grc, char character) {
		return new RenderedGlyph(
			new Glyph($"missing '{character}'",0,0,null),
			grc.position.copy(),
			new Point2(grc.format.size, 0)
			, new Shape());
	}

	private Glyph space = new Glyph("' ' (space)", 0, 0, null);
	private RenderedGlyph spaceGlyph(GlyphRenderingContext grc) {
		return new RenderedGlyph(
			space,
			grc.position.copy(),
			new Point2(grc.format.size, 0)
			, new Shape());
	}

	private Glyph tab = new Glyph(@"'\t' (tab)", 0, 0, null)
		{ columnWidth = 4 };
	private RenderedGlyph tabGlyph(GlyphRenderingContext grc) {
		return new RenderedGlyph(
			tab,
			grc.position.copy(),
			new Point2(grc.format.size * 4, 0)
			, new Shape());
	}

	private Glyph newLine = new Glyph(@"'\n' (newLine)", 0, 0, null);
	private RenderedGlyph newLineGlyph(GlyphRenderingContext grc) {
		return new RenderedGlyph(
			newLine,
			grc.position.copy(),
			new Point2(0, 0)
			, new Shape());
	}
	#endregion

	//private Sprite carets = new Sprite();
	private CaretView caret = new CaretView();
	public void renderCarets() {
		//return;
		var cts = ctx.manipulator.carets;
		if (cts.ch < 0) return;
		if (rendered.Count < cts.ch) return;
		RenderedGlyph rg; IPoint2 pos;
		if (rendered.Count == cts.ch) {
			//rg = rendered[^1]; var pg = rendered[^2];
			//var w = rg.position.x - pg.position.x;
			//pos = rg.position + (w, 0); //we use calculated spacing for better alignment in uniform grid
			//pos = rg.position + (rg.size.x, 0);
			pos = (Point2)(cts.actualPos.x * charWidth, cts.pos.y * lineHeight);
		}else {
			rg = rendered[cts.ch];
			pos = rg.position;
		}
		//cts.pos = new Int2((int)cp.position.x, (int)cp.position.y);
		caret.postion(pos);
	}

	private double charWidth => ctx.format.size * .5;
	private double charHeight => ctx.format.size;
	private double linesSpacing => 4;
	private double lineHeight => charHeight + linesSpacing;

	#region Span border
	public void drawBorder(TextSpan span,  Graphics g) {
		g.clear();
		if (span.length == 0) return;
		var b = getBorder(span);
		g.beginFill(0x3388FF, .1);
		g.lineStyle(1, 0x3388FF);
		foreach (var p in b)
			g.newPath(p);
	}

	public List<Path> getBorder(TextSpan span) {
		var path = new Path();
		var paths = new List<Path>(2) { path };
		var p = ctx.manipulator.getPosition(span.start);
		var ip = p.copy();
		var r = getGlyphRect(p); //first rect in shape.
		path.Add(r.topLeft);
		var ll = lastInLine(p, span);
		path.Add(ll.topRight);
		var nl = lastInLine(p.add(0,1), span);
		if(!nl || nl.right <= r.left) { //selection are not connected. Note this may happen only between first and second line since only first line may start with characters that are not part of the span.
			path.Add(ll.bottomRight, r.bottomLeft, r.topLeft);
			//starting new path
			r = getGlyphRect(p.set(0, p.y));
			path = new Path() { r.topLeft };
			paths.Add(path);
		}else {
			path.Add(ll.bottomRight);

		}

		//below should be in single shape
		ll = lastInLine(p, span);
		if (!ll) { return paths.GetRange(0,1);}
		path.Add(ll.topRight);
		nl = lastInLine(p.add(0, 1), span);
		while (nl) { // moving down
			while (nl && nl.right == ll.right) {
				ll = nl; nl = lastInLine(p.add(0, 1), span);
			}
			path.Add(ll.bottomRight);
			if (nl) { //still in span
				path.Add(nl.topRight);
				ll = nl; nl = lastInLine(p.add(0,1), span);
			}
		}
		//we are at the bottom
		path.Add(ll.bottomRight);
		ll = getGlyphRect(p.set(0, p.y-1));
		path.Add(ll.bottomLeft);
		if(ll.left != r.left) {
			ll = getGlyphRect((0,ip.y + 1));
			path.Add(ll.topLeft, r.bottomLeft);
		}
		path.Add(r.topLeft);

		return paths;
	}

	/// <summary>Returns rect of last character in line at given position that is also in the range of given span.
	/// If the span don't intersect with the line, null is returned.</summary>
	private Rect lastInLine(ColumnLine np, TextSpan span) {
		if (np.y >= lines.Count) return null;
		var l = lines[np.y];
		var x = l.columns;
		ColumnLine last = (l.columns, np.y);
		var llch = ctx.manipulator.getCharacter(last); //global index of last character in line.
		if (llch == tracked.Count) llch--; //on the last line, last returned index is equal to count but we don't want that when marking a span
		if (span.start > llch) return null;
		var d = span.last - llch;
		if (d > 0) return getGlyphRect((x, np.y));
		//span ends before the line ends.
		llch = l.Count + d-1; //TODO: span is updated before glyphs line 
		if(llch < 0) return null; // span ends before line start
		x = l.columnAt(llch);
		return getGlyphRect((x, np.y));
	}


	/// <summary>Returns glyph rect at given absolute char index.</summary>
	private Rect getGlyphRect(int ch) {
		var p = ctx.manipulator.getPosition(ch);
		return new Rect((p.x * charWidth, p.y * charHeight)
			, (charWidth, charHeight));
	}

	public Rect getGlyphRect(ColumnLine p) {
		var g = getGlyphAt(p);
		var gw = g ? g.template.columnWidth : 0;
		return new Rect((p.x * charWidth, p.y * lineHeight)
			, (charWidth*gw, lineHeight));
	}

	private RenderedGlyph getGlyphAt(ColumnLine p) {
		if (p.y >= lines.Count) return null;
		var l = lines[p.y];
		l.glyphAt(p.x, out var g);
		return g;
	}
	#endregion

}

public class TextLineLayout {
	/// <summary>Text that is been laid out.</summary>
	public TextLine line { get; set; }
	public TextManipulator man { get; set; }
	/// <summary>Specifies number of columns in the line.</summary>
	public int columns { get; private set; }
	private RenderedGlyph[] _glyphs;
	public IReadOnlyList<RenderedGlyph> glyphs => _glyphs;
	/// <summary>Number of glyphs/characters in the line.</summary>
	public int Count { get; private set; }

	public TextLineLayout(TextLine l, TextManipulator man) {
		line = l;
		_glyphs = new RenderedGlyph[line.length];
		//produce();
	}

	public void Add(RenderedGlyph r) {
		_glyphs[Count++] = r;
		columns += r.template.columnWidth;
	}

	/// <summary>Outs glyph at given column and returns index of that glyph.
	/// This method considers the fact that a single character/glyph may occupy multiple columns (for example tabs).
	/// Returned value is an index of glyph at given column.</summary>
	/// <param name="column"></param>
	/// <returns></returns>
	public int glyphAt(int column, out RenderedGlyph? glyph) {
		var cc = 0; glyph = null;
		for (int i = 0; i < _glyphs.Length; i++) {
			var ch = _glyphs[i];
			cc += ch.template.columnWidth;
			if (column < cc) {
				glyph = ch; return i;
			}
		}
		if (endsWithNewLine)
			return _glyphs.Length - 1;
		return _glyphs.Length;
	}

	/// <summary>NOT fully implemented!
	/// Returns 1 if (<see cref="endsWithNewLine"/>) and 0 on last line.</summary>
	public int lineEndSize
		=> endsWithNewLine ? 1 : 0;

	public bool endsWithNewLine
		=> Count > 0 &&
			(_glyphs[^1].template.name.StartsWith(@"'\n'")
			|| _glyphs[^1].template.name.StartsWith(@"'\r'"));

	/// <summary>Returns column at given glyph position.</summary>
	public int columnAt(int ch) {
		var c = 0;
		for (int i = 0; i < _glyphs.Length; i++) {
			if (i == ch) break;
			c += glyphs[i].template.columnWidth;
		}return c;
	}

	public override string ToString() {
		return $@"LL[{columns}/{Count}] {line}";
	}

}
