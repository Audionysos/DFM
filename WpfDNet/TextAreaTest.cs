using com.audionysos.text.render;
using System;
using System.Text;
using SixLabors.ImageSharp.Formats;
using WpfDNet.SLtoWPF;
using com.audionysos.text.edit;
using com.audionysos;

namespace WpfDNet; 
public class TextAreaTest {
	private TextAreaView ta;
	private TextAreaView pos;

	public TextAreaTest(SixLaborsToWPFAdapter adapter) {
		adapter.displaySurface.background = (Color)0xFFFFFFFF;
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
		//return;
		var c = ta.manipulator.carets;
		pos.text = $"Ln: {c.pos.y}\tCh: {c.lCh}\tCol: {c.actualPos.x}";
	}

}
