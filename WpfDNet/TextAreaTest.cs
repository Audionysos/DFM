using com.audionysos.text.render;
using System;
using System.Text;
using SixLabors.ImageSharp.Formats;
using WpfDNet.SLtoWPF;

namespace WpfDNet {
	public class TextAreaTest {

		public TextAreaTest(SixLaborsToWPFAdapter adapter) {
			var ta = new TextAreaView();
			adapter.displaySurface.Add(ta.view);
			ta.text =
				"Chrząszcz brzmi w trzcinie w trzebżeszynie" +
				"\n\tŹaba daba doom";
			//adapter.transferBitmap();
		}

	}

}
