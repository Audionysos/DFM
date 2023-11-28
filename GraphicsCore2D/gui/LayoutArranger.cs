using audionysos.display;
using audionysos.geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS = audionysos.gui.LayoutSettings;
using static audionysos.utils.Fantastatics;
using audionysos.graphics.extensions.shapes;
using com.audionysos;

namespace audionysos.gui;
public class LayoutArranger {

	public LayoutArranger() {

	}

	public void arrange(UIElementContainer? parent, UIElement child) {
		//LS pL = parent.layout;
		LS chL = child.layout;
		var avs = parent?.layout.size.actual
			?? child.layout.size.design;

		var ds = chL.size.desired
			.max(chL.size.minimal)
			.min(chL.size.maximal);

		var (w, h) = (ds.width.value, ds.height.value);
		if (ds.width.isRelative)
			w = avs.width * ds.width;
		if (ds.height.isRelative)
			h = avs.height * ds.height;
		var s = chL.size.actual = (w, h);

		var sl = (avs - s)*.5; //size left

		chL.position = each(t => {
				var (p, sl, s, av) = t; //placement, size left(parent), size
				if (p.isInside) return sl * p.value + sl;
				else return av * .5 + av * p.localScale - s * .5;
			}
			, (chL.placement.horizontal, sl.width, s.width, avs.width)
			, (chL.placement.vertical, sl.height, s.height, avs.height)
		);

		//Point2 p = chL.placement;
		//var (x, y) = (0d, 0d);
		//if (chL.placement.horizontal.isInside) {
		//	x = sl.width * p.x + sl.width;
		//}else {
		//	x = sl.width * 2 + (s.width * .5)
		//		* chL.placement.horizontal.localScale();
		//}
		//if (chL.placement.vertical.isInside) {
		//	y = sl.height * p.y + sl.height;
		//} else {
		//	y = sl.height * 2 + (s.height * .5)
		//		* chL.placement.vertical.localScale();
		//}
		//chL.position = (x, y);


	}

}

public class DefaultPainter : GUIPainter {
	public override void paint(UIElement e, DisplayObject view) {
		var s = view as Sprite;
		if (s == null) return;
		var g = s.graphics;
		var vs = e.style;
		var c = vs.background as Color;
		if (c == null)
			c = new Color(0x000000FF);
		//c = vs.colorPalette[];
		g.clear();
		g.beginFill(c);
		var acs = e.layout.size.actual;
		g.drawRect(0,0, acs.width, acs.height);
		g.close();
		s.transform.pos = e.layout.position;
	}
}

public abstract class GUIPainter {
	public abstract void paint(UIElement e, DisplayObject view);
}

