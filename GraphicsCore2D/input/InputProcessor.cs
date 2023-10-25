using audionysos.display;
using audionysos.geom;
using audionysos.graphics.extensions.shapes;
using System;
using System.Collections.Generic;

namespace audionysos.input; 
public abstract class InputProcessor {
	public FocusManager focus { get; private set; } = new FocusManager();

	public abstract void registerInputListener(InputListener il);
	public abstract IPoint2 getSurfacePosition(DisplayPointer p, DisplaySurface s);

}

public class InputListener {
	private List<DisplaySurface> _surfs = new List<DisplaySurface>();

	//GC2D internal
	/// <summary>Register surface for input.</summary>
	/// <param name="surface"></param>
	public void registerSurface(DisplaySurface surface) {
		_surfs.Add(surface);
	}

	#region Temp/debug
	private IReadOnlyList<DisplayObject> noHit = new DisplayObject[0];
	public IReadOnlyList<DisplayObject> hit = new DisplayObject[0];
	private IReadOnlyList<DisplayObject> prevHit = new DisplayObject[0];
	#endregion

	public void pointerMove(InputProcessor ip, DisplayPointer dp) {
		hit = noHit;
		for (int i = 0; i < _surfs.Count; i++) {
			var s = _surfs[i];
			var sp = ip.getSurfacePosition(dp, s);
			dp.position.add(sp);
			var ht = hit = s.hitTest(dp.position);

			if (ht != null) {
				var f = ht[0] as Sprite;
				var p = prevHit.Count > 0 ? prevHit[0] : null;
				if (p != f) {
					p?.dispatcher.firePointerLeft();
					prevHit = new List<DisplayObject>(hit);
				}
				f.dispatcher.firePointerEnter();
			} else {
				if (prevHit.Count > 0) {
					prevHit[0].dispatcher.firePointerLeft();
					prevHit = noHit;
				}
				hit = noHit;
			}
			s.pointerMove(dp);
		}
	}

	public void pointerDown(InputProcessor ip, DisplayPointer dp) {
		var h = hit.Count > 0 ? hit[0] : null;
		h?.dispatcher.firePointerDown();
	}

	public void pointerUp(InputProcessor ip, DisplayPointer dp) {
		var h = hit.Count > 0 ? hit[0] : null;
		h?.dispatcher.firePointerUp();
	}

	public void keyDown(InputProcessor ip, Keyboard.Key k) {
		var f = ip.focus.current as DisplayObject;
		f?.dispatcher.fireKeyDown(new KeyboardEvent() {
			target = f, key = k,
		});
	}

	public void keyUp(InputProcessor ip, Keyboard.Key k) {
		var f = ip.focus.current as DisplayObject;
		f?.dispatcher.fireKeyUp(new KeyboardEvent() {
			target = f, key = k,
		});
	}
}

public class DisplayPointer {
	public int id { get; init; }
	public DisplayPointerType type { get; init; }
	public IPoint2 position { get; set; }

}

public enum DisplayPointerType {
	UNKNOWN,
	MOUSE,
	FINGER,
	PEN,
	OTHER

}
