using audionysos.display;
using audionysos.geom;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;
using System.Collections.Generic;

namespace com.audionysos.text.render; 

public class TextAreaRenderer {
	TextDisplayContext ctx;

	public TextAreaRenderer(TextDisplayContext ctx) {
		this.ctx = ctx;
		ctx.view.view.addChild(caret.view);
	}

	private List<RenderedGlyph> rendered = new List<RenderedGlyph>();

	private Graphics g;
	public void render2() {
		rendered.Clear();
		var man = ctx.manipulator;
		var grc = new GlyphRenderingContext();
		ctx.format.size = 12; 
		grc.gfx = ctx.gfx;
		grc.gfx.clear();
		var dw = 10d; //default width
		var w = 0d; //char width
		var linesSpacing = 4; var tabSize = 4;
		var lns = man.text.lines;
		for (int i = 0; i < lns.Count; i++) {
			var l = lns[i];
			for (int j = 0; j < l.length; j++) {
				var chi = man.getCharInfo(l.start + j);
				//if (chi.character == '\n') continue;
				//if (chi.character == '\t') {
				//	grc.position.x += dw * tabSize;
				//	continue;
				//}
				var rg = renderCharacter(chi, grc);
				w = (rg == null) ? dw : rg.size.x;
				if(rg != null) rendered.Add(rg);
				grc.position.x += w;
			}
			grc.position.x = 0;
			grc.position.y += ctx.format.size + linesSpacing;
		}

		ctx.gfx.close();
		renderCarets();
	}

	private List<TextLineLayout> _lines = new();
	public IReadOnlyList<TextLineLayout> lines => _lines;

	public void render() {
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
		var linesSpacing = 4;

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
				grc.position.x += rg.size.x;
				ctx.container.addChild(v);
			}

			grc.position.x = 0;
			grc.position.y += ctx.format.size + linesSpacing;
		}
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
			rg = rendered[^1];
			pos = rg.position + (rg.size.x, 0);
		}else {
			rg = rendered[cts.ch];
			pos = rg.position;
		}
		//cts.pos = new Int2((int)cp.position.x, (int)cp.position.y);
		caret.postion(pos);
	}
}

public class TextLayouter {

	public void layout(TextManipulator m) {
		var chs = m.infos;
		foreach (var ch in chs) {
			//ch.
		}

	}

}

public class TextLineLayout {
	/// <summary>Text this is been layed out.</summary>
	public TextLine line { get; set; }
	public TextManipulator man { get; set; }
	public int columns { get; private set; }
	private RenderedGlyph[] _glyphs;
	public IReadOnlyList<RenderedGlyph> glyphs => _glyphs;
	private int Count;

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
		}return _glyphs.Length;
	}

	/// <summary>Returns column at given glyph position.</summary>
	public int columnAt(int ch) {
		var c = 0;
		for (int i = 0; i < _glyphs.Length; i++) {
			if (i == ch) break;
			c += glyphs[i].template.columnWidth;
		}return c;
	}

	//private void produce() {
	//	//chars = new RenderedGlyph[line.length];
	//	for (int i = line.start; i < line.end; i++) {
	//		var ch = man.getCharInfo(i);
	//		//ch.
	//	}

	//}
}
