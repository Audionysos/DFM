using audioysos.display;
using audioysos.geom;
using System.Collections.Generic;

namespace audioysos.input {
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

		public void pointerMove(InputProcessor ip, DisplayPointer dp) {
			for (int i = 0; i < _surfs.Count; i++) {
				var s = _surfs[i];
				var sp = ip.getSurfacePosition(dp, s);
			}
		}

	}

	public class DisplayPointer {
		public int id { get; }
		public DisplayPoiterType type { get; }
		public IPoint2 position;

	}

	public class DisplayPoiterType {

	}

}
