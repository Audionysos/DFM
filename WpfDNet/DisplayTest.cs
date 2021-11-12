using audioysos.display;
using com.audionysos.text.render;
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
			test3();
			//test2();
			//adapter.transferBitmap();
			counter();
		}

		private void test3() {
			var a = rectSprite("A", 250, 250, 0x00FF00, 0.3);
			main.addChild(a);
			a.transform.x = 125;
			a.transform.y = 125;
			a.transform.sX = 0.5;
			a.transform.sY = 0.5;

			var b = rectSprite("B",125, 125, 0xFF0000, 0.3);
			b.transform.x = 125;
			a.addChild(b);
			
			var c = rectSprite("C", 62.5, 62.5, 0x0000FF, 0.3);
			c.transform.x = 187.5;
			c.transform.y = 62.5;
			a.addChild(c);

			var d = rectSprite("D", 62.5, 62.5, 0xFFFFFF, 0.3);
			d.transform.x = 62.5;
			b.addChild(d);
		}

		private Sprite rectSprite(string name, double w, double h, uint color = 0xFFFFFF, double a = 1) {
			var s = new Sprite();
			s.name = name;
			var g = s.graphics;
			g.beginFill(color, a);
			g.lineTo(w, 0);
			g.lineTo(w, h);
			g.lineTo(0, h);
			g.lineTo(0, 0);
			return s;
		}

		private TextAreaView ta;
		private void counter() {
			ta = new TextAreaView();
			main.addChild(ta.view);
			ta.view.ENTER_FRAME += count;
		}

		private DateTime lastCheck = DateTime.Now;
		private int frames = 0;
		private void count(DisplayObject obj) {
			var d = DateTime.Now - lastCheck;
			frames++;
			if(d.Seconds > 1) {
				var fps = frames / d.TotalSeconds;
				ta.text = $@"FPS: {fps:###.#}";
				frames = 0; lastCheck = DateTime.Now;
			}
		}

		private void test() {
			var e = new Sprite();
			e.name = "child";
			main.addChild(e);
			var gfx = e.graphics;
			gfx.beginFill(0x00FF00, 1);
			var w = 250; var h = 250;
			gfx.lineTo(w, 0);
			gfx.lineTo(w, h);
			gfx.lineTo(0, h);
			gfx.lineTo(0, 0);
			e.ENTER_FRAME += onFrame;
			//gfx.close();
		}
		private Dictionary<object, (double x, double y)> vels = new Dictionary<object, (double x, double y)>();
		private void test2() {
			var r = new Random();
			vels.Clear();
			for (int i = 0; i < 950; i++) {
				var e = new Sprite();
				e.name = "child";
				main.addChild(e);
				var gfx = e.graphics;
				gfx.beginFill(0x00FF00, 0.2);
				var s = r.Next(5, 15);
				var w = s; var h = s;
				gfx.lineTo(w, 0);
				gfx.lineTo(w, h);
				gfx.lineTo(0, h);
				gfx.lineTo(0, 0);
				e.ENTER_FRAME += onFrame;
				vels.Add(e,
					(r.NextDouble() * 20 - 10
					, r.NextDouble() * 20 - 10));
			}
		}


		//private double vel = 1;
		private void onFrame(DisplayObject obj) {
			//return;
			var v = vels[obj];
			var t = obj.transform;
			t.x += v.x; t.y += v.y;
			if (t.y < 0) t.y = 0; //SixLabors complain about this

			if (v.x > 0 && t.x > 450
				|| v.x < 0 && t.x < 0) v.x *= -1;

			if (v.y > 0 && t.y > 450
				|| v.y < 0 && t.y <= 0) v.y *= -1;

			vels[obj] = v;
		}
	}
}
