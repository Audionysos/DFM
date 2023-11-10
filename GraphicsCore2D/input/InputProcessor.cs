using audionysos.display;
using audionysos.geom;
using audionysos.graphics.extensions.shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace audionysos.input;
public abstract class InputProcessor {
	public FocusManager focus { get; private set; } = new FocusManager();

	public InputProcessor() {
		PinBoard.ip ??= this;
	}

	public abstract void registerInputListener(InputListener il);
	public abstract IPoint2 getSurfacePosition(DisplayPointer p, DisplaySurface s);

	public abstract bool isCurrentClipboard(object? o);
	public abstract object? getClipboard(Type t);
	public abstract void setClipboard(object? o);

	/// <summary>False if null.</summary>
	public static implicit operator bool([NotNullWhen(true)]InputProcessor? p) => p != null;

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

	private InteractiveObject? last => (hit.Count > 0 ? hit[0] : null) as InteractiveObject;

	public void pointerMove(InputProcessor ip, DisplayPointer dp) {
		hit = noHit;
		for (int i = 0; i < _surfs.Count; i++) {
			var s = _surfs[i];
			var sp = ip.getSurfacePosition(dp, s);
			dp.position.add(sp);
			var ht = hit = s.hitTest(dp.position);

			if (ht != null) {
				var f = ht[0] as Sprite;
				var p = (prevHit.Count > 0 ? prevHit[0] : null) as InteractiveObject;
				if (p != f) {
					p?.dispatcher.firePointerLeft();
					prevHit = new List<DisplayObject>(hit);
				}
				f.dispatcher.firePointerEnter();
			} else {
				if (prevHit.Count > 0 && prevHit[0] is InteractiveObject io) {
					io.dispatcher.firePointerLeft();
					prevHit = noHit;
				}
				hit = noHit;
			}
			s.pointerMove(dp);
		}
	}

	public void pointerDown(InputProcessor ip, DisplayPointer dp) {
		last?.dispatcher.firePointerDown();
	}

	public void pointerUp(InputProcessor ip, DisplayPointer dp) {
		last?.dispatcher.firePointerUp();
	}

	public void keyDown(InputProcessor ip, Keyboard.Key k, char ch = '\0') {
		Keyboard.press(k);
		var f = ip.focus.current as InteractiveObject;
		f?.dispatcher.fireKeyDown(new KeyboardEvent() {
			target = f, key = k, character = ch
		});
	}

	public void keyUp(InputProcessor ip, Keyboard.Key k, char ch = '\0') {
		Keyboard.release(k);
		var f = ip.focus.current as InteractiveObject;
		f?.dispatcher.fireKeyUp(new KeyboardEvent() {
			target = f, key = k, character = ch
		});
	}
}

public class DisplayPointer {
	public int id { get; init; }
	public DisplayPointerType type { get; init; }
	private Point2 p = new Point2();
	public IPoint2 position {
		get => p.copy();
		set => p.set(value);
	}

}

public enum DisplayPointerType {
	UNKNOWN,
	MOUSE,
	FINGER,
	PEN,
	OTHER

}

public class PinBoard {
	private static PinBoard g = new PinBoard();
	public static InputProcessor? ip { get; set; }


	public static T? get<T>() {
		var c = g._get<T>();
		if (ip) {
			if (ip.isCurrentClipboard(c))
				return c;
			else {
				g.store(ip.getClipboard(typeof(T)));
				c = g._get<T>();
			}
		}
		return c; 
	}

	public static void set<T>(T o) {
		ip?.setClipboard(o);
		g.store(o);
	}


	private ImmutableArray<byte> current = ImmutableArray<byte>.Empty;
	private Type? type;
	private Converters converters = new Converters();
	private StringToBytes fromString = new StringToBytes();

	public PinBoard() {
		converters.Add(fromString);
		converters.Add(new BytesToString());
	}

	public void store(object o) {
		o = o ?? throw new ArgumentNullException();
		var c = converters.get(o.GetType(), typeof(ImmutableArray<byte>));
		if (c != null) {
			current = (ImmutableArray<byte>)c.convert(o);
			type = o.GetType();
		} else {
			current = fromString.convert(o.ToString() ?? "null");
			type = typeof(string);
		}
	}

	public T? _get<T>() {
		var r = get(typeof(T));
		if (r == null) return default;
		return (T)r;
	}

	public object? get(Type t) {
		if (current.IsDefaultOrEmpty || type == null) return null;
		var c = converters.get(typeof(ImmutableArray<byte>), type);
		if (c == null) return null;
		var o = c.convert(current);
		if (type == t) return o;
		c = converters.get(type, t);
		if(c == null) return null;
		return c.convert(o);
	}

	public ImmutableArray<byte> get() {
		return current;
	}

}

public class StringToBytes : ConverterBase<string, ImmutableArray<byte>> {
	public override ImmutableArray<byte> convert(string o)
		=> ImmutableArray.Create(Encoding.UTF8.GetBytes(o));
}

public class BytesToString : ConverterBase<ImmutableArray<byte>, string> {
	public override string convert(ImmutableArray<byte> o) {
		var a = System.Linq.ImmutableArrayExtensions.ToArray(o);
		return Encoding.UTF8.GetString(a);
	}
}

public class Converters : IEnumerable<IConverter> {
	private List<IConverter> _all = new ();

	public void Add<I, O>(IConverter<I,O> c) {
		_all.Add(c);
	}

	public IConverter<Input, Output>? get<Input, Output>()
		=> get(typeof(Input), typeof(Output)) as IConverter<Input, Output>;

	public IConverter? get(Type from, Type to) {
		foreach (var c in _all) {
			if (c.inputType == from && c.outputType == to)
				return c;
		}
		return null;
	}

	public IEnumerator<IConverter> GetEnumerator()
		=> _all.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> _all.GetEnumerator();
}

public abstract class ConverterBase<From, To> : IConverter<From, To> {
	public Type inputType => typeof(From);
	public Type outputType => typeof(To);
	public virtual int grade => 0;

	public object convert(object input)
		=> convert((From)input);
	[return: NotNull]
	public abstract To convert(From o);
}

public interface IConverter<in From, out To> : IConverter {
	public To convert(From o);
}

public interface IConverter {
	Type inputType { get; }
	Type outputType { get; }
	/// <summary>Specifies number of intermediate converters.</summary>
	int grade { get; }

	public object convert(object input);
}