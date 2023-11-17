using System;
using System.Collections.Generic;

namespace com.audionysos.text.edit; 
/// <summary>Represents selection in a <see cref="TextManipulator"/>.
/// The selection can be composed of any number of <see cref="TextSpan"/>'s while it is a <see cref="TextSpan"/> itself which indicates boundaries of all selected spans (there may be fragments within that range that are not selected).</summary>
public class TextSelection : TextSpan {
	private List<TextSpan> spans = new List<TextSpan>();


	public TextSelection(Text text) : base(text, 0, 0) {
		mutating = MutatingBehavior.STATIC;
	}

	/// <summary>Sets new selection clearing any previous ones.</summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <returns></returns>
	public TextSpan set(int start = 0, int end = int.MaxValue) {
		spans.Clear();
		var ns = new TextSpan(source, start, end);
		spans.Add(ns);
		setTo(ns.start, ns.end);
		return ns;
	}

	public void clear() {
		set(0, 0);
	}
}
