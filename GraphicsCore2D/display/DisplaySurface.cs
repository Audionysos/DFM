using audioysos.collections.tree;
using audioysos.geom;
using System;
using System.Collections.Generic;
using System.Text;
//skatching

namespace audioysos.display {
	public class DisplaySurface {

		public DisplaySurface() {
			var c = new DisplayObjectContainer();
			var d = new Der();
			var o = d as DisplayObject;
			c.addChild(o);
			c.addChild(d);
			//d.
			//c.
		}

	}

	public class Der : DisplayObject {

	}

	public class DisplayObjectContainer : DisplayObject, ITreeNodeClient<DisplayObject> {
		//new public TreeNode<DisplayObject> tree { get; private set; }
		/// <summary>Return this point which is aaociated with this node.</summary>
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

	public abstract class DisplayObject : ITransformProvier, ITreeLeafClient<DisplayObject> {
		public string name { get; set; }
		public Transform transform { get; } = new Transform();
		public Transform globalTransform { get; } = new Transform();
		public DisplaySurface surface { get; }
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

		public override string ToString() {
			return $@"{name} [{GetType().Name}]";	
		}
	}

	public class Polygon {

	}

	public class Transform {
		public double x { get; set; }
		public double y { get; set; }
		public double z { get; set; }
		public double sX { get; set; } = 1;
		public double sY { get; set; } = 1;

		public Transform append(Transform t) {
			sX *= t.sX; sY *= t.sY;
			x = x * sX + t.x;
			y = y * sY + t.y;
			//z = t.z;
			return this;
		}

		public Transform copy() {
			return new Transform() {
				x = x,
				y = y,
				z = z,
			};
		}

		public Transform setTo(Transform transform) {
			var t = transform;
			this.x = t.x;
			this.y = t.y;
			this.z = t.z;
			return this;
		}
	}

	public interface ITransformProvier {
		Transform transform { get; }

	}

	public static class TransformProviderExtensions {

		public static void get() {

		}

	}

	public class ViewsLayouter {
		private IArrangable container { get; set; }
		private List<IArrangable> childs { get; set; }

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

	public interface IArrangable {
		void arragne(int pos, int size);

		IPoint2 pos { get; }
		IPoint2 size { get; }
		IPoint2 minSize { get; }
		IPoint2 maxSize { get; }
	}

}
