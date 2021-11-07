using audioysos.geom;
using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;

namespace com.audionysos.text.render {

	public class TextAreaRenderer {
		TextDisplayContext ctx;

		public TextAreaRenderer(TextDisplayContext ctx) {
			this.ctx = ctx;
		}

		private Graphics g;
		public void render() {
			var man = ctx.manipulator;
			var grc = new GlyphRenderingContext();
			ctx.fromat.size = 11; //TODO: 
			grc.gfx = ctx.gfx;
			var s = 7;
			//for (int i = 0; i < man.text.Count; i++) {
			//	var chi = man.getCharInfo(i);
			//	//if (i != 2) continue;
			//	grc.position.add(new Point2(s, 0));
			//	renderCharacter(chi, grc);
			//	//if(i == 2) break;
			//}
			var lns = man.text.lines;
			for (int i = 0; i < lns.Count; i++) {
				var l = lns[i];
				for (int j = 0; j < l.length; j++) {
					var chi = man.getCharInfo(l.start + j);
					grc.position.add(new Point2(s, 0));
					renderCharacter(chi, grc);
				}
				grc.position.x = 0;
				grc.position.y += ctx.fromat.size;
			}

			ctx.gfx.close();
		}

		private void renderCharacter(CharInfo chi, GlyphRenderingContext grc) {
			var r = ctx.glyphsRenderer;
			var fp = chi.spans[0].attributes.get<ITextFormatProvider>();
			var tf = fp?.textFormat ?? ctx.fromat;
			//Maybe it would be better to do per/span rendering. Need to think about that...
			//g.beginFill(tf.foreground);
			var gf = ctx.glyphs.get(chi.character, tf);
			grc.g = gf;
			grc.format = tf;
			r.render(grc);
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
