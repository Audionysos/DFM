using audionysos.display;
using audionysos.geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.audionysos.text.render;
public class CaretView {
	public Sprite view { get; private set; }
	private Point2 _size;
	public Point2 size {
		get => _size.copy();
		set {
			if(_size.equal(value)) return;
			_size = value.copy();
			draw();
		}
	}

	public CaretView() {
		view = new Sprite();
		size = (1, 12);
		view.ENTER_FRAME += onFrame;
	}

	public void postion(IPoint2 p) {
		view.transform.x = p.x;
		view.transform.y = p.y;
		view.isVisible = true;
		lastCheck = DateTime.Now;
	}

	private DateTime lastCheck = DateTime.Now;
	private void onFrame(DisplayObject v) {
		var e = DateTime.Now - lastCheck;
		if(e.TotalSeconds > .5) {
			v.isVisible = !v.isVisible; 
			lastCheck = DateTime.Now;
		}
	}

	private void draw() {
		var g = view.graphics;
		g.lineStyle(size.x);
		g.lineTo(0, size.y);
		g.close();
	}
}
