using System.Collections.Generic;

namespace com.audionysos.text.edit {
	/// <summary>Represents selection in a <see cref="TextManipulator"/>.
	/// The selection can be composed of any number of <see cref="TextSpan"/>'s while it is a <see cref="TextSpan"/> itselef which idicates boundres of all selected spans (there may be fragments whitn that range that are not selected).</summary>
	public class TextSelection : TextSpan {
		private List<TextSpan> spans = new List<TextSpan>();


		public TextSelection(Text text) : base(text, 0, 0) {}

		/// <summary>Sets new selection clearning any provious ones.</summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public TextSpan set(int start = 0, int end = int.MaxValue) {
			spans.Clear();
			var ns = new TextSpan(source, start, end);
			spans.Add(ns);
			this.start = ns.start;
			this.end = ns.end;
			return ns;
		}

	}
}
