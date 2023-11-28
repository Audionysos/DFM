using audionysos.collections.tree;
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

public abstract class UIElement : ITreeLeafClient<UIElement> {
	public static readonly GUIPainter defaultPainter = new DefaultButtonPainter();

	public TreePoint<UIElement> tree { get; }
	public virtual LayoutSettings layout { get; set; } = new LayoutSettings();
	public UIElementContainer? parent => tree.parent?.data as UIElementContainer;

	public abstract DisplayObject view { get; protected set; }

	internal LayoutArranger arranger = new LayoutArranger();
	public GUIPainter painter { get; set; } = new DefaultPainter();
	public VisualStyle style { get; set; } = new VisualStyle();

	public UIElement() {
		tree = !isContainer() ? new TreePoint<UIElement>(this)
			: new TreeNode<UIElement>(this);
		//tree.ADDED += onAdded;
		view.ENTER_FRAME += onEnterFrame;
	}

	private void onEnterFrame(DisplayObject @object) {
		update();
	}

	internal void update() {
		arranger.arrange(parent, this);
		painter.paint(this, view);
	}

	/// <summary>Indicate if this object suppose to be container that is able to store children and thus <see cref="TreeNode{T}"/> should be created for it as oppose to <see cref="TreePoint{T}"/>.</summary>
	protected virtual bool isContainer() => false;
}

public abstract class UIElementContainer : UIElement, ITreeNodeClient<UIElement> {

	protected override bool isContainer() => true;
	new public TreeNode<UIElement> tree
		=> (TreeNode<UIElement>)base.tree;

	public int Count => tree.children.Count;

	protected UIElementContainer() {
		view = view ?? throw new Exception($@"View of {nameof(UIElementContainer)} must be a {nameof(DisplayObjectContainer)}");
	}

	//public event Action<UIElementContainer, UIElement>? CHILD_ADD;
	//public event Action<UIElementContainer, UIElement>? CHILD_REMOVED;

	//public void addChild(UIElement child) {
	//	tree.addChild(child.tree);
	//	CHILD_ADD?.Invoke(this, child);
	//}
	//public void removeChild(UIElement child) {
	//	tree.removeChild(child.tree);
	//	CHILD_REMOVED?.Invoke(this, child);
	//}

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

public class Polygon {

}

public interface ITransformProvider {
	Transform transform { get; }

}

public static class TransformProviderExtensions {

	public static void get() {

	}

}

public class ViewsLayouter {
	private IArrangeble container { get; set; }
	private List<IArrangeble> childs { get; set; }

	public void layout() {
		var childsSize = getChildrenSize();
	}

	private Point2 getChildrenSize() {
		var s = new Point2(0, 0);
		foreach (var ch in childs) {
			s.add(ch.size);
		}
		return s;

	}
}

public class ObjectArranger {

}

public interface IArrangeble {
	void arragne(int pos, int size);

	IPoint2 pos { get; }
	IPoint2 size { get; }
	IPoint2 minSize { get; }
	IPoint2 maxSize { get; }
}
