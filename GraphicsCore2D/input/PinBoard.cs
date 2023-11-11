using System;
using System.Collections.Immutable;
using System.Text;

namespace audionysos.input;

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
