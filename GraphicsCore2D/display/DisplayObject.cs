﻿using audioysos.collections.tree;
using com.audionysos;
using System;

namespace audioysos.display {
	public abstract class DisplayObject : ITransformProvier, ITreeLeafClient<DisplayObject> {
		public event Action<DisplayObject> ADDED_TO_SURFACE;
		public event Action<DisplayObject> REMOVED_FROM_SURFACE;
		
		public string name { get; set; }
		public Transform transform { get; } = new Transform();
		public Transform globalTransform { get; } = new Transform();
		private DisplaySurface _surf;
		public DisplaySurface surface {
			get => _surf;
			internal set {
				_surf = value;
				if (_surf) ADDED_TO_SURFACE?.Invoke(this);
				else REMOVED_FROM_SURFACE?.Invoke(this);
			}
		}
		public TreePoint<DisplayObject> tree { get; private set; }
		public DisplayObjectContainer parent => tree.parent?.data as DisplayObjectContainer;

		public DisplayObject() {
			tree = !isContainer() ? new TreePoint<DisplayObject>(this)
				: new TreeNode<DisplayObject>(this);
			tree.ADDED += onAdded;
		}

		/// <summary>Indicate if this object suppose to be container that is able to to store children and thus <see cref="TreeNode{T}"/> should be created for it as oppose to <see cref="TreePoint{T}"/>.</summary>
		protected virtual bool isContainer() => false;


		private void onAdded(TreePoint<DisplayObject> obj) {
			//throw new NotImplementedException();
		}

		public Transform getGlobaTransform() {
			var t = globalTransform.setTo(transform);
			tree.forAncestors(d => t.append(d.transform));
			return t;
		}

		/// <inheritdoc/>
		public override string ToString() {
			return $@"{name} [{GetType().Name}]";	
		}
	}

	public class DisplayObjectContainer : DisplayObject, ITreeNodeClient<DisplayObject> {
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

	}

	public class Sprite : DisplayObject {
		public Graphics graphics { get; private set; }
		private IMicroGraphics2D currGraphics = BlankGraphics.instance;

		public Sprite() {
			graphics = new Graphics(currGraphics);
			ADDED_TO_SURFACE += onSurface;
			REMOVED_FROM_SURFACE -= onSurfaceLost;
		}

		private void onSurface(DisplayObject obj) {
			graphics.changeBaseDrawer(
				currGraphics,
				surface.createGraphics()
			);
		}

		private void onSurfaceLost(DisplayObject obj) {
			//TODO: Need to figure out how make sure chached graphics don't crash.
			graphics.changeBaseDrawer(
				currGraphics,
				currGraphics = BlankGraphics.instance
			);
		}
	}

}
