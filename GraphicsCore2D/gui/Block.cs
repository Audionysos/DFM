using audionysos.display;
using System.Collections;
using System.Collections.Generic;
using System;

namespace audionysos.gui;

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
