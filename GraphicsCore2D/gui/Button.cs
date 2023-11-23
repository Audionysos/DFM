using System;
using System.Reflection;
using System.Security.Principal;
using LS = audionysos.gui.Size;
using RP = audionysos.gui.RelativePlacement;
using LD = audionysos.gui.LayoutDimensions;
using System.Numerics;
using System.Collections.Generic;
using System.Collections;
using System.Reflection.Metadata;
using audionysos.display;
using static System.Math;
using audionysos.math;
using audionysos.geom;

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

}

public abstract class GUIPainter {

}

public class VisualStyle {
	public object? colorPalette { get; set; }
	public object? background { get; set; }
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
}


public class DimensionsData {
	public LD desired { get; set; }
	public LD minimal { get; set; }
	public LD maximal { get; set; }
	public LD actual { get; internal set; } = (0, 0);

	public LD fix { set => minimal = maximal = value; }
	public bool isFixed => minimal == maximal;

	public static implicit operator DimensionsData
		((LS w, LS h) t) => new() { desired = t };

	public override string ToString()
		=> $"{actual} <= {desired}";
}


public record struct LayoutDimensions {
	public LS width;
	public LS height;

	public LayoutDimensions(LS width, LS height) {
		this.width = width;
		this.height = height;
	}

	public LD min(LD other)
		=> (width.min(other.width), height.min(other.height));

	public LD max(LD other)
		=> (width.max(other.width), height.max(other.height));

	//TODO:
	public static LD operator -(LD a, LD b)
		=> new LD(a.width - b.width, a.height - b.height);

	public static LD operator *(LD a, double v)
		=> new LD(a.width * v, a.height * v);

	public static implicit operator LD
		((LS w, LS h) t) => new (t.w, t.h);

	public override string ToString()
		=> $"({width} x {height})";
}


public struct Size {
	public static readonly LS ZERO = 0;
	public static readonly LS AUTO = double.NaN;
	public static readonly LS STRETCH = double.PositiveInfinity;

	public static LS Relative<N>(N v) where N : INumber<N>
		=> new LS(double.CreateChecked(v)).relative();

	public LS relative() => value > 0 ? new LS(-value) : this;

	public double value = double.NaN;
	public bool isRelative => value < 0;

	public Size(double value) {
		this.value = value;
	}

	public LS min(LS other) {
		//TODO: Consider other type of size
		if(value.NaN()) return this;
		if (value > 0) return Min(value, other.value);
		return Max(value, other.value);
	}

	public LS max(LS other) {
		//TODO: Consider other type of size
		if (value.NaN()) return this;
		if (value > 0) return Max(value, other.value);
		return Min(value, other.value);
	}

	public static LS operator *(LS s, double v)
		=> s.value * v;

	public static implicit operator LS(double v)
		=> new LS(v);

	public static implicit operator double(LS v)
		=> new LS(v);

	public override string ToString()
		=> $"{value}";
}

public class SurfacePlacement {
	public RP horizontal { get; set; }
	public RP vertical { get; set; }

	public SurfacePlacement() { }
	public SurfacePlacement(RP vertical, RP horizontal) {
		this.horizontal = horizontal;
		this.vertical = vertical;
	}

	public static implicit operator SurfacePlacement
		((RP v, RP h) t) => new(t.v, t.h);

	public static implicit operator Point2(SurfacePlacement p)
		=> (p.horizontal.value, p.vertical.value);

	public override string ToString()
		=> $"({vertical} x {horizontal})";
}

public struct RelativePlacement {
	public static readonly RP BEFORE = -2;
	public static readonly RP ABOVE = -2;
	public static readonly RP TOP_EDGE = -1.5;
	public static readonly RP LEFT_EDGE = -1.5;
	public static readonly RP TOP = -1;
	public static readonly RP LEFT = -1;
	public static readonly RP CENTER = 0;
	public static readonly RP RIGHT = 1;
	public static readonly RP BOTTOM = 1;
	public static readonly RP RIGHT_EDGE = 1.5;
	public static readonly RP BOTTOM_EDGE = 1.5;
	public static readonly RP AFTER = 2;
	public static readonly RP BELOW = 2;

	public static readonly Range<double> inside = (-1d).to(1);

	public double value = double.NaN;
	public RelativePlacement(double value) {
		this.value = value;
	}

	public bool isInside => inside.contains(value);

	public double localScale() {
		if (isInside) return value;
		if (value < 0) return value + 1;
		return value - 1;
	}

	public static implicit operator RP(double v)
		=> new (v);

	public override string ToString()
		=> $"{value}";
}


public static class LayoutExtensions {
	public static LD relative<N>(this (N w, N h) t) where N : INumber<N>
		=> new LD(LS.Relative(t.w), LS.Relative(t.h));

	public static LD toRelative(this LD t)
		=> new LD(t.width.relative(), t.height.relative());
}
