using com.audionysos.text.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.audionysos.text.edit {

	/// <summary>Provides methods for manipulating <see cref="Text"/> and associated data.</summary>
	public class TextManipulator {
		/// <summary>Text that is been manipulated.</summary>
		public Text text { get; private set; }
		/// <summary>Sores information about currently selected text.</summary>
		public TextSelection selection { get; private set; }
		/// <summary>Contains information about curret caret(s) placement.</summary>
		public TextCarets carets { get; private set; }
		/// <summary>Stores infomration about all distingushable text regions.</summary>
		public TextSpans regions { get; private set; }
		/// <summary>Given access to information associated with each character of <see cref="text"/>.</summary>
		public IReadOnlyList<CharInfo> infos { get; private set; }

		#region Initialization

		#region Constructors
		public TextManipulator() {
			text = new Text();
			init();
		}

		public TextManipulator(string text) {
			this.text = text;
			init();
		}
		#endregion

		private void init() {
			selection = new TextSelection(text);
			carets = new TextCarets(text);
		}
		#endregion

		/// <summary>Returns information for <see cref="text"/>'s character at given index.</summary>
		public CharInfo getCharInfo(int index) {
			return null;
		}
	}

	public class CharInfo {
		/// <summary>Stores information about all spans of text the character is associated with.</summary>
		public List<TextSpan> spans { get; private set; } = new List<TextSpan>();
		/// <summary>Specifies a format in which the character is displayed.</summary>
		public ITextFormat format;
		public object rect;
		public char character;

		public CharInfo() {

		}
	}



	#region Attributes
	/// <summary>Sores dynamic composition of attributes.</summary>
	public class Attributes {
		public T get<T>() { return default; }

		internal void Add<T>(T a) {
			throw new NotImplementedException();
		}
	}

	public class TextFormat : ITextFormat, ITextFormatProvider {
		/// <summary>Returns this.</summary>
		public ITextFormat textFormat => this;
		/// <inheritdoc/>
		public double size { get; set; }
		public IFill foreground { get; set; }
		public IFill bacground { get; set; }
		public ITextFont font { get; set; }
	}

	/// <summary>Interface for objects providing <see cref="ITextFormat"/>.</summary>
	public interface ITextFormatProvider {
		ITextFormat textFormat { get; }
	}

	/// <summary>Specifies format that could be applied specifically to text, texs spans, or a single characters.</summary>
	public interface ITextFormat {
		ITextFont font { get; set; }
		/// <summary>Font size.</summary>
		double size { get; set; }
		/// <summary>A fill that is applied to text.</summary>
		IFill foreground { get; set; }
		/// <summary>Fill that is applied to what is behind the text.</summary>
		IFill bacground { get; set; }
	}

	public interface ITextFont {
		public string name { get; }
	} 


	#endregion

}
