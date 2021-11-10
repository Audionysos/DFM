using audioysos.display;
using System;
using System.Collections.Generic;
using System.Text;
using WpfDNet.SLtoWPF;

namespace WpfDNet {
	public class DisplayTest {
		private Sprite main;

		public DisplayTest(SixLaborsToWPFAdapter adapter) {
			main = new Sprite();
			adapter.displaySurface.Add(main);
			test();
			adapter.transferBitmap();
		}

		private void test() {
			var e = new Sprite();
			main.addChild(e);
			var gfx = e.graphics;
			gfx.beginFill(0x00FF00, 1);
			var w = 250; var h = 250;
			gfx.lineTo(w, 0);
			gfx.lineTo(w, h);
			gfx.lineTo(0, h);
			gfx.lineTo(0, 0);
			gfx.close();

		}
	}
}
