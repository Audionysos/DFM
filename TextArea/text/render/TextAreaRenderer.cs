using audioysos.display;
using audioysos.geom;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;
using System.Collections.Generic;

namespace com.audionysos.text.render {

	public class TextAreaRenderer {
		TextDisplayContext ctx;

		public TextAreaRenderer(TextDisplayContext ctx) {
			this.ctx = ctx;
		}

		private List<RenderedGlyph> rendered = new List<RenderedGlyph>();

		private Graphics g;
		public void render() {
			renderCarrets();
			rendered.Clear();
			var man = ctx.manipulator;
			var grc = new GlyphRenderingContext();
			ctx.fromat.size = 11; 
			grc.gfx = ctx.gfx;
			var w = 7; //char width
			var linesSpacing = 4; var tabSize = 4;
			var lns = man.text.lines;
			for (int i = 0; i < lns.Count; i++) {
				var l = lns[i];
				for (int j = 0; j < l.length; j++) {
					var chi = man.getCharInfo(l.start + j);
					if (chi.character == '\n') continue;
					if (chi.character == '\t') {
						grc.position.x += w * tabSize;
						continue;
					}
					grc.position.x += w;
					rendered.Add(renderCharacter(chi, grc));
				}
				grc.position.x = 0;
				grc.position.y += ctx.fromat.size + linesSpacing;
			}

			ctx.gfx.close();
		}

		private RenderedGlyph renderCharacter(CharInfo chi, GlyphRenderingContext grc) {
			var r = ctx.glyphsRenderer;
			var fp = chi.spans[0].attributes.get<ITextFormatProvider>();
			var tf = fp?.textFormat ?? ctx.fromat;
			//Maybe it would be better to do per/span rendering. Need to think about that...
			//g.beginFill(tf.foreground);
			var gf = ctx.glyphs.get(chi.character, tf);
			grc.g = gf;
			grc.format = tf;
			return r.render(grc);
		}

		//private Sprite carrets = new Sprite();
		private Sprite carret = new Sprite();
		private void renderCarrets() {
			var cts = ctx.manipulator.carets;
			var gfx = carret.graphics;
			gfx.clear();
			gfx.lineSyle(1);
			gfx.lineTo(0, ctx.fromat.size);
			gfx.close();
			if (rendered.Count <= cts.ch) return;
			var cp = rendered[cts.ch];
			cts.pos = new Int2((int)cp.position.x, (int)cp.position.y);
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
		public Int4 bounds { get; private set; } = new Int4();
		public Int4[] chars { get; private set; }

		public TextLineLayout(TextLine l, TextManipulator man) {
			line = l;
			produce();
		}

		private void produce() {
			chars = new Int4[line.length];
			for (int i = line.start; i < line.end; i++) {
				var ch = man.getCharInfo(i);
				//ch.
			}

		}
	}

}
