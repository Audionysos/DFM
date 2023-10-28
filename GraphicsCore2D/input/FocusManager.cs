using audionysos.display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace audionysos.input;
public class FocusManager {
	private List<DisplaySurface> surfs = new();

	public object current { get; set; }

	public void tack(DisplaySurface s) {
		surfs.Add(s);
		s.OBJECT_ADD += onNewObject;
	}

	private void onNewObject(DisplaySurface surface, DisplayObject o) {
		if (o is not InteractiveObject i || !i.isFocusable)
			return;
		current = o;
		if (o is DisplayObjectContainer c)
			c.CHILD_ADD += onNewChild;
	}

	private void onNewChild(DisplayObjectContainer container, DisplayObject o) {
		if (o is not InteractiveObject i || !i.isFocusable)
			return;
		current = o;
		if (o is DisplayObjectContainer c)
			c.CHILD_ADD += onNewChild;
	}
}
