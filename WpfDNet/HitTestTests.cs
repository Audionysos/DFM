using audionysos.display;
using audionysos.graphics.extensions.shapes;
using audionysos.input;
using com.audionysos.text.render;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WpfDNet.SLtoWPF;

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
		ta.text = 
			$"Pos: ({p.x:####.0} : {p.y:####.0})\n" +
			$"Hits = {adapter.inputListener.hit.Count}";
	}

	private TextAreaView ta;

	private void info() {
		ta = new TextAreaView();
		ta.view.name = "text";
		ta.view.transform.y = 5;
		ta.view.transform.sX = 2;
		ta.view.transform.sY = 2;
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
	}
}
