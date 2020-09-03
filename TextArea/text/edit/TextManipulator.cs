using com.audionysos.text.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.audionysos.text.edit {

	public class APITest {

		public APITest() {
			var a = new TextManipulator();
			var r = a.regions.Add(0, 5);
			r.attributes.Add(new TextFormat());
		}
	}

	public class TextAreaView {
		Int2 size;
		Int2 renderer;

	}

	public class TextAreaRenderer {
		public TextManipulator area;

		public TextAreaRenderer(TextManipulator a) {
			area = a;
		}

		public void render() {
			for (int i = 0; i < area.text.Count; i++) {
				var chi = area.getCharInfo(i);
				renderCharacter(chi);
			}
		}

		private void renderCharacter(CharInfo chi) {
			var fp = chi.spans[0].attributes.get<ITextFormatProvider>();
			var tf = fp.textFormat;
			//Maybe it would be better do to per/span rendering. Need to think about that...
		}
	}

	#region Attributes
	public class TextFormat : ITextFormat, ITextFormatProvider {
		public ITextFormat textFormat { get; }
	}

	public interface ITextFormatProvider {
		ITextFormat textFormat { get; }
	}
	public interface ITextFormat { }

	#endregion


	public class TextManipulator {

		public Text text { get; private set; }
		public TextSelection selection { get; private set; }
		public TextCarets carets { get; private set; }
		public TextSpans regions { get; private set; }
		public IReadOnlyList<CharInfo> infos { get; private set; }

		public TextManipulator() {
			text = new Text();
			init();
		}


		public TextManipulator(string text) {
			this.text = text;
			init();
		}

		private void init() {
			selection = new TextSelection(text);
			carets = new TextCarets(text);
		}

		public CharInfo getCharInfo(int index) {
			return null;
		}
	}

	public class CharInfo {
		public List<TextSpan> spans { get; private set; } = new List<TextSpan>();

		public CharInfo() {

		}
	}

	public class Attributes {
		public T get<T>() { return default; }

		internal void Add<T>(T a) {
			throw new NotImplementedException();
		}
	}

}
