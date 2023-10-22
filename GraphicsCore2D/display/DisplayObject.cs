﻿using audionysos.collections.tree;
using audionysos.geom;
using com.audionysos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace audionysos.display; 
public abstract class DisplayObject : ITransformProvier, ITreeLeafClient<DisplayObject> {
	public event Action<DisplayObject> ADDED_TO_SURFACE;
	public event Action<DisplayObject> REMOVED_FROM_SURFACE;
	public event Action<DisplayObject> ENTER_FRAME;
	
	public string name { get; set; }
	public Transform transform { get; } = new Transform();
	public Transform globalTransform { get; } = new Transform();
	private DisplaySurface _surf;
	public DisplaySurface surface {
		get => _surf;
		internal set {
			_surf = value;
			if (_surf) {
				if (this is DisplayObjectContainer c)
					c.tree.forDescendants(d => d.surface = _surf);
				ADDED_TO_SURFACE?.Invoke(this);
			} else REMOVED_FROM_SURFACE?.Invoke(this);
		}
	}
	public TreePoint<DisplayObject> tree { get; private set; }
	public DisplayObjectContainer parent => tree.parent?.data as DisplayObjectContainer;

	public DisplayObject() {
		tree = !isContainer() ? new TreePoint<DisplayObject>(this)
			: new TreeNode<DisplayObject>(this);
		tree.ADDED += onAdded;
	}

	/// <summary>Indicate if this object suppose to be container that is able to store children and thus <see cref="TreeNode{T}"/> should be created for it as oppose to <see cref="TreePoint{T}"/>.</summary>
	protected virtual bool isContainer() => false;


	private void onAdded(TreePoint<DisplayObject> obj) {
		surface = parent.surface;
	}

	public Transform getGlobalTransform() {
		var t = globalTransform.setTo(transform);
		tree.forAncestors(d => t.append(d.transform));
		return t;
	}

	internal void update() {
		ENTER_FRAME?.Invoke(this);
		render();
	}

	internal virtual void render() { }

	/// <summary>Returns true if there are any graphics drawn at given position.</summary>
	public virtual bool hitTest(IPoint2 p) => false;

	/// <inheritdoc/>
	public override string ToString() {
		return $@"{name} [{GetType().Name}]";	
	}

}

public class HitTester {

	public IList Test(DisplayObject o, IPoint2 p) {
		return null;
	}
}

public abstract class DisplayObjectContainer : DisplayObject, ITreeNodeClient<DisplayObject> {
	//new public TreeNode<DisplayObject> tree { get; private set; }
	/// <summary>Return tree point which is associated with this node.</summary>
	new public TreeNode<DisplayObject> tree => base.tree as TreeNode<DisplayObject>;

	/// <summary>Number of children present in this container.</summary>
	public int Count => tree.children.Count;

	public DisplayObjectContainer() {
		//tree = new TreeNode<DisplayObject>(this);

	}

	protected override bool isContainer() => true;

	public void addChild(DisplayObject child) => tree.addChild(child.tree);
	public void removeChild(DisplayObject child) => tree.removeChild(child.tree);

	/// <summary>Returns list of all objects in the branch for which <see cref="DisplayObject.hitTest(IPoint2)"/> returned true at given position (including this one).</summary>
	/// <param name="p">Position for hit testing.</param>
	/// <param name="output"></param>
	/// <returns></returns>
	public bool rayCast(IPoint2 p, IList<DisplayObject> output) {
		tree.forDescendants(bool (o) => {
			if (o is DisplayObjectContainer c) {
				if (c.rayCast(p, output)) return true;
			} else if (o.hitTest(p)) return true;
			return false;
		}, backward: true);
		if (hitTest(p)) {
			output.Add(this);
			return false;
		}
		return false;
	}
}

public class Sprite : DisplayObjectContainer {
	public Graphics graphics { get; private set; }
	private IMicroGraphics2D currGraphics = new CachedGraphics();

	public Sprite() {
		graphics = new Graphics(currGraphics);
		ADDED_TO_SURFACE += onSurface;
		REMOVED_FROM_SURFACE -= onSurfaceLost;
	}

	private void onSurface(DisplayObject obj) {
		var cached = currGraphics as CachedGraphics;
		graphics.changeBaseDrawer(
			currGraphics,
			currGraphics = surface.createGraphics() 
		);
		cached?.transferTo(graphics);
	}

	private void onSurfaceLost(DisplayObject obj) {
		//TODO: Need to figure out how make sure cached graphics don't crash.
		graphics.changeBaseDrawer(
			currGraphics,
			currGraphics = BlankGraphics.instance
		);
	}

	internal override void render() {
		getGlobalTransform();
		graphics.transform(globalTransform);
		surface.renderGraphics(graphics.baseGraphics);
		tree.forDescendants(d => d.update());
	}

	public override bool hitTest(IPoint2 p) {
		if (graphics is IInteractiveGraphics2D ig)
			return ig.pointInShape(p);
		return false;
	}
}
