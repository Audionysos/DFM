using audionysos.display;
using audionysos.geom;
using audionysos.math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace audionysos.gui;
public class ScrollBar {

	private Range<double> _r = (0, 1);
	public Range<double> range {
		get => _r;
		set { 
			_r = value;
		}
	}
	public double position { get; set; }
	public double value => range.inter(position);

	public Sprite view { get; }

	IPoint2 size { get; set; }
	DisplayObject background;
	DisplayObject upButton;
	DisplayObject downButton;
	DisplayObject slidingArea;
	DisplayObject handle;

	public ScrollBar() {
		Debug.WriteLine(value);
		range = (-90, 90);
		position = .75;
		Debug.WriteLine(value);
		var uir = 0u.to(100u);
		var v = uir.inter(.336);
		var n = uir.normal(75);
		Debug.WriteLine(v);

		var ir = 0.to(100);
		n = ir.normal(-75);

		//testEnum();
	}

	private void testEnum() {
		Debug.WriteLine("enum");
		var c = 0;
		foreach (var n in 1.5f.to(1.50005f).spread(10)) {
			Debug.WriteLine(n); c++;
		}
		Debug.WriteLine($"count:{c}");

		c = 0;
		foreach (var n in 0d.to(100d).spread(3)) {
			Debug.WriteLine(n); c++;
		}
		Debug.WriteLine($"count:{c}");

		//1.5f.to(1.50005f).all(n => Debug.WriteLine(n));
		//c = 0;
		//2_000_000_000f.to(2_000_001_000f)
		//	.all(n => { Debug.WriteLine(n); c++; });
		//Debug.WriteLine($"count:{c}");
	}
}


public class Block : UIElement {
	public override DisplayObject view { get; protected set; } = new Sprite();
}

public class Panel : UIElementContainer, IReadOnlyList<UIElement>, IUIElementsCollection {

	public override DisplayObject view { get; protected set; } = new Sprite();

	public UIElement this[int index] => tree.children[index].data;
	public IUIElementsCollection children => this;

	public event Action<UIElementContainer, UIElement>? CHILD_ADD;
	public event Action<UIElementContainer, UIElement>? CHILD_REMOVED;

	public void Add(UIElement child) {
		addChild(child);
	}
	public void addChild(UIElement child) {
		tree.addChild(child.tree);
		(view as DisplayObjectContainer)!.addChild(child.view);
		CHILD_ADD?.Invoke(this, child);
	}
	public void removeChild(UIElement child) {
		tree.removeChild(child.tree);
		(view as DisplayObjectContainer)!.removeChild(child.view);
		CHILD_REMOVED?.Invoke(this, child);
	}

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();
	public IEnumerator<UIElement> GetEnumerator() {
		foreach (var ch in tree.children) {
			yield return ch.data;
		}
		yield break;
	}

}

public interface IUIElementsCollection : IReadOnlyList<UIElement>, IEnumerable<UIElement> {
	public void Add(UIElement child);
}
