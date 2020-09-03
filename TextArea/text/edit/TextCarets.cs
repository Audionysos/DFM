using com.audionysos.text.utils;
using System;

namespace com.audionysos.text.edit {

	/// <summary>Represents a master caret that can hold additional child carets.
	/// Any modification of master caret will be also relatively applied to it's children.</summary>
	public class TextCarets : TextCaret {

		public TextCarets(Text text) : base(text) {

		}
	}

	public class TextCaret {
		public event Action<TextCaret> CHANGED;
		private Text text { get; set; }

		private int _c;
		/// <summary>Absolute index of character within the text before which the caret is placed.</summary>
		public int ch {
			get => _c;
		}

		private Int2 _pos = new Int2();
		/// <summary>Characters-columts coordinates of current caret position.</summary>
		public Int2 pos {
			get => _pos;
			set => _pos.set(value);
		}

		public TextCaret(Text text) {
			this.text = text;
			_pos.CHANGED += onPositionChanged;
		}

		private void onPositionChanged(Int2 p, Int2 ch) {
			_c = text.getIndex(p);
		}

	}
}
