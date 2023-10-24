using audionysos.display;
using audionysos.graphics.extensions.shapes;
using audionysos.input;
using com.audionysos.text.render;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WpfDNet.SLtoWPF;
using P = audionysos.geom.Point2;

namespace WpfDNet;
internal class HitTestTests {
	private Sprite main;
	private readonly SixLaborsToWPFAdapter adapter;

	public HitTestTests(SixLaborsToWPFAdapter adapter) {
		main = new Sprite() { name = "Main" };
		adapter.displaySurface.Add(main);
		adapter.displaySurface.POINTER_MOVED += onPointerMoved;
		info();
		test();
		this.adapter = adapter;
	}

	//TODO:Investigate some weird horizontal lines on text
	private void onPointerMoved(DisplaySurface s, DisplayPointer dp) {
		//return;
		var p = dp.position;
		var hit = adapter.inputListener.hit;
		var t  = 
			$"Pos: ({p.x:####.0} : {p.y:####.0})\n" +
			$"Hits = {hit.Count}";
		if (hit.Count > 0) t += "\n";
		foreach (var o in hit) {
			t += o + "\n";
		}
		ta.text = t;
	}

	private TextAreaView ta;

	private void info() {
		ta = new TextAreaView();
		ta.view.name = "text";
		ta.view.transform.y = 5;
		ta.view.transform.sX = 1;
		ta.view.transform.sY = 1;
		main.addChild(ta.view);
		//ta.context.fromat.size = 40;
		ta.text = "Hello\nWorld";
		//ta.view.ENTER_FRAME += count;
	}

	private void test() {
		var ch = new Sprite();
		ch.name = "Child";
		var g = ch.graphics;
		g.beginFill(0x00AA00);
		g.drawRect(0, 0, 50, 50);
		ch.transform.x = 150;
		ch.transform.y = 150;
		main.addChild(ch);

		var ch2 = new Sprite();
		ch2.name = "Child 2";
		g = ch2.graphics;
		g.beginFill(0xAA0000);
		g.drawRect(-25,-25, 50, 50);
		ch2.transform.x = 50;
		ch2.transform.y = 50;
		ch.addChild(ch2);

		ch2.input.POINTER_ENTER += onPointEnter;
		ch2.input.POINTER_LEFT += onPointLeft;
		ch2.input.POINTER_DOWN += onPointerDown;
	}

	private void onPointerDown(DisplayObject o) {
		var s = o as Sprite;
		var g = s.graphics;
		g.clear();
		g.beginFill(0x0000FF);
		g.drawRect(-25, -25, 50, 50);
	}

	private void onPointEnter(DisplayObject o) {
		o.transform.sX = 1.3;
		o.transform.sY = 1.3;
	}

	private void onPointLeft(DisplayObject o) {
		o.transform.sX = 1;
		o.transform.sY = 1;
	}
}
