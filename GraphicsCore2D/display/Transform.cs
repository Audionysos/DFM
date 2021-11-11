//skatching

using audioysos.geom;
using System;

namespace audioysos.display {
	public class Transform {
		public double x { get; set; }
		public double y { get; set; }
		public double z { get; set; }
		public double sX { get; set; } = 1;
		public double sY { get; set; } = 1;

		public Transform append(Transform t) {
			sX *= t.sX; sY *= t.sY;
			x = x * t.sX + t.x;
			y = y * t.sY + t.y;
			//z = t.z;
			return this;
		}

		public Transform copy() {
			return new Transform() {
				x = x,
				y = y,
				z = z,
			};
		}

		public Transform setTo(Transform transform) {
			var t = transform;
			this.x = t.x;
			this.y = t.y;
			this.z = t.z;
			sX = t.sX;
			sY = t.sY;
			return this;
		}

		public void transform(IPoint2 p) {
			p.x *= sX; p.y *= sY;
			p.x += x; p.y += y;
		}
	}

}
