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
		public static GlyphsProvider defaulGlyphsProvider = new DefaultGlyphsProvider();

		public TextManipulator manipulator;
		public GlyphsProvider glyphs = defaulGlyphsProvider;
		public TextAreaRenderer renderer;
		public TextAreaView view;
		public IGraphics2D gfx;
	}

	public class TextAreaView {
		Int2 size;
		public TextDisplayContext context;
		
		/// <summary>Object which renders glyps on this view.</summary>
		public TextAreaRenderer renderer {
			get => context.renderer;
			private set => context.renderer = value;
		}
		public TextManipulator manipulator {
			get => context.manipulator;
			private set => context.manipulator = value;
		}

		/// <summary>Gets or set displayed text as raw string.</summary>
		public string text {
			get {
				return manipulator.text.ToString();
			}
			set {
				manipulator = new TextManipulator(value);
				renderer.render();
			}
		}

		public TextAreaView() {
			var x = context = new TextDisplayContext() {};
			x.view = this;
			manipulator = new TextManipulator();
			renderer = new TextAreaRenderer(context);
			x.gfx
		}

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
