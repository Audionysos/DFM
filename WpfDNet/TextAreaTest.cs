using com.audionysos.text.render;
using System;
using System.Text;
using SixLabors.ImageSharp.Formats;
using WpfDNet.SLtoWPF;
using com.audionysos.text.edit;

namespace WpfDNet; 
public class TextAreaTest {
	private TextAreaView ta;
	private TextAreaView pos;

	public TextAreaTest(SixLaborsToWPFAdapter adapter) {
		ta = new TextAreaView();
		pos = new TextAreaView();
		pos.view.transform.y = adapter.displaySurface.size.y - 10;
		adapter.displaySurface.Add(pos.view);
		displayCaretPosition();

		ta.view.transform.x = ta.view.transform.y = 5;
		adapter.displaySurface.Add(ta.view);
		ta.text =
			"Chrząszcz brzmi w trzcinie w trzebżeszynie" +
			"\n\tŹaba daba doom";

		ta.manipulator.carets.CHANGED += onCaretsMoved;

	}

	private void onCaretsMoved(TextCaret caret) {
		displayCaretPosition();
	}

	private void displayCaretPosition() {
		var cp = ta.manipulator.carets.pos;
		pos.text = $"Ln: {cp.y}	Ch: {cp.x}";
	}

}
