using audionysos.display;
using audionysos.geom;
using audionysos.graphics.extensions.shapes;
using System.Collections.Generic;

namespace audionysos.input; 
public abstract class InputProcessor {

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
				//f.graphics.beginFill(0xFF0000);
				//f.graphics.drawRect(0, 0, 5, 5);
			} else hit = noHit;
			s.pointerMove(dp);
		}
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
