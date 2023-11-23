using audionysos.display;
using audionysos.geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS = audionysos.gui.LayoutSettings;

namespace audionysos.gui;
public class LayoutArranger {

	public LayoutArranger() {

	}

	public void arrange() {
		LS pL = new LS();
		LS chL = new LS();
		var avs = pL.size.actual;

		var ds = chL.size.desired
			.max(chL.size.minimal)
			.min(chL.size.maximal);

		var (w, h) = (ds.width.value, ds.height.value);
		if (ds.width.isRelative)
			w = avs.width * ds.width;
		if (ds.height.isRelative)
			h = avs.height * ds.height;
		var s = chL.size.actual = (w, h);

		var (x, y) = (0d, 0d);
		Point2 p = chL.placement;
		var sl = (avs - s)*.5;
		if (chL.placement.horizontal.isInside) {
			x = sl.width * p.x + sl.width;
		}else {
			x = sl.width * 2 + (s.width * .5)
				* chL.placement.horizontal.localScale();
		}
		if (chL.placement.vertical.isInside) {
			y = sl.height * p.y + sl.height;
		} else {
			y = sl.height * 2 + (s.height * .5)
				* chL.placement.vertical.localScale();
		}
		chL.position = (x, y);
	}

}

public interface IContainer<T> where T : DisplayObject {
	IReadOnlyList<IDisplayable<T>> childs { get; }
}
