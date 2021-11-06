using com.audionysos.text.edit;
using com.audionysos.text.utils;
using System;

namespace com.audionysos.text.render {

	public class APITest {

		public APITest() {
			var ta = new TextAreaView();
			//ta.renderer.


			var a = new TextManipulator();
			var r = a.regions.Add(0, 5);
			r.attributes.Add(new TextFormat() { 
				size = 18,
				foreground = (Color)0xFF0000,
			});
			//a.infos.Count
		}
	}

	/// <summary>Associates all objects need for text displaying/editing controls.</summary>
	public class TextDisplayContext {
		public TextManipulator manipulator;
		public GlyphsProvider glyphs;
		public TextAreaView view;
		public IGraphics2D gfx;
	}

	public class TextAreaView {
		Int2 size;
		/// <summary>Object which renders glyps on this view.</summary>
		public TextAreaRenderer renderer { get; }
		public TextManipulator manipulator;
		public TextDisplayContext context;

	}

	public class TextAreaRenderer {
		TextDisplayContext ctx;
		TextManipulator man;

		public TextAreaRenderer(TextDisplayContext ctx) {
			this.ctx = ctx;
			man = ctx.manipulator;
		}

		private Graphics g;
		public void render() {
			for (int i = 0; i < man.text.Count; i++) {
				var chi = man.getCharInfo(i);
				renderCharacter(chi);
			}

			//var g = new Graphics(null);
			////g.beginFill();
		}

		private void renderCharacter(CharInfo chi) {
			var fp = chi.spans[0].attributes.get<ITextFormatProvider>();
			var tf = fp.textFormat;
			//Maybe it would be better to do per/span rendering. Need to think about that...
			g.beginFill(tf.foreground);
			var gf = ctx.glyphs.get(chi.character, tf);
			//gf.
		}


	}

	public class DefaultGlyphsProvider : GlyphsProvider {
		/// <summary>Glyph used when </summary>
		public Glyph missingGlyph { get; set; }

		public override Glyph get(char c, ITextFormat f) {
			var r = getCached(c, f);
			if (r) return r;
			r = produceGlpyh(c, f);
			return missingGlyph;
		}

		/// <summary>Produces glpyh for given configuration.
		/// Result may be sotred in chache.
		/// Returns null if glyph cannot be produced for the configuration.</summary>
		/// <param name="c"></param>
		/// <param name="f"></param>
		private Glyph produceGlpyh(char c, ITextFormat f) {

			return null;
		}

		/// <summary>Tries to get a glyph from cache.
		/// Returns null if not glyhp for given configuration was produced yet.</summary>
		/// <param name="c"></param>
		/// <param name="f"></param>
		private Glyph getCached(char c, ITextFormat f) {
			return null;
		}
	}

	/// <summary>Provides <see cref="Glyph"/>s for given characters.</summary>
	public abstract class GlyphsProvider {

		public abstract Glyph get(char c, ITextFormat f);

	}

	/// <summary>Represents a single glyph that could be rendered in text area.</summary>
	public class Glyph {
		public object pixelsCache { get; }
		public double width { get; }
		public double height { get; }

		/// <summary>False if null.</summary>
		/// <param name="g"></param>
		public static implicit operator bool(Glyph g) => g!=null;
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
