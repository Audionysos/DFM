﻿using audionysos.collections.tree;
using audionysos.geom;
using audionysos.gui;
using audionysos.input;
using com.audionysos;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
//skatching

namespace audionysos.display;
public abstract class DisplaySurface {
	private List<DisplayObject> _displayed = new List<DisplayObject>(); 
	public event Action<DisplaySurface, DisplayPointer>? POINTER_MOVED;
	public event Action<DisplaySurface, DisplayObject>? OBJECT_ADD;

	public IFill background { get; set; } = (Color)0xD3D3D3FF;

	public DisplaySurface() {}

	public void Add(DisplayObject d) {
		if (d.surface != null) throw new Exception("The object is already added to some display surface.");
		_displayed.Add(d);
		d.surface = this;
		OBJECT_ADD?.Invoke(this, d);
	}

	public void update() {
		clear();
		for (int i = 0; i < _displayed.Count; i++) {
			_displayed[i].update();
		}
	}

	public void clear() => clear<IPoint2>();
	public abstract void clear<P>(IRect<P>? rect = null) where P : IPoint2;

	/// <summary>Create graphics that this surface will display.</summary>
	public abstract IMicroGraphics2D createGraphics();

	/// <summary>Render graphics onto the surface. Given object is the one created with <see cref="createGraphics"/> method.</summary>
	public abstract void renderGraphics(IMicroGraphics2D graphics);

	public void pointerMove(DisplayPointer p) {
		POINTER_MOVED?.Invoke(this, p);
	}

	private List<DisplayObject> hitTestCache = new();
	public IReadOnlyList<DisplayObject>? hitTest(IPoint2 p) {
		hitTestCache.Clear();
		foreach (DisplayObject d in _displayed) {
			if (d is DisplayObjectContainer c) {
				if(c.rayCast(p, hitTestCache))
					return hitTestCache;
			} else if (d.hitTest(p)) {
				hitTestCache.Add(d);
				return hitTestCache;
			}
		}
		return null;
	}

	/// <summary>False if null.</summary>
	public static implicit operator bool([NotNullWhen(true)]DisplaySurface? s) => s!=null;
}


public class Der : DisplayObject {

}

public class DerX : DisplayObjectContainer {

}

public class XXX : IDisplayable<DerX?> {
	public DerX? view { get; }
	//DisplayObject IDisplayable.view { get; }
}

public interface IDisplayable {
	//public DisplayObject view { get; }
}
public interface IDisplayable<T> : IDisplayable where T : DisplayObject {
	/// <summary>Displayable view that could be add to a <see cref="DisplayObject"/>.</summary>
	new public T view { get; }
}

public interface ITransformProvider {
	Transform transform { get; }

}
