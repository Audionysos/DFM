using System;
using System.Threading.Channels;

namespace com.audionysos.text.utils; 

/// <summary>Represents bounds in quantized 2d grid.</summary>
public class Int4 {
	/// <summary>Initial position of the in the grid.</summary>
	public Int2 pos = new Int2();
	/// <summary>Dimensions of the (width-height).</summary>
	public Int2 size = new Int2();

	public Int4() { }
}

/// <summary>Represents trackable position within some quantized 2d space.</summary>
public class Int2 {
	/// <summary>Dispatched when one of object's properties were changed.
	/// First handler argument is this instance, second argument is difference from previous state.</summary>
	public event Action<Int2, Int2>? CHANGED;

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

	public int length => (int)Math.Sqrt(x * x + y * y);

	public Int2(int x = 0, int y = 0) {
		_x = x;
		_y = y;
	}

	public Int2 setTo(int x, int y) {
		var d = this - (x, y);
		if (d.length == 0) return this;
		_x = x; _y = y;
		CHANGED?.Invoke(this, d);
		return this;
	}


	public Int2 set(Int2 o) {
		if (!o) return this;
		return this.set(o.x, o.y);
	}

	public Int2 copy() => new Int2(x, y);

	public static Int2 operator -(Int2 a, Int2 b)
		=> new Int2(a.x - b.x, a.y - b.y);


	public static implicit operator Int2((int x, int y) t)
		=> new Int2(t.x, t.y);

	public override string ToString() {
		return $"({x}, {y})";
	}

	/// <summary>False if null.</summary>
	public static implicit operator bool(Int2 i) => i != null;
}

public static class Int2Extensions {

	/// <summary>Changes coordinates and returns this object.</summary>
	public static T set<T>(this T p, int x, int y) where T : Int2
		=> (T)p.setTo(x, y);

	/// <summary>Increments coordinates and returns this object.</summary>
	public static T add<T>(this T p, int x, int y) where T : Int2
		=> (T)p.set(p.x + x, p.y + y);

	/// <summary>Increments coordinates and returns this object.</summary>
	public static T add<T>(this T p, (int x, int y) t) where T : Int2
		=> (T)p.set(p.x + t.x, p.y + t.y);
}
