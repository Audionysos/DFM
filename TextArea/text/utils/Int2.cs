using System;

namespace com.audionysos.text.utils {

	public class Int2 {
		/// <summary>Dispatched when one of object's properties were changed.
		/// First handler argument is this instance, second argument is difference from previous state.</summary>
		public event Action<Int2, Int2> CHANGED;

		private int _x;
		public int x {
			get => _x;
			set {
				var d = value - _x;
				if (d == 0) return;
				_x = value;
				CHANGED?.Invoke(this, (d, 0));
			}
		}

		private int _y;
		public int y {
			get => _y;
			set {
				var d = value - _y;
				if (d == 0) return;
				_y = value;
				CHANGED?.Invoke(this, (0, d));
			}
		}

		public int lenght => (int)Math.Sqrt(x * x + y * y);

		public Int2(int x = 0, int y = 0) {
			_x = x;
			_y = y;
		}

		public Int2 set(int x, int y) {
			var d = this - (x, y);
			if (d.lenght == 0) return this;
			_x = x; _y = y;
			CHANGED?.Invoke(this, d);
			return this;
		}

		internal Int2 set(Int2 o) {
			if (!o) return this;
			return set(o.x, o.y);
		}

		public static Int2 operator -(Int2 a, Int2 b)
			=> new Int2(a.x - b.x, a.y - b.y);


		public static implicit operator Int2((int x, int y) t)
			=> new Int2(t.x, t.y);

		/// <summary>False if null.</summary>
		public static implicit operator bool(Int2 i) => i != null;
	}
}
