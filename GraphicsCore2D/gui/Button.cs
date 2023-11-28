using System;
using System.Reflection;
using System.Security.Principal;
using S = audionysos.gui.Size;
using P = audionysos.gui.RelativePlacement;
using D = audionysos.gui.LayoutDimensions;
using System.Numerics;
using System.Collections.Generic;
using System.Collections;
using System.Reflection.Metadata;
using audionysos.display;
using static System.Math;
using audionysos.math;
using audionysos.geom;
using com.audionysos;
using audionysos.graphics.extensions.shapes;

namespace audionysos.gui;

public class Button : IDisplayable<Sprite> {
	private Action<Button>? _PRESSED;
	public Action<Button>? PRESSED {
		get => _PRESSED;
		set => _PRESSED += value;
	}
	public event Action<Button>? RELEASED;
	public event Action<Button>? STATE_CHANGED;

	public ButtonStates states { get; set; } = new ButtonStates();

	private ButtonState _state;
	public ButtonState state {
		get => _state;
	}
	#region Default state data
	public object? content {
		get => states[0].content;
		set => states[0].content = value;
	}
	public LayoutSettings layout {
		get => states[0].layout;
		set => states[0].layout = value;
	}
	#endregion

	public GUIPainter painter { get; set; } = new DefaultButtonPainter();
	public VisualStyle style { get; set; } = new VisualStyle();
	public Sprite view { get; } = new Sprite();


	public Button() {
		_state = states[0];
		
	}
}

public class DefaultButtonPainter : GUIPainter {
	public override void paint(UIElement e, DisplayObject view) {
		throw new NotImplementedException();
	}
}



public class VisualStyle {
	public object? colorPalette { get; set; }
	public IFill? background { get; set; }
}


public class ButtonStates : IReadOnlyList<ButtonState> {
	private List<ButtonState> _all = new() {
		new ButtonState() { content = "Button" },
	};

	public ButtonState this[int index] => _all[index];
	public int Count => _all.Count;

	public ButtonState defaul {
		get => _all[0];
		set => _all[0] = value;
	}
	public ButtonStates others => this;

	public void Add(ButtonState s) {
		_all.Add(s);
		var b = new Button();
	}

	public IEnumerator<ButtonState> GetEnumerator() 
		=> _all.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator()
		=> _all.GetEnumerator();

	public override string ToString() {
		return $@"ButtonStates {Count} ";
	}
}

public class ButtonState {
	public object? content { get; set; }
	public LayoutSettings layout { get; set; } = new LayoutSettings();

	public ButtonState? highlighted { get; set; }
	public ButtonState? pressed { get; set; }

	public override string ToString() {
		return $"ButtonState({content})";
	}
}

public class LayoutSettings {
	public SurfacePlacement placement { get; set; } = new();
	public DimensionsData size { get; set; } = new();
	public Point2 position { get; internal set; } = new();

	public Rect bounds => new Rect(position, (Point2)size.actual);
}


public class DimensionsData {
	/// <summary>Default size to be used as size of the parent element if the child has no parent.
	/// This should have absolute values.</summary>
	public D design { get; set; } = (200, 100);
	public D desired { get; set; }
	public D minimal { get; set; }
	public D maximal { get; set; } = D.MAX;
	/// <summary>The size that was actually set by an arranger.</summary>
	public D actual { get; internal set; } = (0, 0);

	/// <summary>Specifies a fixed size by setting both <see cref="minimal"/> and <see cref="maximal"/> properties to the same value.</summary>
	public D fix { set => minimal = maximal = value; }
	/// <summary>Tells if specified size is fixed (see <see cref="fix"/>).</summary>
	public bool isFixed => minimal == maximal;

	public static implicit operator DimensionsData
		((S w, S h) t) => new() { desired = t };

	public override string ToString()
		=> $"{actual} <= {desired}";
}


public record struct LayoutDimensions {
	public static readonly D MAX = (double.MaxValue, double.MaxValue); 

	public S width;
	public S height;

	public LayoutDimensions(S width, S height) {
		this.width = width;
		this.height = height;
	}

	public D min(D other)
		=> (width.min(other.width), height.min(other.height));

	public D max(D other)
		=> (width.max(other.width), height.max(other.height));

	//TODO:
	public static D operator -(D a, D b)
		=> new D(a.width - b.width, a.height - b.height);

	public static D operator *(D a, double v)
		=> new D(a.width * v, a.height * v);

	public static implicit operator D
		((S w, S h) t) => new (t.w, t.h);

	public static implicit operator Point2
		(D d) => new(d.width, d.height);

	public override readonly string ToString()
		=> $"({width} x {height})";
}


public struct Size {
	public static readonly S ZERO = 0;
	public static readonly S AUTO = double.NaN;
	public static readonly S STRETCH = double.PositiveInfinity;

	public static S Relative<N>(N v) where N : INumber<N>
		=> new S(double.CreateChecked(v)).relative();

	public S relative() => value > 0 ? new S(-value) : this;

	public double value = double.NaN;
	public bool isRelative => value < 0;

	public Size(double value) {
		this.value = value;
	}

	public S min(S other) {
		//TODO: Consider other type of size
		if(value.NaN()) return this;
		if (value > 0) return Min(value, other.value);
		return Max(value, other.value);
	}

	public S max(S other) {
		//TODO: Consider other type of size
		if (value.NaN()) return this;
		if (value > 0) return Max(value, other.value);
		return Min(value, other.value);
	}

	public static S operator *(S s, double v)
		=> s.value * v;

	public static implicit operator S(double v)
		=> new S(v);

	public static implicit operator double(S s)
		=> s.value;

	public override string ToString()
		=> $"{value}";
}

public class SurfacePlacement {
	public P horizontal { get; set; }
	public P vertical { get; set; }

	public SurfacePlacement() { }
	public SurfacePlacement(P vertical, P horizontal) {
		this.horizontal = horizontal;
		this.vertical = vertical;
	}

	/// <summary>Crates placement form tuple. Vertical placement goes first.</summary>
	/// <param name="t"></param>
	public static implicit operator SurfacePlacement
		((P v, P h) t) => new(t.v, t.h);

	public static implicit operator Point2(SurfacePlacement p)
		=> (p.horizontal.value, p.vertical.value);

	public override string ToString()
		=> $"({vertical} x {horizontal})";
}

public struct RelativePlacement {
	public static readonly P BEFORE = -2;
	public static readonly P ABOVE = -2;
	public static readonly P TOP_EDGE = -1.5;
	public static readonly P LEFT_EDGE = -1.5;
	public static readonly P TOP = -1;
	public static readonly P LEFT = -1;
	public static readonly P CENTER = 0;
	public static readonly P RIGHT = 1;
	public static readonly P BOTTOM = 1;
	public static readonly P RIGHT_EDGE = 1.5;
	public static readonly P BOTTOM_EDGE = 1.5;
	public static readonly P AFTER = 2;
	public static readonly P BELOW = 2;

	public static readonly Range<double> inside = (-1d).to(1);

	public double value = double.NaN;
	public RelativePlacement(double value) {
		this.value = value;
	}

	public bool isInside => inside.contains(value);

	public double localScale {
		get {
			if (isInside) return value;
			if (value < 0) return value + 1;
			return value - 1;
		}
	}

	public static implicit operator P(double v)
		=> new (v);

	public override string ToString()
		=> $"{value}";
}


public static class LayoutExtensions {
	public static D relative<N>(this (N w, N h) t) where N : INumber<N>
		=> new D(S.Relative(t.w), S.Relative(t.h));

	public static D toRelative(this D t)
		=> new D(t.width.relative(), t.height.relative());
}
